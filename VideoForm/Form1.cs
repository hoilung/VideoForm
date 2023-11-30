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
    public partial class Form1 : Form
    {
        public HKHandler hkHandler = null;
        public Form1()
        {
            InitializeComponent();
            hkHandler = new HKHandler();
            this.Shown += Form1_Shown;
            this.FormClosed += Form1_FormClosed;

            //this.button1.Click += Button1_Click;

            //File.WriteAllText("h.txt", this.Handle.ToString());

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

            hkHandler.Login(set.ip, set.port, set.username, set.password);

            hkHandler.Preview(this.pictureBox1);
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
                            this.VideoSetString = copyData.lpData;
                            if (hkHandler != null)
                            {
                                hkHandler.LogOut();
                            }
                            ShowVideo();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }


        }
    }
}
