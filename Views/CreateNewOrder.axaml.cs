using Avalonia.Controls;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace mvvm_edp.Views;

public partial class CreateNewOrder: Window
{
    public CreateNewOrder()
    {
        InitializeComponent();
        DataContext = new CreateNewOrderViewModel(this);
    }

    private void OnTabChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is CreateNewOrderViewModel vm &&
            sender is TabControl tabControl)
        {
            vm.OnTabChanged(tabControl.SelectedIndex);
        }
    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
}