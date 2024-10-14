using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ScisaApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<Category> Categories { get; set; } = new List<Category>();

    }
}
