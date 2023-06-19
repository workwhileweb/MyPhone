using GoodTimeStudio.MyPhone.OBEX.Headers;

namespace GoodTimeStudio.MyPhone.OBEX.UnitTest.Headers
{
    public class TestAppParameterHeader
    {
        [Fact]
        public void TestBuilderAndInterpreter()
        {
            var originalAppParameter = new AppParameter(0x01, new byte[] { 0x01, 0x02, 0x03, 0x04 });
            AppParameterHeaderBuilder builder = new(originalAppParameter);
            var header = builder.Build();
            
            var dict = header.GetValueAsAppParameters();
            var appParameter = dict[0x01];
            Assert.Equal(originalAppParameter, appParameter);
        }
    }
}
