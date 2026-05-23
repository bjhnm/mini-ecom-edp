using Avalonia.Controls;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace mvvm_edp.Views;

partial class CustomerTab : UserControl
{
    public CustomerTab() { 
        InitializeComponent();
        DataContext ??= new CustomerTabViewModel(new CustomerRepository());
    }

}
