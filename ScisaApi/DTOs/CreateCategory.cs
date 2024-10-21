using ScisaApi.Models;
using System.ComponentModel.DataAnnotations;

namespace ScisaApi.DTOs
{
    public class CreateCategory
    {
        [Required]
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
