# Searchable Dropdown - Product & Supplier Demo

## T?ng Quan

Demo page này minh h?a cách s? d?ng **SearchableDropdown component** v?i d? li?u th?c t? t? database (Products và Suppliers), không s? d?ng icon màu bên trái.

## ? Tính Nãng M?i

### 1. **Multiple Size Variations** (Nhi?u Kích Thý?c)
- ? Extra Small (25% / col-md-3)
- ? Small (33% / col-md-4)
- ? Medium (42% / col-md-5)
- ? Large (50% / col-md-6)
- ? Full Width (100% / col-md-12)

### 2. **Pre-selected Default Values** (Giá Tr? M?c Ð?nh)
- ? T?t c? dropdown có giá tr? ðý?c ch?n s?n khi load
- ? Demo ngay l?p t?c không c?n user interaction
- ? Hi?n th? selected state và details

## C?u Trúc Demo

### 1. **Different Sizes Demo** (Kích Thý?c Khác Nhau)

#### Extra Small (25%)
```razor
<div class="col-md-3">
    <SearchableDropdown TItem="Product"
                        Items="@products.Take(5).ToList()"
                        SelectedItem="@sizeXsProduct"
                        ItemTextField="@(p => p.ProductName)" />
</div>
```
- Hi?n th? tên product
- Top 5 products
- Pre-selected: Product ð?u tiên

#### Small (33%)
```razor
<div class="col-md-4">
    <SearchableDropdown TItem="Supplier"
                        Items="@suppliers"
                        SelectedItem="@sizeSmSupplier"
                        ItemTextField="@(s => s.SupplierName)" />
</div>
```
- Hi?n th? tên supplier
- No pre-selected (ð? demo empty state)

#### Medium (42%)
```razor
<div class="col-md-5">
    <SearchableDropdown TItem="Product"
                        Items="@products"
                        SelectedItem="@sizeMdProduct"
                        ItemTextField="@(p => $"{p.ProductCode} - {p.ProductName}")" />
</div>
```
- Hi?n th? code + name
- Pre-selected: Product th? 2
- Badge "Pre-selected" trong header

#### Large (50%)
```razor
<div class="col-md-6">
    <SearchableDropdown TItem="Product"
                        Items="@products"
                        SelectedItem="@sizeLgProduct" />
</div>
```
- Standard size
- No pre-selected

#### Full Width (100%)
```razor
<div class="col-md-6"> <!-- Inside row, takes full 50% -->
    <SearchableDropdown TItem="Supplier"
                        Items="@suppliers"
                        SelectedItem="@sizeFullSupplier"
                        ItemTextField="@(s => $"{s.SupplierCode} - {s.SupplierName} ({s.Country ?? "N/A"})")" />
</div>
```
- Complex format v?i code, name, country
- Pre-selected: Supplier ð?u tiên

### 2. **Single Product Selection** (Default Selected)
- Pre-selected: Product th? 3
- Hi?n th? full details ngay khi load
- Card màu xanh (success)

### 3. **Single Supplier Selection** (Default Selected)
- Pre-selected: Supplier th? 2
- Hi?n th? full details ngay khi load
- Card màu info

### 4. **Filter Products by Category** (Default Selected)
- Pre-selected category: Category ð?u tiên
- Products dropdown auto-populated
- Pre-selected product: Product ð?u tiên trong category
- Badge hi?n th? s? items trong category

### 5. **View Products by Supplier** (Default Selected)
- Pre-selected supplier: Supplier th? 3
- Products dropdown auto-populated
- Pre-selected product: Product ð?u tiên c?a supplier
- Badge hi?n th? s? products
- Alert hi?n th? selected product + supplier

## Code Implementation

### Set Default Values

```csharp
private void SetDefaultValues()
{
    // Size demo defaults
    sizeXsProduct = products.FirstOrDefault();
    sizeMdProduct = products.Skip(1).FirstOrDefault();
    sizeFullSupplier = suppliers.FirstOrDefault();

    // Main selections defaults
    selectedProduct = products.Skip(2).FirstOrDefault();
    selectedSupplier = suppliers.Skip(1).FirstOrDefault();

    // Category filter default
    if (categories.Any())
    {
        selectedCategory = categories.First();
        filteredProducts = products
            .Where(p => p.Category == selectedCategory)
            .ToList();
        selectedFilteredProduct = filteredProducts.FirstOrDefault();
    }

    // Supplier products default
    if (suppliers.Any())
    {
        selectedSupplierForProducts = suppliers.Skip(2).FirstOrDefault() ?? suppliers.FirstOrDefault();
        if (selectedSupplierForProducts != null)
        {
            supplierProducts = products
                .Where(p => p.SupplierId == selectedSupplierForProducts.Id)
                .ToList();
            selectedSupplierProduct = supplierProducts.FirstOrDefault();
        }
    }
}
```

### Load Data with Defaults

```csharp
private async Task LoadData()
{
    try
    {
        isLoading = true;

        // Parallel loading
        var productsTask = Http.GetFromJsonAsync<List<Product>>("api/products");
        var suppliersTask = Http.GetFromJsonAsync<List<Supplier>>("api/suppliers");
        await Task.WhenAll(productsTask, suppliersTask);

        products = productsTask.Result ?? new();
        suppliers = suppliersTask.Result ?? new();

        // Extract categories
        categories = products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        // ? Set default values
        SetDefaultValues();
    }
    finally
    {
        isLoading = false;
    }
}
```

## Size Guide

### Bootstrap Column Widths

| Size | Bootstrap Class | Width % | Use Case |
|------|----------------|---------|----------|
| XS | `col-md-3` | 25% | Compact lists, IDs |
| SM | `col-md-4` | 33% | Short names |
| MD | `col-md-5` | 42% | Code + Name |
| LG | `col-md-6` | 50% | Standard forms |
| XL | `col-md-12` | 100% | Long descriptions |

### Custom Width

```razor
<div style="width: 200px;">
    <SearchableDropdown ... />
</div>

<div style="width: 50%;">
    <SearchableDropdown ... />
</div>

<div style="max-width: 400px;">
    <SearchableDropdown ... />
</div>
```

## Default Value Strategies

### 1. First Item
```csharp
selectedItem = items.FirstOrDefault();
```

### 2. Skip First (Get Second)
```csharp
selectedItem = items.Skip(1).FirstOrDefault();
```

### 3. By Condition
```csharp
selectedItem = items.FirstOrDefault(x => x.IsDefault);
selectedItem = items.FirstOrDefault(x => x.IsActive);
```

### 4. By ID
```csharp
selectedItem = items.FirstOrDefault(x => x.Id == defaultId);
```

### 5. Last Item
```csharp
selectedItem = items.LastOrDefault();
```

### 6. Random
```csharp
var random = new Random();
selectedItem = items.OrderBy(x => random.Next()).FirstOrDefault();
```

## Visual Indicators

### Badge Count
```razor
<label>
    Select Product:
    @if (items.Any())
    {
        <span class="badge bg-primary">@items.Count items</span>
    }
</label>
```

### Success Checkmark
```razor
@if (selectedItem != null)
{
    <small class="text-success d-block mt-2">
        ? @selectedItem.Name
    </small>
}
```

### Alert with Details
```razor
@if (selectedProduct != null && selectedSupplier != null)
{
    <div class="alert alert-info">
        <strong>Product:</strong> @selectedProduct.Name <br />
        <strong>Supplier:</strong> @selectedSupplier.Name <br />
        <strong>Price:</strong> $@selectedProduct.Price:N2
    </div>
}
```

## Dependent Dropdowns with Defaults

### Example: Category ? Products

```csharp
// 1. Set default category
selectedCategory = categories.First();

// 2. Filter products automatically
filteredProducts = products
    .Where(p => p.Category == selectedCategory)
    .ToList();

// 3. Set default product
selectedFilteredProduct = filteredProducts.FirstOrDefault();
```

### Example: Supplier ? Products

```csharp
// 1. Set default supplier
selectedSupplier = suppliers.Skip(2).FirstOrDefault();

// 2. Filter products automatically
if (selectedSupplier != null)
{
    supplierProducts = products
        .Where(p => p.SupplierId == selectedSupplier.Id)
        .ToList();
    
    // 3. Set default product
    selectedSupplierProduct = supplierProducts.FirstOrDefault();
}
```

## Display Format Examples

### Simple Name
```csharp
ItemTextField="@(p => p.ProductName)"
// Output: "Laptop"
```

### Code + Name
```csharp
ItemTextField="@(p => $"{p.ProductCode} - {p.ProductName}")"
// Output: "PRD001 - Laptop"
```

### Name + Price
```csharp
ItemTextField="@(p => $"{p.ProductName} - ${p.UnitPrice:N2}")"
// Output: "Laptop - $999.99"
```

### Complex Format
```csharp
ItemTextField="@(s => $"{s.SupplierCode} - {s.SupplierName} ({s.Country ?? "N/A"})")"
// Output: "SUP001 - ABC Corp (USA)"
```

### With Status
```csharp
ItemTextField="@(p => $"{p.Name} {(p.InStock ? "?" : "?")}")"
// Output: "Laptop ?"
```

## Responsive Sizes

### Mobile
```razor
<div class="col-12 col-md-6">
    <SearchableDropdown ... />
</div>
```
- 100% width on mobile
- 50% width on desktop

### Tablet & Desktop
```razor
<div class="col-sm-6 col-md-4 col-lg-3">
    <SearchableDropdown ... />
</div>
```
- Mobile: 100%
- Tablet: 50%
- Desktop MD: 33%
- Desktop LG: 25%

## Best Practices

### 1. Always Check Null
```csharp
selectedItem = items.FirstOrDefault();
if (selectedItem != null)
{
    // Safe to use
}
```

### 2. Fallback for Empty Lists
```csharp
selectedItem = items.Any() ? items.First() : null;
```

### 3. Null Coalescing
```csharp
selectedItem = items.Skip(2).FirstOrDefault() ?? items.FirstOrDefault();
```

### 4. Set Dependent Defaults Together
```csharp
private void SetDefaultValues()
{
    // Parent
    selectedCategory = categories.FirstOrDefault();
    
    // Child - depends on parent
    if (selectedCategory != null)
    {
        filteredProducts = GetProductsByCategory(selectedCategory);
        selectedProduct = filteredProducts.FirstOrDefault();
    }
}
```

## Demo Highlights

### ? Immediate Visual Feedback
- Users see selected values right away
- No need to click to see how it works
- Details cards populated

### ? Multiple Examples
- 5+ different size examples
- 4+ different format examples
- 2+ dependent dropdown examples

### ? Real Data
- From database via API
- Actual Product and Supplier entities
- Real relationships (Product ? Supplier)

### ? Professional Layout
- Bootstrap cards with colored headers
- Icons for visual cues
- Badges for counts
- Success messages for confirmation

## Testing Scenarios

1. ? Load page ? All defaults populated
2. ? Size variations ? All sizes render correctly
3. ? Change selection ? Details update
4. ? Category change ? Products auto-filter
5. ? Supplier change ? Products auto-filter
6. ? Search ? Filtering works
7. ? Responsive ? Works on mobile

## Navigation

- **URL**: `/dropdown-product-demo`
- **Menu**: COMPONENTS ? **Dropdown: Products**

## Performance

- ? Single API call for products
- ? Single API call for suppliers
- ? Parallel loading (faster)
- ? Client-side filtering (instant)
- ? No re-rendering on selection

---

**Build Status**: ? Build Successful  
**Features**: Sizes + Defaults  
**Version**: 2.0  
**Date**: 2024
