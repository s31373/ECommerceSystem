using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECommerceSystem.Models;

public class CategorizedProduct
{
    private static readonly List<CategorizedProduct> _extent = new();
    private static int _nextId = 1;


    private CategorizedProduct(string name, string description, string sku, decimal price,
        int stockQuantity, string availabilityType, string conditionType)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty");
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be empty");
        if (price <= 0)
            throw new ArgumentException("Price must be greater than 0");
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative");

        ProductId = _nextId++;
        Name = name;
        Description = description;
        SKU = sku;
        Price = price;
        StockQuantity = stockQuantity;
        AvailabilityType = availabilityType;
        ConditionType = conditionType;

        _extent.Add(this);
    }

    [Key] public int ProductId { get; private set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; }

    [StringLength(2000)] public string Description { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string SKU { get; set; }

    [Range(0.01, double.MaxValue)] public decimal Price { get; set; }

    [Range(0, int.MaxValue)] public int StockQuantity { get; set; }

    public string AvailabilityType { get; }


    public double? Weight { get; private set; }
    public double? Length { get; private set; }
    public double? Width { get; private set; }
    public double? Height { get; private set; }
    public string? WarehouseLocation { get; private set; }


    public long? FileSizeInMB { get; private set; }
    public string? DownloadUrl { get; private set; }
    public string? FileFormat { get; private set; }
    public string? LicenseKey { get; private set; }
    public string ConditionType { get; }


    public DateTime? ManufactureDate { get; private set; }
    public string? WarrantyPeriod { get; private set; }
    public bool? IsSealed { get; private set; }


    public DateTime? RefurbishmentDate { get; private set; }
    public string? RefurbishmentGrade { get; private set; }
    public string? PreviousOwnerType { get; private set; }
    public int? UsageHours { get; private set; }


    public static CategorizedProduct CreatePhysicalNewProduct(
        string name, string description, string sku, decimal price, int stockQuantity,
        double weight, double length, double width, double height, string warehouseLocation,
        DateTime manufactureDate, string warrantyPeriod, bool isSealed)
    {
        if (weight <= 0) throw new ArgumentException("Weight must be positive");
        if (length <= 0 || width <= 0 || height <= 0) throw new ArgumentException("Dimensions must be positive");
        if (string.IsNullOrWhiteSpace(warehouseLocation))
            throw new ArgumentException("Warehouse location cannot be empty");
        if (manufactureDate > DateTime.Now) throw new ArgumentException("Manufacture date cannot be in the future");
        if (string.IsNullOrWhiteSpace(warrantyPeriod)) throw new ArgumentException("Warranty period cannot be empty");

        var product = new CategorizedProduct(name, description, sku, price, stockQuantity, "Physical", "New");
        product.Weight = weight;
        product.Length = length;
        product.Width = width;
        product.Height = height;
        product.WarehouseLocation = warehouseLocation;
        product.ManufactureDate = manufactureDate;
        product.WarrantyPeriod = warrantyPeriod;
        product.IsSealed = isSealed;
        return product;
    }


    public static CategorizedProduct CreatePhysicalRefurbishedProduct(
        string name, string description, string sku, decimal price, int stockQuantity,
        double weight, double length, double width, double height, string warehouseLocation,
        DateTime refurbishmentDate, string refurbishmentGrade, string previousOwnerType, int usageHours)
    {
        if (weight <= 0) throw new ArgumentException("Weight must be positive");
        if (length <= 0 || width <= 0 || height <= 0) throw new ArgumentException("Dimensions must be positive");
        if (string.IsNullOrWhiteSpace(warehouseLocation))
            throw new ArgumentException("Warehouse location cannot be empty");
        if (refurbishmentDate > DateTime.Now) throw new ArgumentException("Refurbishment date cannot be in the future");
        if (string.IsNullOrWhiteSpace(refurbishmentGrade))
            throw new ArgumentException("Refurbishment grade cannot be empty");
        if (refurbishmentGrade != "A" && refurbishmentGrade != "B" && refurbishmentGrade != "C")
            throw new ArgumentException("Refurbishment grade must be A, B, or C");
        if (usageHours < 0) throw new ArgumentException("Usage hours cannot be negative");

        var product = new CategorizedProduct(name, description, sku, price, stockQuantity, "Physical", "Refurbished");
        product.Weight = weight;
        product.Length = length;
        product.Width = width;
        product.Height = height;
        product.WarehouseLocation = warehouseLocation;
        product.RefurbishmentDate = refurbishmentDate;
        product.RefurbishmentGrade = refurbishmentGrade;
        product.PreviousOwnerType = previousOwnerType;
        product.UsageHours = usageHours;
        return product;
    }


    public static CategorizedProduct CreateDigitalNewProduct(
        string name, string description, string sku, decimal price, int stockQuantity,
        long fileSizeInMB, string downloadUrl, string fileFormat, string licenseKey,
        DateTime releaseDate, string warrantyPeriod)
    {
        if (fileSizeInMB <= 0) throw new ArgumentException("File size must be positive");
        if (string.IsNullOrWhiteSpace(downloadUrl)) throw new ArgumentException("Download URL cannot be empty");
        if (string.IsNullOrWhiteSpace(fileFormat)) throw new ArgumentException("File format cannot be empty");
        if (string.IsNullOrWhiteSpace(licenseKey)) throw new ArgumentException("License key cannot be empty");
        if (releaseDate > DateTime.Now) throw new ArgumentException("Release date cannot be in the future");
        if (string.IsNullOrWhiteSpace(warrantyPeriod)) throw new ArgumentException("Warranty period cannot be empty");

        var product = new CategorizedProduct(name, description, sku, price, stockQuantity, "Digital", "New");
        product.FileSizeInMB = fileSizeInMB;
        product.DownloadUrl = downloadUrl;
        product.FileFormat = fileFormat;
        product.LicenseKey = licenseKey;
        product.ManufactureDate = releaseDate;
        product.WarrantyPeriod = warrantyPeriod;
        product.IsSealed = true;
        return product;
    }


    public static CategorizedProduct CreateDigitalRefurbishedProduct(
        string name, string description, string sku, decimal price, int stockQuantity,
        long fileSizeInMB, string downloadUrl, string fileFormat, string licenseKey,
        DateTime transferDate, string refurbishmentGrade, string previousOwnerType)
    {
        if (fileSizeInMB <= 0) throw new ArgumentException("File size must be positive");
        if (string.IsNullOrWhiteSpace(downloadUrl)) throw new ArgumentException("Download URL cannot be empty");
        if (string.IsNullOrWhiteSpace(fileFormat)) throw new ArgumentException("File format cannot be empty");
        if (string.IsNullOrWhiteSpace(licenseKey)) throw new ArgumentException("License key cannot be empty");
        if (transferDate > DateTime.Now) throw new ArgumentException("Transfer date cannot be in the future");
        if (string.IsNullOrWhiteSpace(refurbishmentGrade))
            throw new ArgumentException("Refurbishment grade cannot be empty");

        var product = new CategorizedProduct(name, description, sku, price, stockQuantity, "Digital", "Refurbished");
        product.FileSizeInMB = fileSizeInMB;
        product.DownloadUrl = downloadUrl;
        product.FileFormat = fileFormat;
        product.LicenseKey = licenseKey;
        product.RefurbishmentDate = transferDate;
        product.RefurbishmentGrade = refurbishmentGrade;
        product.PreviousOwnerType = previousOwnerType;
        return product;
    }


    public void CalculateShippingCost()
    {
        if (AvailabilityType != "Physical")
            throw new InvalidOperationException($"Cannot calculate shipping cost for {AvailabilityType} product");
        if (Weight == null || Length == null || Width == null || Height == null)
            throw new InvalidOperationException("Physical dimensions not set");

        var volumetricWeight = Length.Value * Width.Value * Height.Value / 5000;
        var actualWeight = Weight.Value;
        var chargeableWeight = Math.Max(volumetricWeight, actualWeight);

        Console.WriteLine($"Shipping cost calculation for {Name}:");
        Console.WriteLine($"Actual weight: {actualWeight}kg, Volumetric weight: {volumetricWeight:F2}kg");
        Console.WriteLine($"Chargeable weight: {chargeableWeight:F2}kg");
    }

    public void GenerateDownloadLink()
    {
        if (AvailabilityType != "Digital")
            throw new InvalidOperationException($"Cannot generate download link for {AvailabilityType} product");
        if (DownloadUrl == null || LicenseKey == null)
            throw new InvalidOperationException("Digital product details not set");

        Console.WriteLine($"Download link for {Name}:");
        Console.WriteLine($"URL: {DownloadUrl}");
        Console.WriteLine($"License Key: {LicenseKey}");
        Console.WriteLine($"File Format: {FileFormat}, Size: {FileSizeInMB}MB");
    }


    public void DisplayWarrantyInfo()
    {
        if (ConditionType != "New")
            throw new InvalidOperationException($"Warranty info not available for {ConditionType} product");
        if (ManufactureDate == null || WarrantyPeriod == null)
            throw new InvalidOperationException("Warranty details not set");

        Console.WriteLine($"Warranty information for {Name}:");
        Console.WriteLine($"Manufacture Date: {ManufactureDate.Value:yyyy-MM-dd}");
        Console.WriteLine($"Warranty Period: {WarrantyPeriod}");
        Console.WriteLine($"Sealed: {IsSealed}");
    }

    public void DisplayRefurbishmentInfo()
    {
        if (ConditionType != "Refurbished")
            throw new InvalidOperationException($"Refurbishment info not available for {ConditionType} product");
        if (RefurbishmentDate == null || RefurbishmentGrade == null)
            throw new InvalidOperationException("Refurbishment details not set");

        Console.WriteLine($"Refurbishment information for {Name}:");
        Console.WriteLine($"Refurbishment Date: {RefurbishmentDate.Value:yyyy-MM-dd}");
        Console.WriteLine($"Grade: {RefurbishmentGrade}");
        Console.WriteLine($"Previous Owner: {PreviousOwnerType}");
        if (AvailabilityType == "Physical" && UsageHours.HasValue)
            Console.WriteLine($"Usage Hours: {UsageHours}");
    }

    public void DisplayFullProductInfo()
    {
        Console.WriteLine("\n=== Product Information ===");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"SKU: {SKU}");
        Console.WriteLine($"Price: ${Price:F2}");
        Console.WriteLine($"Stock: {StockQuantity}");
        Console.WriteLine($"Type: {AvailabilityType} - {ConditionType}");
        Console.WriteLine($"Description: {Description}");
    }

    public static IReadOnlyList<CategorizedProduct> GetExtent()
    {
        return _extent.AsReadOnly();
    }

    public static IReadOnlyList<CategorizedProduct> GetPhysicalProducts()
    {
        return _extent.Where(p => p.AvailabilityType == "Physical").ToList().AsReadOnly();
    }

    public static IReadOnlyList<CategorizedProduct> GetDigitalProducts()
    {
        return _extent.Where(p => p.AvailabilityType == "Digital").ToList().AsReadOnly();
    }

    public static IReadOnlyList<CategorizedProduct> GetNewProducts()
    {
        return _extent.Where(p => p.ConditionType == "New").ToList().AsReadOnly();
    }

    public static IReadOnlyList<CategorizedProduct> GetRefurbishedProducts()
    {
        return _extent.Where(p => p.ConditionType == "Refurbished").ToList().AsReadOnly();
    }

    public static void ClearExtent()
    {
        _extent.Clear();
        _nextId = 1;
    }
}