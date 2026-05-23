
using System.ComponentModel;

namespace mvvm_edp.Models
{
    public class Customer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int CustomerId { get; set; }

        private string _FirstName;
        public string FirstName
        {
            get => _FirstName;
            set
            {
                if (_FirstName == value) return;
                {
                    _FirstName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
                }
            }
        }

        private string _LastName;
        public string LastName
        {
            get => _LastName;
            set
            {
                if (_LastName == value) return;
                {
                    _LastName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
                }
            }
        }

        private string _Phone;
        public string Phone
        {
            get => _Phone;
            set
            {
                if (_Phone == value) return;
                {
                    _Phone = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
                }
            }
        }
        private string _Address;
        public string Address
        {
            get => _Address;
            set
            {
                if (_Address == value) return;
                {
                    _Address = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Address)));
                }
            }
        }
        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (_IsSelected == value) return;
                {
                    _IsSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }
        public Customer()
        { 
            _IsSelected=false;
        }
        public Customer(int CustomerId, string FirstName, string LastName, string Phone, string Address)
        {
            this.CustomerId = CustomerId;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Phone = Phone;
            this.Address = Address;
            _IsSelected = false;
        }
        public Customer Clone()
        {
            return new Customer
            {
                CustomerId = this.CustomerId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                Phone = this.Phone,
                Address = this.Address,
                IsSelected = this.IsSelected
            };
        }
        public bool IsDifferent(Customer other)
        {
            return FirstName != other.FirstName
                || LastName != other.LastName
                || Phone != other.Phone
                || Address != other.Address
                || CustomerId != other.CustomerId
                || other is null;
        }
    }
}
