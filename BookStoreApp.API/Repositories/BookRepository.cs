using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Book;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Repositories
{
    public class BookRepository: GenericRepository<Book>, IBookRepository
    {
        private readonly IMapper _mapper;
        private readonly BookStoreDbContext _context;

        public BookRepository(BookStoreDbContext context, IMapper mapper) : base(context, mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<BookDetailsDto> GetBookAsync(int id)
        {
            var bookDetailsDto = await _context.Books
               .Include(b => b.Author)
               .ProjectTo<BookDetailsDto>(_mapper.ConfigurationProvider)
               .FirstOrDefaultAsync(b => b.Id == id);

            return bookDetailsDto;
        }

        public async Task<List<BookDto>> GetBookWithAuthorsAsync()
        {
            var bookDtos = await _context.Books
                .Include(b => b.Author)
                .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return bookDtos;
        }
    }
}
