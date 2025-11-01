using BlazorWasmHosted.Shared.Models;

namespace BlazorWasmHosted.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<List<ProductDto>> GetProductsByCategoryAsync(string category);
    Task<List<string>> GetExistingValue(List<string> vcompositeKeyStrings);
    Task<List<ProductDto>> GetProductsBySupplierAsync(int supplierId);
    Task<ProductDto> CreateProductAsync(CreateProductRequest request);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);
}
