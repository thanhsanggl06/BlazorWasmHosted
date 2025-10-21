# Searchable Dropdown Component

A reusable, searchable dropdown component for Blazor with color indicators, built with Bootstrap 5.

## Features

- ? **Real-time Search**: Filter items as you type
- ? **Color Indicators**: Optional colored dots for visual identification
- ? **Keyboard Support**: ESC key to close dropdown
- ? **Bootstrap 5 Styling**: Consistent with Bootstrap design system
- ? **Generic Type Support**: Works with any data type
- ? **Two-way Data Binding**: Uses `@bind-SelectedItem`
- ? **Responsive Design**: Mobile-friendly
- ? **Smooth Animations**: Fade in/out effects
- ? **Custom Scrollbar**: Styled scrollbar for dropdown list

## Component Structure

```
BlazorWasmHosted.Components/
??? Components/
    ??? Dropdown/
        ??? SearchableDropdown.razor
        ??? SearchableDropdown.razor.css
```

## Basic Usage

### 1. Simple Dropdown (No Colors)

```razor
@using BlazorWasmHosted.Components.Components.Dropdown
@using BlazorWasmHosted.Shared.Models

<SearchableDropdown TItem="DropdownOption"
                    Items="@countries"
                    @bind-SelectedItem="@selectedCountry"
                    ItemTextField="@(item => item.Name)"
                    Placeholder="Select a country"
                    SearchPlaceholder="Type to search..." />

@code {
    private List<DropdownOption> countries = new()
    {
        new() { Id = 1, Name = "Japan" },
        new() { Id = 2, Name = "United States" },
        new() { Id = 3, Name = "Vietnam" },
    };
    
    private DropdownOption? selectedCountry;
}
```

### 2. Dropdown with Color Indicators

```razor
<SearchableDropdown TItem="DropdownOption"
                    Items="@teamMembers"
                    @bind-SelectedItem="@selectedMember"
                    ItemTextField="@(item => item.Name)"
                    ItemColorField="@(item => item.Color)"
                    Placeholder="???"
                    SearchPlaceholder="Search team members..." />

@code {
    private List<DropdownOption> teamMembers = new()
    {
        new() { Id = 1, Name = "Ota Masahiro", Color = "#28a745" },
        new() { Id = 2, Name = "RVSC ??", Color = "#dc3545" },
        new() { Id = 3, Name = "RVSC ???", Color = "#6f42c1" },
    };
    
    private DropdownOption? selectedMember;
}
```

### 3. Using Custom Types

```razor
<SearchableDropdown TItem="Product"
                    Items="@products"
                    @bind-SelectedItem="@selectedProduct"
                    ItemTextField="@(p => p.ProductName)"
                    ItemColorField="@(p => p.CategoryColor)"
                    Placeholder="Select a product"
                    SearchPlaceholder="Search products..." />

@code {
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string CategoryColor { get; set; }
        
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

## Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `TItem` | Generic Type | ? Yes | - | The type of items in the dropdown |
| `Items` | `List<TItem>` | ? Yes | `new()` | List of items to display |
| `SelectedItem` | `TItem?` | ? No | `null` | Currently selected item (two-way binding) |
| `SelectedItemChanged` | `EventCallback<TItem>` | ? No | - | Event fired when selection changes |
| `ItemTextField` | `Func<TItem, string>` | ? Yes | - | Function to get display text from item |
| `ItemColorField` | `Func<TItem, string>?` | ? No | `null` | Function to get color code from item |
| `Placeholder` | `string` | ? No | `"???"` | Text shown when no item is selected |
| `SearchPlaceholder` | `string` | ? No | `"Search..."` | Placeholder for search input |
| `Disabled` | `bool` | ? No | `false` | Disable the dropdown |

## Model Requirements

Your data type must implement `Equals()` and `GetHashCode()` for proper selection detection:

```csharp
public class DropdownOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

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

## Styling

The component uses Bootstrap 5 classes and custom CSS. The following Bootstrap icons are used:
- `bi-search` - Search icon
- `bi-chevron-down` - Dropdown toggle icon
- `bi-check` - Selected item indicator
- `bi-exclamation-circle` - No results icon

### Custom Color Palette

Common color codes for color indicators:
```csharp
// Bootstrap colors
"#0d6efd" // Primary (Blue)
"#6c757d" // Secondary (Gray)
"#28a745" // Success (Green)
"#dc3545" // Danger (Red)
"#ffc107" // Warning (Yellow)
"#17a2b8" // Info (Cyan)
"#6f42c1" // Purple
"#fd7e14" // Orange
"#20c997" // Teal
"#e83e8c" // Pink
```

## Events

### Selection Change
```razor
<SearchableDropdown TItem="DropdownOption"
                    Items="@items"
                    SelectedItem="@selected"
                    SelectedItemChanged="@OnSelectionChanged" />

@code {
    private async Task OnSelectionChanged(DropdownOption item)
    {
        Console.WriteLine($"Selected: {item.Name}");
        // Your custom logic here
    }
}
```

## Keyboard Shortcuts

- **ESC** - Close dropdown and clear search
- **Type** - Filter items in real-time

## Browser Support

- ? Chrome/Edge (Chromium)
- ? Firefox
- ? Safari
- ? Mobile browsers

## Demo Page

Visit `/searchable-dropdown-demo` to see the component in action with multiple examples.

## Advanced Scenarios

### With Validation
```razor
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    
    <div class="mb-3">
        <label class="form-label">Department *</label>
        <SearchableDropdown TItem="DropdownOption"
                            Items="@departments"
                            @bind-SelectedItem="@model.Department" />
        <ValidationMessage For="@(() => model.Department)" />
    </div>
    
    <button type="submit" class="btn btn-primary">Submit</button>
</EditForm>
```

### Disabled State
```razor
<SearchableDropdown TItem="DropdownOption"
                    Items="@items"
                    @bind-SelectedItem="@selected"
                    Disabled="@isProcessing" />
```

### Dynamic Items
```razor
@code {
    protected override async Task OnInitializedAsync()
    {
        // Load items from API
        items = await Http.GetFromJsonAsync<List<DropdownOption>>("api/options");
        StateHasChanged();
    }
}
```

## Troubleshooting

### Issue: Dropdown not opening
**Solution**: Make sure Bootstrap CSS is loaded in `index.html`

### Issue: Items not filtering
**Solution**: Check that `ItemTextField` returns the correct property

### Issue: Selected item not showing
**Solution**: Ensure your model implements `Equals()` and `GetHashCode()`

### Issue: Colors not displaying
**Solution**: Verify `ItemColorField` returns valid hex color codes (e.g., `"#ff0000"`)

## Performance

- Search filtering is optimized with LINQ
- Uses `IEnumerable` for memory efficiency
- Minimal re-renders with `StateHasChanged()`

## License

This component is part of the BlazorWasmHosted project.
