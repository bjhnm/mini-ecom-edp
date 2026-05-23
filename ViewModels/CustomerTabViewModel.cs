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

public partial class CustomerTabViewModel : ViewModelBase, ITabViewModel
{
    public string Title { get; }

    public ObservableCollection<Customer> Items { get; }
    public ObservableCollection<Customer> Old_Items { get; }
    public IRepository<Customer> Repository { get; }

    // Filtered collection for DataGrid binding
    public ObservableCollection<Customer> FilteredItems { get; } = new ObservableCollection<Customer>();

    private bool _isLoading;
    private bool _isChanged;
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

    public bool HasSelectedItems => Items.Any(x => x.IsSelected);

    public CustomerTabViewModel(IRepository<Customer> repository)
    {
        Items = new ObservableCollection<Customer>();
        Old_Items = new ObservableCollection<Customer>();
        Repository = repository;
        Title = "Customers";

        Items.CollectionChanged += Items_CollectionChanged;

        _isLoading = true;
        _ = LoadAllItems();

        IsChanged = false;
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (_isLoading) return;
        IsChanged = true;

        if (e.PropertyName == nameof(IsChanged))
            OnPropertyChanged(nameof(HasSelectedItems));
    }

    public async Task LoadAllItems()
    {
        _isLoading = true;
        var result = await Repository.GetAllItemsAsync();

        Old_Items.Clear();
        Items.Clear();

        foreach (var i in result)
        {
            var clone = i.Clone();
            clone.PropertyChanged += Item_PropertyChanged;
            Items.Add(clone);
            Old_Items.Add(i.Clone());
        }

        _isLoading = false;
        IsChanged = false;

        RefreshFilteredItems();
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isLoading) return;

        IsChanged = true;
        OnPropertyChanged(nameof(HasSelectedItems));

        if (e.NewItems != null)
            foreach (Customer c in e.NewItems)
                c.PropertyChanged += Item_PropertyChanged;

        if (e.OldItems != null)
            foreach (Customer c in e.OldItems)
                c.PropertyChanged -= Item_PropertyChanged;

        RefreshFilteredItems();
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

        var filtered = Items.Where(c =>
            (c.FirstName != null && c.FirstName.ToLower().Contains(lowerSearch)) ||
            (c.LastName != null && c.LastName.ToLower().Contains(lowerSearch)) ||
            (c.Phone != null && c.Phone.ToLower().Contains(lowerSearch)) ||
            (c.Address != null && c.Address.ToLower().Contains(lowerSearch)) ||
            c.CustomerId.ToString().Contains(lowerSearch)
        );

        FilteredItems.Clear();
        foreach (var c in filtered)
            FilteredItems.Add(c);
    }

    private void RefreshFilteredItems()
    {
        FilterItems();
    }

    [RelayCommand]
    public async Task CustomerSaveChanges()
    {
        await Repository.RemoveAllItemsAsync(Items.ToList());

        var diff = Items.Where(n =>
        {
            var old = Old_Items.FirstOrDefault(o => o.CustomerId == n.CustomerId);
            return old != null && n.IsDifferent(old);
        }).ToList();

        await Repository.UpdateAllItemsAsync(diff);
        await LoadAllItems();

        IsChanged = false;
    }

    [RelayCommand]
    public async Task CustomerCancelChanges()
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
    public async Task CreateNewCustomer()
    {
        var window = new CreateNewCustomer();
        await window.ShowDialog(App.Current.ApplicationLifetime is
            IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null);

        _ = LoadAllItems();
    }

    public CustomerTabViewModel() { }
}