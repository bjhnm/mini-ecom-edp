using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace mvvm_edp.ViewModels;

public partial class NewOrderPaymentTabViewModel : ObservableValidator, ICreateTab
{
    public ObservableCollection<string> PaymentMethods { get; } =
        new ObservableCollection<string>
        {
            "Cash",
            "Credit Card",
            "Debit Card",
            "PayPal",
            "Unknown"
        };

    public ObservableCollection<string> PaymentStates { get; } =
        new ObservableCollection<string>
        {
            "Complete",
            "Pending",
            "Failed"
        };

    private Window? _owner;
    private readonly CreateNewOrderViewModel _parent;

    public string Title { get; set; }

    public decimal? Amount => _parent.noit.Total;

    public ObservableCollection<OrderProduct> Cart => _parent.noit.ItemList;

    public Customer Buyer => _parent.noct.Selected;

    public NewOrderPaymentTabViewModel() => Title = "Checkout";

    public NewOrderPaymentTabViewModel(CreateNewOrderViewModel vm, Window window)
    {
        Title = "Checkout";
        _parent = vm;
        _owner = window;

        // Hook up total changes to update Amount and Save button
        _parent.noit.PropertyChanged += Noit_PropertyChanged;
    }

    private void Noit_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NewOrderItemTabViewModel.Total))
        {
            OnPropertyChanged(nameof(Amount));
            SaveOrderCommand.NotifyCanExecuteChanged();
        }
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string? paymentstatus;

    partial void OnPaymentstatusChanged(string? oldValue, string? newValue)
    {
        SaveOrderCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string? paymentmethod;

    partial void OnPaymentmethodChanged(string? oldValue, string? newValue)
    {
        SaveOrderCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanSaveOrder))]
    private void SaveOrder(UserControl control)
    {
        _parent.SaveOrder();

        var window = TopLevel.GetTopLevel(control) as Window;
        window?.Close();
    }

    private bool CanSaveOrder()
    {
        return Buyer != null
               && !string.IsNullOrWhiteSpace(paymentmethod)
               && !string.IsNullOrWhiteSpace(paymentstatus);
    }

    [RelayCommand]
    private void Previous() => _parent.PreviousTab();

    public void OnTabActivated() { }
}