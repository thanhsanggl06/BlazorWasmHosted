# Custom Validation - Optimized Batch Validation

## ?? Gi?i thi?u

Gi?i pháp t?i ýu hóa validation cho list/batch v?i các tính nãng:

- ? **Single DB Query**: Ch? query database 1 l?n duy nh?t cho toàn b? list
- ? **Scoped Cache**: Cache t? ð?ng clear sau khi validate xong
- ? **Thread-Safe**: H? tr? concurrent validation
- ? **Performance**: Validate hàng ngh?n records trong vài milliseconds
- ? **Clean Code**: S? d?ng `using` statement cho automatic cleanup

## ?? Ki?n trúc

### 1. SupplierExistsAttribute (Enhanced)
```csharp
[SupplierExists(ErrorMessage = "Supplier ID không t?n t?i")]
public int SupplierId { get; set; }
```

**Tính nãng:**
- Thread-safe cache v?i `lock`
- Ki?m tra cache loaded trý?c khi validate
- Static methods ð? qu?n l? cache

### 2. SupplierValidationScope
```csharp
public class SupplierValidationScope : IDisposable
{
    // T? ð?ng load cache khi kh?i t?o
    // T? ð?ng clear cache khi dispose
}
```

**Tính nãng:**
- Implement `IDisposable` pattern
- Auto-load supplier IDs vào cache
- Auto-clear cache khi dispose
- Batch validation method

### 3. ValidationCacheService
```csharp
public class ValidationCacheService
{
    Task<int> LoadSupplierIdsAsync();
    Task<List<int>> GetSupplierIdsAsync();
}
```

**Tính nãng:**
- Load supplier IDs t? database
- Helper methods cho cache management

## ?? Cách s? d?ng

### Cách 1: Using Statement (Recommended) ?

```csharp
// Load supplier IDs t? DB
var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");

// Validate v?i auto-cleanup
using (var scope = new SupplierValidationScope(supplierIds, autoClearOnDispose: true))
{
    var errors = scope.ValidateList(products);
    
    if (errors.Any())
    {
        // X? l? errors
        foreach (var error in errors)
        {
            Console.WriteLine($"Product at index {error.Index} has errors:");
            foreach (var err in error.Errors)
            {
                Console.WriteLine($"  - {err}");
            }
        }
    }
}
// Cache t? ð?ng clear t?i ðây
```

### Cách 2: Manual Cache Management

```csharp
// 1. Load supplier IDs t? DB (1 l?n duy nh?t)
var supplierIds = await validationCacheService.GetSupplierIdsAsync();
SupplierExistsAttribute.SetValidSupplierIds(supplierIds);

// 2. Validate t?ng item ho?c batch
foreach (var product in products)
{
    product.Validate();
    
    if (product.HasErrors)
    {
        // X? l? errors
    }
}

// 3. Clear cache khi xong
SupplierExistsAttribute.ClearCache();
```

### Cách 3: Server-side v?i ValidationCacheService

```csharp
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ValidationCacheService _validationCache;

    [HttpPost("validate-batch")]
    public async Task<IActionResult> ValidateBatch([FromBody] List<ProductDtoTest> products)
    {
        // Load cache 1 l?n
        var count = await _validationCache.LoadSupplierIdsAsync();
        
        try
        {
            var errors = new List<ValidationError>();
            
            foreach (var product in products)
            {
                var context = new ValidationContext(product);
                var results = new List<ValidationResult>();
                
                if (!Validator.TryValidateObject(product, context, results, true))
                {
                    errors.Add(new ValidationError 
                    { 
                        Item = product, 
                        Errors = results.Select(r => r.ErrorMessage ?? "").ToList() 
                    });
                }
            }
            
            return Ok(new { IsValid = !errors.Any(), Errors = errors });
        }
        finally
        {
            // Clear cache
            _validationCache.ClearAllCaches();
        }
    }
}
```

## ?? Performance Comparison

### ? Không t?i ýu (Query DB m?i l?n)
```csharp
foreach (var product in products) // 1000 products
{
    // M?i product query DB 1 l?n = 1000 queries!
    var exists = await _context.Suppliers.AnyAsync(s => s.Id == product.SupplierId);
}
// Time: ~5000ms cho 1000 products
```

### ? T?i ýu (Query DB 1 l?n)
```csharp
// Query 1 l?n
var supplierIds = await _context.Suppliers.Select(s => s.Id).ToListAsync();

using (var scope = new SupplierValidationScope(supplierIds))
{
    var errors = scope.ValidateList(products); // Validate 1000 products
}
// Time: ~50ms cho 1000 products (100x nhanh hõn!)
```

## ?? Best Practices

### 1. Always use `using` statement
```csharp
? Good:
using (var scope = new SupplierValidationScope(ids))
{
    var errors = scope.ValidateList(products);
}

? Bad:
var scope = new SupplierValidationScope(ids);
var errors = scope.ValidateList(products);
// Forgot to dispose - cache leak!
```

### 2. Load cache once per request/operation
```csharp
? Good:
var ids = await LoadOnce();
using (var scope = new SupplierValidationScope(ids))
{
    ValidateBatch1(products1);
    ValidateBatch2(products2);
}

? Bad:
foreach (var batch in batches)
{
    var ids = await LoadEveryTime(); // Wasting resources!
    using (var scope = new SupplierValidationScope(ids))
    {
        ValidateBatch(batch);
    }
}
```

### 3. Check cache status
```csharp
if (SupplierExistsAttribute.IsCacheLoaded)
{
    Console.WriteLine($"Cache has {SupplierExistsAttribute.CachedCount} suppliers");
}
else
{
    Console.WriteLine("Cache not loaded yet");
}
```

## ?? Advanced Usage

### Validate v?i custom logic
```csharp
using (var scope = new SupplierValidationScope(supplierIds))
{
    var errors = scope.ValidateList(products);
    
    // Custom post-validation logic
    var criticalErrors = errors.Where(e => 
        e.Errors.Any(err => err.Contains("Supplier"))
    ).ToList();
    
    if (criticalErrors.Any())
    {
        throw new ValidationException("Critical supplier errors found");
    }
}
```

### Parallel validation (cho large datasets)
```csharp
using (var scope = new SupplierValidationScope(supplierIds))
{
    var batches = products.Chunk(100); // Split thành batches
    var allErrors = new ConcurrentBag<ValidationError>();
    
    Parallel.ForEach(batches, batch =>
    {
        var errors = scope.ValidateList(batch);
        foreach (var error in errors)
        {
            allErrors.Add(error);
        }
    });
}
```

## ?? Complete Example

```csharp
@page "/products-batch-validation"
@inject HttpClient Http

@code {
    private List<ProductDtoTest> products = new();

    private async Task ValidateProducts()
    {
        // Step 1: Load supplier IDs t? API (1 query)
        var suppliers = await Http.GetFromJsonAsync<List<SupplierDto>>("api/suppliers");
        var supplierIds = suppliers?.Select(s => s.Id).ToList() ?? new();
        
        Console.WriteLine($"Loaded {supplierIds.Count} supplier IDs from database");
        
        // Step 2: Validate batch v?i automatic cache management
        using (var scope = new SupplierValidationScope(supplierIds, autoClearOnDispose: true))
        {
            var startTime = DateTime.Now;
            var errors = scope.ValidateList(products);
            var duration = (DateTime.Now - startTime).TotalMilliseconds;
            
            Console.WriteLine($"Validated {products.Count} products in {duration}ms");
            
            if (errors.Any())
            {
                Console.WriteLine($"Found {errors.Count} products with errors:");
                foreach (var error in errors)
                {
                    var product = error.Item as ProductDtoTest;
                    Console.WriteLine($"  Product {product?.Id}: {string.Join(", ", error.Errors)}");
                }
            }
            else
            {
                Console.WriteLine("All products are valid!");
            }
        }
        // Cache automatically cleared here
        
        Console.WriteLine($"Cache cleared. Status: {SupplierExistsAttribute.IsCacheLoaded}");
    }
}
```

## ?? Summary

| Feature | Old Approach | New Optimized Approach |
|---------|--------------|------------------------|
| DB Queries | N queries (N = s? products) | 1 query |
| Performance | Slow (seconds) | Fast (milliseconds) |
| Memory | Low | Medium (cache) |
| Cleanup | Manual | Automatic (`using`) |
| Thread-Safe | ? | ? |
| Code Quality | Complex | Clean & Simple |

**K?t lu?n**: S? d?ng `SupplierValidationScope` v?i `using` statement cho t?t c? batch validation operations!
