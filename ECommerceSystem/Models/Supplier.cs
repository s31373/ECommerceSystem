using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models
{
    public class Supplier
    {
        private static List<Supplier> _extent = new List<Supplier>();
        private static int _nextId = 1;

        [Key]
        public int SupplierId { get; private set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }

        [Required]
        [Phone]
        public string ContactPhone { get; set; }

        private decimal? _rating;
        [Range(0, 5)]
        
        private List<Product> _products = new List<Product>();
        
        public IReadOnlyList<Product> Products => _products.AsReadOnly();
        public decimal? Rating
        {
            get => _rating;
            set
            {
                if (value.HasValue && (value < 0 || value > 5))
                    throw new ArgumentOutOfRangeException(nameof(Rating), "Rating must be between 0 and 5");
                _rating = value;
            }
        }

        public Supplier(string name, string contactEmail, string contactPhone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Supplier name cannot be empty");
            if (string.IsNullOrWhiteSpace(contactEmail))
                throw new ArgumentException("Contact email cannot be empty");
            if (string.IsNullOrWhiteSpace(contactPhone))
                throw new ArgumentException("Contact phone cannot be empty");

            SupplierId = _nextId++;
            Name = name;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            _rating = null;

            _extent.Add(this);
        }

        public void LeaveRating(decimal rating)
        {
            if (rating < 0 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating));

            Rating = rating;
        }

        public void ViewRating()
        {
            if (Rating.HasValue)
            {
                Console.WriteLine($"Supplier {Name}: {Rating:F1}/5.0");
            }
            else
            {
                Console.WriteLine($"Supplier {Name}: No rating");
            }
        }

        public static IReadOnlyList<Supplier> GetExtent()
        {
            return _extent.AsReadOnly();
        }

        public static void ClearExtent()
        {
            _extent.Clear();
            _nextId = 1;
        }
        
        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (_products.Contains(product))
                throw new InvalidOperationException("Product already exists in supplier's product list");
    
            _products.Add(product);
    
            if (product.Supplier != this)
                product.SetSupplier(this);
        }

        public void RemoveProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (!_products.Contains(product))
                throw new InvalidOperationException("Product not found in supplier's product list");
    
            _products.Remove(product);
    
            if (product.Supplier == this)
                product.SetSupplier(null);
        }
    }
}
