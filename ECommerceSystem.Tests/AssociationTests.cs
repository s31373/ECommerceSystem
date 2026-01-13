using System;
using System.Collections.Generic;
using Xunit;
using ECommerceSystem.Models;

namespace ECommerceSystem.Tests
{
    public class AssociationTests
    {
        
        [Fact]
        public void Order_Create_AddsToCustomerOrders()
        {
            User.ClearExtent();
            Customer.ClearCustomerExtent();
            Order.ClearExtent();
            
            var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
            var customer = new Customer("johndoe", "john@example.com", "password123",
                                       "John", "Doe", "555-1234", address);

            var order = new Order(customer, 10m, 5m);

            Assert.Single(customer.Orders);
            Assert.Contains(order, customer.Orders);
        }

        [Fact]
        public void Order_SetCustomer_UpdatesBothSides()
        {
            User.ClearExtent();
            Customer.ClearCustomerExtent();
            Order.ClearExtent();
            
            var address1 = new Address("123 Main St", "New York", "NY", "10001", "USA");
            var customer1 = new Customer("john", "john@example.com", "password123",
                                        "John", "Doe", "555-1234", address1);
            var address2 = new Address("456 Oak Ave", "Boston", "MA", "02101", "USA");
            var customer2 = new Customer("jane", "jane@example.com", "password456",
                                        "Jane", "Smith", "555-5678", address2);
            var order = new Order(customer1, 10m, 5m);

            order.SetCustomer(customer2);

            Assert.Equal(customer2, order.Customer);
            Assert.Empty(customer1.Orders);
            Assert.Single(customer2.Orders);
            Assert.Contains(order, customer2.Orders);
        }

        [Fact]
        public void Customer_AddOrder_ThrowsIfNull()
        {
            User.ClearExtent();
            Customer.ClearCustomerExtent();
            
            var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
            var customer = new Customer("johndoe", "john@example.com", "password123",
                                       "John", "Doe", "555-1234", address);

            Assert.Throws<ArgumentNullException>(() => customer.AddOrder(null));
        }

        [Fact]
        public void Customer_AddOrder_ThrowsIfDuplicate()
        {
            User.ClearExtent();
            Customer.ClearCustomerExtent();
            Order.ClearExtent();
            
            var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
            var customer = new Customer("johndoe", "john@example.com", "password123",
                                       "John", "Doe", "555-1234", address);
            var order = new Order(customer, 10m, 5m);

            Assert.Throws<InvalidOperationException>(() => customer.AddOrder(order));
        }

        [Fact]
        public void Customer_RemoveOrder_RemovesSuccessfully()
        {
            User.ClearExtent();
            Customer.ClearCustomerExtent();
            Order.ClearExtent();
            
            var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
            var customer = new Customer("johndoe", "john@example.com", "password123",
                                       "John", "Doe", "555-1234", address);
            var order = new Order(customer, 10m, 5m);

            customer.RemoveOrder(order);

            Assert.Empty(customer.Orders);
        }

        [Fact]
        public void Customer_RemoveOrder_ThrowsIfNotFound()
        {
            User.ClearExtent();
            Customer.ClearCustomerExtent();
            Order.ClearExtent();
            
            var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
            var customer1 = new Customer("john", "john@example.com", "password123",
                                        "John", "Doe", "555-1234", address);
            var customer2 = new Customer("jane", "jane@example.com", "password456",
                                        "Jane", "Smith", "555-5678", address);
            var order = new Order(customer2, 10m, 5m);

            Assert.Throws<InvalidOperationException>(() => customer1.RemoveOrder(order));
        }
        
        [Fact]
        public void Supplier_AddProduct_AddsSuccessfully()
        {
            Supplier.ClearExtent();
            Product.ClearExtent();
            
            var supplier = new Supplier("ABC Corp", "contact@abc.com", "555-1234");
            var images = new List<string> { "image1.jpg" };
            var product = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);

            supplier.AddProduct(product);

            Assert.Single(supplier.Products);
            Assert.Contains(product, supplier.Products);
            Assert.Equal(supplier, product.Supplier);
        }

        [Fact]
        public void Product_SetSupplier_UpdatesBothSides()
        {
            Supplier.ClearExtent();
            Product.ClearExtent();
            
            var supplier1 = new Supplier("ABC Corp", "contact@abc.com", "555-1234");
            var supplier2 = new Supplier("XYZ Inc", "contact@xyz.com", "555-5678");
            var images = new List<string> { "image1.jpg" };
            var product = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            
            supplier1.AddProduct(product);

            product.SetSupplier(supplier2);

            Assert.Equal(supplier2, product.Supplier);
            Assert.Empty(supplier1.Products);
            Assert.Single(supplier2.Products);
            Assert.Contains(product, supplier2.Products);
        }

        [Fact]
        public void Supplier_AddProduct_ThrowsIfDuplicate()
        {
            Supplier.ClearExtent();
            Product.ClearExtent();
            
            var supplier = new Supplier("ABC Corp", "contact@abc.com", "555-1234");
            var images = new List<string> { "image1.jpg" };
            var product = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            supplier.AddProduct(product);

            Assert.Throws<InvalidOperationException>(() => supplier.AddProduct(product));
        }

        [Fact]
        public void Supplier_RemoveProduct_RemovesSuccessfully()
        {
            Supplier.ClearExtent();
            Product.ClearExtent();
            
            var supplier = new Supplier("ABC Corp", "contact@abc.com", "555-1234");
            var images = new List<string> { "image1.jpg" };
            var product = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            supplier.AddProduct(product);

            supplier.RemoveProduct(product);

            Assert.Empty(supplier.Products);
            Assert.Null(product.Supplier);
        }

        [Fact]
        public void Product_SetSupplier_ToNull_RemovesFromSupplier()
        {
            Supplier.ClearExtent();
            Product.ClearExtent();
            
            var supplier = new Supplier("ABC Corp", "contact@abc.com", "555-1234");
            var images = new List<string> { "image1.jpg" };
            var product = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            supplier.AddProduct(product);

            product.SetSupplier(null);

            Assert.Null(product.Supplier);
            Assert.Empty(supplier.Products);
        }
        
        [Fact]
        public void Category_SetParentCategory_CreatesHierarchy()
        {
            Category.ClearExtent();
            var electronics = new Category("Electronics", "Electronic devices");
            var computers = new Category("Computers", "Computer hardware");

            // Act
            computers.SetParentCategory(electronics);

            Assert.Equal(electronics, computers.ParentCategory);
            Assert.Single(electronics.Subcategories);
            Assert.Contains(computers, electronics.Subcategories);
        }

        [Fact]
        public void Category_SetParentCategory_ThrowsIfCircular()
        {
            Category.ClearExtent();
            var electronics = new Category("Electronics");
            var computers = new Category("Computers");
            computers.SetParentCategory(electronics);

            Assert.Throws<InvalidOperationException>(() => electronics.SetParentCategory(computers));
        }

        [Fact]
        public void Category_SetParentCategory_ThrowsIfSelf()
        {
            Category.ClearExtent();
            var electronics = new Category("Electronics");

            Assert.Throws<InvalidOperationException>(() => electronics.SetParentCategory(electronics));
        }

        [Fact]
        public void Category_AddSubcategory_AddsSuccessfully()
        {
            Category.ClearExtent();
            var electronics = new Category("Electronics");
            var computers = new Category("Computers");

            electronics.AddSubcategory(computers);

            Assert.Equal(electronics, computers.ParentCategory);
            Assert.Single(electronics.Subcategories);
            Assert.Contains(computers, electronics.Subcategories);
        }

        [Fact]
        public void Category_AddSubcategory_ThrowsIfDuplicate()
        {
            Category.ClearExtent();
            var electronics = new Category("Electronics");
            var computers = new Category("Computers");
            electronics.AddSubcategory(computers);

            Assert.Throws<InvalidOperationException>(() => electronics.AddSubcategory(computers));
        }

        [Fact]
        public void Category_RemoveSubcategory_RemovesSuccessfully()
        {
            Category.ClearExtent();
            var electronics = new Category("Electronics");
            var computers = new Category("Computers");
            electronics.AddSubcategory(computers);

            electronics.RemoveSubcategory(computers);

            Assert.Null(computers.ParentCategory);
            Assert.Empty(electronics.Subcategories);
        }

        [Fact]
        public void Category_ThreeLevelHierarchy_WorksCorrectly()
        {
            Category.ClearExtent();
            var electronics = new Category("Electronics");
            var computers = new Category("Computers");
            var laptops = new Category("Laptops");

            computers.SetParentCategory(electronics);
            laptops.SetParentCategory(computers);

            Assert.Equal(electronics, computers.ParentCategory);
            Assert.Equal(computers, laptops.ParentCategory);
            Assert.Single(electronics.Subcategories);
            Assert.Single(computers.Subcategories);
        }
        
        [Fact]
        public void Category_AddProduct_BySkuSuccessfully()
        {
            Category.ClearExtent();
            Product.ClearExtent();
            
            var category = new Category("Electronics");
            var images = new List<string> { "image1.jpg" };
            var product = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);

            category.AddProduct(product);

            Assert.Single(category.ProductsBySku);
            Assert.True(category.HasProduct("SKU001"));
            Assert.Equal(product, category.GetProductBySku("SKU001"));
        }

        [Fact]
        public void Category_AddProduct_ThrowsIfDuplicateSku()
        {
            Category.ClearExtent();
            Product.ClearExtent();
            
            var category = new Category("Electronics");
            var images = new List<string> { "image1.jpg" };
            var product1 = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            var product2 = new Product("Mouse", "Gaming mouse", "SKU001", 49m, 100, 20, images);
            category.AddProduct(product1);

            Assert.Throws<InvalidOperationException>(() => category.AddProduct(product2));
        }

        [Fact]
        public void Category_GetProductBySku_ThrowsIfNotFound()
        {
            Category.ClearExtent();
            var category = new Category("Electronics");

            Assert.Throws<KeyNotFoundException>(() => category.GetProductBySku("NONEXISTENT"));
        }

        [Fact]
        public void Category_RemoveProduct_BySku()
        {
            Category.ClearExtent();
            Product.ClearExtent();
            
            var category = new Category("Electronics");
            var images = new List<string> { "image1.jpg" };
            var product = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            category.AddProduct(product);

            category.RemoveProduct("SKU001");

            Assert.Empty(category.ProductsBySku);
            Assert.False(category.HasProduct("SKU001"));
        }

        [Fact]
        public void Category_RemoveProduct_ThrowsIfNotFound()
        {
            Category.ClearExtent();
            var category = new Category("Electronics");

            Assert.Throws<InvalidOperationException>(() => category.RemoveProduct("NONEXISTENT"));
        }

        [Fact]
        public void Category_GetAllProducts_ReturnsAllProducts()
        {
            Category.ClearExtent();
            Product.ClearExtent();
            
            var category = new Category("Electronics");
            var images = new List<string> { "image1.jpg" };
            var product1 = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            var product2 = new Product("Mouse", "Gaming mouse", "SKU002", 49m, 100, 20, images);
            category.AddProduct(product1);
            category.AddProduct(product2);

            var products = category.GetAllProducts();

            Assert.Equal(2, products.Count);
            Assert.Contains(product1, products);
            Assert.Contains(product2, products);
        }
        
        [Fact]
        public void ComplexScenario_CustomerWithMultipleOrders()
        {
            User.ClearExtent();
            Customer.ClearCustomerExtent();
            Order.ClearExtent();
            
            var address = new Address("123 Main St", "New York", "NY", "10001", "USA");
            var customer = new Customer("johndoe", "john@example.com", "password123",
                                       "John", "Doe", "555-1234", address);

            var order1 = new Order(customer, 10m, 5m);
            var order2 = new Order(customer, 15m, 8m);
            var order3 = new Order(customer, 20m, 10m);

            Assert.Equal(3, customer.Orders.Count);
            Assert.Contains(order1, customer.Orders);
            Assert.Contains(order2, customer.Orders);
            Assert.Contains(order3, customer.Orders);
        }

        [Fact]
        public void ComplexScenario_SupplierWithMultipleProducts()
        {
            Supplier.ClearExtent();
            Product.ClearExtent();
            
            var supplier = new Supplier("ABC Corp", "contact@abc.com", "555-1234");
            var images = new List<string> { "image1.jpg" };
            var product1 = new Product("Laptop", "Gaming laptop", "SKU001", 1299m, 50, 10, images);
            var product2 = new Product("Mouse", "Gaming mouse", "SKU002", 49m, 100, 20, images);
            var product3 = new Product("Keyboard", "Mechanical keyboard", "SKU003", 149m, 75, 15, images);

            supplier.AddProduct(product1);
            supplier.AddProduct(product2);
            supplier.AddProduct(product3);

            Assert.Equal(3, supplier.Products.Count);
            Assert.Contains(product1, supplier.Products);
            Assert.Contains(product2, supplier.Products);
            Assert.Contains(product3, supplier.Products);
        }

        [Fact]
        public void ComplexScenario_CategoryHierarchyWithProducts()
        {
            Category.ClearExtent();
            Product.ClearExtent();
            
            var electronics = new Category("Electronics");
            var computers = new Category("Computers");
            var laptops = new Category("Laptops");
            
            computers.SetParentCategory(electronics);
            laptops.SetParentCategory(computers);
            
            var images = new List<string> { "image1.jpg" };
            var product1 = new Product("Gaming Laptop", "High-end gaming", "SKU001", 1999m, 30, 5, images);
            var product2 = new Product("Business Laptop", "Professional use", "SKU002", 1299m, 50, 10, images);

            laptops.AddProduct(product1);
            laptops.AddProduct(product2);

            Assert.Equal(2, laptops.ProductsBySku.Count);
            Assert.True(laptops.HasProduct("SKU001"));
            Assert.True(laptops.HasProduct("SKU002"));
        }
    }
}
