# Component Searchable Dropdown - Hý?ng D?n S? D?ng

## T?ng Quan

Component **Searchable Dropdown** là m?t dropdown có th? t?m ki?m, h? tr? hi?n th? màu s?c cho t?ng item, ðý?c xây d?ng v?i Bootstrap 5 và có th? tái s? d?ng d? dàng.

## Tính Nãng Chính

? **T?m ki?m real-time**: G? ð? l?c items ngay l?p t?c  
? **Hi?n th? màu**: Các ch?m tr?n màu tùy ch?n cho m?i item  
? **H? tr? phím t?t**: Nh?n ESC ð? ðóng dropdown  
? **Responsive**: Ho?t ð?ng t?t trên m?i màn h?nh  
? **Generic Type**: Có th? dùng v?i b?t k? ki?u d? li?u nào  
? **Two-way binding**: D? dàng bind d? li?u  

## C?u Trúc File

```
BlazorWasmHosted.Components/
??? Components/
    ??? Dropdown/
        ??? SearchableDropdown.razor           # Component chính
        ??? SearchableDropdown.razor.css       # Styles

BlazorWasmHosted.Shared/
??? Models/
    ??? DropdownOption.cs                      # Model m?u

BlazorWasmHosted.Client/
??? Pages/
    ??? SearchableDropdownDemo.razor           # Demo page
```

## Cách S? D?ng

### 1. Dropdown Ðõn Gi?n (Không Có Màu)

```razor
@using BlazorWasmHosted.Components.Components.Dropdown
@using BlazorWasmHosted.Shared.Models

<SearchableDropdown TItem="DropdownOption"
                    Items="@countries"
                    SelectedItem="@selectedCountry"
                    SelectedItemChanged="@((item) => selectedCountry = item)"
                    ItemTextField="@(item => item.Name)"
                    Placeholder="Ch?n qu?c gia"
                    SearchPlaceholder="T?m ki?m..." />

@code {
    private List<DropdownOption> countries = new()
    {
        new() { Id = 1, Name = "Nh?t B?n" },
        new() { Id = 2, Name = "Vi?t Nam" },
        new() { Id = 3, Name = "M?" },
    };
    
    private DropdownOption? selectedCountry;
}
```

### 2. Dropdown V?i Màu S?c

```razor
<SearchableDropdown TItem="DropdownOption"
                    Items="@members"
                    SelectedItem="@selectedMember"
                    SelectedItemChanged="@((item) => selectedMember = item)"
                    ItemTextField="@(item => item.Name)"
                    ItemColorField="@(item => item.Color)"
                    Placeholder="???"
                    SearchPlaceholder="T?m thành viên..." />

@code {
    private List<DropdownOption> members = new()
    {
        new() { Id = 1, Name = "Nguy?n Vãn A", Color = "#28a745" },
        new() { Id = 2, Name = "Tr?n Th? B", Color = "#dc3545" },
        new() { Id = 3, Name = "Lê Vãn C", Color = "#6f42c1" },
    };
    
    private DropdownOption? selectedMember;
}
```

### 3. S? D?ng V?i Custom Type

```razor
<SearchableDropdown TItem="Product"
                    Items="@products"
                    SelectedItem="@selectedProduct"
                    SelectedItemChanged="@((item) => selectedProduct = item)"
                    ItemTextField="@(p => p.ProductName)"
                    ItemColorField="@(p => p.CategoryColor)"
                    Placeholder="Ch?n s?n ph?m"
                    SearchPlaceholder="T?m s?n ph?m..." />

@code {
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public string CategoryColor { get; set; } = "";
        
        // QUAN TR?NG: Ph?i implement Equals và GetHashCode
        public override bool Equals(object? obj)
        {
            if (obj is Product other)
                return Id == other.Id;
            return false;
        }
        
        public override int GetHashCode() => Id.GetHashCode();
    }
    
    private List<Product> products = new();
    private Product? selectedProduct;
}
```

## Tham S? (Parameters)

| Tham s? | Ki?u | B?t bu?c | M?c ð?nh | Mô t? |
|---------|------|----------|----------|-------|
| `TItem` | Generic | ? Có | - | Ki?u d? li?u c?a items |
| `Items` | `List<TItem>` | ? Có | `new()` | Danh sách items hi?n th? |
| `SelectedItem` | `TItem?` | ? Không | `null` | Item ðý?c ch?n |
| `SelectedItemChanged` | `EventCallback<TItem>` | ? Không | - | Event khi ch?n item m?i |
| `ItemTextField` | `Func<TItem, string>` | ? Có | - | Hàm l?y text hi?n th? |
| `ItemColorField` | `Func<TItem, string>?` | ? Không | `null` | Hàm l?y m? màu |
| `Placeholder` | `string` | ? Không | `"???"` | Text khi chýa ch?n |
| `SearchPlaceholder` | `string` | ? Không | `"Search..."` | Placeholder cho ô t?m ki?m |
| `Disabled` | `bool` | ? Không | `false` | Vô hi?u hóa dropdown |

## Model Class

Model c?a b?n **B?T BU?C** ph?i implement `Equals()` và `GetHashCode()`:

```csharp
public class DropdownOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Description { get; set; }

    // B?T BU?C: Implement ð? component detect ðý?c item ð? ch?n
    public override bool Equals(object? obj)
    {
        if (obj is DropdownOption other)
            return Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
```

## Màu S?c (Color Codes)

Các m? màu ph? bi?n theo Bootstrap 5:

```csharp
"#0d6efd"  // Primary (Xanh dýõng)
"#28a745"  // Success (Xanh lá)
"#dc3545"  // Danger (Ð?)
"#ffc107"  // Warning (Vàng)
"#6c757d"  // Secondary (Xám)
"#17a2b8"  // Info (Cyan)
"#6f42c1"  // Purple (Tím)
"#fd7e14"  // Orange (Cam)
"#20c997"  // Teal (Xanh ng?c)
"#e83e8c"  // Pink (H?ng)
```

## Events

### X? l? khi ch?n item
```razor
<SearchableDropdown TItem="DropdownOption"
                    Items="@items"
                    SelectedItem="@selected"
                    SelectedItemChanged="@OnSelectionChanged" />

@code {
    private async Task OnSelectionChanged(DropdownOption item)
    {
        Console.WriteLine($"Ð? ch?n: {item.Name}");
        // Logic c?a b?n ? ðây
        await DoSomething(item);
    }
}
```

## Phím T?t

- **ESC** - Ðóng dropdown và xóa search
- **G? ch?** - L?c items theo th?i gian th?c

## Ví D? Nâng Cao

### V?i Validation
```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    
    <div class="mb-3">
        <label class="form-label">Ph?ng ban *</label>
        <SearchableDropdown TItem="DropdownOption"
                            Items="@departments"
                            SelectedItem="@model.Department"
                            SelectedItemChanged="@((item) => model.Department = item)" />
        <ValidationMessage For="@(() => model.Department)" />
    </div>
    
    <button type="submit" class="btn btn-primary">G?i</button>
</EditForm>
```

### Disabled State
```razor
<SearchableDropdown TItem="DropdownOption"
                    Items="@items"
                    SelectedItem="@selected"
                    SelectedItemChanged="@((item) => selected = item)"
                    Disabled="@isProcessing" />

@code {
    private bool isProcessing = false;
    
    private async Task ProcessData()
    {
        isProcessing = true;
        await Task.Delay(2000);
        isProcessing = false;
    }
}
```

### Load Data T? API
```razor
@inject HttpClient Http

@code {
    private List<DropdownOption> items = new();
    
    protected override async Task OnInitializedAsync()
    {
        // Load t? API
        items = await Http.GetFromJsonAsync<List<DropdownOption>>("api/options") 
                ?? new();
        StateHasChanged();
    }
}
```

## Demo Page

Truy c?p `/searchable-dropdown-demo` ð? xem các ví d? chi ti?t.

## Troubleshooting

### L?i: Dropdown không m?
**Gi?i pháp**: Ki?m tra Bootstrap CSS ð? ðý?c load trong `index.html` chýa

### L?i: Không filter ðý?c items
**Gi?i pháp**: Ki?m tra `ItemTextField` có tr? v? ðúng property không

### L?i: Item ð? ch?n không hi?n th?
**Gi?i pháp**: Model class ph?i implement `Equals()` và `GetHashCode()`

### L?i: Màu không hi?n th?
**Gi?i pháp**: `ItemColorField` ph?i tr? v? m? màu hex h?p l? (vd: `"#ff0000"`)

## Performance Tips

- Component ð? ðý?c optimize v?i LINQ ð? filter nhanh
- S? d?ng `IEnumerable` ð? ti?t ki?m memory
- Minimize re-renders v?i `StateHasChanged()`

## Browser Support

? Chrome/Edge (Chromium)  
? Firefox  
? Safari  
? Mobile browsers  

---

**Tác gi?**: BlazorWasmHosted Team  
**Version**: 1.0  
**Last Updated**: 2024
