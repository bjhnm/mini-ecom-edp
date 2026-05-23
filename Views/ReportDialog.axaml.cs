using Avalonia.Controls;
using Avalonia.Platform;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views;

public partial class ReportDialog : Window
{
    public ReportDialog()
    {
        InitializeComponent();
        DataContext = new ReportTabViewModel(
            new OrderRepository(),
            new ProductRepository(),
            new CustomerRepository(),
            new PaymentRepository(),
            new OrderItemRepository(),
            this
        );

        using var stream = AssetLoader.Open(new System.Uri("avares://mvvm_edp/Assets/app.ico"));
        Icon = new WindowIcon(stream);
    }
}
