using System.ComponentModel.DataAnnotations;

namespace BlazorWasmHosted.Shared.ValidationAttributes;

/// <summary>
/// [DEPRECATED - Use MultiValidationScope instead]
/// Generic validation scope cho batch validation
/// T? �?ng load cache v� clear sau khi validate
/// 
/// Recommended: Use MultiValidationScope for consistency
/// </summary>
/// <typeparam name="T">Ki?u d? li?u c?a cache (int, string, Guid, etc.)</typeparam>
[Obsolete("Use MultiValidationScope instead for consistency. This will be removed in future version.")]
public class ValidationScope<T> : IDisposable
{
    private readonly string _cacheKey;
    private readonly bool _autoClear;

    /// <summary>
    /// T?o validation scope v?i cache key v� data
    /// </summary>
    /// <param name="cacheKey">T�n cache (VD: "SupplierIds", "Categories")</param>
    /// <param name="validValues">Danh s�ch gi� tr? h?p l?</param>
    /// <param name="autoClearOnDispose">T? �?ng clear cache khi dispose (default: true)</param>
    public ValidationScope(string cacheKey, IEnumerable<T> validValues, bool autoClearOnDispose = true)
    {
        _cacheKey = cacheKey;
        _autoClear = autoClearOnDispose;
        ValidationStore.SetCache(cacheKey, validValues);
    }

    /// <summary>
    /// Validate m?t list items
    /// </summary>
    public List<ValidationError> ValidateList<TItem>(IEnumerable<TItem> items) where TItem : class
    {
        var errors = new List<ValidationError>();
        var index = 0;

        foreach (var item in items)
        {
            var validationContext = new ValidationContext(item);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(item, validationContext, validationResults, true);

            if (!isValid)
            {
                errors.Add(new ValidationError
                {
                    Index = index,
                    Item = item,
                    Errors = validationResults.Select(r => r.ErrorMessage ?? string.Empty).ToList()
                });
            }

            index++;
        }

        return errors;
    }

    public void Dispose()
    {
        if (_autoClear)
        {
            ValidationStore.ClearCache(_cacheKey);
        }
    }
}

/// <summary>
/// ? RECOMMENDED: Multi-cache validation scope
/// Cho ph�p load nhi?u cache c�ng l�c
/// LU�N LU�N d�ng class n�y cho m?i validation (k? c? 1 cache)
/// </summary>
public class MultiValidationScope : IDisposable
{
    private readonly List<string> _cacheKeys = new();
    private readonly bool _autoClear;

    /// <summary>
    /// T?o multi-cache validation scope
    /// </summary>
    /// <param name="autoClearOnDispose">T? �?ng clear cache khi dispose (default: true)</param>
    public MultiValidationScope(bool autoClearOnDispose = true)
    {
        _autoClear = autoClearOnDispose;
    }

    /// <summary>
    /// Load cache cho m?t entity
    /// C� th? g?i nhi?u l?n �? load nhi?u cache
    /// </summary>
    /// <typeparam name="T">Ki?u d? li?u c?a cache</typeparam>
    /// <param name="cacheKey">T�n cache (VD: "SupplierIds", "Categories")</param>
    /// <param name="validValues">Danh s�ch gi� tr? h?p l?</param>
    /// <returns>Ch�nh n� �? c� th? chain method calls</returns>
    public MultiValidationScope LoadCache<T>(string cacheKey, IEnumerable<T> validValues)
    {
        ValidationStore.SetCache(cacheKey, validValues);
        _cacheKeys.Add(cacheKey);
        return this;
    }

    /// <summary>
    /// Validate m?t list items
    /// </summary>
    public List<ValidationError> ValidateList<TItem>(IEnumerable<TItem> items) where TItem : class
    {
        var errors = new List<ValidationError>();
        var index = 0;

        foreach (var item in items)
        {
            var validationContext = new ValidationContext(item);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(item, validationContext, validationResults, true);

            if (!isValid)
            {
                errors.Add(new ValidationError
                {
                    Index = index,
                    Item = item,
                    Errors = validationResults.Select(r => r.ErrorMessage ?? string.Empty).ToList()
                });
            }

            index++;
        }

        return errors;
    }

    public void Dispose()
    {
        if (_autoClear)
        {
            foreach (var cacheKey in _cacheKeys)
            {
                ValidationStore.ClearCache(cacheKey);
            }
        }
    }
}

/// <summary>
/// Validation error information
/// </summary>
public class ValidationError
{
    public int Index { get; set; }
    public object? Item { get; set; }
    public List<string> Errors { get; set; } = new();
}
