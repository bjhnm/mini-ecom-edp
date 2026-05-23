using System;
using System.Collections.Generic;
using System.Text;

namespace mvvm_edp.Models;

public class OrderProduct
{
    public OrderItem Item { get; set; } = null!;
    public Product Product { get; set; } = null!;

    public OrderProduct(OrderItem item, Product product)
    {
        Item = item;
        Product = product;
    }
}
