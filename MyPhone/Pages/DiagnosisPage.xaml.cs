﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace GoodTimeStudio.MyPhone.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DiagnosisPage : Page
    {
        public DiagnosisPageViewModel ViewModel => (DiagnosisPageViewModel)DataContext;

        public DiagnosisPage()
        {
            InitializeComponent();
            DataContext = new DiagnosisPageViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.OnNavigatedTo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.OnNavigatedFrom();
        }
    }
}
