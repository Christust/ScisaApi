using ScisaApi.Models;

namespace ScisaApi.DTOs
{
    public class RetrieveProduct
    {

        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
