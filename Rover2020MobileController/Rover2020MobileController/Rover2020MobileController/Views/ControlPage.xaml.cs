using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rover2020MobileController.ViewModels;
using System.Collections.Generic;

namespace Rover2020MobileController.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControlPage : ContentPage
    {
        ControllerViewModel viewModel;

        //In case we need to modify functionality on the fly in the future
        #region Commands        
        private Command Forward { get; set; }
        private Command Backward { get; set; }
        private Command Left { get; set; }
        private Command Right { get; set; }
        private Command IncreaseSpeed { get; set; }
        private Command DecreaseSpeed { get; set; }
        #endregion

        public ControlPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ControllerViewModel();            
            displayPicker.ItemsSource = viewModel.DisplayVariants;

            //Can have multiple recognizers. One tap will cause an animation, two will trigger an action.
            var tapGestureAnimation = new TapGestureRecognizer();
            var tapGestureTrigger = new TapGestureRecognizer();

            //https://forums.xamarin.com/discussion/155965/make-the-text-of-a-label-fill-up-the-container-auto-font-size
            tapGestureAnimation.Tapped += async (s, e) =>
            {
                // more info about animations:
                // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/animation/simple

                // scale the frame to x1.2
                var scaleUpAnimationTask = powerButtonFrame.ScaleTo(1.2, 500);
                // set opacity to 0 (transparent)
                var fadeOutAnimationTask = powerButtonFrame.FadeTo(0, 500);

                // wait for the 2 animations to finish...
                await Task.WhenAll(scaleUpAnimationTask, fadeOutAnimationTask);

                // scale the frame back to original size
                var scaleDownAnimationTask = powerButtonFrame.ScaleTo(1, 500);
                // set opacity back to 1 (solid)
                var fadeInAnimationTask = powerButtonFrame.FadeTo(1, 500);

                // wait for the 2 animations to finish...
                await Task.WhenAll(scaleDownAnimationTask, fadeInAnimationTask);
            };

            tapGestureTrigger.NumberOfTapsRequired = 2;
            tapGestureTrigger.Command = new Command(async () => await PowerOffDialog());

            powerButtonFrame.GestureRecognizers.Add(tapGestureAnimation);
            powerButtonFrame.GestureRecognizers.Add(tapGestureTrigger);

            upButton.Command = Forward = new Command(_ => viewModel.Move(ControllerViewModel.Direction.FORWARD));
            downButton.Command = Backward = new Command(_ => viewModel.Move(ControllerViewModel.Direction.BACKWARD));
            leftButton.Command = Left = new Command(_ => viewModel.Move(ControllerViewModel.Direction.LEFT));
            rightButton.Command = Right = new Command(_ => viewModel.Move(ControllerViewModel.Direction.RIGHT));

            speedIncreaseButton.Command = IncreaseSpeed = new Command(_ => viewModel.IncreaseSpeed());
            speedDecreaseButton.Command = DecreaseSpeed = new Command(_ => viewModel.DecreaseSpeed());


        }

        private async Task PowerOffDialog()
        {
            bool result = await DisplayAlert("Device Shutdown", 
                "Are you sure you want to power down the rover?", "Yes", "No");

            if (result)
                await viewModel.PowerOffDevice();
        }
    }
}