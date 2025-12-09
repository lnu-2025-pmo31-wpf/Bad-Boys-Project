using App.Data;
using App.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.BLL.Services
{
    public class MediaService
    {
        private readonly AppDbContext _db;

        public MediaService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<MediaItem>> GetAllAsync()
        {
            return await _db.MediaItems
                .Include(m => m.Tags)
                .ToListAsync();
        }

        public async Task AddAsync(MediaItem item)
        {
            item.CreatedAt = DateTime.UtcNow;

            _db.MediaItems.Add(item);
            await _db.SaveChangesAsync();
        }
    }
}
