# Hý?ng D?n S? D?ng Validation Error Injection

## T?ng Quan

`ProductDtoTest` ð? ðý?c c?p nh?t ð? h? tr? inject validation errors t? server mà **KHÔNG C?N validate l?i** trên client.

### Tính Nãng Chính

? Gi? nguyên `ObservableValidator` t? CommunityToolkit.Mvvm  
? Implement `INotifyDataErrorInfo` ð? h? tr? Blazor validation  
? Inject errors vào **non-public members** (private fields)  
? Client **CH? C?N** g?i `SetErrors()` - không làm g? thêm  
? T? ð?ng merge validation errors + injected errors  

---

## Cách S? D?ng Trên Client

### 1. Set Errors T? Server Response

```csharp
// Server tr? v? validation errors d?ng Dictionary
var serverErrors = new Dictionary<string, List<string>>
{
    { "Id", new List<string> { "ID không h?p l?" } },
    { "SupplierId", new List<string> { "Supplier không t?n t?i", "Supplier ð? b? khóa" } },
    { "ProductName", new List<string> { "Tên s?n ph?m ð? t?n t?i" } }
};

// CH? C?N g?i SetErrors - KHÔNG LÀM G? THÊM
product.SetErrors(serverErrors);

// Blazor EditForm s? T? Ð?NG hi?n th? errors
// FlexGrid s? T? Ð?NG detect errors qua INotifyDataErrorInfo
```

### 2. Set Error Cho M?t Property

```csharp
// Set error cho m?t property c? th?
product.SetErrors("SupplierId", new List<string> 
{ 
    "Supplier ID không t?n t?i trong h? th?ng" 
});
```

### 3. Clear Errors

```csharp
// Clear t?t c? injected errors
product.ClearAllErrors();

// Clear error c?a m?t property
product.ClearErrors("SupplierId");
```

### 4. Check Errors

```csharp
// Check có errors không
if (product.HasErrors)
{
    // Get errors c?a m?t property
    var supplierErrors = product.GetErrors("SupplierId");
    
    foreach (var error in supplierErrors)
    {
        Console.WriteLine(error);
    }
}
```

---

## Server-Side: Tr? V? Validation Errors

### Controller Response

```csharp
[HttpPost("validate")]
public IActionResult ValidateProduct([FromBody] ProductDtoTest product)
{
    var errors = new Dictionary<string, List<string>>();
    
    // Validate from database
    if (!_context.Suppliers.Any(s => s.Id == product.SupplierId))
    {
        errors["SupplierId"] = new List<string> { "Supplier không t?n t?i" };
    }
    
    if (_context.Products.Any(p => p.Id == product.Id))
    {
        errors["Id"] = new List<string> { "ID ð? t?n t?i" };
    }
    
    if (errors.Any())
    {
        return BadRequest(new 
        { 
            isValid = false,
            errors = errors 
        });
    }
    
    return Ok(new { isValid = true });
}
```

### Client G?i API và Set Errors

```csharp
private async Task ValidateFromServer()
{
    var response = await Http.PostAsJsonAsync("api/products/validate", product);
    
    if (!response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
        
        if (result?.errors != null)
        {
            // CH? C?N 1 D?NG NÀY - KHÔNG LÀM G? THÊM
            product.SetErrors(result.errors);
        }
    }
}

public class ValidationResponse
{
    public bool isValid { get; set; }
    public Dictionary<string, List<string>>? errors { get; set; }
}
```

---

## Ví D? Trong ProductManagement.razor

```razor
@code {
    private List<ProductDtoTest> products = new();

    private async Task ValidateAllProducts()
    {
        // G?i API ð? validate
        var response = await Http.PostAsJsonAsync("api/products/batch-validate", products);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<BatchValidationResponse>();
            
            // result.errorsByIndex = Dictionary<int, Dictionary<string, List<string>>>
            // Key: index c?a item trong list
            // Value: errors c?a item ðó
            
            if (result?.errorsByIndex != null)
            {
                foreach (var kvp in result.errorsByIndex)
                {
                    var index = kvp.Key;
                    var errors = kvp.Value;
                    
                    // CH? C?N 1 D?NG - Set errors vào item
                    products[index].SetErrors(errors);
                }
            }
        }
        
        // FlexGrid s? T? Ð?NG hi?n th? errors
        await InvokeAsync(StateHasChanged);
    }
}
```

---

## L?i Ích

### ? Client Side
- **Không c?n validate l?i** - ti?t ki?m performance
- **Không c?n load cache** (SupplierIds, ExistingValues, v.v.)
- **Ch? c?n 1 d?ng code**: `product.SetErrors(serverErrors)`
- FlexGrid/EditForm **t? ð?ng** hi?n th? errors

### ? Server Side
- Validate ð?y ð? v?i database
- Tr? v? errors chi ti?t cho t?ng property
- Có th? validate non-public members (Id, SupplierId private fields)

### ? Hi?u Su?t
- Client không t?n th?i gian validate
- Server validate 1 l?n v?i full database context
- Không c?n serialize/deserialize cache l?n

---

## So Sánh Cách C? vs Cách M?i

### ? Cách C? (Client-side validation)
```csharp
// 1. Load cache t? server
var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");
var existingKeys = await Http.GetFromJsonAsync<List<string>>("api/products/keys");

// 2. Load vào ValidationScope
using (var scope = new MultiValidationScope()
    .LoadCache("SupplierIds", supplierIds)
    .LoadCache("ExistingValues", existingKeys))
{
    // 3. Validate t?ng item
    foreach (var product in products)
    {
        product.Validate();
    }
}

// Nhý?c ði?m:
// - Ph?i load nhi?u cache
// - Client ph?i có logic validation gi?ng server
// - T?n bandwidth và th?i gian
```

### ? Cách M?i (Server-side validation + Error injection)
```csharp
// 1. G?i API validate
var response = await Http.PostAsJsonAsync("api/products/validate", products);
var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();

// 2. Set errors - XONG!
foreach (var kvp in result.errorsByIndex)
{
    products[kvp.Key].SetErrors(kvp.Value);
}

// Ýu ði?m:
// - Không c?n load cache
// - Server validate chính xác v?i database
// - Client ch? c?n 1 API call
// - Code ðõn gi?n, r? ràng
```

---

## K?t Lu?n

V?i implementation này, client **CH? C?N**:
1. G?i API validate
2. Nh?n response ch?a errors
3. G?i `product.SetErrors(errors)`

**KHÔNG C?N**:
- ? Load cache
- ? ValidationScope
- ? Validate l?i
- ? B?t c? logic validation nào

M?i th? ð? ðý?c x? l? t? ð?ng thông qua `INotifyDataErrorInfo`! ??
