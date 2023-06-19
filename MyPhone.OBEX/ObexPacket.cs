using GoodTimeStudio.MyPhone.OBEX.Headers;
using GoodTimeStudio.MyPhone.OBEX.Streams;
using GoodTimeStudio.MyPhone.OBEX.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace GoodTimeStudio.MyPhone.OBEX
{
    public class ObexPacket
    {
        public ObexOpcode Opcode { get; set; }

        /// <summary>
        /// Will only be updated after calling ToBuffer()
        /// </summary>
        public ushort PacketLength { get; private set; }

        public Dictionary<HeaderId, ObexHeader> Headers;

        public byte[] BodyBuffer { 
            get => _bodyBuffer ?? throw new ObexException("Body does not exists");
            set => _bodyBuffer = value;
        }
        private byte[]? _bodyBuffer;

        public ObexPacket() : this(new ObexOpcode(ObexOperation.Abort, true)) { }

        public ObexPacket(ObexOpcode op)
        {
            Opcode = op;
            Headers = new Dictionary<HeaderId, ObexHeader>();
        }

        public ObexPacket(ObexOpcode op, params ObexHeader[] headers) : this(op)
        {
            foreach (var h in headers)
            {
                Headers[h.HeaderId] = h;
            }
        }

        public ObexHeader GetHeader(HeaderId headerId)
        {
            if (Headers.TryGetValue(headerId, out var header))
            {
                return header;
            }

            throw new ObexHeaderNotFoundException(headerId);
        }

        public T GetBodyContent<T>(IBufferContentInterpreter<T> interpreter)
        {
            return interpreter.GetValue(BodyBuffer);
        }

        public string GetBodyContentAsUtf8String(bool stringIsNullTerminated)
        {
            return GetBodyContent(new StringInterpreter(System.Text.Encoding.UTF8, stringIsNullTerminated));
        }

        public string GetBodyContentAsUnicodeString(bool stringIsNullTerminated)
        {
            return GetBodyContent(new StringInterpreter(System.Text.Encoding.BigEndianUnicode, stringIsNullTerminated));
        }

        protected virtual void WriteExtraField(DataWriter writer) { }

        /// <summary>
        /// Read extra field after the length field
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>The number of bits required for extra bits</returns>
        protected virtual Task<uint> ReadExtraField(DataReader reader)
        {
            return Task.FromResult<uint>(0);
        }

        public IBuffer ToBuffer()
        {
            using (var writer = new DataWriter())
            using (var exFieldAndHeaderWriter = new DataWriter())
            {
                WriteExtraField(exFieldAndHeaderWriter);

                foreach (var header in Headers.Values)
                {
                    header.WriteToStream(exFieldAndHeaderWriter);
                }

                var exFieldAndHeaderBuffer = exFieldAndHeaderWriter.DetachBuffer();

                writer.WriteByte(Opcode.Value);
                PacketLength = (ushort)(exFieldAndHeaderBuffer.Length + sizeof(ObexOperation) + sizeof(ushort));
                writer.WriteUInt16(PacketLength);
                writer.WriteBuffer(exFieldAndHeaderBuffer);

                return writer.DetachBuffer();
            }
        }

        private async Task ParseHeader(DataReader reader, uint headerSize)
        {
            if (headerSize <= 0)
            {
                Console.WriteLine("Header size to read is zero.");
                return;
            }

            var sizeToRead = headerSize;
            while (sizeToRead > 0)
            {
                var loaded = await reader.LoadAsync(headerSize);
                if (loaded == 0)
                {
                    throw new ObexException("The underlying socket was closed before we were able to read the whole data.");
                }
                sizeToRead -= loaded;
            }

            while (reader.UnconsumedBufferLength > 0)
            {
                var header = ObexHeader.ReadFromStream(reader);
                Headers[header.HeaderId] = header;
            }
        }

        public static Task<ObexPacket> ReadFromStream(DataReader reader)
        {
            return ReadFromStream<ObexPacket>(reader);
        }

        /// <summary>
        /// Read and parse OBEX packet from DataReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="packet">Optional, if this parameter is not null, the data will be read into this parameter</param>
        /// <returns>Loaded OBEX packet</returns>
        public static async Task<T> ReadFromStream<T>(DataReader reader) where T : ObexPacket, new()
        {
            var loaded = await reader.LoadAsync(1);
            if (loaded != 1)
            {
                throw new ObexException("The underlying socket was closed before we were able to read the whole data.");
            }

            ObexOpcode opcode = new (reader.ReadByte());
            var packet = new T();
            packet.Opcode = opcode;

            Console.WriteLine($"ReadFromStream:: Opcode: {packet.Opcode}");

            loaded = await reader.LoadAsync(sizeof(ushort));
            if (loaded != sizeof(ushort))
            {
                throw new ObexException("The underlying socket was closed before we were able to read the whole data.");
            }

            packet.PacketLength = reader.ReadUInt16();
            Console.WriteLine($"packet length: {packet.PacketLength}");

            var extraFieldBits = await packet.ReadExtraField(reader);
            var size = packet.PacketLength - (uint)sizeof(ObexOperation) - sizeof(ushort) - extraFieldBits;
            await packet.ParseHeader(reader, size);
            return packet;
        }

        public override bool Equals(object? obj)
        {
            return obj is ObexPacket packet &&
                   EqualityComparer<ObexOpcode>.Default.Equals(Opcode, packet.Opcode) &&
                   PacketLength == packet.PacketLength &&
                   DictionaryEqualityComparer<HeaderId, ObexHeader>.Default.Equals(Headers, packet.Headers);
        }

        public override int GetHashCode()
        {
            var hashCode = 467068145;
            hashCode = hashCode * -1521134295 + EqualityComparer<ObexOpcode>.Default.GetHashCode(Opcode);
            hashCode = hashCode * -1521134295 + PacketLength.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<HeaderId, ObexHeader>>.Default.GetHashCode(Headers);
            return hashCode;
        }
    }
}
