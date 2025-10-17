namespace BlazorWasmHosted.Shared.Entities;

public class Supplier
{
    public int Id { get; set; }
    
    public required string SupplierCode { get; set; }
    
    public required string SupplierName { get; set; }
    
    public string? ContactPerson { get; set; }
    
    public string? Email { get; set; }
    
    public string? Phone { get; set; }
    
    public string? Address { get; set; }
    
    public string? City { get; set; }
    
    public string? Country { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
