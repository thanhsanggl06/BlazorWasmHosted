# SupplierDropdown Component - Usage Guide

## T?ng Quan

**SupplierDropdown** là component wrapper c?a SearchableDropdown, ðý?c t?i ýu hóa ð? s? d?ng v?i Supplier entities. Component t? ð?ng load d? li?u t? API và cung c?p các format hi?n th? khác nhau.

## Tính Nãng

? **Auto-load data** - T? ð?ng load suppliers t? API  
? **Click outside to close** - Click ra ngoài s? ðóng dropdown  
? **Multiple display formats** - 4 format hi?n th? khác nhau  
? **Search functionality** - T?m ki?m suppliers  
? **Keyboard support** - ESC ð? ðóng  
? **Easy to use** - Ch? c?n 1 d?ng code  

## Basic Usage

### 1. Import Component

```razor
@using BlazorWasmHosted.Components.Components.Dropdown
```

### 2. Simple Usage

```razor
<SupplierDropdown SelectedSupplier="@selectedSupplier"
                  SelectedSupplierChanged="@((s) => selectedSupplier = s)" />

@code {
    private Supplier? selectedSupplier;
}
```

### 3. With All Parameters

```razor
<SupplierDropdown SelectedSupplier="@selectedSupplier"
                  SelectedSupplierChanged="@OnSupplierChanged"
                  DisplayFormat="SupplierDropdown.SupplierDisplayFormat.CodeAndName"
                  Placeholder="Choose a supplier..."
                  SearchPlaceholder="Type to search..."
                  Disabled="@isProcessing" />

@code {
    private Supplier? selectedSupplier;
    private bool isProcessing = false;
    
    private async Task OnSupplierChanged(Supplier supplier)
    {
        selectedSupplier = supplier;
        // Your logic here
    }
}
```

## Display Formats

### 1. NameOnly
```razor
<SupplierDropdown DisplayFormat="SupplierDropdown.SupplierDisplayFormat.NameOnly" />
```
**Output**: `ABC Corporation`

### 2. CodeAndName (Default)
```razor
<SupplierDropdown DisplayFormat="SupplierDropdown.SupplierDisplayFormat.CodeAndName" />
```
**Output**: `SUP001 - ABC Corporation`

### 3. NameWithCountry
```razor
<SupplierDropdown DisplayFormat="SupplierDropdown.SupplierDisplayFormat.NameWithCountry" />
```
**Output**: `ABC Corporation (USA)`

### 4. Full
```razor
<SupplierDropdown DisplayFormat="SupplierDropdown.SupplierDisplayFormat.Full" />
```
**Output**: `SUP001 - ABC Corporation (USA)`

## Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `SelectedSupplier` | `Supplier?` | ? | `null` | Currently selected supplier |
| `SelectedSupplierChanged` | `EventCallback<Supplier>` | ? | - | Event when selection changes |
| `DisplayFormat` | `SupplierDisplayFormat` | ? | `CodeAndName` | How to display supplier |
| `Placeholder` | `string` | ? | "Select supplier..." | Placeholder text |
| `SearchPlaceholder` | `string` | ? | "Search suppliers..." | Search box placeholder |
| `Disabled` | `bool` | ? | `false` | Disable dropdown |

## Public Methods

### RefreshAsync()
Reload suppliers from API
```razor
<SupplierDropdown @ref="supplierDropdown" />

<button @onclick="Refresh">Refresh</button>

@code {
    private SupplierDropdown supplierDropdown = default!;
    
    private async Task Refresh()
    {
        await supplierDropdown.RefreshAsync();
    }
}
```

### GetSuppliers()
Get all loaded suppliers
```razor
@code {
    private void PrintSuppliers()
    {
        var suppliers = supplierDropdown.GetSuppliers();
        Console.WriteLine($"Total suppliers: {suppliers.Count}");
    }
}
```

### IsLoaded
Check if suppliers are loaded
```razor
@if (supplierDropdown.IsLoaded)
{
    <p>Suppliers loaded successfully!</p>
}
```

## Click Outside Feature

Component t? ð?ng ðóng khi:
- ? Click ra ngoài dropdown
- ? Nh?n ESC
- ? Ch?n m?t item

### JavaScript Integration

Component s? d?ng `dropdown.js` ð? x? l? click outside:

**File**: `wwwroot/js/dropdown.js`
```javascript
export function addClickOutsideListener(dropdownElement, dotNetHelper) {
    // Adds click handler
}

export function removeClickOutsideListener() {
    // Removes click handler
}
```

## Real-World Examples

### Example 1: Simple Form
```razor
@page "/order-form"
@using BlazorWasmHosted.Components.Components.Dropdown

<EditForm Model="@order">
    <div class="mb-3">
        <label class="form-label">Supplier *</label>
        <SupplierDropdown SelectedSupplier="@order.Supplier"
                          SelectedSupplierChanged="@((s) => order.Supplier = s)" />
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
</EditForm>

@code {
    private OrderModel order = new();
    
    class OrderModel
    {
        public Supplier? Supplier { get; set; }
    }
}
```

### Example 2: Filter Products by Supplier
```razor
<SupplierDropdown SelectedSupplier="@selectedSupplier"
                  SelectedSupplierChanged="@OnSupplierChanged"
                  DisplayFormat="SupplierDropdown.SupplierDisplayFormat.NameOnly" />

@if (products.Any())
{
    <ul>
        @foreach (var product in products)
        {
            <li>@product.ProductName</li>
        }
    </ul>
}

@code {
    private Supplier? selectedSupplier;
    private List<Product> products = new();
    
    private async Task OnSupplierChanged(Supplier supplier)
    {
        selectedSupplier = supplier;
        products = await LoadProductsBySupplier(supplier.Id);
    }
}
```

### Example 3: With Loading State
```razor
<div class="mb-3">
    <label class="form-label">Supplier</label>
    <SupplierDropdown SelectedSupplier="@selectedSupplier"
                      SelectedSupplierChanged="@OnSupplierChanged"
                      Disabled="@isLoading" />
    
    @if (isLoading)
    {
        <small class="text-muted">Loading products...</small>
    }
</div>

@code {
    private Supplier? selectedSupplier;
    private bool isLoading = false;
    
    private async Task OnSupplierChanged(Supplier supplier)
    {
        isLoading = true;
        try
        {
            selectedSupplier = supplier;
            await LoadRelatedData(supplier);
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

### Example 4: Multiple Dropdowns
```razor
<div class="row">
    <div class="col-md-6">
        <label>Primary Supplier</label>
        <SupplierDropdown SelectedSupplier="@primarySupplier"
                          SelectedSupplierChanged="@((s) => primarySupplier = s)"
                          DisplayFormat="SupplierDropdown.SupplierDisplayFormat.CodeAndName" />
    </div>
    
    <div class="col-md-6">
        <label>Backup Supplier</label>
        <SupplierDropdown SelectedSupplier="@backupSupplier"
                          SelectedSupplierChanged="@((s) => backupSupplier = s)"
                          DisplayFormat="SupplierDropdown.SupplierDisplayFormat.CodeAndName" />
    </div>
</div>

@code {
    private Supplier? primarySupplier;
    private Supplier? backupSupplier;
}
```

### Example 5: With Validation
```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    
    <div class="mb-3">
        <label class="form-label">Supplier *</label>
        <SupplierDropdown SelectedSupplier="@model.Supplier"
                          SelectedSupplierChanged="@((s) => model.Supplier = s)" />
        <ValidationMessage For="@(() => model.Supplier)" />
    </div>
    
    <button type="submit">Submit</button>
</EditForm>

@code {
    private FormModel model = new();
    
    class FormModel
    {
        [Required(ErrorMessage = "Supplier is required")]
        public Supplier? Supplier { get; set; }
    }
    
    private void HandleSubmit()
    {
        // Submit logic
    }
}
```

## Comparison with SearchableDropdown

| Feature | SearchableDropdown | SupplierDropdown |
|---------|-------------------|------------------|
| Data Loading | Manual | Automatic |
| Generic Type | Yes | Fixed (Supplier) |
| Display Format | Custom function | Predefined formats |
| API Integration | No | Built-in |
| Lines of Code | ~10 lines | 1 line |
| Flexibility | High | Medium |
| Ease of Use | Medium | Very Easy |

## When to Use

### Use SupplierDropdown When:
- ? You need to select a Supplier
- ? You want auto-loading from API
- ? Standard display formats are enough
- ? Quick implementation needed

### Use SearchableDropdown When:
- ? Custom entity type
- ? Custom display format
- ? Pre-loaded data
- ? More control needed

## Performance

- ? **Single API Call** - Load suppliers once on init
- ? **Cached Data** - No re-loading on each render
- ? **Efficient Search** - Client-side filtering
- ? **Memory Efficient** - Proper dispose pattern

## Troubleshooting

### Issue: Dropdown not closing on click outside
**Solution**: Make sure `dropdown.js` is in `wwwroot/js/` folder

### Issue: No suppliers showing
**Solution**: Check API endpoint `/api/suppliers` is working

### Issue: Display format not changing
**Solution**: Use correct enum: `SupplierDropdown.SupplierDisplayFormat.XXX`

### Issue: Multiple instances interfering
**Solution**: Each instance is independent, no conflicts

## Advanced Usage

### Custom Event Handling
```razor
<SupplierDropdown SelectedSupplierChanged="@OnSupplierSelected" />

@code {
    private async Task OnSupplierSelected(Supplier supplier)
    {
        // Log
        Console.WriteLine($"Selected: {supplier.SupplierName}");
        
        // Update other components
        await LoadProducts(supplier.Id);
        
        // Show notification
        await ShowNotification($"Supplier {supplier.SupplierName} selected");
        
        // Trigger state change
        StateHasChanged();
    }
}
```

### Conditional Display Format
```razor
<SupplierDropdown DisplayFormat="@GetDisplayFormat()" />

@code {
    private bool isMobile = false;
    
    private SupplierDropdown.SupplierDisplayFormat GetDisplayFormat()
    {
        return isMobile 
            ? SupplierDropdown.SupplierDisplayFormat.NameOnly 
            : SupplierDropdown.SupplierDisplayFormat.CodeAndName;
    }
}
```

### Pre-select Default Supplier
```razor
<SupplierDropdown SelectedSupplier="@defaultSupplier"
                  SelectedSupplierChanged="@((s) => selectedSupplier = s)" />

@code {
    private Supplier? selectedSupplier;
    private Supplier? defaultSupplier;
    
    protected override async Task OnInitializedAsync()
    {
        // Get default supplier from API or settings
        defaultSupplier = await GetDefaultSupplier();
        selectedSupplier = defaultSupplier;
    }
}
```

## Browser Support

? Chrome/Edge (Chromium)  
? Firefox  
? Safari  
? Mobile browsers  

## Dependencies

- `BlazorWasmHosted.Shared` (Supplier entity)
- `BlazorWasmHosted.Components.Components.Dropdown` (SearchableDropdown)
- `System.Net.Http.Json` (GetFromJsonAsync)
- `dropdown.js` (Click outside handler)

## Files

```
BlazorWasmHosted.Components/
??? Components/
    ??? Dropdown/
        ??? SearchableDropdown.razor          # Base component
        ??? SearchableDropdown.razor.css      # Styles
        ??? SupplierDropdown.razor            # Supplier wrapper

BlazorWasmHosted.Client/
??? wwwroot/
    ??? js/
        ??? dropdown.js                        # Click outside handler
```

---

**Version**: 1.0  
**Features**: Auto-load + Click Outside  
**Build Status**: ? Success
