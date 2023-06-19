﻿using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace GoodTimeStudio.MyPhone.OBEX.Utilities
{
    public class WindowsRuntimeBufferEqualityComparer : IEqualityComparer<IBuffer>
    {
        public static WindowsRuntimeBufferEqualityComparer Default { get; } = new();

        public bool Equals(IBuffer? x, IBuffer? y)
        {
            if (x == y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return ByteArrayEqualityComparer.Default.Equals(x.ToArray(), y.ToArray());
        }

        public int GetHashCode(IBuffer obj)
        {
            return ByteArrayEqualityComparer.Default.GetHashCode(obj.ToArray());
        }
    }
}
