namespace BlazorWasmHosted.Shared.Entities;

public class Product
{
    public int Id { get; set; }
    
    public required string ProductCode { get; set; }
    
    public required string ProductName { get; set; }
    
    public required string Category { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public int Quantity { get; set; }
    
    public bool InStock { get; set; } = true;
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign Key
    public int SupplierId { get; set; }
    
    // Navigation property
    public Supplier? Supplier { get; set; }

    // Required for SearchableDropdown component
    public override bool Equals(object? obj)
    {
        if (obj is Product other)
            return Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
