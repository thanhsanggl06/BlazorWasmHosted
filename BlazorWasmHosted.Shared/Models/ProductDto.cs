namespace BlazorWasmHosted.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorWasmHosted.Shared.ValidationAttributes;

public record ProductDto(
    int Id,
    string ProductCode,
    string ProductName,
    string Category,
    decimal UnitPrice,
    int Quantity,
    bool InStock,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int SupplierId,
    string SupplierName,
    decimal TotalValue
);

public record CreateProductRequest(
    string ProductCode,
    string ProductName,
    string Category,
    decimal UnitPrice,
    int Quantity,
    bool InStock,
    string? Description,
    int SupplierId
);

public record UpdateProductRequest(
    string ProductCode,
    string ProductName,
    string Category,
    decimal UnitPrice,
    int Quantity,
    bool InStock,
    string? Description,
    int SupplierId
);

public class ProductDtoTest : ObservableValidator
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên không được để trống")]
    [MinLength(2, ErrorMessage = "Tên phải ít nhất 2 ký tự")]
    public string ProductName { get; set; } = string.Empty;
    
    [SupplierExists(ErrorMessage = "Supplier ID không tồn tại trong hệ thống")]
    public int SupplierId { get; set; }

    // For UI display
    public bool IsValid { get; set; } = true;

    public void Validate()
    {
        ValidateAllProperties();
    }
}