using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoForm.Model
{
    public enum categoryEnum
    {
        none,     
        /// <summary>
        /// hikvision://admin:xxct111111@192.168.1.64:8000
        /// </summary>
        hikvision,
        /// <summary>
        /// uniview://admin:xxct111111@192.168.1.64:80
        /// </summary>
        uniview,
        /// <summary>
        /// file://C:\\Users\\Administrator\\Desktop\\test.mp4
        /// </summary>
        file,
        /// <summary>
        /// rtmp://192.168.1.64/live/stream
        /// </summary>        
        rtmp,
        /// <summary>
        /// rtsp://192.168.1.64:554/live/stream
        /// </summary>
        rtsp,
        /// <summary>
        /// <para>
        /// http://192.168.1.64:8000/live/stream.m3u8
        /// </para>
        /// http://192.168.1.64:8000/mjpg/video.mjpg
        /// </summary>
        http,
    }

    public class VideoSet
    {
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string ip { get; set; }

        public string playUrl { get; set; }
        /// <summary>
        /// hikvision/uniview
        /// 海康威视/宇视
        /// </summary>
        public categoryEnum category { get; set; } = categoryEnum.none;

        /// <summary>
        /// 192.168.1.64:8000|admin|xxct111111
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static VideoSet Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            if (!str.Contains("://"))
                return null;
            VideoSet set = null;

            try
            {
                var uri = new Uri(str);
                var userinfo = uri.UserInfo.Split(':');
                var ip = uri.Host;
                var port = uri.Port;
                var username = userinfo.Length > 0 ? userinfo[0] : string.Empty;
                var password = userinfo.Length > 1 ? userinfo[1] : string.Empty;
                switch (uri.Scheme)
                {
                    case "hikvision":
                        set = new VideoSet() { playUrl = str, category = categoryEnum.hikvision };
                        break;
                    case "uniview":
                        set = new VideoSet() { playUrl = str, category = categoryEnum.uniview };
                        break;
                    case "rtmp":
                        set = new VideoSet() { playUrl = str, category = categoryEnum.rtmp };
                        break;
                    case "rtsp":
                        set = new VideoSet() { playUrl = str, category = categoryEnum.rtsp };
                        break;
                    case "http":
                        set = new VideoSet() { playUrl = str, category = categoryEnum.http };
                        break;
                    case "file":
                        if (File.Exists(uri.LocalPath))
                            set = new VideoSet() { playUrl = uri.LocalPath, category = categoryEnum.file };
                        break;
                    default:
                        break;
                }
                if (set != null)
                {
                    set.ip = ip;
                    set.port = port;
                    set.username = username;
                    set.password = password;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("解析视频地址失败：" + ex.Message);
            }
            return set;
        }
    }
}
