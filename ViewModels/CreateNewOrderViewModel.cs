using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Models;
using mvvm_edp.Repositories;
using MySqlConnector;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace mvvm_edp.ViewModels;

public partial class CreateNewOrderViewModel : ViewModelBase
{
    public ObservableCollection<ICreateTab> Tabs { get; }

    public NewOrderCustomerTabViewModel noct;
    public NewOrderItemTabViewModel noit;
    public NewOrderPaymentTabViewModel nopt;

    public bool[] TabsEnabled => new bool[]
{
    true,
    CanGoNext,
    CanGoNext && nopt != null
};
    private int _selectedTabIndex;

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (_selectedTabIndex == value) return;

            _selectedTabIndex = value;
            OnPropertyChanged(nameof(SelectedTabIndex));

            OnTabChanged(value);
            NextTabCommand.NotifyCanExecuteChanged();
        }
    }

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (_selectedCustomer == value) return;

            _selectedCustomer = value;
            OnPropertyChanged(nameof(SelectedCustomer));

            NextTabCommand.NotifyCanExecuteChanged();
        }
    }

    public CreateNewOrderViewModel(Window owner)
    {
        noct = new NewOrderCustomerTabViewModel(this, owner);
        noit = new NewOrderItemTabViewModel(this, owner);
        nopt = new NewOrderPaymentTabViewModel(this, owner);

        Tabs = new ObservableCollection<ICreateTab>
        {
            noct,
            noit,
            nopt
        };
    }

    public void OnTabChanged(int selectedIndex)
    {
        if (selectedIndex < 0 || selectedIndex >= Tabs.Count)
            return;

        Tabs[selectedIndex]?.OnTabActivated();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    public void NextTab()
    {
        if (SelectedTabIndex < Tabs.Count - 1)
            SelectedTabIndex++;
    }

    [RelayCommand]
    public void PreviousTab()
    {
        if (SelectedTabIndex > 0)
            SelectedTabIndex--;
    }

    public bool CanGoNext =>
        (SelectedTabIndex == 0 && SelectedCustomer != null) ||
        (SelectedTabIndex == 1 && noit.ItemList.Any()) ||
        SelectedTabIndex == 2;
    public void RefreshCanExecute()
    {
        NextTabCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(TabsEnabled));
    }
    public async void SaveOrder()
    {
        var or = new OrderRepository();
        var or_id = await or.AddItemAsync(new Order(noct.Selected.CustomerId));

        var oir = new OrderItemRepository();
        foreach(var it in noit.ItemList)
        {
            oir.AddItemAsync(new OrderItem(or_id, it.Item.ProductId, it.Item.Quantity));
        }

        var op = new PaymentRepository();
        op.AddItemAsync(new Payment(0, or_id, noit.Total, DateTime.Now, nopt.Paymentstatus, nopt.Paymentmethod));
    }

    [RelayCommand]
    public void Cancel(Window window)
    {
        window?.Close();
    }
}