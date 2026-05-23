using mvvm_edp.Models;
using mvvm_edp.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace mvvm_edp.ViewModels
{
    public partial class OrderItemTabViewModel : ViewModelBase, ITabViewModel
    {
        public string Title { get; } = "Ordered Items";

        public ObservableCollection<OrderItem> Items { get; } = new();
        public ObservableCollection<OrderItemDisplay> FilteredItems { get; } = new();

        public IRepository<OrderItem> OrderItemRepository { get; }
        public IRepository<Order> OrderRepository { get; }
        public IRepository<Customer> CustomerRepository { get; }
        public IRepository<Product> ProductRepository { get; }

        private bool _isLoading;
        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value) return;
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                RefreshFilteredItems();
            }
        }

        public OrderItemTabViewModel(
            IRepository<OrderItem> orderItemRepo,
            IRepository<Order> orderRepo,
            IRepository<Customer> customerRepo,
            IRepository<Product> productRepo)
        {
            OrderItemRepository = orderItemRepo;
            OrderRepository = orderRepo;
            CustomerRepository = customerRepo;
            ProductRepository = productRepo;

            Items.CollectionChanged += Items_CollectionChanged;

            _ = LoadAllItems();
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isLoading) return;

            if (e.NewItems != null)
            {
                foreach (OrderItem item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            if (e.OldItems != null)
            {
                foreach (OrderItem item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }

            RefreshFilteredItems();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isLoading) return;
            RefreshFilteredItems();
        }

        public async Task LoadAllItems()
        {
            _isLoading = true;

            var orderItems = await OrderItemRepository.GetAllItemsAsync();
            var orders = await OrderRepository.GetAllItemsAsync();
            var products = await ProductRepository.GetAllItemsAsync();
            var customers = await CustomerRepository.GetAllItemsAsync();

            Items.Clear();
            foreach (var oi in orderItems)
            {
                oi.PropertyChanged += Item_PropertyChanged;
                Items.Add(oi.Clone());
            }

            // Join and create display list
            var displayList = from oi in Items
                              join o in orders on oi.OrderId equals o.OrderId
                              join p in products on oi.ProductId equals p.ProductId
                              join c in customers on o.CustomerId equals c.CustomerId
                              select new OrderItemDisplay
                              {
                                  OrderId = o.OrderId,
                                  CustomerName = $"{c.FirstName} {c.LastName}",
                                  ProductName = p.Name,
                                  ProductPrice = p.Price,
                                  Quantity = oi.Quantity
                              };

            FilteredItems.Clear();
            foreach (var d in displayList)
                FilteredItems.Add(d);

            _isLoading = false;
        }

        private void RefreshFilteredItems()
        {
            if (_isLoading) return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Reload all
                foreach (var item in Items)
                    item.PropertyChanged += Item_PropertyChanged;
                _ = LoadAllItems();
                return;
            }

            var lower = SearchText.ToLower();

            var filtered = FilteredItems
                .Where(f =>
                    f.CustomerName.ToLower().Contains(lower) ||
                    f.ProductName.ToLower().Contains(lower) ||
                    f.OrderId.ToString().Contains(lower) ||
                    f.Quantity.ToString().Contains(lower) ||
                    f.ProductPrice.ToString().Contains(lower)
                )
                .ToList();

            FilteredItems.Clear();
            foreach (var f in filtered)
                FilteredItems.Add(f);
        }
    }
}