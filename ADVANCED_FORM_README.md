# Advanced Form Demo - Core Components Usage

Demo trang form ��ng k? ng�?i d�ng s? d?ng **Core Components** t? `BlazorWasmHosted.Components`.

## ?? M?c ��ch

Trang n�y minh h?a c�ch s? d?ng c�c Core Components (Box, Stack, Grid, Container) �? x�y d?ng form ph?c t?p v?i:
- Layout responsive
- Validation �?y �?
- UI/UX t?t v?i Bootstrap
- Code s?ch v� d? maintain

## ?? Core Components ��?c s? d?ng

### 1. **Container**
```razor
<Container MaxWidth="lg" Class="py-4">
    <!-- N?i dung responsive, max-width 960px (lg) -->
</Container>
```

**Properties:**
- `MaxWidth`: xs, sm, md, lg, xl, xxl, fluid
- `Fixed`: Fixed container t?i breakpoint
- `DisableGutters`: T?t padding tr�i/ph?i

### 2. **Box**
```razor
<Box Bg="white" Border="true" BorderRadius="3" Shadow="default" P="4" Mb="4">
    <!-- Flexible wrapper v?i margin, padding, colors, borders -->
</Box>
```

**Properties:**
- **Spacing**: M, Mt, Mr, Mb, Ml, Mx, My, P, Pt, Pr, Pb, Pl, Px, Py (0-5)
- **Display**: Display (flex, block, inline, grid...)
- **Flexbox**: JustifyContent, AlignItems, FlexDirection, FlexWrap, Gap
- **Sizing**: Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight
- **Colors**: Bg, Color (Bootstrap colors)
- **Border**: Border, BorderColor, BorderRadius
- **Position**: Position, Top, Right, Bottom, Left
- **Text**: TextAlign, FontWeight, FontSize
- **Shadow**: Shadow (sm, default, lg)

### 3. **Stack**
```razor
<Stack Direction="column" Spacing="4">
    <!-- C�c items s?p x?p theo column, gap 4 -->
</Stack>
```

**Properties:**
- `Direction`: column, row, column-reverse, row-reverse
- `Spacing`: 0-5 (gap gi?a items)
- `JustifyContent`: start, center, end, between, around, evenly
- `AlignItems`: start, center, end, baseline, stretch
- `Divider`: Hi?n th? border gi?a items

### 4. **Grid**
```razor
<Grid Container="true" Spacing="3">
    <Grid Item="true" Xs="12" Md="6">
        <!-- Column responsive: full width mobile, half desktop -->
    </Grid>
</Grid>
```

**Properties:**
- `Container`: ��nh d?u l� grid container (row)
- `Item`: ��nh d?u l� grid item (col)
- `Xs, Sm, Md, Lg, Xl, Xxl`: Column width t?i breakpoints (1-12)
- `Spacing`: Gap gi?a items (0-5)
- `JustifyContent`, `AlignItems`: Alignment
- `Direction`: Flex direction

## ??? C?u tr�c form

```
Container (lg)
?? Box (Header - text-center)
?? EditForm
   ?? Box (Card - white, border, shadow)
      ?? Stack (column, spacing-4)
         ?? Box (Personal Info Section)
         ?  ?? Grid (Container, spacing-3)
         ?     ?? Grid Item (xs-12, md-6) - First Name
         ?     ?? Grid Item (xs-12, md-6) - Last Name
         ?     ?? Grid Item (xs-12, md-6) - Email
         ?     ?? Grid Item (xs-12, md-6) - Phone
         ?? Box (Address Section)
         ?  ?? Grid (Container, spacing-3)
         ?? Box (Additional Info Section)
         ?  ?? Grid (Container, spacing-3)
         ?? Box (Preferences Section)
         ?  ?? Stack (column, spacing-3)
         ?? Box (Form Actions - flex, justify-end)
```

## ?? �u �i?m c?a c�ch ti?p c?n n�y

### 1. **T�ch bi?t concerns**
- Layout logic trong components
- Business logic trong @code
- Styling v?i Bootstrap + component props

### 2. **Responsive t? �?ng**
```razor
<Grid Item="true" Xs="12" Md="6">
    <!-- Mobile: full width, Desktop: half width -->
</Grid>
```

### 3. **Code ng?n g?n h�n**
**Tr�?c:**
```html
<div class="mb-4 bg-white border rounded-3 shadow p-4">
```

**Sau:**
```razor
<Box Mb="4" Bg="white" Border="true" BorderRadius="3" Shadow="default" P="4">
```

### 4. **Type-safe**
- Spacing: int (0-5)
- Colors: string (Bootstrap colors)
- IntelliSense support

### 5. **Maintainability**
- Thay �?i spacing: `Spacing="3"` ? `Spacing="4"`
- Thay �?i layout: `Direction="row"` ? `Direction="column"`
- Kh�ng c?n nh? Bootstrap classes

## ?? Validation Rules

| Field | Rules |
|-------|-------|
| First Name | Required, 2-50 chars |
| Last Name | Required, 2-50 chars |
| Email | Required, valid email |
| Phone | Valid phone format (optional) |
| Address | Required |
| City | Required |
| Age | Required, 18-100 |
| Bio | Max 500 chars (optional) |
| Accept Terms | Must be true |

## ?? C�ch s? d?ng

1. Truy c?p `/advanced-form`
2. �i?n th�ng tin v�o form
3. Click "Submit Registration"
4. Xem k?t qu? validation v� success message

## ?? Best Practices

### 1. Nh�m fields theo sections
```razor
<Box> <!-- Section wrapper -->
    <h4>Section Title</h4>
    <Grid Container="true">
        <!-- Fields -->
    </Grid>
</Box>
```

### 2. S? d?ng Stack cho vertical spacing
```razor
<Stack Direction="column" Spacing="2">
    <label>Label</label>
    <InputText />
    <ValidationMessage />
</Stack>
```

### 3. Grid responsive cho form fields
```razor
<Grid Container="true" Spacing="3">
    <Grid Item="true" Xs="12" Md="6"> <!-- 2 columns on desktop -->
        <Stack>...</Stack>
    </Grid>
</Grid>
```

### 4. Box cho visual grouping
```razor
<Box Border="true" BorderRadius="2" P="3" Bg="light">
    <!-- Grouped content -->
</Box>
```

## ?? Notes

- Core Components support t?t c? Bootstrap utilities
- C� th? combine v?i custom CSS classes
- Props ��?c convert th�nh Bootstrap classes ho?c inline styles
- Fully responsive v?i Grid system

## ?? Related

- `/forms` - Basic form demo
- Bootstrap Grid System
- Blazor Forms & Validation
