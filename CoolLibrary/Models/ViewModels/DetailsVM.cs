namespace CoolLibrary.Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Book = new Book();
        }

        public Book Book { get; set; }
        public bool ExistsInCart { get; set; }
    }
}
