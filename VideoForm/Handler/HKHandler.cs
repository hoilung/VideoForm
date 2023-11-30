using System;
using System.Windows.Forms;
using VideoForm.Common;

namespace VideoForm.Handler
{
    public class HKHandler : IDisposable
    {
        private int m_lRealHandle = -1;
        public int m_lUserID = -1;

        private uint iLastErr = 0;
        private string str;
        public HKHandler()
        {
            bool m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (!m_bInitSDK)
            {
                MessageBox.Show("NET_DVR_Init error!");
                //throw new AccessViolationException("初始化海康设备错误");
            }
        }



        #region 登录,注销
        public void Login(string ip, int port, string username, string password)
        {
            if (m_lUserID < 0)
            {
                var userLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO()
                {
                    sDeviceAddress = new byte[129],
                    sUserName = new byte[64],
                    sPassword = new byte[64],
                    bUseAsynLogin = false,
                    byLoginMode = 0,//0-Private, 1-ISAPI, 2-自适应
                    byHttps = 0,//0-不适用tls，1-使用tls 2-自适应
                };
                ip.ToBytes().CopyTo(userLoginInfo.sDeviceAddress, 0);
                username.ToBytes().CopyTo(userLoginInfo.sUserName, 0);
                password.ToBytes().CopyTo(userLoginInfo.sPassword, 0);
                userLoginInfo.wPort = (ushort)port;
                userLoginInfo.cbLoginResult = new CHCNetSDK.LOGINRESULTCALLBACK(LoginCallBack);

                var deviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
                m_lUserID = CHCNetSDK.NET_DVR_Login_V40(ref userLoginInfo, ref deviceInfo);
                if (m_lUserID < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号 Failed to login and output the error code
                                                                               //Log.Error(strErr);
                    MessageBox.Show(str);
                }
                //登录成功
                //userID
            }

        }

        /// <summary>
        /// 登录回调
        /// </summary>
        /// <param name="lUserID"></param>
        /// <param name="dwResult"></param>
        /// <param name="lpDeviceInfo"></param>
        /// <param name="pUser"></param>
        public void LoginCallBack(int lUserID, int dwResult, IntPtr lpDeviceInfo, IntPtr pUser)
        {
            string strLoginCallBack = "登录设备，lUserID：" + lUserID + "，dwResult：" + dwResult;

            if (dwResult == 0)
            {
                uint iErrCode = CHCNetSDK.NET_DVR_GetLastError();
                strLoginCallBack = strLoginCallBack + "，错误号:" + iErrCode;
                //Log.Error(strLoginCallBack);
                MessageBox.Show(strLoginCallBack);
            }
        }
        #endregion

        public void LogOut()
        {
            StopPreview();

            if (m_lUserID >= 0)
            {
                if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_Logout failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                m_lUserID = -1;
            }
        }

        #region 预览，停止预览

        public void Preview(PictureBox RealPlayWnd)
        {
            if (m_lUserID < 0)
            {
                MessageBox.Show("Please login the device firstly");
                return;
            }
            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                RealPlayWnd.Invoke(new MethodInvoker(() =>
                {
                    lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//预览窗口
                }));
                lpPreviewInfo.lChannel = 1;// Int16.Parse(textBoxChannel.Text);//预te览的设备通道
                lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;


                IntPtr pUser = new IntPtr();//用户数据
                                            //打开预览 Start live view 
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_RealPlay_V40 failed, error code= " + iLastErr; //预览失败，输出错误号
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    //预览成功
                    //btnPreview.Text = "Stop Live View";
                }
            }
            else
            {
                StopPreview();
            }

        }


        public void StopPreview()
        {
            if (m_lRealHandle >= 0)
            {
                if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "NET_DVR_StopRealPlay failed, error code= " + iLastErr;
                    MessageBox.Show(str);
                    return;
                }
                m_lRealHandle = -1;
            }
        }

        #endregion


        public void Dispose()
        {
            //释放SDK资源，在程序结束之前调用
            CHCNetSDK.NET_DVR_Cleanup();

        }
    }
}
