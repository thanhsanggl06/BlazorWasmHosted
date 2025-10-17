# Product và Supplier Tables v?i Relationship

## ?? T?ng quan

Ð? t?o thành công 2 b?ng **Products** và **Suppliers** v?i m?i quan h? **Many-to-One** (nhi?u Products thu?c v? m?t Supplier).

## ??? Database Schema

### B?ng Suppliers
```sql
CREATE TABLE Suppliers (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    SupplierCode    NVARCHAR(50) NOT NULL UNIQUE,
    SupplierName    NVARCHAR(200) NOT NULL,
    ContactPerson   NVARCHAR(100) NULL,
    Email           NVARCHAR(100) NULL,
    Phone           NVARCHAR(50) NULL,
    Address         NVARCHAR(500) NULL,
    City            NVARCHAR(100) NULL,
    Country         NVARCHAR(100) NULL,
    IsActive        BIT NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

### B?ng Products
```sql
CREATE TABLE Products (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode     NVARCHAR(50) NOT NULL UNIQUE,
    ProductName     NVARCHAR(200) NOT NULL,
    Category        NVARCHAR(100) NOT NULL,
    UnitPrice       DECIMAL(18,2) NOT NULL,
    Quantity        INT NOT NULL DEFAULT 0,
    InStock         BIT NOT NULL DEFAULT 1,
    Description     NVARCHAR(1000) NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt       DATETIME2 NULL,
    SupplierId      INT NOT NULL,
    
    CONSTRAINT FK_Products_Suppliers_SupplierId 
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
        ON DELETE RESTRICT
);
```

## ?? M?i quan h?

```
Supplier (1) ?? (*) Product
     ?                  ?
   Parent           Child
```

- **M?t Supplier** có th? cung c?p **nhi?u Products**
- **M?t Product** ch? thu?c v? **m?t Supplier**
- **Delete Behavior**: `RESTRICT` - Không th? xóa Supplier n?u c?n Products liên quan

## ?? File Structure

```
BlazorWasmHosted/
??? BlazorWasmHosted.Shared/
?   ??? Entities/
?   ?   ??? Supplier.cs              ? Entity class
?   ?   ??? Product.cs               ? Entity class
?   ??? Models/
?       ??? SupplierDto.cs           ? DTOs
?       ??? ProductDto.cs            ? DTOs
?
??? BlazorWasmHosted.Data/
?   ??? AppDbContext.cs              ? Updated v?i Products & Suppliers DbSets
?   ??? SeedData.cs                  ? Sample data
?   ??? Migrations/
?       ??? 20251017084116_AddProductAndSupplierTables.cs  ?
?
??? BlazorWasmHosted.Services/
?   ??? Interface/
?   ?   ??? ISupplierService.cs      ?
?   ?   ??? IProductService.cs       ?
?   ??? Implementation/
?       ??? SupplierService.cs       ?
?       ??? ProductService.cs        ?
?
??? BlazorWasmHosted.Server/
    ??? Controllers/
    ?   ??? SuppliersController.cs   ?
    ?   ??? ProductsController.cs    ?
    ??? Program.cs                   ? Updated DI registration
```

## ?? API Endpoints

### Suppliers API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/suppliers` | L?y t?t c? suppliers |
| GET | `/api/suppliers/{id}` | L?y supplier theo ID |
| POST | `/api/suppliers` | T?o supplier m?i |
| PUT | `/api/suppliers/{id}` | C?p nh?t supplier |
| DELETE | `/api/suppliers/{id}` | Xóa supplier |

### Products API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | L?y t?t c? products |
| GET | `/api/products/{id}` | L?y product theo ID |
| GET | `/api/products/category/{category}` | L?y products theo category |
| GET | `/api/products/supplier/{supplierId}` | L?y products theo supplier |
| POST | `/api/products` | T?o product m?i |
| PUT | `/api/products/{id}` | C?p nh?t product |
| DELETE | `/api/products/{id}` | Xóa product |

## ?? Sample Data

### 5 Suppliers ðý?c seed:
1. **TechCorp Solutions** (USA) - SUP001
2. **Global Electronics Ltd** (Japan) - SUP002
3. **FurniturePro International** (Italy) - SUP003
4. **BookStore Wholesale** (UK) - SUP004
5. **Office Supplies Co** (Singapore) - SUP005

### 12 Products ðý?c seed:
- 8 s?n ph?m ði?n t? (????)
- 3 s?n ph?m n?i th?t (??)
- 1 s?n ph?m sách (??)

## ?? Entity Configuration

### Supplier Entity
```csharp
public class Supplier
{
    public int Id { get; set; }
    public required string SupplierCode { get; set; }
    public required string SupplierName { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public ICollection<Product> Products { get; set; }
}
```

### Product Entity
```csharp
public class Product
{
    public int Id { get; set; }
    public required string ProductCode { get; set; }
    public required string ProductName { get; set; }
    public required string Category { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public bool InStock { get; set; } = true;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign Key
    public int SupplierId { get; set; }
    
    // Navigation property
    public Supplier? Supplier { get; set; }
}
```

## ?? DTOs

### SupplierDto
```csharp
public record SupplierDto(
    int Id,
    string SupplierCode,
    string SupplierName,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    string? Country,
    bool IsActive,
    DateTime CreatedAt,
    int ProductCount  // Computed property
);
```

### ProductDto
```csharp
public record ProductDto(
    int Id,
    string ProductCode,
    string ProductName,
    string Category,
    decimal UnitPrice,
    int Quantity,
    bool InStock,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int SupplierId,
    string SupplierName,  // From Supplier navigation
    decimal TotalValue    // Computed: UnitPrice * Quantity
);
```

## ?? Cách s? d?ng

### 1. Migration ð? ðý?c applied
```bash
cd BlazorWasmHosted.Data
dotnet ef database update
```

### 2. Sample data t? ð?ng seed khi kh?i ð?ng Server

### 3. Test API v?i Swagger
```
https://localhost:5001/swagger
```

### 4. Test v?i HttpRepl ho?c Postman

**GET All Products:**
```http
GET https://localhost:5001/api/products
```

**GET Products by Supplier:**
```http
GET https://localhost:5001/api/products/supplier/1
```

**GET Products by Category:**
```http
GET https://localhost:5001/api/products/category/????
```

**CREATE Product:**
```http
POST https://localhost:5001/api/products
Content-Type: application/json

{
  "productCode": "P999",
  "productName": "Test Product",
  "category": "????",
  "unitPrice": 10000,
  "quantity": 5,
  "inStock": true,
  "description": "Test description",
  "supplierId": 1
}
```

## ??? Business Rules

### Supplier
- ? SupplierCode ph?i unique
- ? Không th? xóa Supplier n?u c?n Products liên quan
- ? IsActive flag ð? soft delete

### Product
- ? ProductCode ph?i unique
- ? UnitPrice: decimal(18,2)
- ? Quantity m?c ð?nh là 0
- ? InStock m?c ð?nh là true
- ? SupplierId b?t bu?c (Foreign Key)
- ? UpdatedAt t? ð?ng c?p nh?t khi edit

## ?? Query Examples

### L?y t?t c? Products kèm Supplier info:
```csharp
var products = await _context.Products
    .Include(p => p.Supplier)
    .ToListAsync();
```

### L?y Supplier và count Products:
```csharp
var suppliers = await _context.Suppliers
    .Include(s => s.Products)
    .Select(s => new {
        s.Id,
        s.SupplierName,
        ProductCount = s.Products.Count
    })
    .ToListAsync();
```

### L?y Products theo Supplier:
```csharp
var products = await _context.Products
    .Where(p => p.SupplierId == supplierId)
    .Include(p => p.Supplier)
    .ToListAsync();
```

## ?? EF Core Configuration Highlights

```csharp
// Unique constraint on SupplierCode
supplier.HasIndex(x => x.SupplierCode).IsUnique();

// Unique constraint on ProductCode
product.HasIndex(x => x.ProductCode).IsUnique();

// Foreign Key relationship v?i RESTRICT delete
product.HasOne(p => p.Supplier)
       .WithMany(s => s.Products)
       .HasForeignKey(p => p.SupplierId)
       .OnDelete(DeleteBehavior.Restrict);
```

## ?? Next Steps - UI Development

B?n có th? t?o Blazor pages cho:

1. **Supplier Management Page**
   - List suppliers v?i product count
   - CRUD operations
   - Search & filter

2. **Product Management Page** (ð? có README)
   - List products v?i supplier info
   - CRUD operations
   - Filter by category/supplier
   - FlexGrid display

3. **Supplier Details Page**
   - Chi ti?t supplier
   - List products c?a supplier ðó
   - Statistics

## ?? References

- [EF Core Relationships](https://docs.microsoft.com/ef/core/modeling/relationships)
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api/)
- [Blazor HttpClient](https://docs.microsoft.com/aspnet/core/blazor/call-web-api)

## ? Status

- [x] Entity classes
- [x] DbContext configuration
- [x] Migrations
- [x] Seed data
- [x] Service interfaces
- [x] Service implementations
- [x] API Controllers
- [x] DI registration
- [x] Database updated
- [ ] Blazor UI pages (Next step)

---

**Database:** ? Ready  
**API:** ? Ready  
**UI:** ?? Coming soon
