using System;

namespace GoodTimeStudio.MyPhone.Utilities
{
    public static class LongExtensions
    {
        public static string ToHexString(this long l) 
        {
            return Convert.ToHexString(BitConverter.GetBytes(l));
        }

        public static string ToHexString(this ulong l)
        {
            return Convert.ToHexString(BitConverter.GetBytes(l));
        }
    }
}
