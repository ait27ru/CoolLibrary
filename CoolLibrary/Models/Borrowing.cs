using CoolLibrary.Models.Validation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoolLibrary.Models
{
    public class Borrowing
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Book")]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [Display(Name = "User Id")]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [DisplayName("Borrow date")]
        public DateTime BorrowDate { get; set; }

        [DisplayName("Return date")]
        [DateNotBefore("BorrowDate", ErrorMessage = "Return date cannot be earlier than Borrow date.")]
        [FutureDate(ErrorMessage = "Return date must not be in the future.")]
        public DateTime? ReturnDate { get; set; }
    }
}
