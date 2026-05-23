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
using System.Windows.Input;
namespace mvvm_edp.ViewModels;

public partial class ProductTabViewModel : ViewModelBase, ITabViewModel
{
    public string Title { get; }

    public ObservableCollection<Product> Items { get; }

    public ObservableCollection<Product> Old_Items { get; }
    public IRepository<Product> Repository { get; }
    public bool HasSelectedItems => Items.Any(x => x.IsSelected);

    public ICommand ProductSaveChangesCommand { get; }
    public ICommand ProductCancelChangesCommand { get; }

    private string _searchText;
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
    public ObservableCollection<Product> FilteredItems { get; } = new ObservableCollection<Product>();

    private bool _isChanged;
    private bool _isLoading;
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

    public ProductTabViewModel(IRepository<Product> repository)
    {
        Items = new ObservableCollection<Product>();
        Old_Items = new ObservableCollection<Product>();
        Repository = repository;
        Title = "Products";

        ProductSaveChangesCommand = new AsyncRelayCommand(ProductSaveChanges);
        ProductCancelChangesCommand = new AsyncRelayCommand(ProductCancelChanges);

        Items.CollectionChanged += Items_CollectionChanged;

        _isLoading = true;
        _ = LoadAllItems();
        
        IsChanged = false;

        FilteredItems.Clear();
        foreach (var i in Items)
            FilteredItems.Add(i);
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

        var filtered = Items.Where(p =>
            (p.Name != null && p.Name.ToLower().Contains(lowerSearch)) ||
            (p.Description != null && p.Description.ToLower().Contains(lowerSearch)) ||
            p.ProductId.ToString().Contains(lowerSearch) ||
            p.Price.ToString().Contains(lowerSearch)
        );

        FilteredItems.Clear();
        foreach (var p in filtered)
            FilteredItems.Add(p);
    }
    private void RefreshFilteredItems()
    {
        FilterItems();
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if(_isLoading) return;
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
    }

    public async Task ProductSaveChanges()
    {
        await Repository.RemoveAllItemsAsync(Items.ToList());

        var diff = Items.Where(n =>
            {
                var old = Old_Items.FirstOrDefault(o =>
                    o.ProductId == n.ProductId);
                return old != null && n.IsDifferent(old);
            }
            ).ToList();

        await Repository.UpdateAllItemsAsync(diff);
        _ = LoadAllItems(); // Unsafe

        IsChanged = false;
    }
    public async Task ProductCancelChanges()
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
    public async Task CreateNewProduct()
    {
        var window = new CreateNewProduct();
        await window.ShowDialog(App.Current.ApplicationLifetime is
            IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null); // nullable unsafe

        _ = LoadAllItems(); // unsafe
    }
}
