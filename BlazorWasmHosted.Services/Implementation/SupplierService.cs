using BlazorWasmHosted.Data;
using BlazorWasmHosted.Shared.Entities;
using BlazorWasmHosted.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorWasmHosted.Services;

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _context;

    public SupplierService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplierDto>> GetAllSuppliersAsync()
    {
        return await _context.Suppliers
            .Include(s => s.Products)
            .Select(s => new SupplierDto(
                s.Id,
                s.SupplierCode,
                s.SupplierName,
                s.ContactPerson,
                s.Email,
                s.Phone,
                s.Address,
                s.City,
                s.Country,
                s.IsActive,
                s.CreatedAt,
                s.Products.Count
            ))
            .ToListAsync();
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return null;

        return new SupplierDto(
            supplier.Id,
            supplier.SupplierCode,
            supplier.SupplierName,
            supplier.ContactPerson,
            supplier.Email,
            supplier.Phone,
            supplier.Address,
            supplier.City,
            supplier.Country,
            supplier.IsActive,
            supplier.CreatedAt,
            supplier.Products.Count
        );
    }

    public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request)
    {
        var supplier = new Supplier
        {
            SupplierCode = request.SupplierCode,
            SupplierName = request.SupplierName,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            IsActive = true
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return new SupplierDto(
            supplier.Id,
            supplier.SupplierCode,
            supplier.SupplierName,
            supplier.ContactPerson,
            supplier.Email,
            supplier.Phone,
            supplier.Address,
            supplier.City,
            supplier.Country,
            supplier.IsActive,
            supplier.CreatedAt,
            0
        );
    }

    public async Task<SupplierDto?> UpdateSupplierAsync(int id, UpdateSupplierRequest request)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return null;

        supplier.SupplierCode = request.SupplierCode;
        supplier.SupplierName = request.SupplierName;
        supplier.ContactPerson = request.ContactPerson;
        supplier.Email = request.Email;
        supplier.Phone = request.Phone;
        supplier.Address = request.Address;
        supplier.City = request.City;
        supplier.Country = request.Country;
        supplier.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return new SupplierDto(
            supplier.Id,
            supplier.SupplierCode,
            supplier.SupplierName,
            supplier.ContactPerson,
            supplier.Email,
            supplier.Phone,
            supplier.Address,
            supplier.City,
            supplier.Country,
            supplier.IsActive,
            supplier.CreatedAt,
            supplier.Products.Count
        );
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier == null) return false;

        // Check if supplier has products
        if (supplier.Products.Any())
        {
            throw new InvalidOperationException("Cannot delete supplier with existing products.");
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ProductDto>> GetSupplierWithProducts()
    {
        // LEFT JOIN using LINQ query syntax
        var query = from supplier in _context.Suppliers
                    join product in _context.Products
                    on supplier.Id equals product.SupplierId into supplierProducts
                    from product in supplierProducts.DefaultIfEmpty()
                    where supplier.IsActive == true
                    select new { supplier, product };

        // Get the generated SQL query (for debugging)
        var queryString = query.ToQueryString();

        // Execute query and map to ProductDto
        var results = await query
            .Where(sp => sp.product != null) // Only include records where product exists
            .Select(sp => new ProductDto(
                sp.product.Id,
                sp.product.ProductCode,
                sp.product.ProductName,
                sp.product.Category,
                sp.product.UnitPrice,
                sp.product.Quantity,
                sp.product.InStock,
                sp.product.Description,
                sp.product.CreatedAt,
                sp.product.UpdatedAt,
                sp.product.SupplierId,
                sp.supplier.SupplierName,
                sp.product.UnitPrice * sp.product.Quantity // TotalValue
            ))
            .ToListAsync();

        return results;
    }
}
