using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
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
        }
    }
}