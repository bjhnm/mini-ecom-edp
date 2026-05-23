using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Models;
using mvvm_edp.Repositories;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using mvvm_edp.Views;

namespace mvvm_edp.ViewModels;

public partial class PaymentTabViewModel : ViewModelBase, ITabViewModel
{
    public string Title { get; }

    public ObservableCollection<Payment> Items { get; }
    public ObservableCollection<Payment> Old_Items { get; }
    public ObservableCollection<Payment> FilteredItems { get; } = new();

    public IRepository<Payment> Repository { get; }

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
            RefreshFilteredItems();
        }
    }

    public PaymentTabViewModel(IRepository<Payment> repository)
    {
        Items = new ObservableCollection<Payment>();
        Old_Items = new ObservableCollection<Payment>();
        Repository = repository;
        Title = "Payments";

        Items.CollectionChanged += Items_CollectionChanged;

        _isLoading = true;
        _ = LoadAllItems();
        _isLoading = false;

        IsChanged = false;
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading) return;
        IsChanged = true;
        RefreshFilteredItems(); // So filter updates if a property changes
    }

    private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isLoading) return;
        IsChanged = true;

        if (e.NewItems != null)
        {
            foreach (Payment item in e.NewItems)
                item.PropertyChanged += Item_PropertyChanged;
        }

        if (e.OldItems != null)
        {
            foreach (Payment item in e.OldItems)
                item.PropertyChanged -= Item_PropertyChanged;
        }

        RefreshFilteredItems();
    }

    public async Task LoadAllItems()
    {
        _isLoading = true;

        var result = await Repository.GetAllItemsAsync();
        Items.Clear();
        Old_Items.Clear();

        foreach (var i in result)
        {
            var clone = i.Clone();
            clone.PropertyChanged += Item_PropertyChanged;
            Items.Add(clone);
            Old_Items.Add(i.Clone());
        }

        RefreshFilteredItems();
        _isLoading = false;
        IsChanged = false;
    }

    private void RefreshFilteredItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredItems.Clear();
            foreach (var i in Items)
                FilteredItems.Add(i);
            return;
        }

        var lowerSearch = SearchText.ToLower();

        var filtered = Items.Where(p =>
            p.PaymentId.ToString().Contains(lowerSearch) ||
            p.OrderId.ToString().Contains(lowerSearch) ||
            p.Amount.ToString().Contains(lowerSearch) ||
            p.PaymentMethod?.ToLower().Contains(lowerSearch) == true ||
            p.PaymentStatus?.ToLower().Contains(lowerSearch) == true
        );

        FilteredItems.Clear();
        foreach (var i in filtered)
            FilteredItems.Add(i);
    }

    [RelayCommand]
    public async Task PaymentSaveChanges()
    {
        if (_isLoading) return;

        var diff = Items.Where(n =>
        {
            var old = Old_Items.FirstOrDefault(o => o.PaymentId == n.PaymentId);
            return old != null && n.IsDifferent(old);
        }).ToList();

        if (!diff.Any()) return;

        await Repository.UpdateAllItemsAsync(diff);

        await LoadAllItems();
        IsChanged = false;
    }

    [RelayCommand]
    public async Task PaymentCancelChanges()
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
}