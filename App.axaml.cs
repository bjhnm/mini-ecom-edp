using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using mvvm_edp.Models;
using mvvm_edp.ViewModels;
using mvvm_edp.Views;

namespace mvvm_edp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var loginVm = new DatabaseLoginViewModel();
            var login = new DatabaseLoginDialog
            {
                DataContext = loginVm
            };

            desktop.MainWindow = login;
            login.Show();

            loginVm.LoginCompleted += () =>
            {
                if (loginVm.IsConnected)
                {
                    DatabaseConnection.ConnectionString = loginVm.ConnectionString;
                    var main = new MainWindow();
                    desktop.MainWindow = main;
                    main.Show();
                }
                login.Close();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}