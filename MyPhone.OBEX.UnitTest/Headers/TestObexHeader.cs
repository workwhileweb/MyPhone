using GoodTimeStudio.MyPhone.OBEX.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Storage.Streams;

namespace GoodTimeStudio.MyPhone.OBEX.UnitTest.Headers
{
    public class TestObexHeader
    {
        [Theory]
        [InlineData(HeaderId.ConnectionId, ObexHeaderEncoding.FourByteQuantity)]
        [InlineData(HeaderId.Name, ObexHeaderEncoding.UnicodeString)]
        [InlineData(HeaderId.Body, ObexHeaderEncoding.ByteSequence)]
        [InlineData(HeaderId.SingleResponseMode, ObexHeaderEncoding.OneByteQuantity)]
        public void TestHeaderEncoding(HeaderId headerId, ObexHeaderEncoding expectedEncoding)
        {
            var header = new ObexHeader(headerId, 1);
            Assert.Equal(expectedEncoding, header.Encoding);
        }

        [Fact]
        public void TestBuiltinHeaderInterpreter_UnicodeString()
        {
            const string originalString = "Foobar";
            var header = new ObexHeader(HeaderId.Name, originalString, true, Encoding.BigEndianUnicode);
            Assert.Equal(originalString, header.GetValueAsUnicodeString(true));
        }

        [Fact]
        public void TestBuiltinHeaderInterpreter_Utf8String()
        {
            const string originalString = "Foobar";
            var header = new ObexHeader(HeaderId.Name, originalString, false, Encoding.UTF8);
            Assert.Equal(originalString, header.GetValueAsUtf8String(false));
        }

        [Fact]
        public void TestEquals()
        {
            var headerA = new ObexHeader(HeaderId.Body, 32);
            var headerB = new ObexHeader(HeaderId.Body, 32);
            Assert.Equal(headerA, headerB);

            DataWriter writer = new();
            writer.WriteByte(1);
            writer.WriteByte(2);
            headerA = new ObexHeader(HeaderId.Body, writer.DetachBuffer().ToArray());
            writer.Dispose();
            headerB = new ObexHeader(HeaderId.Body, new byte[] { 1, 2 });
            Assert.Equal(headerA, headerB);
        }
    }
}
