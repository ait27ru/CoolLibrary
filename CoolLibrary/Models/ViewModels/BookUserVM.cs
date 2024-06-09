using System.Collections.Generic;

namespace CoolLibrary.Models.ViewModels
{
    public class BookUserVM
    {
        public BookUserVM()
        {
            BookList = new List<Book>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public IList<Book> BookList { get; set; }
    }
}
