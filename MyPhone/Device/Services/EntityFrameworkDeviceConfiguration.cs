using GoodTimeStudio.MyPhone.Data;
using System;
using System.Linq;

namespace GoodTimeStudio.MyPhone.Device.Services
{
    public class EntityFrameworkDeviceConfiguration : IDeviceConfiguration
    {
        private readonly DeviceDbContext _context;

        public EntityFrameworkDeviceConfiguration(DeviceDbContext context)
        {
            _context = context;
        }

        private DeviceConfiguration EnsureFirstEntryExists()
        {
            var configuration = _context.Configurations.FirstOrDefault();
            if (configuration == null)
            {
                configuration = new DeviceConfiguration();
                _context.Configurations.Add(configuration);
                _context.SaveChanges();
            }
            return configuration;
        }

        public string? DeviceId 
        { 
            get => _context.Configurations.FirstOrDefault()?.DeviceId;
            set
            {
                var configuration = EnsureFirstEntryExists();
                if (configuration.DeviceId != value)
                {
                    configuration.DeviceId = value;
                    _context.SaveChanges();
                }
            }
        }

        public DateTime? SmsServiceLastSyncedTime 
        {
            get => _context.Configurations.FirstOrDefault()?.SmsServiceLastSyncedTime;
            set
            {
                var configuration = EnsureFirstEntryExists();
                if (configuration.SmsServiceLastSyncedTime != value)
                {
                    configuration.SmsServiceLastSyncedTime = value;
                    _context.SaveChanges();
                }
            } 
        }

        public DateTime? PhonebookServiceSyncedTime 
        {
            get => _context.Configurations.FirstOrDefault()?.PhonebookServiceLastSyncedTime;
            set
            {
                var configuration = EnsureFirstEntryExists();
                if (configuration.PhonebookServiceLastSyncedTime != value)
                {
                    configuration.PhonebookServiceLastSyncedTime = value;
                    _context.SaveChanges();
                }
            } 
        }

        public TimeSpan? SyncTimeSpan 
        {
            get => _context.Configurations.FirstOrDefault()?.SyncTimeSpan;
            set
            {
                var configuration = EnsureFirstEntryExists();
                if (configuration.SyncTimeSpan != value)
                {
                    configuration.SyncTimeSpan = value;
                    _context.SaveChanges();
                }
            } 
        }
    }
}
