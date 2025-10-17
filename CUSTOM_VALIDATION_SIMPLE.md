# Custom Validation - SupplierExists (Simplified)

## Gi?i thi?u

Custom validation attribute `[SupplierExists]` ��n gi?n �? ki?m tra Supplier ID c� t?n t?i hay kh�ng. Ch? c?n th�m attribute v�o property v� g?i `Validate()` nh� b?nh th�?ng.

## C�ch s? d?ng

### 1. Th�m Attribute v�o Model

```csharp
using BlazorWasmHosted.Shared.ValidationAttributes;

public class ProductDtoTest : ObservableValidator
{
    [SupplierExists(ErrorMessage = "Supplier ID kh�ng t?n t?i trong h? th?ng")]
    public int SupplierId { get; set; }

    public void Validate()
    {
        ValidateAllProperties(); // T? �?ng validate SupplierExists
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

### 3. Validate nh� b?nh th�?ng

```csharp
private void ValidateProduct()
{
    product.Validate(); // T? �?ng check SupplierExists
    
    if (product.HasErrors)
    {
        // X? l? l?i
    }
}
```

## API Methods

```csharp
// Set to�n b? danh s�ch IDs h?p l?
SupplierExistsAttribute.SetValidSupplierIds(new[] { 1, 2, 3, 4, 5 });

// Th�m m?t ID
SupplierExistsAttribute.AddValidSupplierId(6);

// Clear t?t c?
SupplierExistsAttribute.ClearValidSupplierIds();
```

## V� d? ho�n ch?nh

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

��n gi?n v?y th�i! Kh�ng c?n thay �?i logic trong `Validate()` hay `ProductManagement.razor`.
