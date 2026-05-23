using Avalonia.Controls;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views;

public partial class CreateNewCustomer : Window
{
    public CreateNewCustomer()
    {
        InitializeComponent();
        DataContext = new CreateNewCustomerViewModel();
    }

}

