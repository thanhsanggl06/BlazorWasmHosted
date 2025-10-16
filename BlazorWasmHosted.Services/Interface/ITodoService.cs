using BlazorWasmHosted.Shared;

namespace BlazorWasmHosted.Services;

public interface ITodoService
{
    Task<IReadOnlyList<TodoItemDto>> GetAllAsync(CancellationToken ct = default);
    Task<TodoItemDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TodoItemDto> CreateAsync(CreateTodoRequest request, CancellationToken ct = default);
    Task<TodoItemDto?> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}