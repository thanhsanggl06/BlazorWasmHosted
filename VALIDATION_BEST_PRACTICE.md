# ?? Validation Best Practice - ALWAYS Use MultiValidationScope

## ? Quy t?c: LU�N LU�N d�ng `MultiValidationScope`

**K? c? khi ch? c� 1 validation!**

### T?i sao?

1. ? **Consistency** - Code nh?t qu�n trong to�n b? d? �n
2. ? **Scalability** - D? d�ng th�m validation m?i sau n�y
3. ? **Readability** - Ai c?ng bi?t c�ch validate
4. ? **Maintainability** - Kh�ng c?n nh? 2 c�ch kh�c nhau

---

## ?? Pattern chu?n

### ? CORRECT: Lu�n d�ng MultiValidationScope

```csharp
// ? 1 validation
using (var scope = new MultiValidationScope()
    .LoadCache("SupplierIds", supplierIds))
{
    var errors = scope.ValidateList(products);
    products.ForEach(p => p.Validate());
}

// ? 2 validations
using (var scope = new MultiValidationScope()
    .LoadCache("SupplierIds", supplierIds)
    .LoadCache("Categories", categories))
{
    var errors = scope.ValidateList(products);
    products.ForEach(p => p.Validate());
}

// ? 3+ validations
using (var scope = new MultiValidationScope()
    .LoadCache("SupplierIds", supplierIds)
    .LoadCache("Categories", categories)
    .LoadCache("WarehouseIds", warehouseIds))
{
    var errors = scope.ValidateList(products);
    products.ForEach(p => p.Validate());
}
```

### ? DEPRECATED: Kh�ng d�ng ValidationScope<T>

```csharp
// ? DEPRECATED - S? b? x�a trong version sau
using (var scope = new ValidationScope<int>("SupplierIds", supplierIds))
{
    // ...
}
```

---

## ?? Examples

### Example 1: Product v?i 1 validation

```csharp
public class ProductDto : ObservableValidator
{
    [SupplierExists]
    public int SupplierId { get; set; }
}

// ? Validate
private async Task ValidateProducts()
{
    var supplierIds = await GetSupplierIds();
    
    using (var scope = new MultiValidationScope()
        .LoadCache("SupplierIds", supplierIds))
    {
        scope.ValidateList(products);
        products.ForEach(p => p.Validate());
    }
}
```

### Example 2: Product v?i nhi?u validations

```csharp
public class ProductDto : ObservableValidator
{
    [SupplierExists]
    public int SupplierId { get; set; }
    
    [CategoryExists]
    public string Category { get; set; }
    
    [WarehouseExists]
    public int WarehouseId { get; set; }
}

// ? Validate - C�ng pattern!
private async Task ValidateProducts()
{
    var supplierIds = await GetSupplierIds();
    var categories = await GetCategories();
    var warehouseIds = await GetWarehouseIds();
    
    using (var scope = new MultiValidationScope()
        .LoadCache("SupplierIds", supplierIds)
        .LoadCache("Categories", categories)
        .LoadCache("WarehouseIds", warehouseIds))
    {
        scope.ValidateList(products);
        products.ForEach(p => p.Validate());
    }
}
```

### Example 3: Order v?i 4 validations

```csharp
public class OrderDto : ObservableValidator
{
    [CustomerExists] public int CustomerId { get; set; }
    [ProductExists] public int ProductId { get; set; }
    [WarehouseExists] public int WarehouseId { get; set; }
    [PaymentMethodExists] public string PaymentMethod { get; set; }
}

// ? Validate
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

---

## ?? So s�nh

| Scenario | Old Way (?) | New Way (?) |
|----------|-------------|--------------|
| 1 validation | `ValidationScope<int>` | `MultiValidationScope().LoadCache()` |
| 2 validations | Nested `using` ho?c manual | `MultiValidationScope().LoadCache().LoadCache()` |
| 3+ validations | Ph?c t?p | `MultiValidationScope().LoadCache()...` |

---

## ?? Template chu?n

### Template �? copy/paste

```csharp
private async Task Validate{Entity}s()
{
    try
    {
        // 1. Load validation data
        var cache1 = await Get{Cache1}();
        var cache2 = await Get{Cache2}();
        // ... more caches
        
        // 2. Check if data loaded
        if (cache1 == null || !cache1.Any())
            return;
        
        // 3. ? Validate v?i MultiValidationScope
        using (var scope = new MultiValidationScope()
            .LoadCache("{Cache1Key}", cache1)
            .LoadCache("{Cache2Key}", cache2))
        {
            var errors = scope.ValidateList(items);
            
            // 4. Call Validate() for FlexGrid
            foreach (var item in items)
            {
                item.Validate();
            }
            
            // 5. Optional: Log results
            Console.WriteLine($"Validated {items.Count} items - {errors.Count} errors");
        }
        // Cache auto-cleared ?
        
        // 6. Refresh UI
        await InvokeAsync(StateHasChanged);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Validation error: {ex.Message}");
    }
}
```

---

## ?? Migration Guide

N?u b?n �ang d�ng `ValidationScope<T>`, h?y migrate sang `MultiValidationScope`:

### Before (?)

```csharp
using (var scope = new ValidationScope<int>("SupplierIds", supplierIds))
{
    scope.ValidateList(products);
}
```

### After (?)

```csharp
using (var scope = new MultiValidationScope()
    .LoadCache("SupplierIds", supplierIds))
{
    scope.ValidateList(products);
}
```

**Thay �?i:**
1. `new ValidationScope<int>(` ? `new MultiValidationScope()`
2. `"CacheKey", data` ? `.LoadCache("CacheKey", data)`

---

## ? Benefits

| Benefit | Description |
|---------|-------------|
| **Nh?t qu�n** | C�ng 1 pattern cho m?i validation |
| **D? �?c** | Code d? hi?u, d? review |
| **D? maintain** | Ch? c?n nh? 1 c�ch |
| **Scalable** | Th�m validation m?i = th�m `.LoadCache()` |
| **Safe** | Auto-clear t?t c? cache |

---

## ?? Summary

### Rule of Thumb

```
?? LU�N LU�N d�ng MultiValidationScope
?? KH�NG BAO GI? d�ng ValidationScope<T>
?? K? C? KHI CH? C� 1 VALIDATION
```

### Template

```csharp
using (var scope = new MultiValidationScope()
    .LoadCache("Key1", data1)
    .LoadCache("Key2", data2)
    // ... more caches
    )
{
    scope.ValidateList(items);
    items.ForEach(i => i.Validate());
}
```

**Ch? v?y th�i!** ??
