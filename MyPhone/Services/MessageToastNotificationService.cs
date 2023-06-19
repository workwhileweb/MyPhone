﻿using CommunityToolkit.WinUI.Notifications;
using GoodTimeStudio.MyPhone.OBEX;
using Microsoft.Extensions.DependencyInjection;

namespace GoodTimeStudio.MyPhone.Services
{
    public class MessageToastNotificationService : IMessageNotificationService
    {
        public void ShowMessageNotification(BMessage message)
        {
            new ToastContentBuilder()
                .AddText(message.Sender.FormattedName)
                .AddText(message.Body)
                .Show();
        }
    }

    public static class MessageToastNotificationServiceExtensions
    {
        public static IServiceCollection AddMessageToastNotification(this IServiceCollection services)
        {
            services.AddTransient<IMessageNotificationService, MessageToastNotificationService>();
            return services;
        }
    }
}
