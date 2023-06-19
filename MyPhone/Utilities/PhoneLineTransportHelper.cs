﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace GoodTimeStudio.MyPhone.Helpers
{
    public class PhoneLineTransportHelper
    {
        /// <summary>
        /// Retrieve a corresponding <see cref="PhoneLineTransportDevice"/> object from a <see cref="BluetoothDevice"/>
        /// </summary>
        /// <param name="bt">the bluetooth device</param>
        /// <returns>
        /// Return <see cref="PhoneLineTransportDevice"/> if the given <see cref="BluetoothDevice"/> support Hands-Free Profile. 
        /// Otherwise, return null
        /// </returns>
        public static async Task<PhoneLineTransportDevice?> GetPhoneLineTransportFromBluetoothDevice(BluetoothDevice bt)
        {
            // Check SDP first
            var result = await bt.GetRfcommServicesAsync();
            if (result.Error != BluetoothError.Success)
            {
                // Can not get any services from SDP
                return null;
            }

            var hfp = result.Services.Where(svc => svc.ServiceId.Uuid == BluetoothServiceUuid.HandsFreeProfileUuid).ToList();
            if (hfp.Count == 0)
            {
                return null;
            }

            const string deviceInterfaceBluetoothAddressKey = "System.DeviceInterface.Bluetooth.DeviceAddress";
            var phoneLineDevsInfo = await DeviceInformation.FindAllAsync(PhoneLineTransportDevice.GetDeviceSelector(), new[] { deviceInterfaceBluetoothAddressKey });
            var matchPhoneLineDevInfo = phoneLineDevsInfo.Where(dev =>
            {
                var phoneLineDevAddress = (string?)dev.Properties[deviceInterfaceBluetoothAddressKey];
                if (ulong.TryParse(phoneLineDevAddress, NumberStyles.HexNumber, null, out var address))
                {
                    return address == bt.BluetoothAddress;
                }

                return false;
            }).FirstOrDefault();

            return matchPhoneLineDevInfo == null ? null : PhoneLineTransportDevice.FromId(matchPhoneLineDevInfo.Id);
        }
    }
}