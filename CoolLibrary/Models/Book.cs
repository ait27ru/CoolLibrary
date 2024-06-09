using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoolLibrary.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [DisplayName("Published year")]
        [Range(1, 9999)]
        public int? PublishedYear { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, 9999)]
        public int Quantity { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }
    }
}
