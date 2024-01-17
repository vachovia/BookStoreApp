using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models;
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Repositories;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;
        private readonly IAuthorsRepository _authorsRepository;

        public AuthorsController(IAuthorsRepository authorsRepository, IMapper mapper, ILogger<AuthorsController> logger)
        {
            _mapper = mapper;
            _logger = logger;
            _authorsRepository = authorsRepository;
        }

        [HttpGet] // GET: api/Authors/?startindex=0&pagesize=15
        public async Task<ActionResult<VirtualizeResponse<AuthorDto>>> GetAuthors([FromQuery] QueryParameters queryParams)
        {
            _logger.LogInformation($"Request to {nameof(GetAuthors)}");

            try
            {
                var authorDtos = await _authorsRepository.GetAllAsync<AuthorDto>(queryParams);

                return Ok(authorDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(GetAuthors)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }

        [HttpGet("GetAll")] // GET: api/Authors/GetAll // nswag.init
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            _logger.LogInformation($"Request to {nameof(GetAuthors)}");

            try
            {
                var authors = await _authorsRepository.GetAllAsync();

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
        public async Task<ActionResult<AuthorDetailsDto>> GetAuthor(int id)
        {
            try
            {
                var authorDetailsDto = await _authorsRepository.GetAuthorDetailsAsync(id);

                if (authorDetailsDto == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(GetAuthor)} - Id: {id}");

                    return NotFound();
                }

                return authorDetailsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(GetAuthor)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpPut("{id}")] // PUT: api/Authors/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorUpdateDto)
        {
            try
            {
                if (id != authorUpdateDto.Id)
                {
                    _logger.LogWarning($"Update Id invalid in {nameof(PutAuthor)} - Id: {id}");

                    return BadRequest();
                }

                var author = await _authorsRepository.GetAsync(id);

                if (author == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(PutAuthor)} - Id: {id}");

                    return NotFound();
                }

                _mapper.Map(authorUpdateDto, author);

                try
                {
                    await _authorsRepository.UpdateAsync(author);
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
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Author>> PostAuthor(AuthorCreateDto authorDto)
        {
            try
            {
                var author = _mapper.Map<Author>(authorDto);

                await _authorsRepository.AddAsync(author);
                
                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing GET in {nameof(PostAuthor)}");

                return StatusCode(500, Messages.Error500Message);
            }
        }
        
        [HttpDelete("{id}")] // DELETE: api/Authors/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _authorsRepository.GetAsync(id);

                if (author == null)
                {
                    _logger.LogWarning($"Record Not Found: {nameof(DeleteAuthor)} - Id: {id}");

                    return NotFound();
                }

                await _authorsRepository.DeleteAsync(id);

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
            return await _authorsRepository.EntityExists(id);
        }
    }
}
