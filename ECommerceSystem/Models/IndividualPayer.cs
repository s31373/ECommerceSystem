using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class IndividualPayer : Payer
{
    private static readonly List<IndividualPayer> _individualExtent = new();

    public IndividualPayer(string firstName, string lastName, string email, string phoneNumber,
        Address billingAddress, DateTime dateOfBirth, string? nationalId = null)
        : base(email, phoneNumber, billingAddress)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty");
        if (dateOfBirth > DateTime.Now)
            throw new ArgumentException("Date of birth cannot be in the future");
        if (dateOfBirth.AddYears(18) > DateTime.Now)
            throw new ArgumentException("Payer must be at least 18 years old");

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        NationalId = nationalId;

        _individualExtent.Add(this);
    }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; }

    [StringLength(20)] public string? NationalId { get; set; }

    public DateTime DateOfBirth { get; set; }

    public override void DisplayPayerInfo()
    {
        Console.WriteLine($"Individual Payer: {FirstName} {LastName}");
        Console.WriteLine($"Email: {Email}, Phone: {PhoneNumber}");
        Console.WriteLine($"Date of Birth: {DateOfBirth:yyyy-MM-dd}");
        Console.WriteLine($"Billing Address: {BillingAddress}");
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    public static IReadOnlyList<IndividualPayer> GetIndividualExtent()
    {
        return _individualExtent.AsReadOnly();
    }

    public static void ClearIndividualExtent()
    {
        _individualExtent.Clear();
    }
}