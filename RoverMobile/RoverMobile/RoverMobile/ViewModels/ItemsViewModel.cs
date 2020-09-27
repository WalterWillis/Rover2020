using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using RoverMobile.Models;
using RoverMobile.Views;
using System.Threading;
using System.Data.Common;

namespace RoverMobile.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;
        private string _currentScanAddress;
        private CancellationTokenSource tokenSource;

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }
        public Command CancelScanCommand { get; }

        public string CurrentScanAddress 
        { get => _currentScanAddress; set => SetProperty(ref _currentScanAddress, value); }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Item>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            CancelScanCommand = new Command(() => CancelScan());

            tokenSource = new CancellationTokenSource();
        }

        private void CancelScan()
        {
            if (IsBusy)
                tokenSource.Cancel(); 
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                //var items = await DataStore.GetItemsAsync(true);

                await foreach (string status in DataStore.GetIPAddresses(tokenSource.Token))
                {
                    if(status.Contains("found")) //if we get the special status, pull the embedded ID and pull the item
                    {
                        var idSplitter = status.Split(":");
                        Items.Add(await DataStore.GetItemAsync(idSplitter[1]));
                    }
                    else
                        CurrentScanAddress = status;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                tokenSource = new CancellationTokenSource(); //create a new source with a new token
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            await DataStore.SelectItem(item.Id);

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}