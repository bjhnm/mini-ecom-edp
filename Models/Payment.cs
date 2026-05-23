using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace mvvm_edp.Models;

public class Payment : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }

    private string _PaymentMethod;
    public string PaymentMethod
    {
        get => _PaymentMethod;
        set
        {
            if (_PaymentMethod == value) return;
            {
                _PaymentMethod = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentMethod)));
            }
        }
    }

    private string _PaymentStatus;
    public string PaymentStatus
    {
        get => _PaymentStatus;
        set
        {
            if (_PaymentStatus == value) return;
            {
                _PaymentStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentStatus)));
            }
        }
    }
    
    public Payment()
    {
    }

    public Payment(int paymentId, int orderId, decimal amount, DateTime paymentDate, string paymentStatus, string paymentMethod)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        PaymentDate = paymentDate;
        Amount = amount;
        PaymentMethod = paymentMethod;
        PaymentStatus = paymentStatus;
    }

    public bool IsDifferent(Payment other)
    {
        return PaymentId != other.PaymentId 
            || OrderId != other.OrderId
            || PaymentDate != other.PaymentDate 
            || PaymentStatus != other.PaymentStatus 
            || PaymentMethod != other.PaymentMethod
            || Amount != other.Amount
            || other is null;
    }

    public Payment Clone()
    {
        return new Payment(
            PaymentId,
            OrderId,
            Amount,
            PaymentDate,
            PaymentStatus,
            PaymentMethod
            );
    }
}
