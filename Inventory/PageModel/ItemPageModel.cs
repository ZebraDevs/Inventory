using System;
using System.Windows.Input;
using FreshMvvm;
using Xamarin.Forms;

namespace Inventory
{
    public class ItemPageModel : FreshBasePageModel
    {
        // Use IoC to get our repository.
        private Repository _repository = FreshIOC.Container.Resolve<Repository>();

        // Backing data model.
        private Item _item;

        /// <summary>
        /// Public property exposing the item's name for Page binding.
        /// </summary>
        public string ItemName
        {
            get { return _item.Name; }
            set { _item.Name = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Public property exposing the item's barcode for Page binding.
        /// </summary>
        public string ItemBarcode
        {
            get { return _item.Barcode; }
            set { _item.Barcode = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Public property exposing the item's quantity for Page binding.
        /// </summary>
        public int ItemQuantity
        {
            get { return _item.Quantity; }
            set { _item.Quantity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Called whenever the page is navigated to.
        /// Either use a supplied Intem, or create a new one if not supplied.
        /// FreshMVVM does not provide a RaiseAllPropertyChanged,
        /// so we do this for each bound property, room for improvement.
        /// </summary>
        public override void Init(object initData)
        {
            _item = initData as Item;
            if (_item == null) _item = new Item();
            base.Init(initData);
            RaisePropertyChanged(nameof(ItemName));
            RaisePropertyChanged(nameof(ItemBarcode));
        }

        /// <summary>
        /// Command associated with the save action.
        /// Persists the item to the database if the item is valid.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return new Command(async () => {
                    if (_item.IsValid())
                    {
                        await _repository.CreateItem(_item);
                        await CoreMethods.PopPageModel(_item);
                    }
                });
            }
        }
    }
}