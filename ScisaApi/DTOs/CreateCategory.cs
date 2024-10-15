using ScisaApi.Models;

namespace ScisaApi.DTOs
{
    public class CreateCategory
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
