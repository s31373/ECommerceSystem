using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class DeliveryContractor : User
{
    private static readonly List<DeliveryContractor> _deliveryContractorExtent = new();

    public DeliveryContractor(string username, string email, string passwordHash,
        string vehicleType, string licenseNumber, string serviceArea)
        : base(username, email, passwordHash)
    {
        if (string.IsNullOrWhiteSpace(vehicleType))
            throw new ArgumentException("Vehicle type cannot be empty");
        if (vehicleType != "Bike" && vehicleType != "Van" && vehicleType != "Truck")
            throw new ArgumentException("Vehicle type must be Bike, Van, or Truck");
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number cannot be empty");
        if (string.IsNullOrWhiteSpace(serviceArea))
            throw new ArgumentException("Service area cannot be empty");

        VehicleType = vehicleType;
        LicenseNumber = licenseNumber;
        ServiceArea = serviceArea;
        IsAvailable = true;

        _deliveryContractorExtent.Add(this);
    }

    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string VehicleType { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string LicenseNumber { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ServiceArea { get; set; }

    public bool IsAvailable { get; set; }

    public void UpdateShipmentStatus(Shipment shipment, ShipmentStatus newStatus)
    {
        if (shipment == null)
            throw new ArgumentNullException(nameof(shipment));

        // Validate status transition
        if (shipment.Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException("Cannot update delivered shipment");

        if (!IsValidStatusTransition(shipment.Status, newStatus))
            throw new InvalidOperationException($"Invalid status transition from {shipment.Status} to {newStatus}");

        shipment.UpdateStatus(newStatus);
        Console.WriteLine($"Delivery Contractor {Username} updated shipment #{shipment.ShipmentId} to {newStatus}");
    }

    private bool IsValidStatusTransition(ShipmentStatus current, ShipmentStatus next)
    {
        return (current, next) switch
        {
            (ShipmentStatus.Preparing, ShipmentStatus.InTransit) => true,
            (ShipmentStatus.InTransit, ShipmentStatus.OutForDelivery) => true,
            (ShipmentStatus.OutForDelivery, ShipmentStatus.Delivered) => true,
            _ => false
        };
    }

    public static IReadOnlyList<DeliveryContractor> GetDeliveryContractorExtent()
    {
        return _deliveryContractorExtent.AsReadOnly();
    }

    public static void ClearDeliveryContractorExtent()
    {
        _deliveryContractorExtent.Clear();
    }
}