# ?? Examples - Generic Validation Framework

## Example 1: Simple Validation (1 Table)

### Step 1: Create Attribute (5 d?ng)
```csharp
// File: DatabaseExistsValidation.cs

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SupplierExistsAttribute : DatabaseExistsValidationAttribute<int>
{
    protected override string CacheKey => "SupplierIds";
    protected override string EntityName => "Supplier ID";
    
    public SupplierExistsAttribute()
    {
        ErrorMessage = "Supplier ID không t?n t?i";
    }
}
```

### Step 2: Apply to Model (1 d?ng)
```csharp
public class ProductDto : ObservableValidator
{
    [SupplierExists]  // ? Add this!
    public int SupplierId { get; set; }
    
    public void Validate() => ValidateAllProperties();
}
```

### Step 3: Validate (3 d?ng)
```csharp
private async Task ValidateProducts()
{
    var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");
    
    using (var scope = new ValidationScope<int>("SupplierIds", supplierIds))
    {
        var errors = scope.ValidateList(products);
        products.ForEach(p => p.Validate()); // FlexGrid auto-detect
    }
    // Cache auto-cleared ?
}
```

---

## Example 2: Multi-Table Validation

### Models
```csharp
public class OrderDto : ObservableValidator
{
    [CustomerExists]
    public int CustomerId { get; set; }
    
    [ProductExists]
    public int ProductId { get; set; }
    
    [PaymentMethodExists]
    public string PaymentMethod { get; set; }
    
    public void Validate() => ValidateAllProperties();
}
```

### Attributes (Copy/Paste Template)
```csharp
// Customer
public class CustomerExistsAttribute : DatabaseExistsValidationAttribute<int>
{
    protected override string CacheKey => "CustomerIds";
    protected override string EntityName => "Customer ID";
    public CustomerExistsAttribute() { ErrorMessage = "Customer không t?n t?i"; }
}

// Product
public class ProductExistsAttribute : DatabaseExistsValidationAttribute<int>
{
    protected override string CacheKey => "ProductIds";
    protected override string EntityName => "Product ID";
    public ProductExistsAttribute() { ErrorMessage = "Product không t?n t?i"; }
}

// Payment Method
public class PaymentMethodExistsAttribute : DatabaseExistsValidationAttribute<string>
{
    protected override string CacheKey => "PaymentMethods";
    protected override string EntityName => "Payment Method";
    public PaymentMethodExistsAttribute() { ErrorMessage = "Payment Method không h?p l?"; }
}
```

### Validate
```csharp
private async Task ValidateOrders()
{
    // Load t?t c? data c?n thi?t (3 queries)
    var customerIds = await Http.GetFromJsonAsync<List<int>>("api/customers/ids");
    var productIds = await Http.GetFromJsonAsync<List<int>>("api/products/ids");
    var paymentMethods = await Http.GetFromJsonAsync<List<string>>("api/payment-methods");
    
    // Validate v?i MultiValidationScope
    using (var scope = new MultiValidationScope()
        .LoadCache("CustomerIds", customerIds)
        .LoadCache("ProductIds", productIds)
        .LoadCache("PaymentMethods", paymentMethods))
    {
        var errors = scope.ValidateList(orders);
        orders.ForEach(o => o.Validate());
    }
    // T?t c? cache auto-cleared ?
}
```

---

## Example 3: Unique Validation

### Use Case: Check Product Code không trùng

```csharp
// Attribute
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ProductCodeUniqueAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value == null) return ValidationResult.Success;
        
        if (!ValidationStore.IsCacheLoaded("ExistingProductCodes"))
        {
            return new ValidationResult("Product Code cache chýa ðý?c load");
        }
        
        if (value is string code && ValidationStore.Contains("ExistingProductCodes", code))
        {
            return new ValidationResult($"Product Code '{code}' ð? t?n t?i");
        }
        
        return ValidationResult.Success;
    }
}

// Model
public class CreateProductDto : ObservableValidator
{
    [Required]
    [ProductCodeUnique]  // ? Check trùng
    public string ProductCode { get; set; }
}

// Validate
private async Task ValidateNewProducts()
{
    // Load existing codes
    var existingCodes = await Http.GetFromJsonAsync<List<string>>("api/products/codes");
    
    using (var scope = new ValidationScope<string>("ExistingProductCodes", existingCodes))
    {
        var errors = scope.ValidateList(newProducts);
        newProducts.ForEach(p => p.Validate());
    }
}
```

---

## Example 4: Complex Validation (Product Registration)

### Model
```csharp
public class ProductRegistrationDto : ObservableValidator
{
    [Required(ErrorMessage = "Product Code không ðý?c tr?ng")]
    [ProductCodeUnique]
    public string ProductCode { get; set; }
    
    [Required]
    [MinLength(3)]
    public string ProductName { get; set; }
    
    [CategoryExists]
    public string Category { get; set; }
    
    [SupplierExists]
    public int SupplierId { get; set; }
    
    [WarehouseExists]
    public int WarehouseId { get; set; }
    
    public void Validate() => ValidateAllProperties();
}
```

### Component
```razor
@page "/products/register"
@inject HttpClient Http

<h3>Product Registration</h3>

<EditForm Model="@product" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    
    <InputText @bind-Value="product.ProductCode" />
    <ValidationMessage For="@(() => product.ProductCode)" />
    
    <!-- More fields -->
    
    <button type="submit">Register</button>
</EditForm>

@code {
    private ProductRegistrationDto product = new();
    
    protected override async Task OnInitializedAsync()
    {
        // Load all validation data (1 time only)
        await LoadValidationCaches();
    }
    
    private async Task LoadValidationCaches()
    {
        var existingCodes = await Http.GetFromJsonAsync<List<string>>("api/products/codes");
        var categories = await Http.GetFromJsonAsync<List<string>>("api/categories");
        var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");
        var warehouseIds = await Http.GetFromJsonAsync<List<int>>("api/warehouses/ids");
        
        ValidationStore.SetCache("ExistingProductCodes", existingCodes);
        ValidationStore.SetCache("Categories", categories);
        ValidationStore.SetCache("SupplierIds", supplierIds);
        ValidationStore.SetCache("WarehouseIds", warehouseIds);
    }
    
    private async Task HandleSubmit()
    {
        // Validate trý?c khi submit
        product.Validate();
        
        if (product.HasErrors)
        {
            ShowErrors();
            return;
        }
        
        // Submit to API
        await Http.PostAsJsonAsync("api/products", product);
    }
    
    public void Dispose()
    {
        // Clear cache when leaving page
        ValidationStore.ClearAllCaches();
    }
}
```

---

## Example 5: Manual Cache Management

### Use Case: Real-time cache update

```csharp
@code {
    private async Task AddNewSupplier(SupplierDto newSupplier)
    {
        // 1. Add to database
        var result = await Http.PostAsJsonAsync("api/suppliers", newSupplier);
        var created = await result.Content.ReadFromJsonAsync<SupplierDto>();
        
        // 2. Update cache immediately
        ValidationStore.AddToCache("SupplierIds", created.Id);
        
        // Now validation s? pass ngay l?p t?c!
    }
    
    private async Task DeleteSupplier(int supplierId)
    {
        // 1. Delete from database
        await Http.DeleteAsync($"api/suppliers/{supplierId}");
        
        // 2. Remove from cache
        ValidationStore.RemoveFromCache("SupplierIds", supplierId);
    }
    
    private async Task RefreshCache()
    {
        // Clear old cache
        ValidationStore.ClearCache("SupplierIds");
        
        // Reload from database
        var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");
        ValidationStore.SetCache("SupplierIds", supplierIds);
    }
}
```

---

## Example 6: Validation v?i API Endpoint

### Server-side Validation

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ValidationCacheService _validationCache;
    
    [HttpPost("validate-batch")]
    public async Task<IActionResult> ValidateBatch([FromBody] List<ProductDto> products)
    {
        // Load cache
        await _validationCache.LoadSupplierIdsAsync();
        
        try
        {
            // Validate
            using (var scope = new ValidationScope<int>("SupplierIds", new List<int>()))
            {
                var errors = scope.ValidateList(products);
                
                return Ok(new 
                { 
                    IsValid = !errors.Any(),
                    ErrorCount = errors.Count,
                    Errors = errors
                });
            }
        }
        finally
        {
            // Clear cache
            _validationCache.ClearAllCaches();
        }
    }
}
```

---

## Example 7: Performance Comparison

### ? BAD: Query DB m?i l?n
```csharp
foreach (var product in products) // 1000 products
{
    // Query DB 1000 l?n!
    var exists = await _context.Suppliers.AnyAsync(s => s.Id == product.SupplierId);
}
// Time: ~5000ms
```

### ? GOOD: Query DB 1 l?n
```csharp
// Query 1 l?n
var supplierIds = await _context.Suppliers.Select(s => s.Id).ToListAsync();

using (var scope = new ValidationScope<int>("SupplierIds", supplierIds))
{
    var errors = scope.ValidateList(products); // Validate 1000 products
}
// Time: ~50ms (100x faster!)
```

---

## Example 8: Testing

```csharp
[Fact]
public void TestSupplierValidation()
{
    // Arrange
    ValidationStore.SetCache("SupplierIds", new[] { 1, 2, 3 });
    var product = new ProductDto { SupplierId = 999 }; // Invalid
    
    // Act
    product.Validate();
    
    // Assert
    Assert.True(product.HasErrors);
    Assert.Contains("Supplier", product.GetErrors().First());
    
    // Cleanup
    ValidationStore.ClearCache("SupplierIds");
}
```

---

## Quick Reference

### Create New Attribute
```csharp
public class <Entity>ExistsAttribute : DatabaseExistsValidationAttribute<T>
{
    protected override string CacheKey => "<CacheName>";
    protected override string EntityName => "<DisplayName>";
    public <Entity>ExistsAttribute() { ErrorMessage = "<ErrorMsg>"; }
}
```

### Validate Single Table
```csharp
using (var scope = new ValidationScope<T>("CacheKey", data))
{
    scope.ValidateList(items);
    items.ForEach(i => i.Validate());
}
```

### Validate Multiple Tables
```csharp
using (var scope = new MultiValidationScope()
    .LoadCache("Key1", data1)
    .LoadCache("Key2", data2))
{
    scope.ValidateList(items);
    items.ForEach(i => i.Validate());
}
```

### Manual Cache
```csharp
ValidationStore.SetCache("Key", data);
ValidationStore.Contains("Key", value);
ValidationStore.ClearCache("Key");
```

---

## ?? Summary

| Operation | Code Lines | Example |
|-----------|-----------|---------|
| Create attribute | 5 | Example 1 |
| Apply to model | 1 | Example 1 |
| Single table validation | 3 | Example 1 |
| Multi table validation | 5 | Example 2 |
| Unique validation | Custom | Example 3 |
| Complex validation | Combine | Example 4 |

**Total: ~10 d?ng code ð? validate b?t k? entity nào!** ??
