using System.ComponentModel.DataAnnotations;

namespace APIREST2.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^(Administrateur|Lecteur)$")]
        public string Role { get; set; }
    }
}