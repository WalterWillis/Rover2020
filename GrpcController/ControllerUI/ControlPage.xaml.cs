using ControllerUI.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows;
using System.Windows.Controls;


namespace ControllerUI
{
    public partial class ControlPage : Window
    {
        private ControllerViewModel ViewModel { get; set; }

        public ControlPage(IConfiguration configuration)
        {
            InitializeComponent();

            ViewModel = new ControllerViewModel(configuration);
            DataContext = ViewModel;
            displayList.ItemsSource = ViewModel.DisplayVariants;
            //default the selected item to the first
            displayList.SelectedIndex = 0;

            powerButton.Click += PowerOffDialog;
            upButton.Click += (sender, e) => { ViewModel.Move(ControllerViewModel.Direction.FORWARD); };
            downButton.Click += (sender, e) => { ViewModel.Move(ControllerViewModel.Direction.BACKWARD); };
            leftButton.Click += (sender, e) => { ViewModel.Move(ControllerViewModel.Direction.LEFT); };
            stopButton.Click += (sender, e) => { ViewModel.Move(ControllerViewModel.Direction.STOP); };
            rightButton.Click += (sender, e) => { ViewModel.Move(ControllerViewModel.Direction.RIGHT); };

            speedIncreaseButton.Click += (sender, e) => { ViewModel.IncreaseSpeed(); };
            speedDecreaseButton.Click += (sender, e) => { ViewModel.DecreaseSpeed(); };
        }
        private async void PowerOffDialog(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Are you sure you want to power down the rover?";
            string caption = "Device Shutdown";

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result.Equals(MessageBoxResult.Yes))
            {
                await ViewModel.PowerOffDevice();
            }
        }
    }

}
