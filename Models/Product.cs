using System.ComponentModel;

namespace mvvm_edp.Models
{
    public class Product : INotifyPropertyChanged, IModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int ProductId { get; set; }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description == value) return;
                _description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price == value) return;
                _price = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Price)));
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity == value) return;
                _quantity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public Product()
        {

            _isSelected = false;
        }

        public Product(int ProductId, string Name, string Description, decimal Price)
        {

            _isSelected = false;
            IsSelected = false;
            this.ProductId = ProductId;
            this.Name = Name;
            this.Description = Description;
            this.Price = Price;
            this.Quantity = 0;
        }
        public Product Clone()

        {
            return new Product
            {
                ProductId = this.ProductId,
                Name = this.Name,
                Description = this.Description,
                Price = this.Price,
                Quantity = this.Quantity,
                IsSelected = this.IsSelected
            };
        }
        public bool IsDifferent(Product other)
        {
            return Name != other.Name
                || Description != other.Description
                || Price != other.Price
                || Quantity != other.Quantity
                || ProductId != other.ProductId 
                || other is null;
        }
    }
}