using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IMapper _mapper;
        private readonly BookStoreDbContext _context;

        public GenericRepository(BookStoreDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);

           _context.Set<T>().Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if(id == null)
            {
                return null;
            }

            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> EntityExists(int id)
        {
            var entity = await GetAsync(id);

            return entity != null;
        }

        public async Task<VirtualizeResponse<TResult>> GetAllAsync<TResult>(QueryParameters queryParams) where TResult : class
        {
            var totalSize = await _context.Set<T>().CountAsync();

            var items = await _context.Set<T>()
                .Skip(queryParams.StartIndex)
                .Take(queryParams.PageSize)
                .ProjectTo<TResult>(
                    _mapper.ConfigurationProvider
                ).ToListAsync();

            var response = new VirtualizeResponse<TResult> { Items = items, TotalSize = totalSize };

            return response;
        }
    }
}
