using GoodTimeStudio.MyPhone.Device.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoodTimeStudio.MyPhone.Extensions
{
    public static class DeviceServiceExtensions
    {
        public static IServiceCollection AddEntityFrameworkMessageStore(this IServiceCollection services)
        {
            services.AddScoped<IMessageStore, EntityFrameworkMessageStore>();
            return services;
        }

        public static IServiceCollection AddEntityFrameworkContactStore(this IServiceCollection services)
        {
            services.AddScoped<IContactStore, EntityFrameworkContactStore>();
            return services;
        }

        public static IServiceCollection AddEntityFrameworkDeviceConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IDeviceConfiguration, EntityFrameworkDeviceConfiguration>();
            return services;
        }
    }
}
