using Avalonia.Controls;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views;

public partial class OrderDetails : Window
{
    public OrderDetails()
    {
        InitializeComponent();
    }

    public OrderDetails(OrderDetailsViewModel vm) : this()
    {
        DataContext = vm;
    }

    private void OnCloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}
