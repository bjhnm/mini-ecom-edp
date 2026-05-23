using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using mvvm_edp.Models;
using mvvm_edp.Repositories;
using mvvm_edp.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
namespace mvvm_edp.ViewModels;

public partial class NewOrderCustomerTabViewModel : ViewModelBase, ICreateTab
{
    private Window? _owner;
    public string Title { get; }
    public ObservableCollection<Customer> CustomerList => VM.Items;
    public CustomerTabViewModel VM { get; set; }
    private readonly CreateNewOrderViewModel _parent;
    public IRelayCommand NextTabCommandFromParent => _parent.NextTabCommand;
    public bool CanProceed =>
    Selected != null;

    public Customer? Selected
    {
        get => _parent.SelectedCustomer;
        set
        {
            if (_parent.SelectedCustomer == value) return;

            _parent.SelectedCustomer = value;

            OnPropertyChanged(nameof(Selected));
            OnPropertyChanged(nameof(CanProceed));

            NextCommand.NotifyCanExecuteChanged();
        }
    }

    public NewOrderCustomerTabViewModel()
    {
        Title = "Choose Customer";
    }
    public NewOrderCustomerTabViewModel(Window window)
    {
        Title = "Choose Customer";
        _owner = window;
        VM = new CustomerTabViewModel(new CustomerRepository());
    }
    public NewOrderCustomerTabViewModel(CreateNewOrderViewModel parent, Window window)
    {
        Title = "Choose Customer";
        _owner = window;
        _parent = parent;
        VM = new CustomerTabViewModel(new CustomerRepository());
    }
    [RelayCommand]
    public async Task CreateNewCustomer(UserControl control)
    {
        var owner = GetOwnerFromControl(control);
        if (owner == null) return;

        var window = new CreateNewCustomer();
        await window.ShowDialog(owner);
        await VM.LoadAllItems();

    }
    public void SetOwner(Window owner)
    {
        _owner = owner;
    }
    [RelayCommand]
    public void CancelCreate(UserControl control)
    {
        var uc = GetOwnerFromControl(control);
        uc.Close();
    }
    private Window? GetOwnerFromControl(UserControl control)
    {
        return TopLevel.GetTopLevel(control) as Window;
    }
    public void OnTabActivated()
    {
    }

    [RelayCommand(CanExecute = nameof(CanProceed))]
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
