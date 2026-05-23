using Avalonia.Controls;
using Avalonia.Interactivity;
using mvvm_edp.Repositories;
using mvvm_edp.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using mvvm_edp.Models;

namespace mvvm_edp.Views;

public partial class NewOrderItemTab : UserControl
{
    private NewOrderItemTabViewModel vm;
    public Customer? selectedCustomer;


    public NewOrderItemTab()
    {
        InitializeComponent();
    }
}