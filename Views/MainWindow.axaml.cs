using Avalonia.Controls;
using Avalonia.Platform;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this);

        using var stream = AssetLoader.Open(new System.Uri("avares://mvvm_edp/Assets/app.ico"));
        Icon = new WindowIcon(stream);
    }
}