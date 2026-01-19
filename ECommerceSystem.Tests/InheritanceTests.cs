using ECommerceSystem.Models;
using Xunit;

namespace ECommerceSystem.Tests;

public class InheritanceTests
{
    [Fact]
    public void Admin_Creation_Success()
    {
        var admin = new Admin("admin1", "admin@test.com", "password123",
            "IT", "Full");

        Assert.Equal("admin1", admin.Username);
        Assert.Equal("IT", admin.Department);
        Assert.Equal("Full", admin.PermissionLevel);
    }

    [Fact]
    public void Admin_InvalidPermissionLevel_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Admin("admin1", "admin@test.com", "password123", "IT", "SuperUser"));
    }

    [Fact]
    public void InventoryManager_MonitorStock_Success()
    {
        User.ClearExtent();
        Product.ClearExtent();
        InventoryManager.ClearInventoryManagerExtent();

        var manager = new InventoryManager("manager1", "manager@test.com", "password123",
            "Warehouse A", 5);
        var product = new Product("Low Stock Item", "Test", "SKU001", 100m, 5, 10,
            new List<string> { "img1.jpg" });

        manager.MonitorStock();

        Assert.True(product.CheckIfLowStock());
    }

    [Fact]
    public void DeliveryContractor_UpdateShipmentStatus_ValidTransition_Success()
    {
        User.ClearExtent();
        Customer.ClearCustomerExtent();
        Order.ClearExtent();
        Shipment.ClearExtent();
        DeliveryContractor.ClearDeliveryContractorExtent();

        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var customer = new Customer("user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address);
        var order = new Order(customer, 10m, 5m);
        var shipment = new Shipment(order, "FedEx", "TRACK123", DateTime.Now.AddDays(3));
        var contractor = new DeliveryContractor("driver1", "driver@test.com", "password123",
            "Van", "DL123456", "Downtown");

        contractor.UpdateShipmentStatus(shipment, ShipmentStatus.InTransit);

        Assert.Equal(ShipmentStatus.InTransit, shipment.Status);
    }

    [Fact]
    public void DeliveryContractor_UpdateShipmentStatus_InvalidTransition_ThrowsException()
    {
        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var customer = new Customer("user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address);
        var order = new Order(customer, 10m, 5m);
        var shipment = new Shipment(order, "FedEx", "TRACK123", DateTime.Now.AddDays(3));
        var contractor = new DeliveryContractor("driver1", "driver@test.com", "password123",
            "Van", "DL123456", "Downtown");

        Assert.Throws<InvalidOperationException>(() =>
            contractor.UpdateShipmentStatus(shipment, ShipmentStatus.Delivered));
    }

    [Fact]
    public void EmployeeCustomer_OverlappingInheritance_AdminRole_Success()
    {
        User.ClearExtent();
        EmployeeCustomer.ClearExtent();

        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var employeeCustomer = new EmployeeCustomer(
            "user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address,
            "Admin", ("IT", "Full"));

        Assert.Equal("John", employeeCustomer.FirstName);
        Assert.Equal("Doe", employeeCustomer.LastName);
        Assert.Equal("Admin", employeeCustomer.EmployeeRole);
        Assert.Equal("IT", employeeCustomer.Department);
        Assert.Equal("Full", employeeCustomer.PermissionLevel);
    }

    [Fact]
    public void EmployeeCustomer_CallAdminMethod_WhenInventoryManager_ThrowsException()
    {
        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var employeeCustomer = new EmployeeCustomer(
            "user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address,
            "InventoryManager", ("Warehouse A", 5));

        Assert.Throws<InvalidOperationException>(() => employeeCustomer.ManageUserAccounts());
    }

    [Fact]
    public void EmployeeCustomer_CustomerFunctionality_WorksRegardlessOfEmployeeRole()
    {
        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var employeeCustomer = new EmployeeCustomer(
            "user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address,
            "DeliveryContractor", ("Van", "DL123", "Downtown"));

        employeeCustomer.EarnLoyaltyPoints(100);

        Assert.Equal(100, employeeCustomer.LoyaltyPoints);
    }

    [Fact]
    public void DynamicPayment_CreateCreditCard_Success()
    {
        Order.ClearExtent();
        DynamicPayment.ClearExtent();

        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var customer = new Customer("user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address);
        var order = new Order(customer, 10m, 5m);

        var payment = DynamicPayment.CreateCreditCardPayment(
            100m, order, "4111111111111111", "John Doe", DateTime.Now.AddYears(2));

        Assert.Equal("CreditCard", payment.PaymentType);
        Assert.Equal("4111111111111111", payment.CardNumber);
    }

    [Fact]
    public void DynamicPayment_ChangeToCreditCard_FromPayPal_Success()
    {
        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var customer = new Customer("user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address);
        var order = new Order(customer, 10m, 5m);
        var payment = DynamicPayment.CreatePayPalPayment(
            100m, order, "john@paypal.com", "John Doe");

        payment.ChangeToCreditCard("4111111111111111", "John Doe", DateTime.Now.AddYears(2));

        Assert.Equal("CreditCard", payment.PaymentType);
        Assert.Equal("4111111111111111", payment.CardNumber);
        Assert.Null(payment.PayPalEmail);
    }

    [Fact]
    public void DynamicPayment_ProcessPayPalWhenCreditCard_ThrowsException()
    {
        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var customer = new Customer("user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address);
        var order = new Order(customer, 10m, 5m);
        var payment = DynamicPayment.CreateCreditCardPayment(
            100m, order, "4111111111111111", "John Doe", DateTime.Now.AddYears(2));

        Assert.Throws<InvalidOperationException>(() => payment.ProcessPayPalPayment());
    }

    [Fact]
    public void DynamicPayment_ConvertPointsToMoney_Success()
    {
        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var customer = new Customer("user1", "user@test.com", "password123",
            "John", "Doe", "1234567890", address);
        var order = new Order(customer, 10m, 5m);
        var payment = DynamicPayment.CreateWalletPayment(100m, order, 1000, 0.1m);

        var money = payment.ConvertPointsToMoney();

        Assert.Equal(100m, money);
    }

    [Fact]
    public void IndividualPayer_Creation_Success()
    {
        Payer.ClearExtent();
        IndividualPayer.ClearIndividualExtent();

        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var dob = DateTime.Now.AddYears(-25);

        var payer = new IndividualPayer("John", "Doe", "john@test.com", "1234567890",
            address, dob, "SSN123");

        Assert.Equal("John", payer.FirstName);
        Assert.Equal("Doe", payer.LastName);
        Assert.Equal(25, payer.GetAge());
    }

    [Fact]
    public void IndividualPayer_UnderAge_ThrowsException()
    {
        var address = new Address("123 Main St", "City", "State", "12345", "Country");
        var dob = DateTime.Now.AddYears(-17);
        Assert.Throws<ArgumentException>(() =>
            new IndividualPayer("John", "Doe", "john@test.com", "1234567890",
                address, dob));
    }

    [Fact]
    public void LegalEntityPayer_Creation_Success()
    {
        Payer.ClearExtent();
        LegalEntityPayer.ClearLegalEntityExtent();

        var address = new Address("123 Business St", "City", "State", "12345", "Country");
        var regDate = DateTime.Now.AddYears(-5);

        var payer = new LegalEntityPayer("Tech Corp", "TAX123", "Jane Smith",
            "corp@test.com", "9876543210", address,
            regDate, "Technology");

        Assert.Equal("Tech Corp", payer.CompanyName);
        Assert.Equal("TAX123", payer.TaxId);
        Assert.Equal(5, payer.GetYearsInBusiness());
    }

    [Fact]
    public void CategorizedProduct_PhysicalNew_Creation_Success()
    {
        CategorizedProduct.ClearExtent();

        var product = CategorizedProduct.CreatePhysicalNewProduct(
            "Laptop", "High-end laptop", "SKU001", 1200m, 50,
            2.5, 35, 25, 2, "Warehouse A",
            DateTime.Now.AddMonths(-1), "2 years", true);

        Assert.Equal("Physical", product.AvailabilityType);
        Assert.Equal("New", product.ConditionType);
        Assert.Equal(2.5, product.Weight);
        Assert.True(product.IsSealed);
    }

    [Fact]
    public void CategorizedProduct_PhysicalRefurbished_Creation_Success()
    {
        CategorizedProduct.ClearExtent();

        var product = CategorizedProduct.CreatePhysicalRefurbishedProduct(
            "Laptop", "Refurbished laptop", "SKU002", 800m, 20,
            2.5, 35, 25, 2, "Warehouse B",
            DateTime.Now.AddMonths(-1), "A", "Corporate", 500);

        Assert.Equal("Physical", product.AvailabilityType);
        Assert.Equal("Refurbished", product.ConditionType);
        Assert.Equal("A", product.RefurbishmentGrade);
        Assert.Equal(500, product.UsageHours);
    }

    [Fact]
    public void CategorizedProduct_DigitalNew_Creation_Success()
    {
        CategorizedProduct.ClearExtent();

        var product = CategorizedProduct.CreateDigitalNewProduct(
            "Software", "Productivity software", "SKU003", 99m, 1000,
            500, "https://download.example.com/software", "EXE", "LICENSE-KEY-123",
            DateTime.Now.AddMonths(-2), "1 year");

        Assert.Equal("Digital", product.AvailabilityType);
        Assert.Equal("New", product.ConditionType);
        Assert.Equal(500, product.FileSizeInMB);
        Assert.Equal("LICENSE-KEY-123", product.LicenseKey);
    }

    [Fact]
    public void CategorizedProduct_DigitalRefurbished_Creation_Success()
    {
        CategorizedProduct.ClearExtent();

        var product = CategorizedProduct.CreateDigitalRefurbishedProduct(
            "Software", "Transferred license", "SKU004", 50m, 100,
            500, "https://download.example.com/software", "EXE", "TRANSFERRED-KEY",
            DateTime.Now.AddMonths(-1), "B", "Individual");

        Assert.Equal("Digital", product.AvailabilityType);
        Assert.Equal("Refurbished", product.ConditionType);
        Assert.Equal("B", product.RefurbishmentGrade);
    }

    [Fact]
    public void CategorizedProduct_CallShippingOnDigital_ThrowsException()
    {
        var product = CategorizedProduct.CreateDigitalNewProduct(
            "Software", "Productivity software", "SKU003", 99m, 1000,
            500, "https://download.example.com/software", "EXE", "LICENSE-KEY-123",
            DateTime.Now.AddMonths(-2), "1 year");

        Assert.Throws<InvalidOperationException>(() => product.CalculateShippingCost());
    }

    [Fact]
    public void CategorizedProduct_CallDownloadLinkOnPhysical_ThrowsException()
    {
        var product = CategorizedProduct.CreatePhysicalNewProduct(
            "Laptop", "High-end laptop", "SKU001", 1200m, 50,
            2.5, 35, 25, 2, "Warehouse A",
            DateTime.Now.AddMonths(-1), "2 years", true);

        Assert.Throws<InvalidOperationException>(() => product.GenerateDownloadLink());
    }

    [Fact]
    public void CategorizedProduct_CallWarrantyOnRefurbished_ThrowsException()
    {
        var product = CategorizedProduct.CreatePhysicalRefurbishedProduct(
            "Laptop", "Refurbished laptop", "SKU002", 800m, 20,
            2.5, 35, 25, 2, "Warehouse B",
            DateTime.Now.AddMonths(-1), "A", "Corporate", 500);

        Assert.Throws<InvalidOperationException>(() => product.DisplayWarrantyInfo());
    }

    [Fact]
    public void CategorizedProduct_CallRefurbishmentOnNew_ThrowsException()
    {
        var product = CategorizedProduct.CreatePhysicalNewProduct(
            "Laptop", "High-end laptop", "SKU001", 1200m, 50,
            2.5, 35, 25, 2, "Warehouse A",
            DateTime.Now.AddMonths(-1), "2 years", true);

        Assert.Throws<InvalidOperationException>(() => product.DisplayRefurbishmentInfo());
    }

    [Fact]
    public void CategorizedProduct_GetPhysicalProducts_ReturnsOnlyPhysical()
    {
        CategorizedProduct.ClearExtent();
        var physical1 = CategorizedProduct.CreatePhysicalNewProduct(
            "Laptop", "High-end laptop", "SKU001", 1200m, 50,
            2.5, 35, 25, 2, "Warehouse A",
            DateTime.Now.AddMonths(-1), "2 years", true);
        var digital1 = CategorizedProduct.CreateDigitalNewProduct(
            "Software", "Productivity software", "SKU003", 99m, 1000,
            500, "https://download.example.com/software", "EXE", "LICENSE-KEY-123",
            DateTime.Now.AddMonths(-2), "1 year");

        var physicalProducts = CategorizedProduct.GetPhysicalProducts();

        Assert.Single(physicalProducts);
        Assert.Equal("Physical", physicalProducts[0].AvailabilityType);
    }
}