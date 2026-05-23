using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace mvvm_edp.Models
{
    public class Order : INotifyPropertyChanged, IModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int OrderId{ get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }

        public Order()
        {
        }

        public Order(int orderId, int customerId, DateTime orderDate)
        {
            OrderId = orderId;
            CustomerId = customerId;
            OrderDate = orderDate;
        }

        public Order (int customerId)
        {
            CustomerId = customerId;
        }

        public bool IsDifferent(Order other)
        {
            return other.OrderId != OrderId ||
                other.CustomerId != CustomerId ||
                other.OrderDate != OrderDate;
        }

        public Order Clone()
        {
            return new Order(
                OrderId,
                CustomerId,
                OrderDate
                );
        }
    }
}
