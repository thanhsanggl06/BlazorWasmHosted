namespace BlazorWasmHosted.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorWasmHosted.Shared.ValidationAttributes;
using System.Collections.Generic;

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

public class ProductDtoTest :  ObservableValidator
{
    private int id;

    [CustomValidation(typeof(ProductDtoTest), nameof(ValidatePrimaryKey))]
    public int Id
    {
        get => this.id;
        set => SetProperty(ref this.id, value, false);
    }

    [Required(ErrorMessage = "Tên không được để trống")]
    [MinLength(2, ErrorMessage = "Tên phải ít nhất 2 ký tự")]
    public string ProductName { get; set; } = string.Empty;
    
    private int supplierId;

    [SupplierExists(ErrorMessage = "Supplier ID không tồn tại trong hệ thống")]
    [ValueExists(ErrorMessage = "Id_SupplierID không tồn tại trong hệ thống")]
    [CustomValidation(typeof(ProductDtoTest), nameof(ValidatePrimaryKey))]
    public int SupplierId
    {
        get => this.supplierId;
        set => SetProperty(ref this.supplierId, value, false);
    }


    // For UI display
    public bool IsValid { get; set; } = true;

    public string PrimaryKey => Id.ToString() + "_" + SupplierId.ToString();

    public void Validate()
    {

        ValidateAllProperties();
    }

    public static ValidationResult? ValidatePrimaryKey(object value, ValidationContext context)
    {
        var instance = context.ObjectInstance as ProductDtoTest;
        if (instance == null)
        {
            return ValidationResult.Success;
        }

        if (!instance.PrimaryKey.Equals("8_888"))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            "Primary key 8_888 đã tồn tại",
            new[] { nameof(Id), nameof(SupplierId) }
        );
    }
    
}