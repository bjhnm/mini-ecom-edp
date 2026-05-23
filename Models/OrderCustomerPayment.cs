using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace mvvm_edp.Models;

public class OrderCustomerPayment : IModel, INotifyPropertyChanged
{


    public event PropertyChangedEventHandler? PropertyChanged;
    public Order Order { get; }
    public Customer Customer { get; }
    public Payment Payment { get; set; }
    public ObservableCollection<OrderItem> OrderItems{ get; }
    public string CustomerName => $"{Customer.FirstName} {Customer.LastName}";

    public OrderCustomerPayment() { }
    public OrderCustomerPayment(Order order, Customer customer, Payment payment, List<OrderItem> orderItems)
    {
        Order = order;
        Customer = customer;
        Payment = payment;
        OrderItems = new ObservableCollection<OrderItem>(orderItems);
        SubscribeToChildren();
    }

    private void SubscribeToChildren()
    {
        if (Order is INotifyPropertyChanged order)
            order.PropertyChanged += ChildChanged;

        if (Customer is INotifyPropertyChanged customer)
            customer.PropertyChanged += ChildChanged;

        if (Payment is INotifyPropertyChanged payment)
            payment.PropertyChanged += ChildChanged;

        foreach (var item in OrderItems)
        {
            if (item is INotifyPropertyChanged notify)
                notify.PropertyChanged += ChildChanged;
        }

        OrderItems.CollectionChanged += OrderItems_CollectionChanged;
    }

    private void OrderItems_CollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(nameof(OrderItems)));
    }

    private void ChildChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this,
            new PropertyChangedEventArgs(null));
    }

    public static List<OrderCustomerPayment> JoinAll(List<Order> orders, List<Customer> customers, List<Payment> payments, List<OrderItem> orderItems)
    {
        var join = new List<OrderCustomerPayment>(
            from order in orders
            let customer = customers.FirstOrDefault(c => c.CustomerId == order.CustomerId)
            let payment = payments.FirstOrDefault(p => p.OrderId == order.OrderId)
            let items = orderItems.Where(oi => oi.OrderId == order.OrderId).ToList()
            where customer != null && payment != null
            select new OrderCustomerPayment(order, customer, payment, items)).ToList();
        return join;
    }
    public OrderCustomerPayment Clone()
    {
        return new OrderCustomerPayment(
            Order.Clone(),
            Customer.Clone(),
            Payment.Clone(),
            OrderItems.Select(x => x.Clone()).ToList()
            );
    }

    public bool IsDifferent(OrderCustomerPayment other)
    {
        return Order.IsDifferent(other.Order)
            || Customer.IsDifferent(other.Customer)
            || Payment.IsDifferent(other.Payment)
            || OrderItems.Count != other.OrderItems.Count
            || OrderItems.Zip(other.OrderItems, (a, b) => a.IsDifferent(b)).Any(x => x)
            || other is null;


    }

}
