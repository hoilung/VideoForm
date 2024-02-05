using NETSDKHelper;
using System;


namespace VideoForm.Handler
{
    public class UniViewHandler : IDisposable
    {
        public IntPtr m_lpDevHandle = IntPtr.Zero;//用户登录句柄
        private IntPtr m_lpRealPlay = IntPtr.Zero;//播放成功句柄        

        public UniViewHandler()
        {
            int iRet = NETDEVSDK.NETDEV_Init();
            if (NETDEVSDK.TRUE != iRet)
            {
                this.OnMsg("it is not a admin oper");
            }
        }
        public event Action<string> Msg;

        public virtual void OnMsg(string msg)
        {
            this.Msg?.Invoke(msg);
        }

        public void Dispose()
        {
            NETDEVSDK.NETDEV_Cleanup();
        }

        #region 登录注销

        public void Login(string m_ip, int m_port, string m_userName, string m_password, NETDEMO.NETDEMO_DEVICE_TYPE_E m_eDeviceType = NETDEMO.NETDEMO_DEVICE_TYPE_E.NETDEMO_DEVICE_IPC_OR_NVR)
        {
            if (NETDEMO.NETDEMO_DEVICE_TYPE_E.NETDEMO_DEVICE_INVALID == m_eDeviceType)
            {
                return;
            }
            NETDEV_DEVICE_LOGIN_INFO_S pstDevLoginInfo = new NETDEV_DEVICE_LOGIN_INFO_S();
            NETDEV_SELOG_INFO_S pstSELogInfo = new NETDEV_SELOG_INFO_S();
            pstDevLoginInfo.szIPAddr = m_ip;
            pstDevLoginInfo.dwPort = m_port;
            pstDevLoginInfo.szUserName = m_userName;
            pstDevLoginInfo.szPassword = m_password;
            if (NETDEMO.NETDEMO_DEVICE_TYPE_E.NETDEMO_DEVICE_VMS == m_eDeviceType)
            {
                pstDevLoginInfo.dwLoginProto = (int)NETDEV_LOGIN_PROTO_E.NETDEV_LOGIN_PROTO_PRIVATE;
                this.OnMsg("暂时不支持 vms");
                return;
            }
            else
            {
                pstDevLoginInfo.dwLoginProto = (int)NETDEV_LOGIN_PROTO_E.NETDEV_LOGIN_PROTO_ONVIF;
            }
            m_lpDevHandle = NETDEVSDK.NETDEV_Login_V30(ref pstDevLoginInfo, ref pstSELogInfo);
            if (m_lpDevHandle == IntPtr.Zero)
            {
                this.OnMsg(m_ip + " : " + m_port + " login " + NETDEVSDK.NETDEV_GetLastError());
            }
            //获取视频通道
            //int pdwChlCount = 256;
            //IntPtr pstVideoChlList = new IntPtr();
            //pstVideoChlList = Marshal.AllocHGlobal(256 * Marshal.SizeOf(typeof(NETDEV_VIDEO_CHL_DETAIL_INFO_S)));
            //var chrst = NETDEVSDK.NETDEV_QueryVideoChlDetailList(m_lpDevHandle, ref pdwChlCount, pstVideoChlList);
            //if (chrst == NETDEVSDK.TRUE)
            //{

            //}
            //Marshal.FreeHGlobal(pstVideoChlList);
            //NETDEV_DEVICE_INFO_S pstDevInfo = new NETDEV_DEVICE_INFO_S();
            //NETDEVSDK.NETDEV_GetDeviceInfo(m_lpDevHandle, ref pstDevInfo);
            //
        }

        public void LogOut()
        {
            StopRealPlay();
            if (m_lpDevHandle != IntPtr.Zero)
            {
                NETDEVSDK.NETDEV_Logout(m_lpDevHandle);
            }
        }

        #endregion

        #region 预览 停止预览

        public void StartRealPlay(System.Windows.Forms.PictureBox RealPlayWnd)
        {
            NETDEV_PREVIEWINFO_S stPreviewInfo = new NETDEV_PREVIEWINFO_S();
            RealPlayWnd.Invoke(new System.Windows.Forms.MethodInvoker(() =>
            {
                stPreviewInfo.hPlayWnd = RealPlayWnd.Handle;
            }));
            stPreviewInfo.dwChannelID = 1;
            stPreviewInfo.dwLinkMode = 1;// (int)NETDEV_PROTOCAL_E.NETDEV_TRANSPROTOCAL_RTPTCP;
            stPreviewInfo.dwStreamType = 1;// (int)NETDEV_LIVE_STREAM_INDEX_E.NETDEV_LIVE_STREAM_INDEX_AUX;//0主码流 1子码流
            m_lpRealPlay = NETDEVSDK.NETDEV_RealPlay(m_lpDevHandle, ref stPreviewInfo, IntPtr.Zero, IntPtr.Zero);
            if (m_lpRealPlay == IntPtr.Zero)
            {
                return;
            }
        }

        public void StopRealPlay()
        {
            if (m_lpRealPlay == IntPtr.Zero)
                return;
            if (NETDEVSDK.NETDEV_StopRealPlay(m_lpRealPlay) == NETDEVSDK.FALSE)
            {
                return;
            }

        }

        #endregion
    }
}
