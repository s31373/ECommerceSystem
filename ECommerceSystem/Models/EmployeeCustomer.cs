using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceSystem.Models;

public class EmployeeCustomer : User
{
    private static readonly List<EmployeeCustomer> _extent = new();

    private readonly AdminComponent? _adminComponent;

    private readonly CustomerComponent _customerComponent;
    private readonly DeliveryContractorComponent? _deliveryContractorComponent;

    private readonly InventoryManagerComponent? _inventoryManagerComponent;

    public EmployeeCustomer(
        string username, string email, string passwordHash,
        string firstName, string lastName, string phoneNumber, Address shippingAddress,
        string employeeRole, object employeeRoleData)
        : base(username, email, passwordHash)
    {
        if (string.IsNullOrWhiteSpace(employeeRole))
            throw new ArgumentException("Employee role cannot be empty");
        if (employeeRole != "Admin" && employeeRole != "InventoryManager" && employeeRole != "DeliveryContractor")
            throw new ArgumentException("Employee role must be Admin, InventoryManager, or DeliveryContractor");

        _customerComponent = new CustomerComponent(firstName, lastName, phoneNumber, shippingAddress);

        EmployeeRole = employeeRole;
        switch (employeeRole)
        {
            case "Admin":
                var adminData = employeeRoleData as (string department, string permissionLevel)?;
                if (!adminData.HasValue)
                    throw new ArgumentException("Admin data must be provided as (department, permissionLevel)");
                _adminComponent = new AdminComponent(adminData.Value.department, adminData.Value.permissionLevel);
                break;

            case "InventoryManager":
                var imData = employeeRoleData as (string warehouseLocation, int yearsOfExperience)?;
                if (!imData.HasValue)
                    throw new ArgumentException("Inventory Manager data must be provided");
                _inventoryManagerComponent =
                    new InventoryManagerComponent(imData.Value.warehouseLocation, imData.Value.yearsOfExperience);
                break;

            case "DeliveryContractor":
                var dcData = employeeRoleData as (string vehicleType, string licenseNumber, string serviceArea)?;
                if (!dcData.HasValue)
                    throw new ArgumentException("Delivery Contractor data must be provided");
                _deliveryContractorComponent = new DeliveryContractorComponent(dcData.Value.vehicleType,
                    dcData.Value.licenseNumber, dcData.Value.serviceArea);
                break;
        }

        _extent.Add(this);
    }

    public string FirstName
    {
        get => _customerComponent.FirstName;
        set => _customerComponent.FirstName = value;
    }

    public string LastName
    {
        get => _customerComponent.LastName;
        set => _customerComponent.LastName = value;
    }

    public string PhoneNumber
    {
        get => _customerComponent.PhoneNumber;
        set => _customerComponent.PhoneNumber = value;
    }

    public int LoyaltyPoints
    {
        get => _customerComponent.LoyaltyPoints;
        set => _customerComponent.LoyaltyPoints = value;
    }

    public Address ShippingAddress
    {
        get => _customerComponent.ShippingAddress;
        set => _customerComponent.ShippingAddress = value;
    }

    public string EmployeeRole { get; }

    public string Department
    {
        get
        {
            if (_adminComponent == null)
                throw new InvalidOperationException($"User is not an Admin, they are a {EmployeeRole}");
            return _adminComponent.Department;
        }
        set
        {
            if (_adminComponent == null)
                throw new InvalidOperationException($"User is not an Admin, they are a {EmployeeRole}");
            _adminComponent.Department = value;
        }
    }

    public string PermissionLevel
    {
        get
        {
            if (_adminComponent == null)
                throw new InvalidOperationException($"User is not an Admin, they are a {EmployeeRole}");
            return _adminComponent.PermissionLevel;
        }
        set
        {
            if (_adminComponent == null)
                throw new InvalidOperationException($"User is not an Admin, they are a {EmployeeRole}");
            _adminComponent.PermissionLevel = value;
        }
    }

    public string WarehouseLocation
    {
        get
        {
            if (_inventoryManagerComponent == null)
                throw new InvalidOperationException($"User is not an Inventory Manager, they are a {EmployeeRole}");
            return _inventoryManagerComponent.WarehouseLocation;
        }
        set
        {
            if (_inventoryManagerComponent == null)
                throw new InvalidOperationException($"User is not an Inventory Manager, they are a {EmployeeRole}");
            _inventoryManagerComponent.WarehouseLocation = value;
        }
    }

    public int YearsOfExperience
    {
        get
        {
            if (_inventoryManagerComponent == null)
                throw new InvalidOperationException($"User is not an Inventory Manager, they are a {EmployeeRole}");
            return _inventoryManagerComponent.YearsOfExperience;
        }
        set
        {
            if (_inventoryManagerComponent == null)
                throw new InvalidOperationException($"User is not an Inventory Manager, they are a {EmployeeRole}");
            _inventoryManagerComponent.YearsOfExperience = value;
        }
    }

    public string VehicleType
    {
        get
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            return _deliveryContractorComponent.VehicleType;
        }
        set
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            _deliveryContractorComponent.VehicleType = value;
        }
    }

    public string LicenseNumber
    {
        get
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            return _deliveryContractorComponent.LicenseNumber;
        }
        set
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            _deliveryContractorComponent.LicenseNumber = value;
        }
    }

    public string ServiceArea
    {
        get
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            return _deliveryContractorComponent.ServiceArea;
        }
        set
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            _deliveryContractorComponent.ServiceArea = value;
        }
    }

    public bool IsAvailable
    {
        get
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            return _deliveryContractorComponent.IsAvailable;
        }
        set
        {
            if (_deliveryContractorComponent == null)
                throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
            _deliveryContractorComponent.IsAvailable = value;
        }
    }

    public void EarnLoyaltyPoints(int points)
    {
        _customerComponent.EarnLoyaltyPoints(points);
    }

    public void ApplyLoyaltyPoints(int points)
    {
        _customerComponent.ApplyLoyaltyPoints(points);
    }

    public void ViewOrderHistory()
    {
        _customerComponent.ViewOrderHistory(UserId);
    }

    public void ManageUserAccounts()
    {
        if (_adminComponent == null)
            throw new InvalidOperationException($"User is not an Admin, they are a {EmployeeRole}");
        _adminComponent.ManageUserAccounts(Username);
    }

    public void ViewSystemConfiguration()
    {
        if (_adminComponent == null)
            throw new InvalidOperationException($"User is not an Admin, they are a {EmployeeRole}");
        _adminComponent.ViewSystemConfiguration(Username);
    }

    public void MonitorStock()
    {
        if (_inventoryManagerComponent == null)
            throw new InvalidOperationException($"User is not an Inventory Manager, they are a {EmployeeRole}");
        _inventoryManagerComponent.MonitorStock(Username);
    }

    public void CoordinateWithSuppliers()
    {
        if (_inventoryManagerComponent == null)
            throw new InvalidOperationException($"User is not an Inventory Manager, they are a {EmployeeRole}");
        _inventoryManagerComponent.CoordinateWithSuppliers(Username);
    }

    public void UpdateShipmentStatus(Shipment shipment, ShipmentStatus newStatus)
    {
        if (_deliveryContractorComponent == null)
            throw new InvalidOperationException($"User is not a Delivery Contractor, they are a {EmployeeRole}");
        _deliveryContractorComponent.UpdateShipmentStatus(shipment, newStatus, Username);
    }

    public new static IReadOnlyList<EmployeeCustomer> GetExtent()
    {
        return _extent.AsReadOnly();
    }

    public new static void ClearExtent()
    {
        _extent.Clear();
    }

    private class CustomerComponent
    {
        public CustomerComponent(string firstName, string lastName, string phoneNumber, Address shippingAddress)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty");
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty");
            if (shippingAddress == null)
                throw new ArgumentNullException(nameof(shippingAddress));

            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            ShippingAddress = shippingAddress;
            LoyaltyPoints = 0;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int LoyaltyPoints { get; set; }
        public Address ShippingAddress { get; set; }

        public void EarnLoyaltyPoints(int points)
        {
            if (points < 0)
                throw new ArgumentException("Points cannot be negative");
            LoyaltyPoints += points;
        }

        public void ApplyLoyaltyPoints(int points)
        {
            if (points < 0)
                throw new ArgumentException("Points cannot be negative");
            if (points > LoyaltyPoints)
                throw new InvalidOperationException("Insufficient loyalty points");
            LoyaltyPoints -= points;
        }

        public void ViewOrderHistory(int userId)
        {
            var orders = Order.GetExtent().Where(o => o.Customer.UserId == userId).ToList();
            Console.WriteLine($"Order history for {FirstName} {LastName}:");
            foreach (var order in orders)
                Console.WriteLine($"Order #{order.OrderId}, Date: {order.OrderDate}, Status: {order.Status}");
        }
    }

    private class AdminComponent
    {
        public AdminComponent(string department, string permissionLevel)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Department cannot be empty");
            if (string.IsNullOrWhiteSpace(permissionLevel))
                throw new ArgumentException("Permission level cannot be empty");
            if (permissionLevel != "Basic" && permissionLevel != "Intermediate" && permissionLevel != "Full")
                throw new ArgumentException("Permission level must be Basic, Intermediate, or Full");

            Department = department;
            PermissionLevel = permissionLevel;
            HireDate = DateTime.Now;
        }

        public string Department { get; set; }
        public string PermissionLevel { get; set; }
        public DateTime HireDate { get; set; }

        public void ManageUserAccounts(string username)
        {
            Console.WriteLine($"Admin {username} managing user accounts with {PermissionLevel} permissions");
        }

        public void ViewSystemConfiguration(string username)
        {
            Console.WriteLine($"Admin {username} from {Department} viewing system configuration");
        }
    }

    private class InventoryManagerComponent
    {
        public InventoryManagerComponent(string warehouseLocation, int yearsOfExperience)
        {
            if (string.IsNullOrWhiteSpace(warehouseLocation))
                throw new ArgumentException("Warehouse location cannot be empty");
            if (yearsOfExperience < 0)
                throw new ArgumentException("Years of experience cannot be negative");

            WarehouseLocation = warehouseLocation;
            YearsOfExperience = yearsOfExperience;
            HireDate = DateTime.Now;
        }

        public string WarehouseLocation { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime HireDate { get; set; }

        public void MonitorStock(string username)
        {
            var lowStockProducts = Product.GetExtent().Where(p => p.CheckIfLowStock()).ToList();
            Console.WriteLine($"Inventory Manager {username} monitoring {lowStockProducts.Count} low-stock products");
            foreach (var product in lowStockProducts)
                Console.WriteLine($"- {product.Name}: {product.StockQuantity} units (minimum: {product.MinimumStock})");
        }

        public void CoordinateWithSuppliers(string username)
        {
            Console.WriteLine($"Inventory Manager {username} coordinating with suppliers from {WarehouseLocation}");
        }
    }

    private class DeliveryContractorComponent
    {
        public DeliveryContractorComponent(string vehicleType, string licenseNumber, string serviceArea)
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
        }

        public string VehicleType { get; set; }
        public string LicenseNumber { get; set; }
        public string ServiceArea { get; set; }
        public bool IsAvailable { get; set; }

        public void UpdateShipmentStatus(Shipment shipment, ShipmentStatus newStatus, string username)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (shipment.Status == ShipmentStatus.Delivered)
                throw new InvalidOperationException("Cannot update delivered shipment");

            if (!IsValidStatusTransition(shipment.Status, newStatus))
                throw new InvalidOperationException($"Invalid status transition from {shipment.Status} to {newStatus}");

            shipment.UpdateStatus(newStatus);
            Console.WriteLine($"Delivery Contractor {username} updated shipment #{shipment.ShipmentId} to {newStatus}");
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
    }
}