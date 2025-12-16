using BadBoys.DAL;
using BadBoys.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.BLL.Services;

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

    public async Task<List<Media>> SearchAsync(string query)
    {
        return await _context.Media
            .Where(m => m.Title.Contains(query) || m.Genre.Contains(query))
            .ToListAsync();
    }

    public async Task AddAsync(Media item)
    {
        _context.Media.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Media item)
    {
        _context.Media.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Media item)
    {
        _context.Media.Remove(item);
        await _context.SaveChangesAsync();
    }
}
