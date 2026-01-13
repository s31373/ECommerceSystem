using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECommerceSystem.Models
{
    public class Product
    {
        private static List<Product> _extent = new List<Product>();
        private static int _nextId = 1;

        [Key]
        public int ProductId { get; private set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        private string _sku;
        private List<Category> _categories = new List<Category>();

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string SKU 
        { 
            get => _sku;
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("SKU cannot be empty");
                if (_sku == value) 
                    return;
        
                string oldSku = _sku;
                _sku = value;
                
                foreach (var category in _categories.ToList())
                {
                    category.UpdateProductKey(oldSku, this);
                }
            }
        }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int MinimumStock { get; set; }

        private List<string> _images;
        public IReadOnlyList<string> Images => _images.AsReadOnly();

        public DateTime CreatedDate { get; private set; }
        public DateTime LastModifiedDate { get; set; }

        public Supplier? Supplier { get; set; }

        public Product(string name, string description, string sku, decimal price, 
                      int stockQuantity, int minimumStock, List<string> images)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty");
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty");
            if (price <= 0)
                throw new ArgumentException("Price must be greater than 0");
            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative");
            if (minimumStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative");
            if (images == null || images.Count < 1 || images.Count > 5)
                throw new ArgumentException("Product must have between 1 and 5 images");

            ProductId = _nextId++;
            Name = name;
            Description = description;
            _sku = sku;
            Price = price;
            StockQuantity = stockQuantity;
            MinimumStock = minimumStock;
            _images = new List<string>(images);
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;

            _extent.Add(this);
        }

        public void AddImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL cannot be empty");
            if (_images.Count >= 5)
                throw new InvalidOperationException("Cannot add more than 5 images");

            _images.Add(imageUrl);
            LastModifiedDate = DateTime.Now;
        }

        public void RemoveImage(string imageUrl)
        {
            if (_images.Count <= 1)
                throw new InvalidOperationException("Product must have at least 1 image");

            _images.Remove(imageUrl);
            LastModifiedDate = DateTime.Now;
        }

        public bool CheckIfLowStock()
        {
            return StockQuantity <= MinimumStock;
        }

        public void ViewProductSupplier()
        {
            if (Supplier != null)
            {
                Console.WriteLine($"Supplier: {Supplier.Name}");
                Console.WriteLine($"Contact: {Supplier.ContactEmail}, {Supplier.ContactPhone}");
            }
            else
            {
                Console.WriteLine("No supplier assigned");
            }
        }

        public static IReadOnlyList<Product> GetExtent()
        {
            return _extent.AsReadOnly();
        }

        public static void ClearExtent()
        {
            _extent.Clear();
            _nextId = 1;
        }
        
        public void SetSupplier(Supplier newSupplier)
        {
            if (Supplier == newSupplier)
                return;
    
            if (Supplier != null && Supplier.Products.Contains(this))
            {
                Supplier.RemoveProduct(this);
            }
    
            Supplier = newSupplier;
    
            if (newSupplier != null && !newSupplier.Products.Contains(this))
            {
                newSupplier.AddProduct(this);
            }
        }
        
        internal void AddToCategory(Category category)
        {
            if (!_categories.Contains(category))
                _categories.Add(category);
        }

        internal void RemoveFromCategory(Category category)
        {
            _categories.Remove(category);
        }
    }
}
