using Avalonia.Controls;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;


namespace mvvm_edp.Views;

public partial class PaymentTab : UserControl
{
    public PaymentTab()
    {
        InitializeComponent();
        DataContext = new PaymentTabViewModel(new PaymentRepository());
    }
}
