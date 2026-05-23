using Avalonia.Controls;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;


namespace mvvm_edp.Views;

public partial class ProductTab : UserControl
{
    public ProductTab()
    {
        InitializeComponent();
        DataContext ??= new ProductTabViewModel(new ProductRepository());
    }
}
