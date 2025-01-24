using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIREST2.Models
{
    public class Publisher
    {
        public Publisher()
        {
            Books = new List<Book>();
        }
    
        [Key]
        public int Id { get; set; }
    
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    
        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }
    
        [Range(1800, 2024)]
        public int? FoundedYear { get; set; }
    
        [JsonIgnore]
        public ICollection<Book> Books { get; set; }
    }
}