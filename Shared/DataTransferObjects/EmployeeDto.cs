
namespace Shared.DataTransferObjects
{
    public record EmployeeDto
    {
        public Guid Id { get; init; }
        public string? Name { get; set; }
        public int Age { get; init; }
        public string? Position { get; init; }
    }
}
