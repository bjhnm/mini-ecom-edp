using Avalonia.Controls;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views;

public partial class CreateNewProduct : Window
{
    public CreateNewProduct()
    {
        InitializeComponent();
        DataContext = new CreateNewProductViewModel();
    }

}

