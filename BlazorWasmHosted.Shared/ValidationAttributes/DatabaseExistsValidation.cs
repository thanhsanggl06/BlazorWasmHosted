using System.ComponentModel.DataAnnotations;

namespace BlazorWasmHosted.Shared.ValidationAttributes;

/// <summary>
/// Generic base class cho tất cả database existence validation
/// Tái sử dụng cho bất kỳ entity nào (Supplier, Category, Product, etc.)
/// </summary>
public abstract class DatabaseExistsValidationAttribute<T> : ValidationAttribute
{
    protected abstract string CacheKey { get; }
    protected abstract string EntityName { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        // Check if cache is loaded
        if (!ValidationStore.IsCacheLoaded(CacheKey))
        {
            return new ValidationResult(
                $"{EntityName} cache chưa được load. Vui lòng load cache trước khi validate.",
                new[] { validationContext.MemberName ?? string.Empty }
            );
        }

        

        // Check if value exists in cache
        if (value is T typedValue)
        {
            if (!ValidationStore.Contains(CacheKey, typedValue))
            {

                return new ValidationResult(
                    ErrorMessage ?? $"{EntityName} '{typedValue}' không tồn tại trong hệ thống",
                    new[] { validationContext.MemberName ?? string.Empty }
                );
            }
        }

        return ValidationResult.Success;
    }
}

/// <summary>
/// Validation attribute để kiểm tra Supplier ID có tồn tại không
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class SupplierExistsAttribute : DatabaseExistsValidationAttribute<int>
{
    protected override string CacheKey => "SupplierIds";
    protected override string EntityName => "Supplier ID";

    public SupplierExistsAttribute()
    {
        ErrorMessage = "Supplier ID không tồn tại trong hệ thống";
    }
}

/// <summary>
/// Validation attribute để kiểm tra Category có tồn tại không
/// Ví dụ cho validation với string
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class CategoryExistsAttribute : DatabaseExistsValidationAttribute<string>
{
    protected override string CacheKey => "Categories";
    protected override string EntityName => "Category";

    public CategoryExistsAttribute()
    {
        ErrorMessage = "Category không tồn tại trong hệ thống";
    }
}

/// <summary>
/// Validation attribute để kiểm tra Product Code có tồn tại không (cho unique check)
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ProductCodeUniqueAttribute : ValidationAttribute
{
    private const string CacheKey = "ProductCodes";

    public ProductCodeUniqueAttribute()
    {
        ErrorMessage = "Product Code đã tồn tại trong hệ thống";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (!ValidationStore.IsCacheLoaded(CacheKey))
        {
            return new ValidationResult(
                "Product Code cache chưa được load. Vui lòng load cache trước khi validate.",
                new[] { validationContext.MemberName ?? string.Empty }
            );
        }

        if (value is string productCode)
        {
            // Check if code already exists (for unique validation)
            if (ValidationStore.Contains(CacheKey, productCode))
            {
                return new ValidationResult(
                    ErrorMessage ?? $"Product Code '{productCode}' đã tồn tại trong hệ thống",
                    new[] { validationContext.MemberName ?? string.Empty }
                );
            }
        }

        return ValidationResult.Success;
    }
}
