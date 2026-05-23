using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Repositories;
using System;

namespace mvvm_edp.ViewModels;

public partial class ProfileTabViewModel : ViewModelBase, ITabViewModel
{
    public string Title => "Profile";

    public string Username { get; } = "Admin";
    public string Role { get; } = "Administrator";
    public string Email { get; } = "admin@shop.com";
    public string MemberSince { get; } = "January 2024";
    public string LastLogin { get; } = DateTime.Now.ToString("MMMM dd, yyyy h:mm tt");
}
