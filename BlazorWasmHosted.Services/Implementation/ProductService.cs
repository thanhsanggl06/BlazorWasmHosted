using BlazorWasmHosted.Data;
using BlazorWasmHosted.Shared.Entities;
using BlazorWasmHosted.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmHosted.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Supplier)
            .Select(p => new ProductDto(
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Category,
                p.UnitPrice,
                p.Quantity,
                p.InStock,
                p.Description,
                p.CreatedAt,
                p.UpdatedAt,
                p.SupplierId,
                p.Supplier!.SupplierName,
                p.UnitPrice * p.Quantity
            ))
            .ToListAsync();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        return new ProductDto(
            product.Id,
            product.ProductCode,
            product.ProductName,
            product.Category,
            product.UnitPrice,
            product.Quantity,
            product.InStock,
            product.Description,
            product.CreatedAt,
            product.UpdatedAt,
            product.SupplierId,
            product.Supplier!.SupplierName,
            product.UnitPrice * product.Quantity
        );
    }

    public async Task<List<ProductDto>> GetProductsByCategoryAsync(string category)
    {
        return await _context.Products
            .Include(p => p.Supplier)
            .Where(p => p.Category == category)
            .Select(p => new ProductDto(
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Category,
                p.UnitPrice,
                p.Quantity,
                p.InStock,
                p.Description,
                p.CreatedAt,
                p.UpdatedAt,
                p.SupplierId,
                p.Supplier!.SupplierName,
                p.UnitPrice * p.Quantity
            ))
            .ToListAsync();
    }

    public async Task<List<ProductDto>> GetProductsBySupplierAsync(int supplierId)
    {
        return await _context.Products
            .Include(p => p.Supplier)
            .Where(p => p.SupplierId == supplierId)
            .Select(p => new ProductDto(
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Category,
                p.UnitPrice,
                p.Quantity,
                p.InStock,
                p.Description,
                p.CreatedAt,
                p.UpdatedAt,
                p.SupplierId,
                p.Supplier!.SupplierName,
                p.UnitPrice * p.Quantity
            ))
            .ToListAsync();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            ProductCode = request.ProductCode,
            ProductName = request.ProductName,
            Category = request.Category,
            UnitPrice = request.UnitPrice,
            Quantity = request.Quantity,
            InStock = request.InStock,
            Description = request.Description,
            SupplierId = request.SupplierId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Load supplier information
        await _context.Entry(product).Reference(p => p.Supplier).LoadAsync();

        return new ProductDto(
            product.Id,
            product.ProductCode,
            product.ProductName,
            product.Category,
            product.UnitPrice,
            product.Quantity,
            product.InStock,
            product.Description,
            product.CreatedAt,
            product.UpdatedAt,
            product.SupplierId,
            product.Supplier!.SupplierName,
            product.UnitPrice * product.Quantity
        );
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        product.ProductCode = request.ProductCode;
        product.ProductName = request.ProductName;
        product.Category = request.Category;
        product.UnitPrice = request.UnitPrice;
        product.Quantity = request.Quantity;
        product.InStock = request.InStock;
        product.Description = request.Description;
        product.SupplierId = request.SupplierId;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Reload supplier if changed
        if (product.SupplierId != request.SupplierId)
        {
            await _context.Entry(product).Reference(p => p.Supplier).LoadAsync();
        }

        return new ProductDto(
            product.Id,
            product.ProductCode,
            product.ProductName,
            product.Category,
            product.UnitPrice,
            product.Quantity,
            product.InStock,
            product.Description,
            product.CreatedAt,
            product.UpdatedAt,
            product.SupplierId,
            product.Supplier!.SupplierName,
            product.UnitPrice * product.Quantity
        );
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}
