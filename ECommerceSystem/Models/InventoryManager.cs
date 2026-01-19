using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ECommerceSystem.Models;

public class InventoryManager : User
{
    private static readonly List<InventoryManager> _inventoryManagerExtent = new();

    public InventoryManager(string username, string email, string passwordHash,
        string warehouseLocation, int yearsOfExperience)
        : base(username, email, passwordHash)
    {
        if (string.IsNullOrWhiteSpace(warehouseLocation))
            throw new ArgumentException("Warehouse location cannot be empty");
        if (yearsOfExperience < 0)
            throw new ArgumentException("Years of experience cannot be negative");

        WarehouseLocation = warehouseLocation;
        YearsOfExperience = yearsOfExperience;
        HireDate = DateTime.Now;

        _inventoryManagerExtent.Add(this);
    }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string WarehouseLocation { get; set; }

    public DateTime HireDate { get; set; }

    [Range(0, int.MaxValue)] public int YearsOfExperience { get; set; }

    public void MonitorStock()
    {
        var lowStockProducts = Product.GetExtent().Where(p => p.CheckIfLowStock()).ToList();
        Console.WriteLine($"Inventory Manager {Username} monitoring {lowStockProducts.Count} low-stock products");
        foreach (var product in lowStockProducts)
            Console.WriteLine($"- {product.Name}: {product.StockQuantity} units (minimum: {product.MinimumStock})");
    }

    public void CoordinateWithSuppliers()
    {
        Console.WriteLine($"Inventory Manager {Username} coordinating with suppliers from {WarehouseLocation}");
    }

    public static IReadOnlyList<InventoryManager> GetInventoryManagerExtent()
    {
        return _inventoryManagerExtent.AsReadOnly();
    }

    public static void ClearInventoryManagerExtent()
    {
        _inventoryManagerExtent.Clear();
    }
}