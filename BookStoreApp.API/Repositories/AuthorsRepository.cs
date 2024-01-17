using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Author;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Repositories
{
    public class AuthorsRepository: GenericRepository<Author>, IAuthorsRepository
    {
        private readonly IMapper _mapper;
        private readonly BookStoreDbContext _context;

        public AuthorsRepository(BookStoreDbContext context, IMapper mapper) : base(context, mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<AuthorDetailsDto> GetAuthorDetailsAsync(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .ProjectTo<AuthorDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(a => a.Id == id);

            return author;
        }
    }
}
