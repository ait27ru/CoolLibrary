using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CoolLibrary.Models.ViewModels
{
    public class BookVM
    {
        public Book Book { get; set; }
        public IEnumerable<SelectListItem> GenreSelectList { get; set; }
    }
}
