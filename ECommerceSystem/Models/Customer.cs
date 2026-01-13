using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECommerceSystem.Models
{
    public class Customer : User
    {
        private static List<Customer> _customerExtent = new List<Customer>();

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int LoyaltyPoints { get; set; }

        public Address ShippingAddress { get; set; }
        
        private List<Order> _orders = new List<Order>();
        
        public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

        public Customer(string username, string email, string passwordHash, 
                       string firstName, string lastName, string phoneNumber, Address shippingAddress)
            : base(username, email, passwordHash)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty");
            if (shippingAddress == null)
                throw new ArgumentNullException(nameof(shippingAddress));

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            ShippingAddress = shippingAddress;
            LoyaltyPoints = 0;

            _customerExtent.Add(this);
        }

        public void EarnLoyaltyPoints(int points)
        {
            if (points < 0)
                throw new ArgumentException("Points cannot be negative");
            LoyaltyPoints += points;
        }

        public void ApplyLoyaltyPoints(int points)
        {
            if (points < 0)
                throw new ArgumentException("Points cannot be negative");
            if (points > LoyaltyPoints)
                throw new InvalidOperationException("Insufficient loyalty points");
            LoyaltyPoints -= points;
        }

        public void ViewOrderHistory()
        {
            var orders = Order.GetExtent().Where(o => o.Customer.UserId == this.UserId).ToList();
            Console.WriteLine($"Order history for {FirstName} {LastName}:");
            foreach (var order in orders)
            {
                Console.WriteLine($"Order #{order.OrderId}, Date: {order.OrderDate}, Status: {order.Status}");
            }
        }

        public static IReadOnlyList<Customer> GetCustomerExtent()
        {
            return _customerExtent.AsReadOnly();
        }

        public static void ClearCustomerExtent()
        {
            _customerExtent.Clear();
        }
        
        public void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (_orders.Contains(order))
                throw new InvalidOperationException("Order already exists in customer's order list");
    
            _orders.Add(order);
        }

        public void RemoveOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            if (!_orders.Contains(order))
                throw new InvalidOperationException("Order not found in customer's order list");
    
            _orders.Remove(order);
        }
    }
}
