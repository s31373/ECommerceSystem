using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECommerceSystem.Models
{
    public class Order
    {
        private static List<Order> _extent = new List<Order>();
        private static int _nextId = 1;

        [Key]
        public int OrderId { get; private set; }

        public Customer Customer { get; set; }
        public DateTime OrderDate { get; private set; }

        public OrderStatus Status { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ShippingFee { get; set; }

        [StringLength(500)]
        public string SpecialInstructions { get; set; }

        private List<OrderedItem> _orderedItems;
        public IReadOnlyList<OrderedItem> OrderedItems => _orderedItems.AsReadOnly();

        public decimal Subtotal
        {
            get => _orderedItems.Sum(item => item.Subtotal);
        }

        public decimal Total
        {
            get => Subtotal + TaxAmount + ShippingFee;
        }

        public Order(Customer customer, decimal taxAmount, decimal shippingFee, string specialInstructions = "")
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));
            if (taxAmount < 0)
                throw new ArgumentException("Tax amount cannot be negative");
            if (shippingFee < 0)
                throw new ArgumentException("Shipping fee cannot be negative");

            OrderId = _nextId++;
            Customer = customer;
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;
            TaxAmount = taxAmount;
            ShippingFee = shippingFee;
            SpecialInstructions = specialInstructions ?? "";
            _orderedItems = new List<OrderedItem>();

            _extent.Add(this);
    
            customer.AddOrder(this);
        }

        public void AddOrderedItem(OrderedItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _orderedItems.Add(item);
        }

        public void ViewOrderStatus()
        {
            Console.WriteLine($"Order #{OrderId} - Status: {Status}");
            Console.WriteLine($"Order Date: {OrderDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Total: ${Total:F2}");
        }

        public static IReadOnlyList<Order> GetExtent()
        {
            return _extent.AsReadOnly();
        }

        public static void ClearExtent()
        {
            _extent.Clear();
            _nextId = 1;
        }
        
        public void SetCustomer(Customer newCustomer)
        {
            if (newCustomer == null)
                throw new ArgumentNullException(nameof(newCustomer));
            if (Customer == newCustomer)
                return;
    
            Customer?.RemoveOrder(this);
    
            Customer = newCustomer;
    
            newCustomer.AddOrder(this);
        }
    }
}
