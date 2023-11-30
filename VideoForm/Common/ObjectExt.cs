using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoForm.Common
{
    public static class ObjectExt
    {

        public static byte[] ToBytes(this string t)
        {
            return Encoding.Default.GetBytes(t);
        }
    }
}
