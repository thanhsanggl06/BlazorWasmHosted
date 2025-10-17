using BlazorWasmHosted.Shared.ValidationAttributes;
using Microsoft.EntityFrameworkCore;
using BlazorWasmHosted.Data;

namespace BlazorWasmHosted.Services;

/// <summary>
/// Helper service to load validation data from database
/// </summary>
public class ValidationCacheService
{
    private readonly AppDbContext _context;

    public ValidationCacheService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Load all supplier IDs from database into validation cache
    /// </summary>
    public async Task<int> LoadSupplierIdsAsync()
    {
        var supplierIds = await _context.Suppliers
            .Select(s => s.Id)
            .ToListAsync();

        ValidationStore.SetCache("SupplierIds", supplierIds);
        
        return supplierIds.Count;
    }

    /// <summary>
    /// Get all supplier IDs from database (without caching)
    /// </summary>
    public async Task<List<int>> GetSupplierIdsAsync()
    {
        return await _context.Suppliers
            .Select(s => s.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Check if a supplier exists in database
    /// </summary>
    public async Task<bool> SupplierExistsAsync(int supplierId)
    {
        return await _context.Suppliers.AnyAsync(s => s.Id == supplierId);
    }

    /// <summary>
    /// Clear all validation caches
    /// </summary>
    public void ClearAllCaches()
    {
        ValidationStore.ClearAllCaches();
    }
}
