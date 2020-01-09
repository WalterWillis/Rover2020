using ControllerUI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;


namespace ControllerUI
{
    public partial class ControlPage : Window
    {
        private ControllerViewModel ViewModel { get; set; }

        public ControlPage()
        {
            InitializeComponent();

            ViewModel = new ControllerViewModel();
            DataContext = ViewModel;
            displayList.ItemsSource = ViewModel.DisplayVariants;
            displayList.SelectedIndex = 0;

            powerButton.Click += PowerOffDialog;
            upButton.Click += MoveForward;
            downButton.Click += MoveBackward;
            leftButton.Click += MoveLeft;
            rightButton.Click += MoveRight;

            speedIncreaseButton.Click += IncreaseSpeed;
            speedDecreaseButton.Click += DecreaseSpeed;
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

        private void Move(object sender, RoutedEventArgs e)
        {
            var keyword = (e.Source as Button).Content.ToString();
            Enum.TryParse(keyword, out ControllerViewModel.Direction direction);
            MessageBox.Show(keyword);
            ViewModel.Move(direction);
        }
        private void MoveForward(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(ControllerViewModel.Direction.FORWARD);
        }

        private void MoveBackward(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(ControllerViewModel.Direction.BACKWARD);
        }

        private void MoveLeft(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(ControllerViewModel.Direction.LEFT);
        }
        private void MoveRight(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(ControllerViewModel.Direction.RIGHT);
        }

        private void IncreaseSpeed(object sender, RoutedEventArgs e)
        {
            ViewModel.IncreaseSpeed();
        }

        private void DecreaseSpeed(object sender, RoutedEventArgs e)
        {
            ViewModel.DecreaseSpeed();
        }
    }

}
