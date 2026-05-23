using Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace mvvm_edp.Models;

public class OrderItem : INotifyPropertyChanged
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    private int _Quantity;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Quantity 
    {
        get => _Quantity;
        set
        {
            if (_Quantity == value) return;
            _Quantity = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
        }
    }

    public OrderItem()
    {
    }

    public OrderItem(int productId, int quantity)
    {
        OrderId = 0;
        ProductId = productId;
        Quantity = quantity;
    }

    public OrderItem(int orderId, int productId, int quantity)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
    }

    public bool IsDifferent(OrderItem other)
    {
        return OrderId != other.OrderId
            || ProductId != other.ProductId
            || Quantity != other.Quantity
            || other is null;
    }

    public OrderItem Clone()
    {
        return new OrderItem(
            OrderId,
            ProductId,
            Quantity
            );
    }

}
