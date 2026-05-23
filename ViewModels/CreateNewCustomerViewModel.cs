using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Models;
using System.ComponentModel.DataAnnotations;
using mvvm_edp.Repositories;
using System;
using System.Threading.Tasks;

namespace mvvm_edp.ViewModels;

public partial class CreateNewCustomerViewModel : ObservableValidator
{
    private readonly CustomerRepository repo = new CustomerRepository();



    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerViewModel), nameof(NameCheck))]
    private string? firstname;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerViewModel), nameof(NameCheck))]
    private string? lastname;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerViewModel), nameof(PhoneCheck))]
    private string? phone;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerViewModel), nameof(AddressCheck))]
    private string? address;


    public CreateNewCustomerViewModel()
    {
        Firstname = "";
        Lastname= "";
        Phone  = "";
        Address = "";
    }

    public static ValidationResult? NameCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewCustomerViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success : new ValidationResult("Name cannot be empty, and atleast 1 character");
    }
    public static ValidationResult? PhoneCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewCustomerViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success : new ValidationResult("Phone cannot be empty, and atleast 5 character");
    }
    public static ValidationResult? AddressCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewCustomerViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success : new ValidationResult("Address cannot be empty, and atleast 5 characters");
    }



    public bool ValidateName(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length >= 1;
    }

    [RelayCommand]
    public async Task SaveChanges(Window window)
    {
        ValidateAllProperties();

        if (HasErrors) return;

        var customer = new Customer
        {
            FirstName = Firstname is not null ? Firstname : "Unknown",
            LastName = Lastname is not null ? Lastname : "Unknown",
            Phone = Phone is not null ? Phone : "Unknown",
            Address = Address is not null ? Address : "Unknown",

        };

        try
        {
            await repo.AddItemAsync(customer);
            window?.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    public void Cancel(Window window)
    {
        window?.Close();
    }
}