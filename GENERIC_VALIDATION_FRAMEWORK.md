# ?? Generic Validation Framework - Reusable & Scalable

## ?? T?ng quan

Framework validation t?ng quát, d? tái s? d?ng cho B?T K? entity/b?ng nào trong d? án.

### Core Components

```
ValidationStore.cs              ? Centralized cache management
DatabaseExistsValidation.cs     ? Generic validation attributes
ValidationScope.cs              ? Batch validation v?i auto-cleanup
```

## ?? Cách s? d?ng

### 1. T?o Custom Validation Attribute (CH? 5 D?NG CODE!)

```csharp
// File: DatabaseExistsValidation.cs

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SupplierExistsAttribute : DatabaseExistsValidationAttribute<int>
{
    protected override string CacheKey => "SupplierIds";
    protected override string EntityName => "Supplier ID";
    
    public SupplierExistsAttribute()
    {
        ErrorMessage = "Supplier ID không t?n t?i trong h? th?ng";
    }
}
```

**Mu?n validate Category? Ch? c?n copy/paste và ð?i tên:**

```csharp
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class CategoryExistsAttribute : DatabaseExistsValidationAttribute<string>
{
    protected override string CacheKey => "Categories";
    protected override string EntityName => "Category";
    
    public CategoryExistsAttribute()
    {
        ErrorMessage = "Category không t?n t?i";
    }
}
```

### 2. Áp d?ng vào Model

```csharp
public class ProductDtoTest : ObservableValidator
{
    [Required]
    public string ProductName { get; set; }
    
    [SupplierExists]  // ? Ch? c?n thêm attribute!
    public int SupplierId { get; set; }
    
    [CategoryExists]  // ? D? dàng thêm validation khác!
    public string Category { get; set; }
    
    public void Validate()
    {
        ValidateAllProperties();
    }
}
```

### 3. Validate trong Component (CH? 3 BÝ?C!)

```csharp
private async Task ValidateAllProducts()
{
    // Bý?c 1: Load data t? API
    var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");
    
    // Bý?c 2: Validate v?i using statement
    using (var scope = new ValidationScope<int>("SupplierIds", supplierIds))
    {
        var errors = scope.ValidateList(products);
        
        // FlexGrid auto-detect
        foreach (var item in products)
        {
            item.Validate();
        }
    }
    
    // Bý?c 3: Cache t? ð?ng cleared! ?
}
```

## ?? Validate nhi?u b?ng cùng lúc

```csharp
private async Task ValidateAllProducts()
{
    // Load data t? API
    var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");
    var categories = await Http.GetFromJsonAsync<List<string>>("api/categories");
    var productCodes = await Http.GetFromJsonAsync<List<string>>("api/products/codes");
    
    // Validate v?i multi-cache scope
    using (var scope = new MultiValidationScope(autoClearOnDispose: true)
        .LoadCache("SupplierIds", supplierIds)
        .LoadCache("Categories", categories)
        .LoadCache("ProductCodes", productCodes))
    {
        var errors = scope.ValidateList(products);
        
        foreach (var item in products)
        {
            item.Validate();
        }
    }
    // T?t c? cache t? ð?ng cleared! ?
}
```

## ?? ValidationStore API

### Set/Get Cache
```csharp
// Set cache
ValidationStore.SetCache("SupplierIds", new[] { 1, 2, 3, 4, 5 });

// Get cache
var cache = ValidationStore.GetCache<int>("SupplierIds");

// Contains
bool exists = ValidationStore.Contains("SupplierIds", 3); // true
```

### Add/Remove
```csharp
// Add single value
ValidationStore.AddToCache("SupplierIds", 6);

// Remove single value
ValidationStore.RemoveFromCache("SupplierIds", 6);
```

### Clear
```csharp
// Clear m?t cache
ValidationStore.ClearCache("SupplierIds");

// Clear t?t c? cache
ValidationStore.ClearAllCaches();
```

### Info
```csharp
// Check loaded
bool loaded = ValidationStore.IsCacheLoaded("SupplierIds");

// Get count
int count = ValidationStore.GetCacheCount("SupplierIds");

// Get all keys
var keys = ValidationStore.GetAllCacheKeys();
```

## ?? Ví d? th?c t?

### Example 1: Product v?i Supplier và Category

```csharp
public class ProductDto : ObservableValidator
{
    [Required]
    public string ProductCode { get; set; }
    
    [Required]
    public string ProductName { get; set; }
    
    [SupplierExists]
    public int SupplierId { get; set; }
    
    [CategoryExists]
    public string Category { get; set; }
    
    public void Validate() => ValidateAllProperties();
}

// Trong component
private async Task ValidateProducts()
{
    var supplierIds = await GetSupplierIds();
    var categories = await GetCategories();
    
    using (var scope = new MultiValidationScope()
        .LoadCache("SupplierIds", supplierIds)
        .LoadCache("Categories", categories))
    {
        var errors = scope.ValidateList(products);
        products.ForEach(p => p.Validate());
    }
}
```

### Example 2: Unique Code Validation

```csharp
// T?o attribute
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ProductCodeUniqueAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is string code && ValidationStore.Contains("ProductCodes", code))
        {
            return new ValidationResult("Product Code ð? t?n t?i");
        }
        return ValidationResult.Success;
    }
}

// S? d?ng
public class CreateProductDto : ObservableValidator
{
    [Required]
    [ProductCodeUnique]  // ? Check trùng code
    public string ProductCode { get; set; }
}
```

### Example 3: Order v?i nhi?u validations

```csharp
public class OrderDto : ObservableValidator
{
    [CustomerExists]
    public int CustomerId { get; set; }
    
    [ProductExists]
    public int ProductId { get; set; }
    
    [WarehouseExists]
    public int WarehouseId { get; set; }
    
    [PaymentMethodExists]
    public string PaymentMethod { get; set; }
}

// Component
private async Task ValidateOrders()
{
    using (var scope = new MultiValidationScope()
        .LoadCache("CustomerIds", await GetCustomerIds())
        .LoadCache("ProductIds", await GetProductIds())
        .LoadCache("WarehouseIds", await GetWarehouseIds())
        .LoadCache("PaymentMethods", await GetPaymentMethods()))
    {
        scope.ValidateList(orders);
        orders.ForEach(o => o.Validate());
    }
}
```

## ?? T?o validation attributes m?i

### Template cho Int ID validation

```csharp
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class <Entity>ExistsAttribute : DatabaseExistsValidationAttribute<int>
{
    protected override string CacheKey => "<Entity>Ids";
    protected override string EntityName => "<Entity> ID";
    
    public <Entity>ExistsAttribute()
    {
        ErrorMessage = "<Entity> ID không t?n t?i";
    }
}

// Example:
// CustomerExistsAttribute
// WarehouseExistsAttribute
// EmployeeExistsAttribute
```

### Template cho String validation

```csharp
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class <Entity>ExistsAttribute : DatabaseExistsValidationAttribute<string>
{
    protected override string CacheKey => "<Entity>s";
    protected override string EntityName => "<Entity>";
    
    public <Entity>ExistsAttribute()
    {
        ErrorMessage = "<Entity> không t?n t?i";
    }
}

// Example:
// CategoryExistsAttribute
// PaymentMethodExistsAttribute
// StatusExistsAttribute
```

### Template cho Guid validation

```csharp
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class <Entity>ExistsAttribute : DatabaseExistsValidationAttribute<Guid>
{
    protected override string CacheKey => "<Entity>Guids";
    protected override string EntityName => "<Entity> GUID";
    
    public <Entity>ExistsAttribute()
    {
        ErrorMessage = "<Entity> GUID không t?n t?i";
    }
}
```

## ?? Best Practices

### ? DO

```csharp
// 1. Luôn dùng using statement
using (var scope = new ValidationScope<int>("SupplierIds", ids))
{
    // validation logic
}

// 2. Load cache 1 l?n cho c? batch
var ids = await LoadOnce();
using (var scope = new ValidationScope<int>("SupplierIds", ids))
{
    ValidateBatch1();
    ValidateBatch2();
}

// 3. Dùng MultiValidationScope cho nhi?u cache
using (var scope = new MultiValidationScope()
    .LoadCache("Cache1", data1)
    .LoadCache("Cache2", data2))
{
    // validation logic
}
```

### ? DON'T

```csharp
// 1. Không load cache nhi?u l?n
foreach (var batch in batches)
{
    var ids = await LoadEveryTime(); // ? L?ng phí!
}

// 2. Không quên dispose
var scope = new ValidationScope<int>("Key", ids);
// ... forgot to dispose ? memory leak! ?

// 3. Không validate trý?c khi load cache
item.Validate(); // ? Cache chýa load!
```

## ?? Summary

| Feature | Code Required | Reusable |
|---------|--------------|----------|
| **T?o attribute m?i** | 5 d?ng | ? 100% |
| **Áp d?ng vào model** | 1 d?ng | ? |
| **Validate trong component** | 3-5 d?ng | ? |
| **Clear cache** | Automatic | ? |
| **Multi-table validation** | Fluent API | ? |
| **Thread-safe** | Built-in | ? |

## ?? K?t lu?n

Framework này cho phép b?n:

1. ? T?o validation attribute m?i ch? v?i **5 d?ng code**
2. ? Validate nhi?u b?ng cùng lúc v?i **MultiValidationScope**
3. ? T? ð?ng qu?n l? cache v?i **using statement**
4. ? Reusable 100% cho m?i entity trong d? án
5. ? Thread-safe và performance cao

**Ch? c?n copy template và ð?i tên entity!** ??
