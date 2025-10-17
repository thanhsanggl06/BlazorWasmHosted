# Product & Supplier Registration Pages

## Overview
Two comprehensive registration pages for Products and Suppliers with full validation and modern UI using Core Components (Box, Stack, Grid, Container).

## Pages Created

### 1. Product Registration (`/product-registration`)
**File:** `BlazorWasmHosted.Client/Pages/ProductRegistration.razor`

#### Features:
- ? Full form validation with `DataAnnotationsValidator`
- ? Loads active suppliers from API
- ? Responsive layout using Core Components
- ? Success/Error message handling
- ? Loading states for better UX
- ? Reset form functionality
- ? Navigation to product list

#### Form Sections:
1. **Basic Information**
   - Product Code (required, regex validated)
   - Product Name (required, 2-200 characters)
   - Category (dropdown selection)
   - Supplier (loaded from API)

2. **Pricing & Inventory**
   - Unit Price (required, 0.01 - 999,999,999.99)
   - Quantity (required, non-negative)
   - In Stock status (checkbox)

3. **Description**
   - Product Description (optional, max 1000 characters)

#### Validations:
```csharp
[Required(ErrorMessage = "Product code is required")]
[StringLength(50, MinimumLength = 2)]
[RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Uppercase letters, numbers, and hyphens only")]
public string ProductCode { get; set; }

[Required(ErrorMessage = "Product name is required")]
[StringLength(200, MinimumLength = 2)]
public string ProductName { get; set; }

[Required(ErrorMessage = "Category is required")]
[StringLength(100)]
public string Category { get; set; }

[Required(ErrorMessage = "Unit price is required")]
[Range(0.01, 999999999.99)]
public decimal UnitPrice { get; set; }

[Required(ErrorMessage = "Quantity is required")]
[Range(0, int.MaxValue)]
public int Quantity { get; set; }

[Required(ErrorMessage = "Supplier is required")]
[Range(1, int.MaxValue)]
public int SupplierId { get; set; }

[StringLength(1000)]
public string? Description { get; set; }
```

### 2. Supplier Registration (`/supplier-registration`)
**File:** `BlazorWasmHosted.Client/Pages/SupplierRegistration.razor`

#### Features:
- ? Full form validation with `DataAnnotationsValidator`
- ? Comprehensive address fields
- ? Responsive layout using Core Components
- ? Success/Error message handling
- ? Loading states for better UX
- ? Reset form functionality
- ? Navigation to supplier list
- ? Registration guidelines info card

#### Form Sections:
1. **Basic Information**
   - Supplier Code (required, regex validated)
   - Supplier Name (required, 2-200 characters)
   - Contact Person (optional, max 100 characters)
   - Active Status (checkbox)

2. **Contact Information**
   - Email (required, valid email format)
   - Phone Number (required, 7-20 characters)

3. **Address Information**
   - Street Address (required, 5-200 characters)
   - City (required, 2-100 characters)
   - Country (dropdown with common countries)

#### Validations:
```csharp
[Required(ErrorMessage = "Supplier code is required")]
[StringLength(50, MinimumLength = 2)]
[RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Uppercase letters, numbers, and hyphens only")]
public string SupplierCode { get; set; }

[Required(ErrorMessage = "Supplier name is required")]
[StringLength(200, MinimumLength = 2)]
public string SupplierName { get; set; }

[StringLength(100)]
public string? ContactPerson { get; set; }

[Required(ErrorMessage = "Email is required")]
[EmailAddress(ErrorMessage = "Invalid email format")]
[StringLength(100)]
public string Email { get; set; }

[Required(ErrorMessage = "Phone number is required")]
[Phone(ErrorMessage = "Invalid phone format")]
[StringLength(20, MinimumLength = 7)]
public string Phone { get; set; }

[Required(ErrorMessage = "Address is required")]
[StringLength(200, MinimumLength = 5)]
public string Address { get; set; }

[Required(ErrorMessage = "City is required")]
[StringLength(100, MinimumLength = 2)]
public string City { get; set; }

[Required(ErrorMessage = "Country is required")]
[StringLength(100)]
public string Country { get; set; }
```

## Core Components Used

Both pages extensively use Core Components for layout:

### Container
```razor
<Container MaxWidth="lg" Class="py-4">
    <!-- Content -->
</Container>
```

### Box
```razor
<Box Bg="white" Border="true" BorderRadius="3" Shadow="default" P="4" Mb="4">
    <!-- Form content -->
</Box>
```

### Stack
```razor
<Stack Direction="column" Spacing="4">
    <!-- Stacked elements -->
</Stack>
```

### Grid
```razor
<Grid Container="true" Spacing="3">
    <Grid Item="true" Xs="12" Md="6">
        <!-- Form field -->
    </Grid>
</Grid>
```

## API Integration

### Product Registration
- **Endpoint:** `POST /api/products`
- **Request Type:** `CreateProductRequest`
- **Success:** Returns created product data
- **Error:** Returns error message

### Supplier Registration
- **Endpoint:** `POST /api/suppliers`
- **Request Type:** `CreateSupplierRequest`
- **Success:** Returns created supplier data
- **Error:** Returns error message

### Supplier Loading (for Product form)
- **Endpoint:** `GET /api/suppliers`
- **Response:** `List<SupplierDto>`
- **Filter:** Only active suppliers shown

## Navigation Updates

### NavMenu.razor
Added navigation links:
```razor
<!-- Data Management Section -->
<NavLink href="products">Products</NavLink>
<NavLink href="product-registration">Register Product</NavLink>
<NavLink href="suppliers">Suppliers</NavLink>
<NavLink href="supplier-registration">Register Supplier</NavLink>
```

### Home.razor
Added links to registration pages in Data Management card with descriptions.

## User Experience Features

### Loading States
- Supplier dropdown shows "Loading suppliers..." while fetching
- Submit button shows spinner and "Submitting..." text
- All buttons disabled during submission

### Success Messages
- Green alert box with all submitted data
- Options to:
  - Register another item
  - View list of all items

### Error Handling
- Red alert box for errors
- Specific error messages from API
- General exception handling

### Form Reset
- Clears all fields
- Resets validation state
- Hides success/error messages

### Navigation
- "Back to [List]" button
- Automatic navigation after viewing success

## Validation Messages

All validation messages are:
- Clear and specific
- Shown below each field
- Color-coded (red for errors)
- Real-time validation on blur/change

## Responsive Design

Both forms are fully responsive:
- **Mobile (xs):** Single column layout
- **Tablet (md):** Two-column layout for appropriate fields
- **Desktop (lg):** Contained within max-width container

## Categories (Product)
Pre-defined categories:
- Electronics
- Furniture
- Books
- Office Supplies
- Computers
- Accessories
- Other

## Countries (Supplier)
Pre-defined countries including:
- Vietnam
- United States
- United Kingdom
- China
- Japan
- Singapore
- And more...

## Usage Examples

### Registering a Product
1. Navigate to `/product-registration`
2. Fill in Product Code (e.g., "P010")
3. Enter Product Name
4. Select Category
5. Choose Supplier
6. Set Unit Price and Quantity
7. Check/uncheck In Stock
8. Add optional description
9. Click "Register Product"

### Registering a Supplier
1. Navigate to `/supplier-registration`
2. Fill in Supplier Code (e.g., "SUP006")
3. Enter Supplier Name
4. Add Contact Person (optional)
5. Enter Email and Phone
6. Fill in complete address
7. Select Country
8. Check/uncheck Active Status
9. Click "Register Supplier"

## Best Practices Implemented

1. **Separation of Concerns**
   - UI in Razor components
   - Validation in model classes
   - API calls isolated in methods

2. **User Feedback**
   - Loading indicators
   - Success confirmations
   - Error messages
   - Validation hints

3. **Code Reusability**
   - Core Components for consistent UI
   - Validation attributes
   - Common patterns

4. **Accessibility**
   - Proper labels
   - ARIA attributes
   - Semantic HTML

5. **Performance**
   - Async/await patterns
   - Efficient state management
   - Minimal re-renders

## Testing Tips

1. **Valid Data**
   - Use uppercase codes: "P010", "SUP006"
   - Provide all required fields
   - Use valid email format

2. **Invalid Data**
   - Try lowercase product codes (should fail)
   - Skip required fields
   - Use invalid email formats
   - Test boundary values

3. **Edge Cases**
   - Very long text in description
   - Special characters in codes
   - Network errors
   - Duplicate codes

## Future Enhancements

Potential improvements:
- [ ] Duplicate code checking
- [ ] Image upload for products
- [ ] Batch import functionality
- [ ] Draft save feature
- [ ] Auto-suggest for categories
- [ ] Supplier search/filter in dropdown
- [ ] Address validation/autocomplete
- [ ] Rich text editor for descriptions
- [ ] Preview before submit
- [ ] Print/export registration receipt

## Related Files

- `BlazorWasmHosted.Client/Pages/ProductRegistration.razor`
- `BlazorWasmHosted.Client/Pages/SupplierRegistration.razor`
- `BlazorWasmHosted.Client/Layout/NavMenu.razor`
- `BlazorWasmHosted.Client/Pages/Home.razor`
- `BlazorWasmHosted.Shared/Models/ProductDto.cs`
- `BlazorWasmHosted.Shared/Models/SupplierDto.cs`
- `BlazorWasmHosted.Server/Controllers/ProductsController.cs`
- `BlazorWasmHosted.Server/Controllers/SuppliersController.cs`
