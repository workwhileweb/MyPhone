using System.Collections.Generic;
using System.Linq;

namespace GoodTimeStudio.MyPhone.OBEX.Utilities
{
    public class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public static ByteArrayEqualityComparer Default { get; } = new();

        public bool Equals(byte[]? x, byte[]? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return Enumerable.SequenceEqual(x, y);
        }

        public int GetHashCode(byte[] obj)
        {
            unchecked
            {
                var result = 0;
                foreach (var b in obj)
                    result = (result * 31) ^ b;
                return result;
            }
        }
    }
}
