using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views;

public partial class OrderTab : UserControl
{
    public OrderTab()
    {
        InitializeComponent();

        AttachedToVisualTree += OnAttached;
    }

    private void OnAttached(object? sender, VisualTreeAttachmentEventArgs e)
    {
        var window = TopLevel.GetTopLevel(this) as Window;

        if (window == null)
            return;

        DataContext = new OrderTabViewModel(
            window,
            new OrderItemRepository(),
            new PaymentRepository(),
            new OrderRepository(),
            new CustomerRepository());

        AttachedToVisualTree -= OnAttached;
    }
}