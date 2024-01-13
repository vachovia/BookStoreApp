﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Book;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly BookStoreDbContext _context;
        private readonly ILogger<BooksController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(BookStoreDbContext context, IMapper mapper, ILogger<BooksController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet] // GET: api/Books
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            _logger.LogInformation($"Request to {nameof(GetBooks)}");

            try
            {
                //***** Here we have Select * From Books *******
                //var books = await _context.Books.ToListAsync();
                //var bookDtos = _mapper.Map<List<BookDto>>(books);

                //**** Here EF is efficient and Selects few Book ****
                //**** columns and Author.FirstName with LastName ***
                var bookDtos = await _context.Books
                    .Include(b => b.Author)
                    .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(GetBooks)}");

                return StatusCode(500, Messages.Error500Message);
            }            
        }
        
        [HttpGet("{id}")] // GET: api/Books/5
        public async Task<ActionResult<BookDetailsDto>> GetBook(int id)
        {
            try
            {
                var bookDto = await _context.Books
                .Include(b => b.Author)
                .ProjectTo<BookDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(b => b.Id == id);

                if (bookDto == null)
                {
                    return NotFound();
                }

                return bookDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(GetBook)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }
                
        [HttpPut("{id}")] // PUT: api/Books/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
        {
            try
            {
                if (id != bookDto.Id)
                {
                    return BadRequest();
                }

                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                var picName = Path.GetFileName(book.Image);

                var path = $"{_webHostEnvironment.WebRootPath}\\BookCoverImages\\{picName}";

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                if (!string.IsNullOrEmpty(bookDto.ImageData))
                {
                    bookDto.Image = CreateFile(bookDto.ImageData, bookDto.OriginalImageName);
                }

                _mapper.Map(bookDto, book);

                _context.Entry(book).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await BookExistsAsync(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(PutBook)}");

                return StatusCode(500, Messages.Error500Message);
            }            
        }

        
        [HttpPost] // POST: api/Books
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Book>> PostBook(BookCreateDto bookDto)
        {
            try
            {
                var book = _mapper.Map<Book>(bookDto);

                book.Image = CreateFile(bookDto.Image, bookDto.OriginalImageName);

                _context.Books.Add(book);

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBook", new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(PostBook)}");

                return StatusCode(500, Messages.Error500Message);
            }            
        }

        
        [HttpDelete("{id}")] // DELETE: api/Books/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                _context.Books.Remove(book);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(DeleteBook)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }

        private string CreateFile(string imageBase64, string imageName)
        {
            var url = HttpContext.Request.Host.Value;
            var ext = Path.GetExtension(imageName);
            var fileName = $"{Guid.NewGuid()}{ext}";            
            var path = $"{_webHostEnvironment.WebRootPath}\\BookCoverImages\\{fileName}";

            byte[] image = Convert.FromBase64String(imageBase64);

            var fileStream = System.IO.File.Create(path);
            fileStream.Write(image, 0, image.Length);
            fileStream.Close();

            return $"https://{url}/BookCoverImages/{fileName}";
        }
    }
}
