using CommunityToolkit.Mvvm.ComponentModel;

namespace GoodTimeStudio.MyPhone.Models
{
    public partial class DeviceServiceProviderInformation : ObservableObject
    {
        [ObservableProperty]
        private DeviceServiceProviderState _state;

        [ObservableProperty]
        private string? _statusMessage;
    }
}
