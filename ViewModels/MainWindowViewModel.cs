using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using mvvm_edp.Repositories;
using System.Threading.Tasks;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;

namespace mvvm_edp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<ITabViewModel> Tabs { get; }

    public MainWindowViewModel()
    {
        Tabs = new ObservableCollection<ITabViewModel>
        {
            new ProductTabViewModel(new ProductRepository()),
            new CustomerTabViewModel(new CustomerRepository()),
            new OrderTabViewModel(new Window(), new OrderItemRepository(), new PaymentRepository(), new OrderRepository(), new CustomerRepository()),
            new OrderItemTabViewModel(new OrderItemRepository(), new OrderRepository(), new CustomerRepository(), new ProductRepository()),
            new PaymentTabViewModel(new PaymentRepository())
        };
    }

    public MainWindowViewModel(Window window)
    {
        Tabs = new ObservableCollection<ITabViewModel>
        {
            new ProductTabViewModel(new ProductRepository()),
            new CustomerTabViewModel(new CustomerRepository()),
            new OrderTabViewModel(window, new OrderItemRepository(), new PaymentRepository(), new OrderRepository(), new CustomerRepository()),
            new OrderItemTabViewModel(new OrderItemRepository(), new OrderRepository(), new CustomerRepository(), new ProductRepository()),
            new PaymentTabViewModel(new PaymentRepository())
        };
    }

    private static Window? GetMainWindow()
    {
        return App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;
    }

    [RelayCommand]
    private async Task ShowProfileDialog()
    {
        var owner = GetMainWindow();
        if (owner is null) return;
        var dialog = new ProfileDialog();
        await dialog.ShowDialog(owner);
    }

    [RelayCommand]
    private async Task ShowAboutUsDialog()
    {
        var owner = GetMainWindow();
        if (owner is null) return;
        var dialog = new AboutUsDialog();
        await dialog.ShowDialog(owner);
    }

    [RelayCommand]
    private async Task ShowReportDialog()
    {
        var owner = GetMainWindow();
        if (owner is null) return;
        var dialog = new ReportDialog();
        await dialog.ShowDialog(owner);
    }
}
