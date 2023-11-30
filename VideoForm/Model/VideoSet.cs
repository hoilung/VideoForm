﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoForm.Model
{
    public class VideoSet
    {
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string ip { get; set; }

        /// <summary>
        /// 192.168.1.64:8000|admin|xxct111111
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static VideoSet Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var array = str.Split('|');
            if (array.Length == 3 && array[0].Contains(":"))
            {
                try
                {
                    VideoSet videoSet = new VideoSet();
                    var kv = array[0].Split(':');
                    videoSet.ip = kv[0];
                    videoSet.port = int.Parse(kv[1]);
                    videoSet.username = array[1];
                    videoSet.password = array[2];
                    return videoSet;
                }
                catch (Exception ex)
                {

                }
            }

            return null;
        }
    }
}
