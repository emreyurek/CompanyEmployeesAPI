
namespace Shared.DataTransferObjects
{
    public record EmployeeDto
    {
        public Guid Id { get; init; }
        public int Age { get; init; }
        public string? Position { get; set; }
    }
}
