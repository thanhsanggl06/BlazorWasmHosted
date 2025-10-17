using BlazorWasmHosted.Shared.Models;

namespace BlazorWasmHosted.Services;

public interface ISupplierService
{
    Task<List<SupplierDto>> GetAllSuppliersAsync();
    Task<SupplierDto?> GetSupplierByIdAsync(int id);
    Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request);
    Task<SupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierRequest request);
    Task<bool> DeleteSupplierAsync(int id);
}
