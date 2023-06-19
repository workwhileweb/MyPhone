using GoodTimeStudio.MyPhone.Helpers;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace GoodTimeStudio.MyPhone.DeviceTest
{
    public class TestPhoneLineTransportHelper : IAssemblyFixture<BluetoothDeviceFixture>
    {
        private readonly BluetoothDeviceFixture _bthFixture;

        public TestPhoneLineTransportHelper(BluetoothDeviceFixture fixture)
        {
            _bthFixture = fixture;
        }

        [Fact]
        public async Task TestGetPhoneLineTransportFromBluetoothDevice()
        {
            using (var device = await BluetoothDevice.FromIdAsync(_bthFixture.BluetoothDeviceId))
            {
                var phoneLine = await PhoneLineTransportHelper.GetPhoneLineTransportFromBluetoothDevice(device);
                Assert.NotNull(phoneLine);

                var phoneLineDevInfo = await DeviceInformation.CreateFromIdAsync(phoneLine!.DeviceId);
                Assert.Equal(device.Name, phoneLineDevInfo.Name);
                Console.WriteLine($"Corresponding PhoneLineTransportDevice: {phoneLineDevInfo.Id}");
            }
        }


    }
}
