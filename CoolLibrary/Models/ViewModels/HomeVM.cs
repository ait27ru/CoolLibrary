using System.Collections.Generic;

namespace CoolLibrary.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
    }
}
