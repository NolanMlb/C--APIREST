using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIREST2.Models
{
    public class Category
    {
        public Category()
        {
            Books = new List<Book>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; }
    }
}