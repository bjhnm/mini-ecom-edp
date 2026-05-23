using Avalonia.Controls;
using Avalonia.Interactivity;
using mvvm_edp.ViewModels;


namespace mvvm_edp.Views;

public partial class NewOrderCustomerTab : UserControl
{
    public NewOrderCustomerTab()
    {
        InitializeComponent();
    }

    private async void OpenDialogButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is NewOrderCustomerTabViewModel vm)
            await vm.CreateNewCustomer(this);
    }

    private void CancelDialogButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is NewOrderCustomerTabViewModel vm)
            vm.CancelCreate(this);
    }
}