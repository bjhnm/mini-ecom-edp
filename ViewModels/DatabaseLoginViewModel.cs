using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySqlConnector;

namespace mvvm_edp.ViewModels;

public partial class DatabaseLoginViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _server = "localhost";

    [ObservableProperty]
    private string _port = "3306";

    [ObservableProperty]
    private string _database = "mini_ecom";

    [ObservableProperty]
    private string _user = "root";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _errorMessage = "";

    public bool CanConnect => !IsConnecting;

    [ObservableProperty]
    private bool _isConnecting;

    public bool IsConnected { get; private set; }
    public string ConnectionString { get; private set; } = "";

    public event Action? LoginCompleted;

    partial void OnIsConnectingChanged(bool value)
    {
        OnPropertyChanged(nameof(CanConnect));
    }

    private async Task RunOnUiThread(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
            return;
        }
        await Dispatcher.UIThread.InvokeAsync(action);
    }

    [RelayCommand]
    private async Task Connect()
    {
        await RunOnUiThread(() =>
        {
            IsConnecting = true;
            ErrorMessage = "";
        });

        var connStr = $"Server={Server};User={User};Password={Password};Database={Database};Port={Port}";

        try
        {
            await using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();
            await RunOnUiThread(() =>
            {
                ConnectionString = connStr;
                IsConnected = true;
                LoginCompleted?.Invoke();
            });
        }
        catch (Exception ex)
        {
            await RunOnUiThread(() =>
            {
                ErrorMessage = $"Connection failed: {ex.Message}";
            });
        }
        finally
        {
            await RunOnUiThread(() =>
            {
                IsConnecting = false;
            });
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        LoginCompleted?.Invoke();
    }
}
