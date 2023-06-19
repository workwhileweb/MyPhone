using GoodTimeStudio.MyPhone.OBEX.Streams;
using System;
using System.Buffers.Binary;

namespace GoodTimeStudio.MyPhone.OBEX.Headers
{
    public class AppParameterHeaderInterpreter : IBufferContentInterpreter<AppParameterDictionary>
    {
        public AppParameterDictionary GetValue(ReadOnlySpan<byte> buffer)
        {
            AppParameterDictionary dict = new();
            for (var i = 0; i < buffer.Length;)
            {
                var tagId = buffer[i++];
                var len = BinaryPrimitives.ReverseEndianness(buffer[i++]);
                dict[tagId] = new AppParameter(tagId, buffer.Slice(i, len).ToArray());
                i += len;
            }
            return dict;
        }
    }
}
