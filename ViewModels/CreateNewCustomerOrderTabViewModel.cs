using CommunityToolkit.Mvvm.ComponentModel;
using mvvm_edp.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mvvm_edp.ViewModels;

public partial class CreateNewCustomerOrderTabViewModel : ObservableValidator
{
    public string Title { get; set; }

    private readonly CustomerRepository repo = new CustomerRepository();



    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerOrderTabViewModel), nameof(NameCheck))]
    private string? firstname;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerOrderTabViewModel), nameof(NameCheck))]
    private string? lastname;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerOrderTabViewModel), nameof(PhoneCheck))]
    private string? phone;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewCustomerOrderTabViewModel), nameof(AddressCheck))]
    private string? address;

    public static ValidationResult? NameCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewCustomerOrderTabViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success : new ValidationResult("Name cannot be empty, and atleast 1 character");
    }
    public static ValidationResult? PhoneCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewCustomerOrderTabViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success : new ValidationResult("Phone cannot be empty, and atleast 5 character");
    }
    public static ValidationResult? AddressCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewCustomerOrderTabViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success : new ValidationResult("Address cannot be empty, and atleast 5 characters");
    }



    public bool ValidateName(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length >= 1;
    }

    public CreateNewCustomerOrderTabViewModel()
    {
        Firstname = "";
        Lastname = "";
        Phone = "";
        Address = "";
    }


}
