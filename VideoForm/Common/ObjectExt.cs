using System.Text;

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
