# ?? Validation Best Practice - ALWAYS Use MultiValidationScope

## ? Quy t?c: LUÔN LUÔN dùng `MultiValidationScope`

**K? c? khi ch? có 1 validation!**

### T?i sao?

1. ? **Consistency** - Code nh?t quán trong toàn b? d? án
2. ? **Scalability** - D? dàng thêm validation m?i sau này
3. ? **Readability** - Ai c?ng bi?t cách validate
4. ? **Maintainability** - Không c?n nh? 2 cách khác nhau

---

## ?? Pattern chu?n

### ? CORRECT: Luôn dùng MultiValidationScope

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

### ? DEPRECATED: Không dùng ValidationScope<T>

```csharp
// ? DEPRECATED - S? b? xóa trong version sau
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

// ? Validate - Cùng pattern!
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

## ?? So sánh

| Scenario | Old Way (?) | New Way (?) |
|----------|-------------|--------------|
| 1 validation | `ValidationScope<int>` | `MultiValidationScope().LoadCache()` |
| 2 validations | Nested `using` ho?c manual | `MultiValidationScope().LoadCache().LoadCache()` |
| 3+ validations | Ph?c t?p | `MultiValidationScope().LoadCache()...` |

---

## ?? Template chu?n

### Template ð? copy/paste

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

N?u b?n ðang dùng `ValidationScope<T>`, h?y migrate sang `MultiValidationScope`:

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

**Thay ð?i:**
1. `new ValidationScope<int>(` ? `new MultiValidationScope()`
2. `"CacheKey", data` ? `.LoadCache("CacheKey", data)`

---

## ? Benefits

| Benefit | Description |
|---------|-------------|
| **Nh?t quán** | Cùng 1 pattern cho m?i validation |
| **D? ð?c** | Code d? hi?u, d? review |
| **D? maintain** | Ch? c?n nh? 1 cách |
| **Scalable** | Thêm validation m?i = thêm `.LoadCache()` |
| **Safe** | Auto-clear t?t c? cache |

---

## ?? Summary

### Rule of Thumb

```
?? LUÔN LUÔN dùng MultiValidationScope
?? KHÔNG BAO GI? dùng ValidationScope<T>
?? K? C? KHI CH? CÓ 1 VALIDATION
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

**Ch? v?y thôi!** ??
