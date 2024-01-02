using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly BookStoreDbContext _context;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(BookStoreDbContext context, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        [HttpGet] // GET: api/Authors
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            _logger.LogInformation($"Request to {nameof(GetAuthors)}");

            try
            {
                var authors = await _context.Authors.ToListAsync();

                var authorDtos = _mapper.Map<List<AuthorDto>>(authors);

                return Ok(authorDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(GetAuthors)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpGet("{id}")] // GET: api/Authors/5
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetAuthor)} - Id: {id}");

                    return NotFound();
                }

                var authorDto = _mapper.Map<AuthorDto>(author);

                return authorDto; // Means Ok(author) by default
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(GetAuthor)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpPut("{id}")] // PUT: api/Authors/5
        public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorUpdateDto)
        {
            try
            {
                if (id != authorUpdateDto.Id)
                {
                    _logger.LogWarning($"Update Id invalid in {nameof(PutAuthor)} - Id: {id}");

                    return BadRequest();
                }

                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(PutAuthor)} - Id: {id}");

                    return NotFound();
                }

                //var updatedAuthor = _mapper.Map<AuthorDto>(authorUpdateDto);

                _mapper.Map(authorUpdateDto, author);

                _context.Entry(author).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await AuthorExists(id))
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
                _logger.LogError(ex, $"Error performing GET in {nameof(PutAuthor)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpPost] // POST: api/Authors
        public async Task<ActionResult<Author>> PostAuthor(AuthorCreateDto authorDto)
        {
            try
            {
                var author = _mapper.Map<Author>(authorDto);

                _context.Authors.Add(author);

                await _context.SaveChangesAsync();

                // return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(PostAuthor)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpDelete("{id}")] // DELETE: api/Authors/5
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(DeleteAuthor)} - Id: {id}");

                    return NotFound();
                }

                _context.Authors.Remove(author);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(DeleteAuthor)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(e => e.Id == id);
        }
    }
}
