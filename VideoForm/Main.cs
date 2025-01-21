using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoForm.Common;
using VideoForm.Handler;
using VideoForm.Model;

namespace VideoForm
{
    public partial class Main : Form
    {
        public HKHandler hkHandler = null;
        public UniViewHandler uniViewHandler = null;
        public Main()
        {
            InitializeComponent();
            try
            {
                hkHandler = new HKHandler();//海康
                hkHandler.Msg += Handler_Msg;

                uniViewHandler = new UniViewHandler();//宇视
                uniViewHandler.Msg += Handler_Msg;
            }
            catch (Exception ex)
            {
                Handler_Msg(ex.Message);
            }
            this.Shown += Form1_Shown;
            this.FormClosed += Form1_FormClosed;
            Model.Conf.Instance.Init();

            if (Conf.Instance.Item != null)
            {
                switch (Conf.Instance.Item.APP_StartPosition)
                {
                    case 0:
                        this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                        break;
                    case 1:
                        this.StartPosition = FormStartPosition.CenterScreen;
                        break;
                    case 2:
                        SetFormPosition();
                        break;
                }                  
                switch (Conf.Instance.Item.APP_BorderStyle)
                {
                    case 0:
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                        break;
                    case 1:
                        this.FormBorderStyle = FormBorderStyle.FixedSingle;
                        break;                    
                }
                this.Size = Conf.Instance.Item.GetSize();
            }
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
        private void SetFormPosition()
        {
            // 1. 获取屏幕工作区域 (除去任务栏等系统元素)
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // 2. 获取鼠标位置
            Point mousePoint = Cursor.Position;

            // 3. 计算窗口居中于鼠标的目标位置
            int targetLeft = mousePoint.X - this.Width / 2;
            int targetTop = mousePoint.Y - this.Height / 2;

            // 4. 边界检查： 检查窗口是否会超出屏幕工作区域
            // 检查左边界
            if (targetLeft < workingArea.Left)
            {
                targetLeft = workingArea.Left; //如果超出，则设置为屏幕工作区域左边界
            }
            // 检查右边界
            if (targetLeft + this.Width > workingArea.Right)
            {
                targetLeft = workingArea.Right - this.Width; // 如果超出，则将窗口左边界设置为屏幕工作区域右边界减去窗口宽度
            }
            // 检查上边界
            if (targetTop < workingArea.Top)
            {
                targetTop = workingArea.Top; // 如果超出，则将窗口顶部位置设置为屏幕工作区顶部
            }
            // 检查下边界
            if (targetTop + this.Height > workingArea.Bottom)
            {
                targetTop = workingArea.Bottom - this.Height; //如果超出设置为屏幕工作区域的底部减去窗口的高度
            }

            // 5. 设置窗口位置
            this.Location = new Point(targetLeft, targetTop);

            // 如果要默认显示在屏幕中央，则取消注释此部分代码, 此时上面的超出屏幕的校正判断会被忽略
            /* if ((targetLeft < workingArea.Left || (targetLeft + this.Width > workingArea.Right)) || (targetTop < workingArea.Top  || (targetTop + this.Height > workingArea.Bottom)))
            {
              //如果超出屏幕，则显示在屏幕中央
                 this.StartPosition = FormStartPosition.CenterScreen;
             } else {
                 this.Location = new Point(targetLeft, targetTop);
              }
            */
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
                    this.Handler_Msg($"开始预览：{set.ip}");
                    hkHandler.Preview(this.pictureBox1);
                    break;
                case Model.categoryEnum.uniview:
                    uniViewHandler.Login(set.ip, set.port, set.username, set.password);
                    this.Handler_Msg($"开始预览：{set.ip}");
                    uniViewHandler.StartRealPlay(this.pictureBox1);
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
