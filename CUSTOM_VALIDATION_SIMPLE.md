# Custom Validation - SupplierExists (Simplified)

## Gi?i thi?u

Custom validation attribute `[SupplierExists]` ðõn gi?n ð? ki?m tra Supplier ID có t?n t?i hay không. Ch? c?n thêm attribute vào property và g?i `Validate()` nhý b?nh thý?ng.

## Cách s? d?ng

### 1. Thêm Attribute vào Model

```csharp
using BlazorWasmHosted.Shared.ValidationAttributes;

public class ProductDtoTest : ObservableValidator
{
    [SupplierExists(ErrorMessage = "Supplier ID không t?n t?i trong h? th?ng")]
    public int SupplierId { get; set; }

    public void Validate()
    {
        ValidateAllProperties(); // T? ð?ng validate SupplierExists
    }
}
```

### 2. Load Valid Supplier IDs

```csharp
protected override async Task OnInitializedAsync()
{
    // Load t? API ho?c database
    var suppliers = await SupplierService.GetAllSuppliersAsync();
    SupplierExistsAttribute.SetValidSupplierIds(suppliers.Select(s => s.Id));
}
```

### 3. Validate nhý b?nh thý?ng

```csharp
private void ValidateProduct()
{
    product.Validate(); // T? ð?ng check SupplierExists
    
    if (product.HasErrors)
    {
        // X? l? l?i
    }
}
```

## API Methods

```csharp
// Set toàn b? danh sách IDs h?p l?
SupplierExistsAttribute.SetValidSupplierIds(new[] { 1, 2, 3, 4, 5 });

// Thêm m?t ID
SupplierExistsAttribute.AddValidSupplierId(6);

// Clear t?t c?
SupplierExistsAttribute.ClearValidSupplierIds();
```

## Ví d? hoàn ch?nh

```csharp
@inject ISupplierService SupplierService

@code {
    private ProductDtoTest product = new();

    protected override async Task OnInitializedAsync()
    {
        // 1. Load valid supplier IDs
        var suppliers = await SupplierService.GetAllSuppliersAsync();
        SupplierExistsAttribute.SetValidSupplierIds(suppliers.Select(s => s.Id));
        
        // 2. T?o product
        product = new ProductDtoTest 
        { 
            Id = 1, 
            ProductName = "Test", 
            SupplierId = 999 // Invalid!
        };
        
        // 3. Validate
        product.Validate();
        
        // 4. Ki?m tra l?i
        if (product.HasErrors)
        {
            Console.WriteLine("Validation failed!");
        }
    }
}
```

Ðõn gi?n v?y thôi! Không c?n thay ð?i logic trong `Validate()` hay `ProductManagement.razor`.
