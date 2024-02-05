using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoForm.Common;
using VideoForm.Handler;

namespace VideoForm
{
    public partial class Main : Form
    {
        public HKHandler hkHandler = null;
        public UniViewHandler uniViewHandler = null;
        public Main()
        {
            InitializeComponent();
            hkHandler = new HKHandler();//海康
            hkHandler.Msg += Handler_Msg;


            uniViewHandler = new UniViewHandler();//宇视
            uniViewHandler.Msg += Handler_Msg;

            this.Shown += Form1_Shown;
            this.FormClosed += Form1_FormClosed;

            //this.button1.Click += Button1_Click;

            //File.WriteAllText("h.txt", this.Handle.ToString());

        }


        private void Handler_Msg(string obj)
        {
            try
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    toolStripStatusLabel1.Text = obj;
                }));
            }
            catch (Exception)
            {

            }
        }


        //private void Button1_Click(object sender, EventArgs e)
        //{
        //    //var wp = WINDOWSMSG.FindWindow("Form1", "视频预览");
        //    //var p1 = WINDOWSMSG.FindWindow(null, "视频预览");
        //    //WINDOWSMSG.PostMessageString(p1, "192.168.1.64:8000|admin|xxct111111");
        //    WINDOWSMSG.SendMessageString(this.Handle, "192.168.1.64:8000|admin|xxct111111");
        //}

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            hkHandler.LogOut();
            hkHandler.Dispose();

            uniViewHandler.LogOut();
            uniViewHandler.Dispose();
        }

        public string VideoSetString { get; set; } = string.Empty;


        private void Form1_Shown(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                ShowVideo();
            });
        }


        public void ShowVideo()
        {
            var set = Model.VideoSet.Parse(this.VideoSetString);
            if (set == null) return;

            hkHandler.LogOut();
            uniViewHandler.LogOut();
            this.Handler_Msg($"准备登录：{set.ip}");
            switch (set.category)
            {
                case Model.categoryEnum.hikvision:                    
                    hkHandler.Login(set.ip, set.port, set.username, set.password);                    
                    hkHandler.Preview(this.pictureBox1);
                    this.Handler_Msg($"开始预览：{set.ip}");
                    break;
                case Model.categoryEnum.uniview:
                    uniViewHandler.Login(set.ip, set.port, set.username, set.password);
                    uniViewHandler.StartRealPlay(this.pictureBox1);
                    this.Handler_Msg($"开始预览：{set.ip}");
                    break;
                default:
                    this.Handler_Msg($"不支持的设备类型");
                    break;
            }
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WINDOWSMSG.WM_COPYDATA:
                    try
                    {
                        var copyData = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                        if (!string.IsNullOrEmpty(copyData.lpData))
                        {
                            if (this.VideoSetString == copyData.lpData)
                                return;
                            this.VideoSetString = copyData.lpData;
                            ShowVideo();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Handler_Msg(ex.Message);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }


        }
    }
}
