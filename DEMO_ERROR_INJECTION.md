# Demo: Inject Validation Errors Vào ProductDtoTest

## ? Ð? Hoàn Thành

`ProductDtoTest` ð? ðý?c c?p nh?t v?i các tính nãng:
- ? Gi? nguyên `ObservableValidator`
- ? Implement `INotifyDataErrorInfo` 
- ? Inject errors vào **non-public members** (private fields `id`, `supplierId`)
- ? Client **CH? C?N** g?i `SetErrors()` - không làm g? thêm

---

## Cách S? D?ng Ðõn Gi?n

### 1. Set Errors T? Server (Client CH? C?N 1 D?NG)

```csharp
// Server tr? v? errors d?ng Dictionary
var serverErrors = new Dictionary<string, List<string>>
{
    { "Id", new List<string> { "ID không h?p l?", "ID ð? t?n t?i" } },
    { "SupplierId", new List<string> { "Supplier không t?n t?i" } },
    { "ProductName", new List<string> { "Tên không ðý?c ð? tr?ng" } }
};

// CH? C?N 1 D?NG NÀY - XONG!
product.SetErrors(serverErrors);

// Blazor EditForm, FlexGrid s? T? Ð?NG hi?n th? errors
// V? ProductDtoTest implement INotifyDataErrorInfo
```

### 2. Set Error Cho 1 Property

```csharp
// Set error cho SupplierId
product.SetErrors("SupplierId", new List<string> 
{ 
    "Supplier không t?n t?i",
    "Supplier ð? b? khóa" 
});
```

### 3. Clear Errors

```csharp
// Clear ALL injected errors
product.ClearAllErrors();

// Clear error c?a 1 property
product.ClearInjectedErrors("SupplierId");
```

### 4. Check Errors

```csharp
// Check có errors không
if (product.HasErrors)
{
    // Get errors c?a m?t property (bao g?m c? validation errors + injected errors)
    var errors = product.GetErrors("SupplierId");
    
    foreach (var error in errors)
    {
        Console.WriteLine(error); // string error message
    }
}
```

---

## Demo Code Trong ProductManagement.razor

### Scenario 1: Validate T? Server API

```csharp
private async Task ValidateFromServer()
{
    try
    {
        // G?i API validate
        var response = await Http.PostAsJsonAsync("api/products/validate", products);
        
        if (!response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();
            
            // result.errorsByIndex = Dictionary<int, Dictionary<string, List<string>>>
            // Key: index c?a product trong list
            // Value: Dictionary<propertyName, List<errorMessages>>
            
            if (result?.errorsByIndex != null)
            {
                foreach (var kvp in result.errorsByIndex)
                {
                    var index = kvp.Key;
                    var productErrors = kvp.Value;
                    
                    // CH? C?N 1 D?NG - Set errors vào product
                    products[index].SetErrors(productErrors);
                }
            }
        }
        
        // FlexGrid/EditForm t? ð?ng hi?n th? errors
        await InvokeAsync(StateHasChanged);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

// Response model
public class ValidationResponse
{
    public bool IsValid { get; set; }
    public Dictionary<int, Dictionary<string, List<string>>>? errorsByIndex { get; set; }
}
```

### Scenario 2: Manual Inject Errors

```csharp
private void TestInjectErrors()
{
    var product = products[0]; // First product
    
    // T?o errors manually
    var errors = new Dictionary<string, List<string>>
    {
        { "Id", new List<string> { "ID ph?i l?n hõn 0" } },
        { "SupplierId", new List<string> 
            { 
                "Supplier không t?n t?i",
                "Supplier ID ph?i là s? dýõng"
            } 
        }
    };
    
    // Set errors - XONG!
    product.SetErrors(errors);
    
    // UI t? ð?ng update
    StateHasChanged();
}
```

### Scenario 3: Clear Errors Sau Khi Fix

```csharp
private void ClearProductErrors()
{
    foreach (var product in products)
    {
        // Clear t?t c? injected errors
        product.ClearAllErrors();
    }
    
    // Ho?c clear t?ng property
    products[0].ClearInjectedErrors("SupplierId");
    
    StateHasChanged();
}
```

---

## Server-Side Controller Example

```csharp
[HttpPost("validate")]
public IActionResult ValidateProducts([FromBody] List<ProductDtoTest> products)
{
    var errorsByIndex = new Dictionary<int, Dictionary<string, List<string>>>();
    
    for (int i = 0; i < products.Count; i++)
    {
        var product = products[i];
        var productErrors = new Dictionary<string, List<string>>();
        
        // Validate Id
        if (!_context.Products.Any(p => p.Id == product.Id))
        {
            productErrors["Id"] = new List<string> { "ID không t?n t?i trong database" };
        }
        
        // Validate SupplierId
        if (!_context.Suppliers.Any(s => s.Id == product.SupplierId))
        {
            productErrors["SupplierId"] = new List<string> 
            { 
                "Supplier không t?n t?i",
                $"Không t?m th?y Supplier ID {product.SupplierId}"
            };
        }
        
        // Validate ProductName
        if (string.IsNullOrWhiteSpace(product.ProductName))
        {
            productErrors["ProductName"] = new List<string> { "Tên s?n ph?m không ðý?c ð? tr?ng" };
        }
        
        // N?u có l?i th? add vào result
        if (productErrors.Any())
        {
            errorsByIndex[i] = productErrors;
        }
    }
    
    if (errorsByIndex.Any())
    {
        return BadRequest(new 
        {
            isValid = false,
            errorsByIndex = errorsByIndex
        });
    }
    
    return Ok(new { isValid = true });
}
```

---

## Cách Ho?t Ð?ng Bên Trong

### INotifyDataErrorInfo Implementation

```csharp
// 1. Dictionary lýu injected errors
private readonly Dictionary<string, List<string>> _injectedErrors = new();

// 2. Dummy field ð? trigger ErrorsChanged event
private int _errorTrigger;

// 3. Method ð? raise ErrorsChanged event
private void RaiseErrorsChanged(string propertyName)
{
    // S? d?ng SetProperty v?i dummy field
    // ObservableValidator s? t? ð?ng raise ErrorsChanged event
    SetProperty(ref _errorTrigger, _errorTrigger + 1, false, propertyName);
}

// 4. GetErrors() merge c? validation errors + injected errors
public new IEnumerable GetErrors(string? propertyName)
{
    var errors = new List<string>();
    
    // Get validation errors t? base class
    var baseErrors = base.GetErrors(propertyName);
    // ... add to errors list
    
    // Get injected errors t? dictionary
    if (_injectedErrors.TryGetValue(propertyName, out var injectedErrors))
    {
        errors.AddRange(injectedErrors);
    }
    
    return errors;
}

// 5. HasErrors check c? 2 lo?i
public new bool HasErrors => base.HasErrors || _injectedErrors.Any();
```

---

## So Sánh: Trý?c vs Sau

### ? TRÝ?C (Client-side validation - ph?c t?p)

```csharp
// 1. Load cache t? server (nhi?u API calls)
var supplierIds = await Http.GetFromJsonAsync<List<int>>("api/suppliers/ids");
var existingKeys = await Http.GetFromJsonAsync<List<string>>("api/products/keys");
var categories = await Http.GetFromJsonAsync<List<string>>("api/categories");

// 2. Load vào ValidationScope
using (var scope = new MultiValidationScope()
    .LoadCache("SupplierIds", supplierIds)
    .LoadCache("ExistingValues", existingKeys)
    .LoadCache("Categories", categories))
{
    // 3. Validate t?ng item
    foreach (var product in products)
    {
        product.Validate(); // Ch?y validation attributes
    }
}

// 4. Refresh UI
StateHasChanged();
```

**Nhý?c ði?m:**
- Ph?i load nhi?u cache (t?n bandwidth)
- Client ph?i có logic validation gi?ng server
- Không validate ðý?c business rules ph?c t?p
- Không th? validate v?i database real-time

### ? SAU (Server-side validation + Error injection - ðõn gi?n)

```csharp
// 1. G?I API VALIDATE - 1 API CALL DUY NH?T
var response = await Http.PostAsJsonAsync("api/products/validate", products);
var result = await response.Content.ReadFromJsonAsync<ValidationResponse>();

// 2. SET ERRORS - XONG!
foreach (var kvp in result.errorsByIndex)
{
    products[kvp.Key].SetErrors(kvp.Value);
}

// 3. Refresh UI
StateHasChanged();
```

**Ýu ði?m:**
- ? CH? 1 API call
- ? Server validate chính xác v?i database
- ? Validate ðý?c business rules ph?c t?p
- ? Client code ðõn gi?n, r? ràng
- ? Không c?n cache, không t?n bandwidth
- ? Inject ðý?c vào non-public members (private fields)

---

## Test Nhanh

```csharp
// T?o product
var product = new ProductDtoTest
{
    Id = 1,
    ProductName = "Test Product",
    SupplierId = 999
};

// Inject errors manually
product.SetErrors(new Dictionary<string, List<string>>
{
    { "Id", new List<string> { "Error 1", "Error 2" } },
    { "SupplierId", new List<string> { "Supplier không t?n t?i" } }
});

// Check
Console.WriteLine(product.HasErrors); // True

// Get errors
var idErrors = product.GetErrors("Id");
foreach (var error in idErrors)
{
    Console.WriteLine(error);
    // Output:
    // Error 1
    // Error 2
}

// Clear
product.ClearAllErrors();
Console.WriteLine(product.HasErrors); // False
```

---

## K?t Lu?n

V?i implementation này, **client CH? C?N**:

1. G?i API validate
2. Nh?n response v?i errors
3. G?i `product.SetErrors(errors)`

**KHÔNG C?N**:
- ? Load cache
- ? ValidationScope  
- ? Validate l?i
- ? Logic validation ph?c t?p

M?i th? t? ð?ng! FlexGrid, EditForm s? t? detect errors qua `INotifyDataErrorInfo` ??
