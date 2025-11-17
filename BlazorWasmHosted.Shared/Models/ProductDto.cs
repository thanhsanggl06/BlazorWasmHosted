namespace BlazorWasmHosted.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlazorWasmHosted.Shared.ValidationAttributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

public class ProductDtoTest : ObservableValidator, INotifyDataErrorInfo
{
    private int id;
    private int supplierId;
    
    // Dictionary to store injected errors [PropertyName -> List of error messages]
    private readonly Dictionary<string, List<string>> _injectedErrors = new();
    
    // Dummy field to trigger ErrorsChanged event via SetProperty
    private int _errorTrigger;

    [CustomValidation(typeof(ProductDtoTest), nameof(ValidatePrimaryKey))]
    public int Id
    {
        get => this.id;
        set => SetProperty(ref this.id, value, false);
    }

    [Required(ErrorMessage = "Tên không ðý?c ð? tr?ng")]
    [MinLength(2, ErrorMessage = "Tên ph?i ít nh?t 2 k? t?")]
    public string ProductName { get; set; } = string.Empty;

    [SupplierExists(ErrorMessage = "Supplier ID không t?n t?i trong h? th?ng")]
    [ValueExists(ErrorMessage = "Id_SupplierID không t?n t?i trong h? th?ng")]
    [CustomValidation(typeof(ProductDtoTest), nameof(ValidatePrimaryKey))]
    public int SupplierId
    {
        get => this.supplierId;
        set => SetProperty(ref this.supplierId, value, false);
    }

    // For UI display
    public bool IsValid { get; set; } = true;

    public string PrimaryKey => Id.ToString() + "_" + SupplierId.ToString();

    #region INotifyDataErrorInfo Implementation

    /// <summary>
    /// Trigger ErrorsChanged event for a specific property
    /// Uses SetProperty trick to invoke the event
    /// </summary>
    private void RaiseErrorsChanged(string propertyName)
    {
        // Trigger validation for the specific property to raise ErrorsChanged
        // This is a workaround since ObservableValidator doesn't expose OnErrorsChanged
        SetProperty(ref _errorTrigger, _errorTrigger + 1, false, propertyName);
    }

    /// <summary>
    /// Inject validation errors from server without triggering validation
    /// Client ch? c?n g?i method này ð? set errors
    /// </summary>
    public void SetErrors(Dictionary<string, List<string>> errors)
    {
        if (errors == null)
        {
            ClearAllErrors();
            return;
        }

        // Clear existing injected errors
        _injectedErrors.Clear();

        // Add new errors and trigger change notification
        foreach (var kvp in errors)
        {
            _injectedErrors[kvp.Key] = kvp.Value.ToList();
            RaiseErrorsChanged(kvp.Key);
        }
    }

    /// <summary>
    /// Set errors for a specific property
    /// </summary>
    public void SetErrors(string propertyName, List<string> errorMessages)
    {
        if (string.IsNullOrEmpty(propertyName))
            return;

        if (errorMessages == null || !errorMessages.Any())
        {
            ClearInjectedErrors(propertyName);
        }
        else
        {
            _injectedErrors[propertyName] = errorMessages.ToList();
            RaiseErrorsChanged(propertyName);
        }
    }

    /// <summary>
    /// Clear all injected errors
    /// </summary>
    public void ClearAllErrors()
    {
        var propertiesToNotify = _injectedErrors.Keys.ToList();
        _injectedErrors.Clear();

        foreach (var propertyName in propertiesToNotify)
        {
            RaiseErrorsChanged(propertyName);
        }
    }

    /// <summary>
    /// Clear injected errors for a specific property
    /// </summary>
    public void ClearInjectedErrors(string propertyName)
    {
        if (_injectedErrors.Remove(propertyName))
        {
            RaiseErrorsChanged(propertyName);
        }
    }

    /// <summary>
    /// Get all errors (both validation and injected) for a specific property
    /// Hides base class method to merge with injected errors
    /// </summary>
    public new System.Collections.IEnumerable GetErrors(string? propertyName)
    {
        var errors = new List<string>();

        // Get validation errors from base ObservableValidator
        var baseErrors = base.GetErrors(propertyName);
        if (baseErrors != null)
        {
            foreach (var error in baseErrors)
            {
                if (error is ValidationResult validationResult)
                {
                    if (!string.IsNullOrEmpty(validationResult.ErrorMessage))
                        errors.Add(validationResult.ErrorMessage);
                }
                else
                {
                    errors.Add(error?.ToString() ?? string.Empty);
                }
            }
        }

        // Add injected errors
        if (!string.IsNullOrEmpty(propertyName) && _injectedErrors.TryGetValue(propertyName, out var injectedErrors))
        {
            errors.AddRange(injectedErrors);
        }

        return errors;
    }

    /// <summary>
    /// Check if object has any errors (validation or injected)
    /// </summary>
    public new bool HasErrors => base.HasErrors || _injectedErrors.Any();

    #endregion

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
            "Primary key 8_888 ð? t?n t?i",
            new[] { nameof(Id), nameof(SupplierId) }
        );
    }
}