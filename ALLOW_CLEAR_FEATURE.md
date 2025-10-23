# AllowClear Feature - SearchableDropdown & SupplierDropdown

## Overview

Tính nãng **AllowClear** cho phép user clear selection (set v? null) trong dropdown.

## Features

? **Clear button** - Button ð? v?i icon X  
? **Optional** - Default: `false`  
? **Conditional display** - Ch? hi?n khi có selection  
? **Null handling** - Set SelectedItem v? `default` (null)  

---

## SearchableDropdown Component

### New Parameter

```csharp
[Parameter] public bool AllowClear { get; set; } = false;
```

### UI Changes

```razor
@if (AllowClear && SelectedItem != null)
{
    <button type="button"
            class="dropdown-item text-danger"
            @onclick="ClearSelection">
        <i class="bi bi-x-circle me-2"></i>Clear selection
    </button>
    <div class="dropdown-divider"></div>
}
```

### Clear Method

```csharp
private async Task ClearSelection()
{
    SelectedItem = default;
    await SelectedItemChanged.InvokeAsync(default!);
    IsOpen = false;
    searchText = "";
}
```

---

## Usage Examples

### 1. Basic Usage

```razor
<SearchableDropdown TItem="Product"
                    Items="@products"
                    SelectedItem="@selectedProduct"
                    SelectedItemChanged="@((p) => selectedProduct = p)"
                    ItemTextField="@(p => p.ProductName)"
                    AllowClear="true" />
```

### 2. SupplierDropdown

```razor
<SupplierDropdown SelectedSupplier="@supplier"
                  SelectedSupplierChanged="@((s) => supplier = s)"
                  AllowClear="true" />
```

### 3. With Event Handler

```razor
<SearchableDropdown TItem="Supplier"
                    Items="@suppliers"
                    SelectedItem="@selectedSupplier"
                    SelectedItemChanged="@OnSupplierChanged"
                    AllowClear="true" />

@code {
    private Supplier? selectedSupplier;
    
    private async Task OnSupplierChanged(Supplier? supplier)
    {
        selectedSupplier = supplier;
        
        if (supplier == null)
        {
            Console.WriteLine("Selection cleared");
        }
        else
        {
            Console.WriteLine($"Selected: {supplier.SupplierName}");
        }
    }
}
```

### 4. Conditional AllowClear

```razor
<SearchableDropdown TItem="Product"
                    Items="@products"
                    SelectedItem="@selectedProduct"
                    SelectedItemChanged="@((p) => selectedProduct = p)"
                    AllowClear="@(!isRequired)" />

@code {
    private bool isRequired = false; // Toggle based on logic
}
```

---

## Visual Behavior

### Before Selection
```
??????????????????????????????????
? Select...                   ?  ?
??????????????????????????????????
  ? Click
??????????????????????????????????
? Search...                      ?
??????????????????????????????????
? Item 1                         ?
? Item 2                         ?
? Item 3                    ?    ? ? Selected
??????????????????????????????????
```

### After Selection (AllowClear=true)
```
??????????????????????????????????
? Item 3                      ?  ?
??????????????????????????????????
  ? Click
??????????????????????????????????
? Search...                      ?
??????????????????????????????????
? ? Clear selection          ? ? RED, at top
??????????????????????????????????
? Item 1                         ?
? Item 2                         ?
? Item 3                    ?    ?
??????????????????????????????????
```

### After Clear
```
??????????????????????????????????
? Select...                   ?  ? ? Back to placeholder
??????????????????????????????????
```

---

## CSS Styling

### Clear Button
```html
<button class="dropdown-item text-danger">
    <i class="bi bi-x-circle me-2"></i>Clear selection
</button>
```

**Bootstrap classes:**
- `dropdown-item` - Standard dropdown item
- `text-danger` - Red text (#dc3545)

**Bootstrap icon:**
- `bi bi-x-circle` - X icon trong circle
- `me-2` - Margin right

### Divider
```html
<div class="dropdown-divider"></div>
```

---

## Parameters Comparison

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `SelectedItem` | `TItem?` | `null` | Current selection |
| `SelectedItemChanged` | `EventCallback<TItem>` | - | Event when changed |
| `AllowClear` | `bool` | `false` | ? Show clear button |
| `Items` | `List<TItem>` | `new()` | Dropdown items |
| `Placeholder` | `string` | "Select..." | Empty text |
| `Disabled` | `bool` | `false` | Disable dropdown |

---

## Code Flow

### 1. User Selects Item
```csharp
private async Task SelectItem(TItem item)
{
    SelectedItem = item;                              // Set item
    await SelectedItemChanged.InvokeAsync(item);      // Trigger event
    IsOpen = false;                                   // Close dropdown
    searchText = "";                                  // Clear search
}
```

### 2. User Clicks Clear
```csharp
private async Task ClearSelection()
{
    SelectedItem = default;                           // Set to null
    await SelectedItemChanged.InvokeAsync(default!);  // Trigger event with null
    IsOpen = false;                                   // Close dropdown
    searchText = "";                                  // Clear search
}
```

### 3. UI Re-renders
```razor
@GetDisplayText()  // Returns Placeholder if SelectedItem is null
```

---

## SupplierDropdown Integration

### Parameter Pass-through

```razor
<!-- SupplierDropdown.razor -->
<SearchableDropdown TItem="Supplier"
                    AllowClear="@AllowClear" />  ? Pass through

@code {
    [Parameter] public bool AllowClear { get; set; } = false;
}
```

### Usage

```razor
<!-- Your page -->
<SupplierDropdown SelectedSupplier="@supplier"
                  SelectedSupplierChanged="@((s) => supplier = s)"
                  AllowClear="true" />  ? Enable clear

@if (supplier == null)
{
    <p>No supplier selected</p>
}
else
{
    <p>Selected: @supplier.SupplierName</p>
}
```

---

## Use Cases

### 1. Optional Fields
```razor
<!-- User can clear optional supplier -->
<SupplierDropdown SelectedSupplier="@optionalSupplier"
                  SelectedSupplierChanged="@((s) => optionalSupplier = s)"
                  AllowClear="true" />
```

### 2. Filter Reset
```razor
<!-- Clear filter to show all items -->
<SearchableDropdown TItem="string"
                    Items="@categories"
                    SelectedItem="@filterCategory"
                    SelectedItemChanged="@OnCategoryChanged"
                    Placeholder="All categories"
                    AllowClear="true" />

@code {
    private async Task OnCategoryChanged(string? category)
    {
        filterCategory = category;
        
        if (category == null)
        {
            // Show all products
            filteredProducts = allProducts;
        }
        else
        {
            // Filter by category
            filteredProducts = allProducts.Where(p => p.Category == category).ToList();
        }
    }
}
```

### 3. Multi-step Form
```razor
<!-- User can go back and change selection -->
<SupplierDropdown SelectedSupplier="@selectedSupplier"
                  SelectedSupplierChanged="@OnSupplierChanged"
                  AllowClear="true"
                  Disabled="@isStep3" />

@code {
    private async Task OnSupplierChanged(Supplier? supplier)
    {
        selectedSupplier = supplier;
        
        if (supplier == null)
        {
            // Reset dependent fields
            selectedProducts.Clear();
            currentStep = 1;
        }
    }
}
```

---

## Testing Scenarios

### ? Functional Tests

1. **Clear button visibility**
   - ? Not visible when `AllowClear=false`
   - ? Not visible when no selection
   - ? Visible when `AllowClear=true` AND has selection

2. **Clear behavior**
   - ? Click Clear ? SelectedItem becomes null
   - ? Event fires with null value
   - ? Dropdown closes
   - ? Placeholder displays

3. **Re-selection after clear**
   - ? Can select item again
   - ? Clear button reappears
   - ? Normal selection flow

### ? Edge Cases

1. **Multiple clears**
   - ? Clear when already null (no error)
   - ? Clear button hides after clear

2. **Disabled state**
   - ? Cannot open dropdown when disabled
   - ? Clear not accessible when disabled

3. **Null handling**
   - ? `default!` safely handles nullable types
   - ? Event callback receives null

---

## Migration Guide

### Before (No Clear)

```razor
<SupplierDropdown SelectedSupplier="@supplier"
                  SelectedSupplierChanged="@((s) => supplier = s)" />

<!-- Manual clear button -->
<button @onclick="@(() => supplier = null)">Clear</button>
```

### After (With Clear)

```razor
<SupplierDropdown SelectedSupplier="@supplier"
                  SelectedSupplierChanged="@((s) => supplier = s)"
                  AllowClear="true" />

<!-- Built-in clear button ? -->
```

---

## Benefits

| Feature | Before | After |
|---------|--------|-------|
| Clear UI | Manual button | Built-in |
| Consistency | Varies | Standardized |
| UX | Extra click | Inside dropdown |
| Code | Extra markup | 1 parameter |
| Styling | Custom | Bootstrap |

---

## Summary

### New Code
- ? 1 new parameter: `AllowClear`
- ? 1 new method: `ClearSelection()`
- ? 1 new UI section: Clear button + divider

### Usage
```razor
<!-- Enable clear -->
AllowClear="true"

<!-- Disable clear (default) -->
AllowClear="false"
```

### Result
- ? User-friendly clear option
- ? Null handling
- ? Event notification
- ? Bootstrap styling

---

**Version:** 1.0  
**Build Status:** ? Success  
**Breaking Changes:** None (backward compatible)
