using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rover2020MobileController.Services;
using Rover2020MobileController.Views;

namespace Rover2020MobileController
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
