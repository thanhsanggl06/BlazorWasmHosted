namespace BlazorWasmHosted.Shared.Models;

public record SupplierDto(
    int Id,
    string SupplierCode,
    string SupplierName,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    string? Country,
    bool IsActive,
    DateTime CreatedAt,
    int ProductCount
);

public record CreateSupplierRequest(
    string SupplierCode,
    string SupplierName,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    string? Country
);

public record UpdateSupplierRequest(
    string SupplierCode,
    string SupplierName,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    string? City,
    string? Country,
    bool IsActive
);
