using Avalonia.Controls;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views;

public partial class OrderItemTab : UserControl
{
    public OrderItemTab()
    {
        InitializeComponent();
        DataContext = new OrderItemTabViewModel(new OrderItemRepository(), new OrderRepository(), new CustomerRepository(), new ProductRepository());
    }
}
