using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class LegalEntityPayer : Payer
{
    private static readonly List<LegalEntityPayer> _legalEntityExtent = new();

    public LegalEntityPayer(string companyName, string taxId, string legalRepresentative,
        string email, string phoneNumber, Address billingAddress,
        DateTime registrationDate, string? industry = null)
        : base(email, phoneNumber, billingAddress)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name cannot be empty");
        if (string.IsNullOrWhiteSpace(taxId))
            throw new ArgumentException("Tax ID cannot be empty");
        if (string.IsNullOrWhiteSpace(legalRepresentative))
            throw new ArgumentException("Legal representative cannot be empty");
        if (registrationDate > DateTime.Now)
            throw new ArgumentException("Registration date cannot be in the future");

        CompanyName = companyName;
        TaxId = taxId;
        LegalRepresentative = legalRepresentative;
        Industry = industry;
        RegistrationDate = registrationDate;

        _legalEntityExtent.Add(this);
    }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string CompanyName { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string TaxId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string LegalRepresentative { get; set; }

    [StringLength(100)] public string? Industry { get; set; }

    public DateTime RegistrationDate { get; set; }

    public override void DisplayPayerInfo()
    {
        Console.WriteLine($"Legal Entity Payer: {CompanyName}");
        Console.WriteLine($"Tax ID: {TaxId}");
        Console.WriteLine($"Legal Representative: {LegalRepresentative}");
        Console.WriteLine($"Email: {Email}, Phone: {PhoneNumber}");
        Console.WriteLine($"Industry: {Industry ?? "N/A"}");
        Console.WriteLine($"Registered: {RegistrationDate:yyyy-MM-dd}");
        Console.WriteLine($"Billing Address: {BillingAddress}");
    }

    public int GetYearsInBusiness()
    {
        return DateTime.Now.Year - RegistrationDate.Year;
    }

    public static IReadOnlyList<LegalEntityPayer> GetLegalEntityExtent()
    {
        return _legalEntityExtent.AsReadOnly();
    }

    public static void ClearLegalEntityExtent()
    {
        _legalEntityExtent.Clear();
    }
}