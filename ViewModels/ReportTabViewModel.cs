using System;
using System.Collections.ObjectModel;
using mvvm_edp.Models;
using mvvm_edp.Repositories;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using mvvm_edp.Services;

namespace mvvm_edp.ViewModels
{
    public enum ReportType { Sales, Inventory, Customers }

    public class ReportTabViewModel : ViewModelBase, ITabViewModel
    {
        public string Title { get; } = "Reports";
        private readonly OrderRepository _orderRepo;
        private readonly ProductRepository _productRepo;
        private readonly CustomerRepository _customerRepo;
        private readonly PaymentRepository _paymentRepo;
        private readonly OrderItemRepository _orderItemRepo;

        private ObservableCollection<object> _items = new ObservableCollection<object>();
        public ObservableCollection<object> Items
        {
            get => _items;
            set { _items = value; OnPropertyChanged(nameof(Items)); }
        }
        private ReportType _selectedReport = ReportType.Sales;
        public ReportType SelectedReport 
        { 
            get => _selectedReport; 
            set { if (_selectedReport == value) return; _selectedReport = value; OnPropertyChanged(nameof(SelectedReport)); SelectedReportIndex = (int)_selectedReport; _ = RefreshAsync(); }
        }

        private int _selectedReportIndex = 0;
        public int SelectedReportIndex
        {
            get => _selectedReportIndex;
            set
            {
                if (_selectedReportIndex == value) return;
                _selectedReportIndex = value;
                OnPropertyChanged(nameof(SelectedReportIndex));
                SelectedReport = (ReportType)value;
            }
        }
        public ICommand RefreshCommand { get; }
        public ICommand ExportCommand { get; }
        public Window? ParentWindow { get; }

        public ReportTabViewModel(OrderRepository orderRepo, ProductRepository productRepo, CustomerRepository customerRepo, PaymentRepository paymentRepo, OrderItemRepository orderItemRepo, Window? parent = null)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _paymentRepo = paymentRepo;
            _orderItemRepo = orderItemRepo;
            ParentWindow = parent;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
            ExportCommand = new AsyncRelayCommand(ExportAsync);

            _ = RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            var list = new ObservableCollection<object>();
            switch (SelectedReport)
            {
                case ReportType.Sales:
                    var orders = await _orderRepo.GetAllItemsAsync();
                    var customers = await _customerRepo.GetAllItemsAsync();
                    var payments = await _paymentRepo.GetAllItemsAsync();
                    foreach (var o in orders)
                    {
                        var cust = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId);
                        var pay = payments.FirstOrDefault(p => p.OrderId == o.OrderId);
                        list.Add(new SalesReportItem
                        {
                            OrderId = o.OrderId,
                            CustomerName = cust != null ? $"{cust.FirstName} {cust.LastName}" : "N/A",
                            OrderDate = o.OrderDate,
                            Total = pay?.Amount ?? 0,
                            PaymentMethod = pay?.PaymentMethod ?? "N/A",
                            PaymentStatus = pay?.PaymentStatus ?? "N/A"
                        });
                    }
                    break;
                case ReportType.Inventory:
                    var products = await _productRepo.GetAllItemsAsync();
                    foreach (var p in products) list.Add(p);
                    break;
                case ReportType.Customers:
                    var customers2 = await _customerRepo.GetAllItemsAsync();
                    foreach (var c in customers2) list.Add(c);
                    break;
            }
            Items = list;
        }

        public async Task ExportAsync()
        {
            var username = Environment.UserName;

            var parent = ParentWindow;
            if (parent is null && App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                parent = desktop.MainWindow;

            if (parent is null) return;

            var topLevel = TopLevel.GetTopLevel(parent);
            if (topLevel is null) return;

            var options = new FilePickerSaveOptions
            {
                Title = "Save Report",
                DefaultExtension = "xlsx",
                SuggestedFileName = SelectedReport.ToString() + "Report.xlsx",
                FileTypeChoices = new[] { new FilePickerFileType("Excel Files") { Patterns = new[] { "*.xlsx" } } }
            };

            var result = await topLevel.StorageProvider.SaveFilePickerAsync(options);
            if (result is null) return;

            var svc = new ReportService();

            if (SelectedReport == ReportType.Sales)
            {
                var orders = await _orderRepo.GetAllItemsAsync();
                var customers = await _customerRepo.GetAllItemsAsync();
                var payments = await _paymentRepo.GetAllItemsAsync();
                var orderItems = await _orderItemRepo.GetAllItemsAsync();
                var products = await _productRepo.GetAllItemsAsync();
                await svc.ExportSalesReportAsync(orders, customers, payments, orderItems, products, result.Path.LocalPath, username);
            }
            else if (SelectedReport == ReportType.Inventory)
            {
                var list = Items.Any() ? Items.Cast<Product>().ToList() : await _productRepo.GetAllItemsAsync();
                await svc.ExportInventoryReportAsync(list, result.Path.LocalPath, username);
            }
            else if (SelectedReport == ReportType.Customers)
            {
                var list = Items.Any() ? Items.Cast<Customer>().ToList() : await _customerRepo.GetAllItemsAsync();
                await svc.ExportCustomerReportAsync(list, result.Path.LocalPath, username);
            }
        }
    }
}