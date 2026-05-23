using CommunityToolkit.Mvvm.ComponentModel;
using mvvm_edp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace mvvm_edp.ViewModels
{
    public partial class OrderDetailsViewModel : ObservableObject
    {
        public ObservableCollection<OrderItemDisplay> Items { get; } = new();
        public string OrderSummary { get; }

        // Parameterless constructor for design-time
        public OrderDetailsViewModel()
        {
            OrderSummary = "Order #0 - Customer: N/A - Date: N/A";
        }

        // Runtime constructor
        public OrderDetailsViewModel(OrderCustomerPayment ocp, IEnumerable<Product> allProducts)
        {
            OrderSummary = $"Order #{ocp.Order.OrderId} - Customer: {ocp.CustomerName} - Date: {ocp.Order.OrderDate:yyyy-MM-dd}";

            if (ocp.OrderItems != null)
            {
                foreach (var item in ocp.OrderItems)
                {
                    var product = allProducts.FirstOrDefault(p => p.ProductId == item.ProductId);

                    Items.Add(new OrderItemDisplay
                    {
                        OrderId = ocp.Order.OrderId,
                        CustomerName = ocp.CustomerName,
                        ProductName = product?.Name ?? "Unknown",
                        ProductPrice = product?.Price ?? 0,
                        Quantity = item.Quantity
                    });
                }
            }
        }
    }
}