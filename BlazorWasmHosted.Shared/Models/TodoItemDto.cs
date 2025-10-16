namespace BlazorWasmHosted.Shared;

public record TodoItemDto(
    int Id,
    string Title,
    bool IsDone,
    DateTime CreatedAt
);

public record CreateTodoRequest(string Title);

public record UpdateTodoRequest(string Title, bool IsDone);
