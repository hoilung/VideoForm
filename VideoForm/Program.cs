using System;
using System.Windows.Forms;
using VideoForm.Common;

namespace VideoForm
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //只有一个参数的时候，替换显示图像
            if (args.Length == 1)
            {
                //var ps = System.Diagnostics.Process.GetProcessesByName("VideoForm");
                var p1 = WINDOWSMSG.FindWindow(null, "视频预览...");
                if (p1 != IntPtr.Zero)// (ps.Length > 1)
                {

                    WINDOWSMSG.SendMessageString(p1, args[0]);
                    Application.Exit();
                    return;
                }

            }

            var main = new Form1();
            //main.VideoSetString = "192.168.1.199:8000|admin|xxct111111|hikvision";
            //main.VideoSetString = "192.168.1.199:8000|admin|xxct111111|uniview";
            if (args.Length > 0)
            {
                main.VideoSetString = args[0];
            }
            Application.Run(main);
        }
    }
}
