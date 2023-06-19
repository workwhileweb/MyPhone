﻿using Microsoft.Extensions.DependencyInjection;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace GoodTimeStudio.MyPhone.Services
{
    /// <summary>
    /// Packaged app local settings implementation based on <see cref="ApplicationData"/>
    /// </summary>
    /// <remarks>
    /// This implementation requires package identity
    /// </remarks>
    public sealed class AppDataLocalSettingsService : ISettingsService
    {

        /// <summary>
        /// The <see cref="IPropertySet"/> with the settings targeted by the current instance.
        /// </summary>
        private readonly IPropertySet SettingsStorage = ApplicationData.Current.LocalSettings.Values;

        public T? GetValue<T>(string key)
        {
            if (SettingsStorage.TryGetValue(key, out var value))
            {
                return (T)value!;
            }

            return default;
        }

        public void SetValue<T>(string key, T? value)
        {
            if (!SettingsStorage.ContainsKey(key))
            {
                SettingsStorage.Add(key, value);
            }
            else
            {
                SettingsStorage[key] = value;
            }
        }
    }

    public static class AppDataLocalSettingsServiceExtensions
    {
        public static IServiceCollection AddAppDataLocalSettings(this IServiceCollection services)
        {
            services.AddTransient<ISettingsService, AppDataLocalSettingsService>();
            return services;
        }
    }
}
