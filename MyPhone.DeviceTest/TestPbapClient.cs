using GoodTimeStudio.MyPhone.OBEX.Bluetooth;
using GoodTimeStudio.MyPhone.OBEX.Pbap;

namespace GoodTimeStudio.MyPhone.DeviceTest
{
    public class TestPbapClient : BaseTestObexClient<PbapClient>
    {
        public TestPbapClient(BluetoothDeviceFixture fixture) : base(fixture)
        {
        }

        public override BluetoothObexClientSession<PbapClient> CreateSession()
        {
            return new BluetoothPbapClientSession(Device);
        }

        [Fact]
        public async Task TestGetAllContacts()
        {
            var contacts = (await Session.ObexClient!.GetAllContactsAsync()).ToList();
            Assert.True(contacts.Count > 0);
        }
    }
}
