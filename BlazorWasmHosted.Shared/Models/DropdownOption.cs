namespace BlazorWasmHosted.Shared.Models
{
    public class DropdownOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string? Description { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is DropdownOption other)
                return Id == other.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
