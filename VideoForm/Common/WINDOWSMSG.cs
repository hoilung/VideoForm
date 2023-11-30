using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VideoForm.Common
{
    public class WINDOWSMSG
    {
        //    [DllImport("User32.dll", EntryPoint = "SendMessage")]
        //    public static extern IntPtr SendMessage(int hWnd, int msg, IntPtr wParam, CopyDataStruct lParam);//窗口句柄、

        // 定义需要使用的Windows消息常量  
        public const int WM_COPYDATA = 0x004A;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        public static void SendMessageString(IntPtr hWnd, string text)
        {
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = IntPtr.Zero;
            cds.lpData = text;
            cds.cbData = Encoding.Unicode.GetBytes(cds.lpData).Length + 2;

            var rst = SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ref cds);
            if (rst != IntPtr.Zero)
            {
                //成功                
            }
            else
            {
                //失败
            }
        }


    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpData;
    }



}
