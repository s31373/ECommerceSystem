using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceSystem.Models;

public class Admin : User
{
    private static readonly List<Admin> _adminExtent = new();

    public Admin(string username, string email, string passwordHash,
        string department, string permissionLevel)
        : base(username, email, passwordHash)
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

        _adminExtent.Add(this);
    }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Department { get; set; }

    [Required] public string PermissionLevel { get; set; }

    public DateTime HireDate { get; set; }

    public void ManageUserAccounts()
    {
        Console.WriteLine($"Admin {Username} managing user accounts with {PermissionLevel} permissions");
    }

    public void ViewSystemConfiguration()
    {
        Console.WriteLine($"Admin {Username} from {Department} viewing system configuration");
    }

    public static IReadOnlyList<Admin> GetAdminExtent()
    {
        return _adminExtent.AsReadOnly();
    }

    public static void ClearAdminExtent()
    {
        _adminExtent.Clear();
    }
}