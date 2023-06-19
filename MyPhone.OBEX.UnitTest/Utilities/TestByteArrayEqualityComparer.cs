namespace GoodTimeStudio.MyPhone.OBEX.UnitTest.Utilities
{
    public class TestByteArrayEqualityComparer
    {
        [Fact]
        public void TestEquals()
        {
            var a = new byte[] { 1, 2, 3 };
            var b = new byte[] { 1, 2, 3 };
            Assert.Equal(a, b);
        }

        [Fact]
        public void TestEquals_LargeArray()
        {
            var a = new byte[65535];
            Random.Shared.NextBytes(a);
            var b = new byte[65535];
            a.CopyTo(b, 0);
            Assert.Equal(a, b);
        }
    }
}
