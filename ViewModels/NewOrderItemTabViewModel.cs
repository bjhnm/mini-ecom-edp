using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Models;
using mvvm_edp.Repositories;
using mvvm_edp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace mvvm_edp.ViewModels;

public partial class NewOrderItemTabViewModel : ViewModelBase, ICreateTab
{
    private Window? _owner;
    public string Title { get; }

    public ProductTabViewModel VM { get; }
    private readonly CreateNewOrderViewModel _parent;

    public ObservableCollection<Product> ProductList => VM.Items;

    public ObservableCollection<OrderProduct> ItemList { get; }
    public IRelayCommand NextTabCommandFromParent => _parent.NextTabCommand;
    public IRelayCommand PreviousTabCommandFromParent => _parent.PreviousTabCommand;

    public decimal Total => ItemList.Sum(x => x.Product.Price * x.Item.Quantity);

    public NewOrderItemTabViewModel()
    {
        Title = "Select Items";

        VM = new ProductTabViewModel(new ProductRepository());
        ItemList = new ObservableCollection<OrderProduct>();

        HookEvents();
    }

    public NewOrderItemTabViewModel(CreateNewOrderViewModel vm,Window window)
    {
        Title = "Select Items";

        _owner = window;
        _parent = vm;

        VM = new ProductTabViewModel(new ProductRepository());
        ItemList = new ObservableCollection<OrderProduct>();

        HookEvents();
    }
    private void HookEvents()
    {
        ItemList.CollectionChanged += ItemList_CollectionChanged;
    }

    private void ItemList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (OrderProduct item in e.NewItems)
            {
                item.Item.PropertyChanged += OrderItem_PropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (OrderProduct item in e.OldItems)
            {
                item.Item.PropertyChanged -= OrderItem_PropertyChanged;
            }
        }

        OnPropertyChanged(nameof(Total));

        _parent?.RefreshCanExecute(); 
    }

    private void OrderItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(OrderItem.Quantity))
        {
            OnPropertyChanged(nameof(Total));
            _parent?.RefreshCanExecute(); 
        }
    }

    public void SetOwner(Window owner)
    {
        _owner = owner;
    }

    [RelayCommand]
    public void CancelCreate(UserControl control)
    {
        var uc = GetOwnerFromControl(control);
        uc?.Close();
    }

    private Window? GetOwnerFromControl(UserControl control)
    {
        return TopLevel.GetTopLevel(control) as Window;
    }
    [RelayCommand]
    public void ProductToItem(Product product)
    {
        if (product == null) return;

        ProductList.Remove(product);

        var order = new OrderProduct(
            new OrderItem(product.ProductId, 1),
            product);

        ItemList.Add(order);

        OnPropertyChanged(nameof(Total));
    }

    [RelayCommand]
    public void ItemToProduct(OrderProduct product)
    {
        if (product == null) return;

        ItemList.Remove(product);
        ProductList.Add(product.Product);

        OnPropertyChanged(nameof(Total));
    }
    public void OnTabActivated()
    {
    }
    [RelayCommand]
    private void Next()
    {
        _parent.NextTab();
    }

    [RelayCommand]
    private void Previous()
    {
        _parent.PreviousTab();
    }
}