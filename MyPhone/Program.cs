using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoodTimeStudio.MyPhone
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();

            // As of Windows App SDK 1.1.4, async main will causes Clipboard API not working
            // https://docs.microsoft.com/en-us/answers/questions/780749/winui-3-clipboard-api-not-working.html
            var isRedirect = DecideRedirect().Result;

            if (!isRedirect)
            {
                Launch();
            }
        }

        private static void Launch()
        {
            Microsoft.UI.Xaml.Application.Start(p =>
            {
                DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = new App();
            });
        }

        // Single-instance redirect, redirect Activated to the main instance 
        private static async Task<bool> DecideRedirect()
        {
            var isRedirect = false;
            var mainInstance = AppInstance.FindOrRegisterForKey("main");
            var currentInstance = AppInstance.GetCurrent();
            var activationArgs = currentInstance.GetActivatedEventArgs();
            if (mainInstance.IsCurrent)
            {

            }
            else
            {
                isRedirect = true;
                await mainInstance.RedirectActivationToAsync(activationArgs);
            }

            return isRedirect;
        }
    }
}
