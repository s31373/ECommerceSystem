using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class Shipment
{
    private static readonly List<Shipment> _extent = new();
    private static int _nextId = 1;

    public Shipment(Order order, string carrier, string trackingNumber, DateTime? estimatedDeliveryDate = null)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));
        if (string.IsNullOrWhiteSpace(carrier))
            throw new ArgumentException("Carrier cannot be empty");
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number cannot be empty");
        if (estimatedDeliveryDate.HasValue && estimatedDeliveryDate.Value < DateTime.Now)
            throw new ArgumentException("Estimated delivery date cannot be in the past");

        ShipmentId = _nextId++;
        Order = order;
        Carrier = carrier;
        TrackingNumber = trackingNumber;
        ShipmentDate = DateTime.Now;
        EstimatedDeliveryDate = estimatedDeliveryDate;
        Status = ShipmentStatus.Preparing;

        _extent.Add(this);
    }

    [Key] public int ShipmentId { get; }

    public Order Order { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Carrier { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string TrackingNumber { get; set; }

    public DateTime ShipmentDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }

    public ShipmentStatus Status { get; set; }

    public void UpdateStatus(ShipmentStatus newStatus)
    {
        if (Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException("Cannot update delivered shipment");

        if (newStatus == ShipmentStatus.Delivered) ActualDeliveryDate = DateTime.Now;

        Status = newStatus;
    }

    public void ViewShipmentStatus()
    {
        Console.WriteLine($"Shipment #{ShipmentId} - {Carrier} ({TrackingNumber})");
        Console.WriteLine($"Status: {Status}");
        Console.WriteLine($"Shipped: {ShipmentDate:yyyy-MM-dd}");
        if (EstimatedDeliveryDate.HasValue)
            Console.WriteLine($"Estimated Delivery: {EstimatedDeliveryDate.Value:yyyy-MM-dd}");
        if (ActualDeliveryDate.HasValue)
            Console.WriteLine($"Delivered: {ActualDeliveryDate.Value:yyyy-MM-dd}");
    }

    public static IReadOnlyList<Shipment> GetExtent()
    {
        return _extent.AsReadOnly();
    }

    public static void ClearExtent()
    {
        _extent.Clear();
        _nextId = 1;
    }
}