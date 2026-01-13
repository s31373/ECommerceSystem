using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECommerceSystem.Models
{
    public class Category
    {
        private static List<Category> _extent = new List<Category>();
        private static int _nextId = 1;

        [Key]
        public int CategoryId { get; private set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        private Category _parentCategory;
        public Category ParentCategory
        {
            get => _parentCategory;
            private set => _parentCategory = value;
        }

        private List<Category> _subcategories = new List<Category>();
        public IReadOnlyList<Category> Subcategories => _subcategories.AsReadOnly();

        private Dictionary<string, Product> _productsBySku = new Dictionary<string, Product>();
        public IReadOnlyDictionary<string, Product> ProductsBySku => _productsBySku;

        public Category(string name, string description = "", Category parentCategory = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty");

            CategoryId = _nextId++;
            Name = name;
            Description = description ?? "";

            _extent.Add(this);

            if (parentCategory != null)
            {
                SetParentCategory(parentCategory);
            }
        }

        public void SetParentCategory(Category parent)
        {
            if (parent == this)
                throw new InvalidOperationException("Category cannot be its own parent");

            if (parent != null && IsDescendantOf(parent))
                throw new InvalidOperationException("Cannot create circular category hierarchy");

            if (_parentCategory != null)
            {
                _parentCategory.RemoveSubcategory(this);
            }

            _parentCategory = parent;

            if (parent != null && !parent.Subcategories.Contains(this))
            {
                parent.AddSubcategory(this);
            }
        }

        public void AddSubcategory(Category subcategory)
        {
            if (subcategory == null)
                throw new ArgumentNullException(nameof(subcategory));
            if (subcategory == this)
                throw new InvalidOperationException("Category cannot be its own subcategory");
            if (_subcategories.Contains(subcategory))
                throw new InvalidOperationException("Subcategory already exists");

            if (IsDescendantOf(subcategory))
                throw new InvalidOperationException("Cannot create circular category hierarchy");

            _subcategories.Add(subcategory);

            if (subcategory.ParentCategory != this)
            {
                subcategory._parentCategory = this;
            }
        }

        public void RemoveSubcategory(Category subcategory)
        {
            if (subcategory == null)
                throw new ArgumentNullException(nameof(subcategory));
            if (!_subcategories.Contains(subcategory))
                throw new InvalidOperationException("Subcategory not found");

            _subcategories.Remove(subcategory);

            if (subcategory.ParentCategory == this)
            {
                subcategory._parentCategory = null;
            }
        }

        private bool IsDescendantOf(Category category)
        {
            if (category == null)
                return false;

            Category current = this;
            while (current != null)
            {
                if (current == category)
                    return true;
                current = current.ParentCategory;
            }
            return false;
        }

        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(product.SKU))
                throw new ArgumentException("Product SKU cannot be empty");
            if (_productsBySku.ContainsKey(product.SKU))
                throw new InvalidOperationException($"Product with SKU '{product.SKU}' already exists in this category");

            _productsBySku[product.SKU] = product;
            product.AddToCategory(this); 
        }

        public void RemoveProduct(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty");
            if (!_productsBySku.ContainsKey(sku))
                throw new InvalidOperationException($"Product with SKU '{sku}' not found in this category");

            var product = _productsBySku[sku];
            _productsBySku.Remove(sku);
            product.RemoveFromCategory(this);
        }

        public Product GetProductBySku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty");
            if (!_productsBySku.ContainsKey(sku))
                throw new KeyNotFoundException($"Product with SKU '{sku}' not found in this category");

            return _productsBySku[sku];
        }

        public bool HasProduct(string sku)
        {
            return !string.IsNullOrWhiteSpace(sku) && _productsBySku.ContainsKey(sku);
        }

        public IReadOnlyList<Product> GetAllProducts()
        {
            return _productsBySku.Values.ToList().AsReadOnly();
        }

        public static IReadOnlyList<Category> GetExtent()
        {
            return _extent.AsReadOnly();
        }

        public static void ClearExtent()
        {
            _extent.Clear();
            _nextId = 1;
        }

        public override string ToString()
        {
            return $"{Name} ({_productsBySku.Count} products, {_subcategories.Count} subcategories)";
        }
        
        internal void UpdateProductKey(string oldSku, Product product)
        {
            if (_productsBySku.ContainsKey(oldSku))
            {
                _productsBySku.Remove(oldSku);
                _productsBySku[product.SKU] = product;
            }
        }
    }
}
