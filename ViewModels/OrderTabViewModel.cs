using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Models;
using mvvm_edp.Repositories;
using mvvm_edp.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;

namespace mvvm_edp.ViewModels;

public partial class OrderTabViewModel : ViewModelBase, ITabViewModel
{
    public string Title { get; }
    private Window? _owner;

    public ObservableCollection<OrderCustomerPayment> Items { get; }
    public ObservableCollection<OrderCustomerPayment> Old_Items { get; }

    public ObservableCollection<OrderCustomerPayment> FilteredItems { get; } = new();

    public IRepository<OrderItem> OrderItemRepository { get; }
    public IRepository<Customer> CustomerRepository { get; }
    public IRepository<Payment> PaymentRepository { get; }
    public IRepository<Order> OrderRepository { get; }

    private bool _isChanged;
    private bool _isLoading;
    private string _searchText;

    public bool IsChanged
    {
        get => _isChanged;
        set
        {
            if (_isChanged == value) return;
            _isChanged = value;
            OnPropertyChanged(nameof(IsChanged));
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value) return;
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            FilterItems();
        }
    }

    public OrderTabViewModel(
        Window window,
        IRepository<OrderItem> orderItemRepository,
        IRepository<Payment> paymentRepository,
        IRepository<Order> orderRepository,
        IRepository<Customer> customerRepository)
    {
        OrderItemRepository = orderItemRepository;
        OrderRepository = orderRepository;
        CustomerRepository = customerRepository;
        PaymentRepository = paymentRepository;
        _owner = window;

        Title = "Orders";
        Items = new ObservableCollection<OrderCustomerPayment>();
        Old_Items = new ObservableCollection<OrderCustomerPayment>();

        Items.CollectionChanged += Items_CollectionChanged;

        _isLoading = true;
        _ = LoadAllItems();

        IsChanged = false;
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading) return;
        IsChanged = true;
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isLoading) return;
        IsChanged = true;

        if (e.NewItems != null)
        {
            foreach (OrderCustomerPayment item in e.NewItems)
                item.PropertyChanged += Item_PropertyChanged;
        }

        if (e.OldItems != null)
        {
            foreach (OrderCustomerPayment item in e.OldItems)
                item.PropertyChanged -= Item_PropertyChanged;
        }

        RefreshFilteredItems();
    }

    public async Task LoadAllItems()
    {
        _isLoading = true;

        try
        {
            var orders = await OrderRepository.GetAllItemsAsync();
            var customers = await CustomerRepository.GetAllItemsAsync();
            var orderItems = await OrderItemRepository.GetAllItemsAsync();
            var payments = await PaymentRepository.GetAllItemsAsync();

            Items.Clear();
            Old_Items.Clear();

            var joinList = OrderCustomerPayment.JoinAll(orders, customers, payments, orderItems);

            foreach (var ocp in joinList)
            {
                ocp.PropertyChanged += Item_PropertyChanged;
                Items.Add(ocp);
                Old_Items.Add(ocp.Clone());
            }

            RefreshFilteredItems();
            IsChanged = false;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void FilterItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredItems.Clear();
            foreach (var i in Items)
                FilteredItems.Add(i);
            return;
        }

        var lowerSearch = SearchText.ToLower();

        var filtered = Items.Where(ocp =>
            ocp.Order.OrderId.ToString().Contains(lowerSearch) ||
            (ocp.CustomerName != null && ocp.CustomerName.ToLower().Contains(lowerSearch)) ||
            (ocp.Payment?.Amount.ToString().Contains(lowerSearch) ?? false) ||
            (ocp.Payment?.PaymentStatus?.ToLower().Contains(lowerSearch) ?? false) ||
            (ocp.Payment?.PaymentMethod?.ToLower().Contains(lowerSearch) ?? false) // <- Add this
        );

        FilteredItems.Clear();
        foreach (var ocp in filtered)
            FilteredItems.Add(ocp);
    }

    private void RefreshFilteredItems()
    {
        FilterItems();
    }

    [RelayCommand]
    public async Task OrderSaveChanges()
    {
        if (_isLoading) return;

        var diff = Items.Where(n =>
        {
            var old = Old_Items.FirstOrDefault(o => o.Order.OrderId == n.Order.OrderId);
            return old != null && n.IsDifferent(old);
        }).ToList();

        if (!diff.Any()) return;

        var paymentList = diff.Select(ocp => ocp.Payment).Where(p => p != null).ToList();

        if (!paymentList.Any()) return;

        try
        {
            await PaymentRepository.UpdateAllItemsAsync(paymentList);
            await LoadAllItems();
            IsChanged = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task OrderCancelChanges()
    {
        _isLoading = true;

        Items.Clear();
        foreach (var i in Old_Items.Select(x => x.Clone()))
        {
            i.PropertyChanged += Item_PropertyChanged;
            Items.Add(i);
        }

        RefreshFilteredItems();
        _isLoading = false;
        IsChanged = false;
    }

    [RelayCommand]
    public async Task CreateNewOrder()
    {
        var window = new CreateNewOrder();

        if (_owner == null) return;
        await window.ShowDialog(_owner);

        await LoadAllItems();
    }
    [RelayCommand]
    public async Task OpenOrderDetails(OrderCustomerPayment ocp)
    {
        if (ocp == null) return;
        var productRepository = new ProductRepository();

        var allProducts = await productRepository.GetAllItemsAsync();

        var window = new OrderDetails(new OrderDetailsViewModel(ocp, allProducts));

        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            await window.ShowDialog(desktop.MainWindow);
        }
    }
}