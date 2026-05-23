using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Repositories;

namespace mvvm_edp.ViewModels;

public partial class AboutUsTabViewModel : ViewModelBase, ITabViewModel
{
    public string Title => "About Us";

    public string AppName { get; } = "Mini E-Commerce";
    public string Version { get; } = "1.0.0";
    public string Description { get; } = "A lightweight e-commerce management dashboard built with Avalonia UI and .NET MVVM architecture.";
    public string ContactEmail { get; } = "support@miniecommerce.com";
    public string Website { get; } = "www.miniecommerce.com";
}
