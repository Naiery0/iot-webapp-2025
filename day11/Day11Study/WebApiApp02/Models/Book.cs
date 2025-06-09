using System.ComponentModel.DataAnnotations;

namespace WebApiApp02.Models
{
    public class Book
    {
        [Key]
        public int Idx { get; set; }
        [Required]
        public string Names { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public DateOnly ReleaseDate { get; set; }
    }
}
