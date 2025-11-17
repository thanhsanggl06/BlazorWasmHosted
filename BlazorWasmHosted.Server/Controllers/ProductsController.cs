using BlazorWasmHosted.Services;
using BlazorWasmHosted.Shared.Models;
using BlazorWasmHosted.Shared.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace BlazorWasmHosted.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDtoTest>>> GetAll()
    {
        try
        {
            var listData = new List<ProductDtoTest>
            {
                new ProductDtoTest { Id = 1, ProductName = "AB", SupplierId = 1 },
                new ProductDtoTest { Id = 2, ProductName = "CD", SupplierId = 2 },
                new ProductDtoTest { Id = 3, ProductName = "EF", SupplierId = 2 },
                new ProductDtoTest { Id = 4, ProductName = "GH", SupplierId = 4 },
                new ProductDtoTest { Id = 5, ProductName = "CDD", SupplierId = 5 },
                new ProductDtoTest { Id = 6, ProductName = "", SupplierId = 6 },        // Invalid: empty name
                new ProductDtoTest { Id = 7, ProductName = "X", SupplierId = 999 },     // Invalid: short name + invalid supplier
                new ProductDtoTest { Id = 8, ProductName = "Product 8", SupplierId = 888 }, // Invalid: invalid supplier
            };

            foreach (var item in listData)
            {
                item.Validate();
            }

            var products = await _productService.GetAllProductsAsync();
            return Ok(listData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    [HttpPost("existingValues")]
    public async Task<ActionResult<ProductDto>> GetExistingValues(List<string> valueChecks)
    {
        try
        {
            var existingValues = await _productService.GetExistingValue(valueChecks);
            return Ok(existingValues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting existing values");
            return StatusCode(500, "An error occurred while retrieving existing values");
        }
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<ProductDto>>> GetByCategory(string category)
    {
        try
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by category {Category}", category);
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<ActionResult<List<ProductDto>>> GetBySupplier(int supplierId)
    {
        try
        {
            var products = await _productService.GetProductsBySupplierAsync(supplierId);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products by supplier {SupplierId}", supplierId);
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request)
    {
        try
        {
            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, request);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {Id}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {Id}", id);
            return StatusCode(500, "An error occurred while deleting the product");
        }
    }
}
