# Component Searchable Dropdown - H�?ng D?n S? D?ng

## T?ng Quan

Component **Searchable Dropdown** l� m?t dropdown c� th? t?m ki?m, h? tr? hi?n th? m�u s?c cho t?ng item, ��?c x�y d?ng v?i Bootstrap 5 v� c� th? t�i s? d?ng d? d�ng.

## T�nh N�ng Ch�nh

? **T?m ki?m real-time**: G? �? l?c items ngay l?p t?c  
? **Hi?n th? m�u**: C�c ch?m tr?n m�u t�y ch?n cho m?i item  
? **H? tr? ph�m t?t**: Nh?n ESC �? ��ng dropdown  
? **Responsive**: Ho?t �?ng t?t tr�n m?i m�n h?nh  
? **Generic Type**: C� th? d�ng v?i b?t k? ki?u d? li?u n�o  
? **Two-way binding**: D? d�ng bind d? li?u  

## C?u Tr�c File

```
BlazorWasmHosted.Components/
??? Components/
    ??? Dropdown/
        ??? SearchableDropdown.razor           # Component ch�nh
        ??? SearchableDropdown.razor.css       # Styles

BlazorWasmHosted.Shared/
??? Models/
    ??? DropdownOption.cs                      # Model m?u

BlazorWasmHosted.Client/
??? Pages/
    ??? SearchableDropdownDemo.razor           # Demo page
```

## C�ch S? D?ng

### 1. Dropdown ��n Gi?n (Kh�ng C� M�u)

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

### 2. Dropdown V?i M�u S?c

```razor
<SearchableDropdown TItem="DropdownOption"
                    Items="@members"
                    SelectedItem="@selectedMember"
                    SelectedItemChanged="@((item) => selectedMember = item)"
                    ItemTextField="@(item => item.Name)"
                    ItemColorField="@(item => item.Color)"
                    Placeholder="???"
                    SearchPlaceholder="T?m th�nh vi�n..." />

@code {
    private List<DropdownOption> members = new()
    {
        new() { Id = 1, Name = "Nguy?n V�n A", Color = "#28a745" },
        new() { Id = 2, Name = "Tr?n Th? B", Color = "#dc3545" },
        new() { Id = 3, Name = "L� V�n C", Color = "#6f42c1" },
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
        
        // QUAN TR?NG: Ph?i implement Equals v� GetHashCode
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

| Tham s? | Ki?u | B?t bu?c | M?c �?nh | M� t? |
|---------|------|----------|----------|-------|
| `TItem` | Generic | ? C� | - | Ki?u d? li?u c?a items |
| `Items` | `List<TItem>` | ? C� | `new()` | Danh s�ch items hi?n th? |
| `SelectedItem` | `TItem?` | ? Kh�ng | `null` | Item ��?c ch?n |
| `SelectedItemChanged` | `EventCallback<TItem>` | ? Kh�ng | - | Event khi ch?n item m?i |
| `ItemTextField` | `Func<TItem, string>` | ? C� | - | H�m l?y text hi?n th? |
| `ItemColorField` | `Func<TItem, string>?` | ? Kh�ng | `null` | H�m l?y m? m�u |
| `Placeholder` | `string` | ? Kh�ng | `"???"` | Text khi ch�a ch?n |
| `SearchPlaceholder` | `string` | ? Kh�ng | `"Search..."` | Placeholder cho � t?m ki?m |
| `Disabled` | `bool` | ? Kh�ng | `false` | V� hi?u h�a dropdown |

## Model Class

Model c?a b?n **B?T BU?C** ph?i implement `Equals()` v� `GetHashCode()`:

```csharp
public class DropdownOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Description { get; set; }

    // B?T BU?C: Implement �? component detect ��?c item �? ch?n
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

## M�u S?c (Color Codes)

C�c m? m�u ph? bi?n theo Bootstrap 5:

```csharp
"#0d6efd"  // Primary (Xanh d��ng)
"#28a745"  // Success (Xanh l�)
"#dc3545"  // Danger (�?)
"#ffc107"  // Warning (V�ng)
"#6c757d"  // Secondary (X�m)
"#17a2b8"  // Info (Cyan)
"#6f42c1"  // Purple (T�m)
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
        Console.WriteLine($"�? ch?n: {item.Name}");
        // Logic c?a b?n ? ��y
        await DoSomething(item);
    }
}
```

## Ph�m T?t

- **ESC** - ��ng dropdown v� x�a search
- **G? ch?** - L?c items theo th?i gian th?c

## V� D? N�ng Cao

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

Truy c?p `/searchable-dropdown-demo` �? xem c�c v� d? chi ti?t.

## Troubleshooting

### L?i: Dropdown kh�ng m?
**Gi?i ph�p**: Ki?m tra Bootstrap CSS �? ��?c load trong `index.html` ch�a

### L?i: Kh�ng filter ��?c items
**Gi?i ph�p**: Ki?m tra `ItemTextField` c� tr? v? ��ng property kh�ng

### L?i: Item �? ch?n kh�ng hi?n th?
**Gi?i ph�p**: Model class ph?i implement `Equals()` v� `GetHashCode()`

### L?i: M�u kh�ng hi?n th?
**Gi?i ph�p**: `ItemColorField` ph?i tr? v? m? m�u hex h?p l? (vd: `"#ff0000"`)

## Performance Tips

- Component �? ��?c optimize v?i LINQ �? filter nhanh
- S? d?ng `IEnumerable` �? ti?t ki?m memory
- Minimize re-renders v?i `StateHasChanged()`

## Browser Support

? Chrome/Edge (Chromium)  
? Firefox  
? Safari  
? Mobile browsers  

---

**T�c gi?**: BlazorWasmHosted Team  
**Version**: 1.0  
**Last Updated**: 2024
