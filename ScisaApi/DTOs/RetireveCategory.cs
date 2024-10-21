using ScisaApi.Models;

namespace ScisaApi.DTOs
{
    public class RetrieveCategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<RetrieveCategoryProducts> Products { get; set; } = new List<RetrieveCategoryProducts>();
    }
}
