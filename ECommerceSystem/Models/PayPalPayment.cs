using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class PayPalPayment : Payment
{
    public PayPalPayment(decimal amount, Order order, string payPalEmail, string payPalAccountName)
        : base(amount, order)
    {
        if (string.IsNullOrWhiteSpace(payPalEmail))
            throw new ArgumentException("PayPal email cannot be empty");
        if (string.IsNullOrWhiteSpace(payPalAccountName))
            throw new ArgumentException("PayPal account name cannot be empty");

        PayPalEmail = payPalEmail;
        PayPalAccountName = payPalAccountName;
    }

    [Required] [EmailAddress] public string PayPalEmail { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string PayPalAccountName { get; set; }
}