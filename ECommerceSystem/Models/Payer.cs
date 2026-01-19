using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public abstract class Payer
{
    private static readonly List<Payer> _extent = new();
    private static int _nextId = 1;

    protected Payer(string email, string phoneNumber, Address billingAddress)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty");
        if (billingAddress == null)
            throw new ArgumentNullException(nameof(billingAddress));

        PayerId = _nextId++;
        Email = email;
        PhoneNumber = phoneNumber;
        BillingAddress = billingAddress;

        _extent.Add(this);
    }

    [Key] public int PayerId { get; private set; }

    [Required] [EmailAddress] public string Email { get; set; }

    [Required] [Phone] public string PhoneNumber { get; set; }

    public Address BillingAddress { get; set; }

    public abstract void DisplayPayerInfo();

    public static IReadOnlyList<Payer> GetExtent()
    {
        return _extent.AsReadOnly();
    }

    public static void ClearExtent()
    {
        _extent.Clear();
        _nextId = 1;
    }
}