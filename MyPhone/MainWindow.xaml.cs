﻿using GoodTimeStudio.MyPhone.RootPages;
using GoodTimeStudio.MyPhone.Services;
using GoodTimeStudio.MyPhone.Utilities;
using H.NotifyIcon;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.Win32;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GoodTimeStudio.MyPhone
{
    /// <summary>
    /// The main window of My Phone. Only one instance will be created.
    /// </summary>
    public sealed partial class MainWindow : Window, IDisposable
    {
        public static XamlRoot XamlRoot => _instance.windowRoot.XamlRoot;
        public static DispatcherQueue WindowDispatcher => _instance.DispatcherQueue;
        public static string AppTitleDisplayName => ResourceLoader.GetForViewIndependentUse("Resources").GetString("AppDisplayName");

        private readonly ISettingsService _settingsService;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static MainWindow _instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private bool _oobeCompleted;
        private IntPtr hWnd;

        private readonly WindowsSystemDispatcherQueueHelper _wsdqHelper;
        private ISystemBackdropControllerWithTargets? _controller;
        private SystemBackdropConfiguration? _configuration;

        public MainWindow(ISettingsService settingsService) : base()
        {
            InitializeComponent();
            _instance = this;
            _settingsService = settingsService;
            _oobeCompleted = settingsService.GetValue<bool>(settingsService.KeyOobeIsCompleted);
            Title = AppTitleDisplayName;

            // Hide default title bar.
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar); // Set XAML element as a draggable region.

            //Register a handler for when the window changes focus
            Activated += MainWindow_Activated;

            // AppWindow Interop
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Closing += AppWindow_Closing;

            // Resize the window 
            var dpi = PInvoke.GetDpiForWindow((Windows.Win32.Foundation.HWND)hWnd);
            var factor = dpi / 96d;
            appWindow.Resize(new Windows.Graphics.SizeInt32(Convert.ToInt32(1200 * factor), Convert.ToInt32(800 * factor)));

            // Set system backdrop (Mica / Acrylic)
            _wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            if (TrySetWindowBackdrop())
            {
                ((FrameworkElement)Content).ActualThemeChanged += MainWindow_ActualThemeChanged;
                windowRoot.Background = new SolidColorBrush() { Opacity = 0 };
            }


            if (_oobeCompleted && App.Current.DeviceManager != null)
            {
                rootFrame.Navigate(typeof(MainPage));
            }
            else
            {
                rootFrame.Navigate(typeof(OobePage));
            }
        }

        /// <summary>
        /// Attempt to enable window backdrop
        /// </summary>
        /// <returns>Turn if success. False if window backdrop is not supported in current environment </returns>
        private bool TrySetWindowBackdrop()
        {
            if (MicaController.IsSupported())
            {
                SetSystemBackdropController<MicaController>();
                return true;
            }
            if (DesktopAcrylicController.IsSupported())
            {
                SetSystemBackdropController<DesktopAcrylicController>();
                return true;
            }
            return false;
        }

        private void SetSystemBackdropController<TSystemBackdropController>() where TSystemBackdropController : ISystemBackdropControllerWithTargets, new()
        {
            _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            _configuration = new SystemBackdropConfiguration
            {
                IsInputActive = true,
            };
            SetConfigurationSourceTheme();

            _controller = new TSystemBackdropController();
            _controller.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
            _controller.SetSystemBackdropConfiguration(_configuration);
        }

        private void SetConfigurationSourceTheme()
        {
            Debug.Assert(_configuration != null);

            switch (((FrameworkElement)Content).ActualTheme)
            {
                case ElementTheme.Dark: _configuration.Theme = SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: _configuration.Theme = SystemBackdropTheme.Light; break;
                case ElementTheme.Default: _configuration.Theme = SystemBackdropTheme.Default; break;
            }
        }

        private void MainWindow_ActualThemeChanged(FrameworkElement sender, object args)
        {
            if (_configuration != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            #region Handle ExtendsContentIntoTitleBar
            var defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["WindowCaptionForeground"];
            var inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["WindowCaptionForegroundDisabled"];

            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
            #endregion

            #region Handle SystemBackgrop
            if (_configuration != null)
            {
                _configuration.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
            }
            #endregion
        }

        private void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            this.Hide(enableEfficiencyMode: true);
            args.Cancel = true;
        }

        private void OpenAppCommand_ExecuteRequested(Microsoft.UI.Xaml.Input.XamlUICommand sender, Microsoft.UI.Xaml.Input.ExecuteRequestedEventArgs args)
        {
            this.Show(disableEfficiencyMode: true);
            PInvoke.SetForegroundWindow((Windows.Win32.Foundation.HWND)hWnd);
        }

        private void ExitCommand_ExecuteRequested(Microsoft.UI.Xaml.Input.XamlUICommand sender, Microsoft.UI.Xaml.Input.ExecuteRequestedEventArgs args)
        {
            Close();
        }

        public void Dispose()
        {
            if (_controller != null)
            {
                _controller.Dispose();
            }
        }
    }
}
