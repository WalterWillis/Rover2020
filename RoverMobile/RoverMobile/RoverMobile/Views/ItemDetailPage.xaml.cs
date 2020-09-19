using System.ComponentModel;
using Xamarin.Forms;
using RoverMobile.ViewModels;

namespace RoverMobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}