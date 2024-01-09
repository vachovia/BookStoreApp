using BookStoreApp.API.Models.Book;

namespace BookStoreApp.API.Models.Author
{
    public class AuthorDetailsDto: AuthorDto
    {
        public List<BookDto> Books { get; set; }
    }
}
