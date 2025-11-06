using BlazorWasmHosted.Services;
using BlazorWasmHosted.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasmHosted.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    private readonly ILogger<SuppliersController> _logger;

    public SuppliersController(ISupplierService supplierService, ILogger<SuppliersController> logger)
    {
        _supplierService = supplierService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<SupplierDto>>> GetAll()
    {
        try
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting suppliers");
            return StatusCode(500, "An error occurred while retrieving suppliers");
        }
    }

    [HttpGet("ids")]
    public async Task<ActionResult<List<int>>> GetAllIds()
    {
        try
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            var ids = suppliers.Select(s => s.Id).ToList();
            return Ok(ids);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supplier IDs");
            return StatusCode(500, "An error occurred while retrieving supplier IDs");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id)
    {
        try
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supplier {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the supplier");
        }
    }

    [HttpGet("test")]
    public async Task<ActionResult<List<ProductDto>>> GetSupplierWithProducts()
    {
        try
        {
            var products = await _supplierService.GetSupplierWithProducts();
            if (products == null)
                return NotFound();

            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving the supplier");
        }
    }

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierRequest request)
    {
        try
        {
            var supplier = await _supplierService.CreateSupplierAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier");
            return StatusCode(500, "An error occurred while creating the supplier");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SupplierDto>> Update(int id, UpdateSupplierRequest request)
    {
        try
        {
            var supplier = await _supplierService.UpdateSupplierAsync(id, request);
            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating supplier {Id}", id);
            return StatusCode(500, "An error occurred while updating the supplier");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting supplier {Id}", id);
            return StatusCode(500, "An error occurred while deleting the supplier");
        }
    }
}
