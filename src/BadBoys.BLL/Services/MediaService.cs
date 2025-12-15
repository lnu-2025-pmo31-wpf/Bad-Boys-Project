using BadBoys.DAL;
using BadBoys.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.BLL.Services
{
    public class MediaService
    {
        private readonly AppDbContext _context;

        public MediaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Media>> GetAllAsync()
        {
            return await _context.Media.ToListAsync();
        }

        public async Task AddAsync(Media media)
        {
            _context.Media.Add(media);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Media media)
        {
            _context.Media.Update(media);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Media.FindAsync(id);
            if (entity != null)
            {
                _context.Media.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
