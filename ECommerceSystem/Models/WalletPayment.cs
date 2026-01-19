using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class WalletPayment : Payment
{
    public WalletPayment(decimal amount, Order order, int loyaltyPointsUsed, decimal conversionRate)
        : base(amount, order)
    {
        if (loyaltyPointsUsed < 0)
            throw new ArgumentException("Loyalty points used cannot be negative");
        if (conversionRate <= 0)
            throw new ArgumentException("Conversion rate must be greater than 0");

        LoyaltyPointsUsed = loyaltyPointsUsed;
        ConversionRate = conversionRate;
    }

    [Range(0, int.MaxValue)] public int LoyaltyPointsUsed { get; set; }

    [Range(0, double.MaxValue)] public decimal ConversionRate { get; set; }

    public decimal ConvertPointsToMoney()
    {
        return LoyaltyPointsUsed * ConversionRate;
    }
}