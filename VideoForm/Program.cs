using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;
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
            var currdir = Application.StartupPath;
            string _runtimes = Path.Combine(currdir, "runtimes");
            if (File.Exists(currdir + "\\hk_dll.zip") == false)
            {
                ExtractNormalFileInResx(Properties.Resources.hk_dll, currdir + "\\hk_dll.zip");
                Thread.Sleep(500);
            }
            if (File.Exists(currdir + "\\uniview_dll.zip") == false)
            {
                ExtractNormalFileInResx(Properties.Resources.uniview_dll, currdir + "\\uniview_dll.zip");
                Thread.Sleep(500);
            }
            if (Directory.Exists(_runtimes) == false)
            {
                Directory.CreateDirectory(_runtimes);
            }
            if (!File.Exists(_runtimes + "\\HCNetSDK.dll"))
            {
                ZipFile.ExtractToDirectory(currdir + "\\hk_dll.zip", _runtimes);
                Thread.Sleep(500);
            }
            if (!File.Exists(_runtimes + "\\NetDEVSDK.dll"))
            {
                ZipFile.ExtractToDirectory(currdir + "\\uniview_dll.zip", _runtimes);
                Thread.Sleep(500);
            }


            //只有一个参数的时候，替换显示图像
            if (args.Length == 1)
            {
                if (args[0].StartsWith("conf"))
                {
                    Model.Conf.Instance.Create();
                    return;
                }

                //var ps = System.Diagnostics.Process.GetProcessesByName("VideoForm");
                var p1 = WINDOWSMSG.FindWindow(null, "视频预览...");
                if (p1 != IntPtr.Zero)// (ps.Length > 1)
                {

                    WINDOWSMSG.SendMessageString(p1, args[0]);
                    Application.Exit();
                    return;
                }

            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var main = new Main();
            //main.VideoSetString = "192.168.1.199:8000|admin|xxct111111|hikvision";
            //main.VideoSetString = "192.168.1.199:80|admin|xxct111111|uniview";
            if (args.Length > 0)
            {
                main.VideoSetString = args[0];
            }
            Application.Run(main);
        }

        static void ExtractNormalFileInResx(byte[] resource, String path)
        {
            FileStream file = new FileStream(path, FileMode.Create);
            file.Write(resource, 0, resource.Length);
            file.Flush();
            file.Close();
        }


    }
}
