using System.Buffers.Binary;

namespace GoodTimeStudio.MyPhone.OBEX.Extensions
{
    public static class IntExtensions
    {
        public static byte[] ToBigEndianBytes(this int i)
        {
            var ret = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32BigEndian(ret, i);
            return ret;
        }

        public static byte[] ToBigEndianBytes(this ushort us)
        {
            var ret = new byte[sizeof(ushort)];
            BinaryPrimitives.WriteUInt16BigEndian(ret, us);
            return ret;
        }

        public static byte[] ToBigEndianBytes(this byte b)
        {
            return new[] { BinaryPrimitives.ReverseEndianness(b) };
        }
    }
}
