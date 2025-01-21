using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoForm.Model
{
    public class Conf
    {
        //singleton pattern
        private static Conf instance = null;
        private Conf() { }
        public static Conf Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Conf();
                }
                return instance;
            }
        }
        public ConfItem Item { get; set; }

        public void Create()
        {
            //获取当前程序的路径
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            //生成配置文件
            string confPath = System.IO.Path.Combine(path, "conf.ini");
            File.WriteAllText(confPath, VideoForm.Properties.Resources.conf_ini, Encoding.UTF8);
        }
        public void Init()
        {
            //获取当前程序的路径
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            //生成配置文件
            string confPath = System.IO.Path.Combine(path, "conf.ini");
            if (!File.Exists(confPath))
                return;
            Item = new ConfItem();
            string[] lines = File.ReadAllLines(confPath, Encoding.UTF8).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            foreach (string line in lines)
            {
                if (line.StartsWith("#"))
                    continue;
                string[] items = line.Split('=');
                if (items.Length < 2)
                    continue;
                string key = items[0].Trim();
                string valueStr = items[1].Trim();
                string value = Regex.Match(valueStr, @"^\d+(,\d+)*").Value;
                if (string.IsNullOrEmpty(value))
                    continue;
                switch (key)
                {
                    case "APP_StartPosition":
                        Item.APP_StartPosition = int.Parse(value);
                        break;
                    case "APP_BorderStyle":
                        Item.APP_BorderStyle = int.Parse(value);
                        break;
                    case "APP_Size":
                        Item.APP_Size = value;
                        break;
                    case "HK_Channel":
                        Item.HK_Channel = int.Parse(value);
                        break;
                    case "HK_StreamType":
                        Item.HK_StreamType = int.Parse(value);
                        break;
                    case "HK_LinkMode":
                        Item.HK_LinkMode = int.Parse(value);
                        break;
                    case "UV_Channel":
                        Item.UV_Channel = int.Parse(value);
                        break;
                    case "UV_StreamType":
                        Item.UV_StreamType = int.Parse(value);
                        break;
                    case "UV_LinkMode":
                        Item.UV_LinkMode = int.Parse(value);
                        break;
                }
            }
        }
    }
    public class ConfItem
    {
        #region Common
        /// <summary>
        /// 0：系统默认位置 1:屏幕中央 2:自动位置
        /// </summary>
        public int APP_StartPosition  { get; set; } = 0;
        /// <summary>
        ///边框样式：0-可调整，1-不可以调整
        /// </summary>
        public int APP_BorderStyle { get; set; } = 0;
        /// <summary>
        /// 窗口大小
        /// </summary>
        public string APP_Size { get; set; } = "480,360";//外边框 "496,421";

        public Size GetSize()
        {
            string[] sizes = APP_Size.Split(',');
            if (sizes.Length < 2)
                return new Size(480+16, 360+61);
            int width = int.Parse(sizes[0]);
            int height = int.Parse(sizes[1]);
            if (width<480)
                width = 480;
            if (height < 360)
                height = 360;
            return new Size(width+16, height+61);
        }
        #endregion
        #region HK
        /// <summary>
        /// 通道号
        /// </summary>
        public int HK_Channel { get; set; } = -1;
        /// <summary>
        /// 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
        /// </summary>
        public int HK_StreamType { get; set; } = -1;
        /// <summary>
        /// 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
        /// </summary>
        public int HK_LinkMode { get; set; } = -1;
        #endregion

        #region UV
        /// <summary>
        /// //通道号
        /// </summary>
        public int UV_Channel { get; set; } = -1;
        /// <summary>
        /// //0主码流，1子码流
        /// </summary>
        public int UV_StreamType { get; set; } = -1;
        /// <summary>
        /// 0:udp,1:tcp
        /// </summary>
        public int UV_LinkMode { get;  set; }=-1;
        #endregion
    }

}
