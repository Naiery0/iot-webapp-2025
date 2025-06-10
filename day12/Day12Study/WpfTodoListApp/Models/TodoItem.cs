using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfTodoListApp.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string Title { get; set; }
        [Required]
        [Column(TypeName = "CHAR(8)")]
        public string TodoDate { get; set; }
        public bool IsComplete { get; set; }

    }
}
