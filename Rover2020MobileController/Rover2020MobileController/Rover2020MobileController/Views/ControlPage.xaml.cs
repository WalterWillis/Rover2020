using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rover2020MobileController.ViewModels;

namespace Rover2020MobileController.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControlPage : ContentPage
    {
        ControllerViewModel viewModel;

        public ControlPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ControllerViewModel();
            TapGestureRecognizer recognizer = (TapGestureRecognizer)powerButtonFrame
                .FindByName("Recognizer");
            recognizer.Command = new Command(async () => await PowerOffDialog());
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