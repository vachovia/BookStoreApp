using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Models.Book;

namespace BookStoreApp.API.Configuration
{
    public class MapperConfig: Profile
    {
        public MapperConfig()
        {
            CreateMap<AuthorDto, Author>().ReverseMap();
            CreateMap<AuthorCreateDto, Author>().ReverseMap();
            CreateMap<AuthorUpdateDto, Author>().ReverseMap();

            // CreateMap<BookDto, Book>().ReverseMap(); initial map but Author Name needs special approach
            CreateMap<Book, BookDto>().ForMember(q => q.AuthorName, d => d.MapFrom(map => $"{map.Author.FirstName} {map.Author.LastName}")).ReverseMap();
            CreateMap<Book, BookDetailsDto>().ForMember(q => q.AuthorName, d => d.MapFrom(map => $"{map.Author.FirstName} {map.Author.LastName}")).ReverseMap();

            CreateMap<BookCreateDto, Book>().ReverseMap();
            CreateMap<BookUpdateDto, Book>().ReverseMap();
        }
    }
}
