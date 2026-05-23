using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mvvm_edp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mvvm_edp.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace mvvm_edp.ViewModels;

public partial class CreateNewProductViewModel : ObservableValidator
{
    private readonly ProductRepository repo = new ProductRepository();



    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewProductViewModel), nameof(NameCheck))]
    private string? name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewProductViewModel), nameof(DescriptionCheck))]
    private string description;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(CreateNewProductViewModel), nameof(PriceCheck))]
    private string price;

    public CreateNewProductViewModel()
    {
        Name = "";
        Description = "";
        Price = "";
    }

    public static ValidationResult? NameCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewProductViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success: new ValidationResult("Name cannot be empty, and atleast 5 characters");
    }
    public static ValidationResult? DescriptionCheck(string value, ValidationContext context)
    {
        var vm = (CreateNewProductViewModel)context.ObjectInstance;

        return vm.ValidateName(value) ? ValidationResult.Success : new ValidationResult("Description cannot be empty, and atleast 5 characters");
    }

    public static ValidationResult? PriceCheck(string? value, ValidationContext context)
    {

        if (string.IsNullOrWhiteSpace(value))
            return new ValidationResult("Price cannot be empty");
        if (!decimal.TryParse(value, out decimal parsedValue))
            return new ValidationResult("Invalid decimal value");
        if (parsedValue < 0) 
            return new ValidationResult("Price cannot be negative");
        return ValidationResult.Success;
    }


    public bool ValidateName(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length >= 5;
    }

    [RelayCommand]
    public async Task SaveChanges(Window window)
    {
        ValidateAllProperties();

        if (HasErrors) return;

        var product = new Product
        {
            Name = this.Name,
            Description = this.Description,
            Price = decimal.Parse(this.Price)
        };

        try
        {
            await repo.AddItemAsync(product);
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