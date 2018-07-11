using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Inventory
{
    public class ItemListPageModel : FreshBasePageModel
    {
        private Repository _repository = FreshIOC.Container.Resolve<Repository>();
        private Item _selectedItem = null;

        /// <summary>
        /// Collection used for binding to the Page's item list view.
        /// </summary>
        public ObservableCollection<Item> Items { get; private set; }

        /// <summary>
        /// Used to bind with the list view's SelectedItem property.
        /// Calls the EditItemCommand to start the editing.
        /// </summary>
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (value != null) EditItemCommand.Execute(value);
            }
        }

        public ItemListPageModel()
        {
            Items = new ObservableCollection<Item>();
        }

        /// <summary>
        /// Called whenever the page is navigated to.
        /// Here we are ignoring the init data and just loading the items.
        /// </summary>
        public override void Init(object initData)
        {
            LoadItems();
            if (Items.Count() < 1)
            {
                CreateSampleData();
            }

        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            var scanner = FreshIOC.Container.Resolve<IScanner>();

            scanner.Enable();
            scanner.OnScanDataCollected += ScannedDataCollected;
            scanner.OnStatusChanged += ScannedStatusChanged;

            var config = new ZebraScannerConfig();
            config.IsUPCE0 = false;
            config.IsUPCE1 = false;

            scanner.SetConfig(config);
        } 

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            var scanner = FreshIOC.Container.Resolve<IScanner>();

            if (null != scanner)
            {
                scanner.Disable();
                scanner.OnScanDataCollected -= ScannedDataCollected;
                scanner.OnStatusChanged -= ScannedStatusChanged;
            }
            base.ViewIsDisappearing(sender, e);
        }

        /// <summary>
        /// Called whenever the page is navigated to, but from a pop action.
        /// Here we are just updating the item list with most recent data.
        /// </summary>
        /// <param name="returnedData"></param>
        public override void ReverseInit(object returnedData)
        {
            LoadItems();
            base.ReverseInit(returnedData);
        }

        /// <summary>
        /// Command associated with the add item action.
        /// Navigates to the ItemPageModel with no Init object.
        /// </summary>
        public ICommand AddItemCommand
        {
            get
            {
                return new Command(async () => {
                    await CoreMethods.PushPageModel<ItemPageModel>();
                });
            }
        }

        /// <summary>
        /// Command associated with the edit item action.
        /// Navigates to the ItemPageModel with the selected item as the Init object.
        /// </summary>
        public ICommand EditItemCommand
        {
            get
            {
                return new Command(async (item) => {
                    await CoreMethods.PushPageModel<ItemPageModel>(item);
                });
            }
        }

        /// <summary>
        /// Repopulate the collection with updated items data.
        /// Note: For simplicity, we wait for the async db call to complete,
        /// recommend making better use of the async potential.
        /// </summary>
        private void LoadItems()
        {
            Items.Clear();
            Task<List<Item>> getItemTask = _repository.GetAllItems();
            getItemTask.Wait();
            foreach (var item in getItemTask.Result)
            {
                Items.Add(item);
            }
        }

        /// <summary>
        /// Uses the SQLite Async capability to insert sample data on multiple threads.
        /// </summary>
        private void CreateSampleData()
        {
            var item1 = new Item
            {
                Name = "Milk",
                Barcode = "8001234567890",
                Quantity = 10
            };

            var item2 = new Item
            {
                Name = "Soup",
                Barcode = "8002345678901",
                Quantity = 5
            };

            var item3 = new Item
            {
                Name = "Water",
                Barcode = "8003456789012",
                Quantity = 20
            };

            var task1 = _repository.CreateItem(item1);
            var task2 = _repository.CreateItem(item2);
            var task3 = _repository.CreateItem(item3);

            // Don't proceed until all the async inserts are complete.
            var allTasks = Task.WhenAll(task1, task2, task3);
            allTasks.Wait();

            LoadItems();
        }

        private void ScannedDataCollected(object sender, StatusEventArgs a_status)
        {
            Barcode barcode = new Barcode();
            barcode.Data = a_status.Data;
            barcode.Type = a_status.BarcodeType;

            Item item;

            Task<List<Item>> getItemTask = _repository.GetItem(barcode.Data);
            getItemTask.Wait();
            if (getItemTask.Result.Count() < 1)
            {
                item = new Item { Name = "", Barcode = barcode.Data };
            }
            else
            {
                item = getItemTask.Result.First<Item>();
            }


            CoreMethods.PushPageModel<ItemPageModel>(item);

        }

        private void ScannedStatusChanged(object sender, string a_message)
        {
            string status = a_message;
        }
    }
}
