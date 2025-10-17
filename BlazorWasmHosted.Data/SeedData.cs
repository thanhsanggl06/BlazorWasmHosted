using BlazorWasmHosted.Shared.Entities;

namespace BlazorWasmHosted.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        // Check if data already exists
        if (context.Suppliers.Any() || context.Products.Any())
        {
            return; // DB has been seeded
        }

        // Seed Suppliers
        var suppliers = new[]
        {
            new Supplier
            {
                SupplierCode = "SUP001",
                SupplierName = "TechCorp Solutions",
                ContactPerson = "John Smith",
                Email = "john@techcorp.com",
                Phone = "+1-555-0100",
                Address = "123 Tech Street",
                City = "San Francisco",
                Country = "USA",
                IsActive = true
            },
            new Supplier
            {
                SupplierCode = "SUP002",
                SupplierName = "Global Electronics Ltd",
                ContactPerson = "Sarah Johnson",
                Email = "sarah@globalelec.com",
                Phone = "+1-555-0200",
                Address = "456 Innovation Ave",
                City = "Tokyo",
                Country = "Japan",
                IsActive = true
            },
            new Supplier
            {
                SupplierCode = "SUP003",
                SupplierName = "FurniturePro International",
                ContactPerson = "Michael Chen",
                Email = "michael@furnipro.com",
                Phone = "+1-555-0300",
                Address = "789 Design Boulevard",
                City = "Milan",
                Country = "Italy",
                IsActive = true
            },
            new Supplier
            {
                SupplierCode = "SUP004",
                SupplierName = "BookStore Wholesale",
                ContactPerson = "Emily Brown",
                Email = "emily@bookstore.com",
                Phone = "+1-555-0400",
                Address = "321 Library Lane",
                City = "London",
                Country = "UK",
                IsActive = true
            },
            new Supplier
            {
                SupplierCode = "SUP005",
                SupplierName = "Office Supplies Co",
                ContactPerson = "David Lee",
                Email = "david@officesupplies.com",
                Phone = "+1-555-0500",
                Address = "654 Business Park",
                City = "Singapore",
                Country = "Singapore",
                IsActive = true
            }
        };

        context.Suppliers.AddRange(suppliers);
        context.SaveChanges();

        // Seed Products
        var products = new[]
        {
            new Product
            {
                ProductCode = "P001",
                ProductName = "???????",
                Category = "????",
                UnitPrice = 85000,
                Quantity = 15,
                InStock = true,
                Description = "???????????PC",
                SupplierId = suppliers[0].Id // TechCorp
            },
            new Product
            {
                ProductCode = "P002",
                ProductName = "????????",
                Category = "????",
                UnitPrice = 2500,
                Quantity = 50,
                InStock = true,
                Description = "Bluetooth?????",
                SupplierId = suppliers[1].Id // Global Electronics
            },
            new Product
            {
                ProductCode = "P003",
                ProductName = "???????",
                Category = "??",
                UnitPrice = 25000,
                Quantity = 8,
                InStock = true,
                Description = "??????????",
                SupplierId = suppliers[2].Id // FurniturePro
            },
            new Product
            {
                ProductCode = "P004",
                ProductName = "LED ???? 27???",
                Category = "????",
                UnitPrice = 35000,
                Quantity = 12,
                InStock = true,
                Description = "4K???????",
                SupplierId = suppliers[1].Id // Global Electronics
            },
            new Product
            {
                ProductCode = "P005",
                ProductName = "??????????",
                Category = "????",
                UnitPrice = 8500,
                Quantity = 25,
                InStock = true,
                Description = "RGB ????????",
                SupplierId = suppliers[0].Id // TechCorp
            },
            new Product
            {
                ProductCode = "P006",
                ProductName = "??????",
                Category = "??",
                UnitPrice = 4500,
                Quantity = 20,
                InStock = true,
                Description = "??????LED???",
                SupplierId = suppliers[4].Id // Office Supplies
            },
            new Product
            {
                ProductCode = "P007",
                ProductName = "??: Blazor??",
                Category = "??",
                UnitPrice = 3200,
                Quantity = 0,
                InStock = false,
                Description = "Blazor????????",
                SupplierId = suppliers[3].Id // BookStore
            },
            new Product
            {
                ProductCode = "P008",
                ProductName = "USB?? 7???",
                Category = "????",
                UnitPrice = 1800,
                Quantity = 35,
                InStock = true,
                Description = "USB 3.0??",
                SupplierId = suppliers[0].Id // TechCorp
            },
            new Product
            {
                ProductCode = "P009",
                ProductName = "??????",
                Category = "??",
                UnitPrice = 2200,
                Quantity = 18,
                InStock = true,
                Description = "???????? 90x45cm",
                SupplierId = suppliers[4].Id // Office Supplies
            },
            new Product
            {
                ProductCode = "P010",
                ProductName = "??????",
                Category = "????",
                UnitPrice = 6500,
                Quantity = 22,
                InStock = true,
                Description = "??????????????",
                SupplierId = suppliers[1].Id // Global Electronics
            },
            new Product
            {
                ProductCode = "P011",
                ProductName = "Web??? 1080p",
                Category = "????",
                UnitPrice = 7800,
                Quantity = 14,
                InStock = true,
                Description = "Full HD ??????",
                SupplierId = suppliers[0].Id // TechCorp
            },
            new Product
            {
                ProductCode = "P012",
                ProductName = "????? 3?",
                Category = "??",
                UnitPrice = 1500,
                Quantity = 30,
                InStock = true,
                Description = "????????????",
                SupplierId = suppliers[4].Id // Office Supplies
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
