# Product & Supplier Management Pages - C1 FlexGrid

## ?? Overview

Hai trang qu?n l? Product và Supplier s? d?ng **C1.Blazor.Grid FlexGrid** v?i style tham kh?o t? code m?u ðý?c cung c?p.

## ? Tính nãng

### Product Management Page (`/products`)
- ? Hi?n th? danh sách products trong FlexGrid
- ? Statistics cards (t?ng products, in stock, total quantity, total value)
- ? Search by product name/code
- ? Filter by category
- ? Filter by supplier
- ? Double-click ð? xem details
- ? Delete product v?i confirmation
- ? C1DataPager cho phân trang
- ? Custom cell templates v?i badges

### Supplier Management Page (`/suppliers`)
- ? Hi?n th? danh sách suppliers trong FlexGrid
- ? Statistics cards (total suppliers, active, products count, countries)
- ? Search by supplier name/code
- ? Filter by country
- ? Filter by active status
- ? Double-click ð? xem details
- ? View products button - navigate to /products filtered by supplier
- ? Delete supplier v?i validation (check products count)
- ? C1DataPager cho phân trang

## ?? Key Components Used

```razor
@using C1.Blazor.Grid
@using C1.Blazor.DataPager
@using C1.DataCollection
@using BlazorWasmHosted.Shared.Models
@using BlazorWasmHosted.Components
```

## ?? C?u trúc code tham kh?o

### FlexGrid Configuration

```razor
<FlexGrid @ref="flexGrid"
          ItemsSource="@collection"
          IsReadOnly="true"
          AllowSorting="true"
          ShowSort="true"
          SelectionMode="GridSelectionMode.Row"
          HeadersVisibility="GridHeadersVisibility.All"
          ColumnOptionsMenuVisibility="GridColumnOptionsMenuVisibility.Visible"
          AutoGenerateColumns="false"
          Style="@("height:500px")"
          CellDoubleTapped="OnCellDoubleTapped">
    <FlexGridColumns>
        <GridColumn Binding="PropertyName" 
                    Header="@GetHeaderWithIcon("PropertyName", "Display Name")" 
                    Width="120" />
        <!-- More columns -->
    </FlexGridColumns>
</FlexGrid>
```

### Event Handler Signature

**QUAN TR?NG**: Event handler cho FlexGrid ph?i có signature ðúng:

```csharp
private void OnCellDoubleTapped(object? sender, GridInputEventArgs e)
{
    if (e.CellType == GridCellType.Cell && flexGrid != null)
    {
        var item = flexGrid.Rows[e.CellRange.Row].DataItem as YourDto;
        // Process item
    }
}
```

###Paged Collection

```csharp
private C1PagedDataCollection<ProductDto>? productCollection;

private void UpdateCollection()
{
    productCollection = new C1PagedDataCollection<ProductDto>(filteredData)
    {
        PageSize = 10
    };
}
```

### Filter Implementation

```csharp
private void ApplyFilters()
{
    filteredData = sourceData.AsQueryable()
        .Where(x => string.IsNullOrEmpty(searchText) || 
                   x.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .Where(x => string.IsNullOrEmpty(selectedCategory) || 
                   x.Category == selectedCategory)
        .ToList();

    UpdateCollection();
    StateHasChanged();
}
```

### Select Element v?i Filter

**Cách 1: S? d?ng value + @onchange**
```razor
<select class="form-select" 
        value="@selectedCategory" 
        @onchange="OnCategoryChanged">
    <option value="">All</option>
    @foreach (var cat in categories)
    {
        <option value="@cat">@cat</option>
    }
</select>

@code {
    private void OnCategoryChanged(ChangeEventArgs e)
    {
        selectedCategory = e.Value?.ToString() ?? "";
        ApplyFilters();
    }
}
```

**Cách 2: S? d?ng @bind v?i after**
```razor
<select class="form-select" @bind="selectedCategory" @bind:after="ApplyFilters">
    <option value="">All</option>
    @foreach (var cat in categories)
    {
        <option value="@cat">@cat</option>
    }
</select>
```

## ?? Layout Structure

```
Container
??? Page Header
??? Statistics Cards (4 cards in row)
??? Toolbar Card
?   ??? Search Input
?   ??? Category/Country Filter
?   ??? Supplier/Status Filter
?   ??? Refresh Button
??? FlexGrid Card
    ??? Card Header (title + count)
    ??? Card Body (FlexGrid)
    ??? Card Footer (DataPager)
```

## ??? Implementation Steps

### Step 1: T?o Products.razor

```razor
@page "/products"
@using C1.Blazor.Grid
@using C1.Blazor.DataPager
@using C1.DataCollection
@using BlazorWasmHosted.Shared.Models
@using BlazorWasmHosted.Components
@inject HttpClient Http
@inject IMessageBoxService MessageBox

<PageTitle>Products</PageTitle>

<div class="container-fluid py-4">
    <!-- Statistics Cards -->
    <div class="row mb-4">
        <div class="col-md-3">
            <div class="card border-primary">
                <div class="card-body text-center">
                    <h6>Total Products</h6>
                    <h3>@products.Count</h3>
                </div>
            </div>
        </div>
        <!-- More cards -->
    </div>

    <!-- FlexGrid -->
    <FlexGrid ItemsSource="@productCollection" ...>
        <FlexGridColumns>
            <GridColumn Binding="ProductName" Header="Product Name" />
            <!-- More columns -->
        </FlexGridColumns>
    </FlexGrid>

    <C1DataPager Source="@productCollection" />
</div>

@code {
    private List<ProductDto> products = new();
    private C1PagedDataCollection<ProductDto>? productCollection;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }
    
    private async Task LoadData()
    {
        var response = await Http.GetFromJsonAsync<List<ProductDto>>("api/products");
        if (response != null)
        {
            products = response;
            productCollection = new C1PagedDataCollection<ProductDto>(products)
            {
                PageSize = 10
            };
        }
    }
}
```

### Step 2: T?o Suppliers.razor

Týõng t? nhý Products.razor nhýng:
- S? d?ng `SupplierDto`
- API endpoint: `/api/suppliers`
- Filters khác: Country, Active Status
- Thêm button "View Products" navigate ð?n `/products?supplierId={id}`

### Step 3: Custom Cell Templates

```razor
<GridColumn Binding="UnitPrice" Header="Price" Format="N0">
    <CellTemplate>
        <span class="badge bg-success">
            ¥@context.Item.UnitPrice.ToString("N0")
        </span>
    </CellTemplate>
</GridColumn>

<GridColumn Binding="InStock" Header="Status">
    <CellTemplate>
        @if (context.Item.InStock)
        {
            <span class="badge bg-success">
                <i class="bi bi-check-circle"></i> In Stock
            </span>
        }
        else
        {
            <span class="badge bg-secondary">
                <i class="bi bi-x-circle"></i> Out
            </span>
        }
    </CellTemplate>
</GridColumn>
```

### Step 4: Action Column v?i Buttons

```razor
<GridColumn Header="Actions" Width="100" AllowSorting="false">
    <CellTemplate>
        <button class="btn btn-sm btn-outline-danger"
                @onclick="() => DeleteItem(context.Item)">
            <i class="bi bi-trash"></i>
        </button>
    </CellTemplate>
</GridColumn>

@code {
    private async Task DeleteItem(ProductDto product)
    {
        var confirmed = await MessageBox.ConfirmAsync(
            $"Delete {product.ProductName}?",
            "Confirm"
        );
        
        if (confirmed)
        {
            var response = await Http.DeleteAsync($"api/products/{product.Id}");
            if (response.IsSuccessStatusCode)
            {
                await LoadData();
            }
        }
    }
}
```

## ?? Filtering & Search

### Search Input
```razor
<input type="text" 
       class="form-control" 
       placeholder="Search..."
       @bind="searchText"
       @bind:event="oninput"
       @onkeyup="ApplyFilters" />
```

### Category Filter (Select)
```razor
<select class="form-select" value="@selectedCategory" @onchange="OnCategoryChanged">
    <option value="">All Categories</option>
    @foreach (var category in categories)
    {
        <option value="@category">@category</option>
    }
</select>

@code {
    private void OnCategoryChanged(ChangeEventArgs e)
    {
        selectedCategory = e.Value?.ToString() ?? "";
        ApplyFilters();
    }
}
```

### Apply Filters Method
```csharp
private void ApplyFilters()
{
    filteredProducts = products.AsQueryable()
        .Where(p => string.IsNullOrEmpty(searchText) || 
                   p.ProductName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .Where(p => string.IsNullOrEmpty(selectedCategory) || 
                   p.Category == selectedCategory)
        .ToList();

    productCollection = new C1PagedDataCollection<ProductDto>(filteredProducts)
    {
        PageSize = pageSize
    };
    
    StateHasChanged();
}
```

## ?? Statistics Cards

```razor
<div class="row mb-4">
    <div class="col-md-3">
        <div class="card border-primary shadow-sm">
            <div class="card-body text-center">
                <h6 class="text-muted mb-2">Total Products</h6>
                <h3 class="text-primary mb-0">@products.Count</h3>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="card border-success shadow-sm">
            <div class="card-body text-center">
                <h6 class="text-muted mb-2">In Stock</h6>
                <h3 class="text-success mb-0">@products.Count(p => p.InStock)</h3>
            </div>
        </div>
    </div>
    <!-- More cards -->
</div>
```

## ?? Testing

### Test Products Page
1. Navigate to `/products`
2. Check data loads from API
3. Test search functionality
4. Test category filter
5. Test supplier filter
6. Double-click row to view details
7. Test delete with confirmation
8. Test pagination

### Test Suppliers Page
1. Navigate to `/suppliers`
2. Check data loads from API
3. Test search functionality
4. Test country filter
5. Test active status filter
6. Double-click row to view details
7. Click "View Products" button
8. Try delete supplier with products (should fail)
9. Test pagination

## ?? Notes

### FlexGrid Event Handlers
- **CellDoubleTapped**: `void (object? sender, GridInputEventArgs e)`
- Không th? dùng async Task, ph?i dùng void
- N?u c?n await, dùng discard pattern: `_ = MessageBox.AlertAsync(...)`

### Data Binding Issues
- Avoid using `@bind` and `@onchange` together
- Use `value` + `@onchange` ho?c `@bind` + `@bind:after` (NET 8+)

### Performance
- S? d?ng `C1PagedDataCollection` cho large datasets
- PageSize nên là 10-20 items
- Filter trên client-side n?u data < 1000 items
- Filter trên server-side n?u data > 1000 items

## ?? References

- [C1 FlexGrid Documentation](https://www.grapecity.com/componentone/docs/blazor/online-flexgrid/overview.html)
- [C1 DataPager Documentation](https://www.grapecity.com/componentone/docs/blazor/online-datapager/overview.html)
- API Endpoints: See `PRODUCT_SUPPLIER_DATABASE.md`

## ? Checklist

- [ ] Products.razor created
- [ ] Suppliers.razor created
- [ ] Added to NavMenu
- [ ] Added to Home page
- [ ] Tested all filters
- [ ] Tested CRUD operations
- [ ] Tested with MessageBox
- [ ] Tested pagination
- [ ] Tested responsive layout

---

**Lýu ?**: Do có m?t s? compatibility issues v?i C1.Blazor.Grid event handlers và Razor compiler, b?n có th? c?n ði?u ch?nh code d?a trên phiên b?n C1.Blazor.Grid b?n ðang s? d?ng.

Tham kh?o code m?u ð? cung c?p (Home.razor v?i FlexGrid) ð? implement chính xác event handlers và bindings.
