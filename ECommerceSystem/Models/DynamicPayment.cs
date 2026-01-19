using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class DynamicPayment
{
    private static readonly List<DynamicPayment> _extent = new();
    private static int _nextId = 1;

    private DynamicPayment(decimal amount, Order order, string paymentType)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");
        if (order == null)
            throw new ArgumentNullException(nameof(order));
        if (string.IsNullOrWhiteSpace(paymentType))
            throw new ArgumentException("Payment type cannot be empty");

        PaymentId = _nextId++;
        Amount = amount;
        PaymentDate = DateTime.Now;
        Status = PaymentStatus.Pending;
        Order = order;
        PaymentType = paymentType;

        _extent.Add(this);
    }

    [Key] public int PaymentId { get; }

    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }

    public Order Order { get; set; }
    public string PaymentType { get; private set; }

    public string? CardNumber { get; private set; }
    public string? CardholderName { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? PayPalEmail { get; private set; }
    public string? PayPalAccountName { get; private set; }
    public int? LoyaltyPointsUsed { get; private set; }
    public decimal? ConversionRate { get; private set; }

    public static DynamicPayment CreateCreditCardPayment(decimal amount, Order order,
        string cardNumber, string cardholderName, DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number cannot be empty");
        if (string.IsNullOrWhiteSpace(cardholderName))
            throw new ArgumentException("Cardholder name cannot be empty");
        if (expiryDate < DateTime.Now)
            throw new ArgumentException("Card has expired");

        var payment = new DynamicPayment(amount, order, "CreditCard");
        payment.CardNumber = cardNumber;
        payment.CardholderName = cardholderName;
        payment.ExpiryDate = expiryDate;
        return payment;
    }

    public static DynamicPayment CreatePayPalPayment(decimal amount, Order order,
        string payPalEmail, string payPalAccountName)
    {
        if (string.IsNullOrWhiteSpace(payPalEmail))
            throw new ArgumentException("PayPal email cannot be empty");
        if (string.IsNullOrWhiteSpace(payPalAccountName))
            throw new ArgumentException("PayPal account name cannot be empty");

        var payment = new DynamicPayment(amount, order, "PayPal");
        payment.PayPalEmail = payPalEmail;
        payment.PayPalAccountName = payPalAccountName;
        return payment;
    }

    public static DynamicPayment CreateWalletPayment(decimal amount, Order order,
        int loyaltyPointsUsed, decimal conversionRate)
    {
        if (loyaltyPointsUsed < 0)
            throw new ArgumentException("Loyalty points used cannot be negative");
        if (conversionRate <= 0)
            throw new ArgumentException("Conversion rate must be greater than 0");

        var payment = new DynamicPayment(amount, order, "Wallet");
        payment.LoyaltyPointsUsed = loyaltyPointsUsed;
        payment.ConversionRate = conversionRate;
        return payment;
    }

    public void ChangeToCreditCard(string cardNumber, string cardholderName, DateTime expiryDate)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number cannot be empty");
        if (string.IsNullOrWhiteSpace(cardholderName))
            throw new ArgumentException("Cardholder name cannot be empty");
        if (expiryDate < DateTime.Now)
            throw new ArgumentException("Card has expired");
        ClearPayPalFields();
        ClearWalletFields();
        PaymentType = "CreditCard";
        CardNumber = cardNumber;
        CardholderName = cardholderName;
        ExpiryDate = expiryDate;
        Status = PaymentStatus.Pending;
    }

    public void ChangeToPayPal(string payPalEmail, string payPalAccountName)
    {
        if (string.IsNullOrWhiteSpace(payPalEmail))
            throw new ArgumentException("PayPal email cannot be empty");
        if (string.IsNullOrWhiteSpace(payPalAccountName))
            throw new ArgumentException("PayPal account name cannot be empty");
        ClearCreditCardFields();
        ClearWalletFields();
        PaymentType = "PayPal";
        PayPalEmail = payPalEmail;
        PayPalAccountName = payPalAccountName;
        Status = PaymentStatus.Pending;
    }

    public void ChangeToWallet(int loyaltyPointsUsed, decimal conversionRate)
    {
        if (loyaltyPointsUsed < 0)
            throw new ArgumentException("Loyalty points used cannot be negative");
        if (conversionRate <= 0)
            throw new ArgumentException("Conversion rate must be greater than 0");
        ClearCreditCardFields();
        ClearPayPalFields();
        PaymentType = "Wallet";
        LoyaltyPointsUsed = loyaltyPointsUsed;
        ConversionRate = conversionRate;
        Status = PaymentStatus.Pending;
    }

    private void ClearCreditCardFields()
    {
        CardNumber = null;
        CardholderName = null;
        ExpiryDate = null;
    }

    private void ClearPayPalFields()
    {
        PayPalEmail = null;
        PayPalAccountName = null;
    }

    private void ClearWalletFields()
    {
        LoyaltyPointsUsed = null;
        ConversionRate = null;
    }

    public void ProcessCreditCardPayment()
    {
        if (PaymentType != "CreditCard")
            throw new InvalidOperationException(
                $"Cannot process credit card payment when payment type is {PaymentType}");
        if (CardNumber == null || CardholderName == null || ExpiryDate == null)
            throw new InvalidOperationException("Credit card details are not set");

        Console.WriteLine($"Processing credit card payment for {CardholderName}");
        Status = PaymentStatus.Completed;
    }

    public void ProcessPayPalPayment()
    {
        if (PaymentType != "PayPal")
            throw new InvalidOperationException($"Cannot process PayPal payment when payment type is {PaymentType}");
        if (PayPalEmail == null || PayPalAccountName == null)
            throw new InvalidOperationException("PayPal details are not set");

        Console.WriteLine($"Processing PayPal payment for {PayPalAccountName} ({PayPalEmail})");
        Status = PaymentStatus.Completed;
    }

    public void ProcessWalletPayment()
    {
        if (PaymentType != "Wallet")
            throw new InvalidOperationException($"Cannot process wallet payment when payment type is {PaymentType}");
        if (LoyaltyPointsUsed == null || ConversionRate == null)
            throw new InvalidOperationException("Wallet details are not set");

        Console.WriteLine($"Processing wallet payment using {LoyaltyPointsUsed} loyalty points");
        Status = PaymentStatus.Completed;
    }

    public decimal ConvertPointsToMoney()
    {
        if (PaymentType != "Wallet")
            throw new InvalidOperationException($"Cannot convert points when payment type is {PaymentType}");
        if (LoyaltyPointsUsed == null || ConversionRate == null)
            throw new InvalidOperationException("Wallet details are not set");

        return LoyaltyPointsUsed.Value * ConversionRate.Value;
    }

    public void ViewPaymentStatus()
    {
        Console.WriteLine($"Payment #{PaymentId} - Type: {PaymentType}, Status: {Status}");
        Console.WriteLine($"Amount: ${Amount:F2}");
        Console.WriteLine($"Date: {PaymentDate:yyyy-MM-dd HH:mm}");

        switch (PaymentType)
        {
            case "CreditCard":
                Console.WriteLine($"Card: {CardholderName} ending in {CardNumber?.Substring(CardNumber.Length - 4)}");
                break;
            case "PayPal":
                Console.WriteLine($"PayPal: {PayPalAccountName} ({PayPalEmail})");
                break;
            case "Wallet":
                Console.WriteLine($"Loyalty Points: {LoyaltyPointsUsed}");
                break;
        }
    }

    public static IReadOnlyList<DynamicPayment> GetExtent()
    {
        return _extent.AsReadOnly();
    }

    public static void ClearExtent()
    {
        _extent.Clear();
        _nextId = 1;
    }
}