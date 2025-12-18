using System;
using BadBoys.DAL;
using BadBoys.DAL.Entities;
using BadBoys.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.BLL.Services;

public class MediaService
{
    private readonly AppDbContext _context;

    public MediaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Media>> GetAllAsync(int userId)
    {
        return await _context.Media
            .Include(m => m.User)
            .Include(m => m.Tags)
            .Where(m => m.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Media>> SearchAsync(string query, int userId)
    {
        return await _context.Media
            .Include(m => m.Tags)
            .Where(m => m.UserId == userId &&
                   (m.Title.Contains(query) ||
                    m.Description.Contains(query) ||
                    m.Author.Contains(query) ||
                    m.Genre.Contains(query) ||
                    m.Tags.Any(t => t.Name.Contains(query))))
            .ToListAsync();
    }

    public async Task<List<Media>> GetFilteredAsync(
        int userId,
        string? searchTerm = null,
        MediaType? type = null,
        string? genre = null,
        int? minYear = null,
        int? maxYear = null,
        MediaStatus? status = null,
        double? minRating = null,
        double? maxRating = null)
    {
        var query = _context.Media
            .Include(m => m.Tags)
            .Where(m => m.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(m => m.Title.Contains(searchTerm) ||
                                   m.Description.Contains(searchTerm) ||
                                   m.Author.Contains(searchTerm) ||
                                   m.Genre.Contains(searchTerm) ||
                                   m.Tags.Any(t => t.Name.Contains(searchTerm)));
        }

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(m => m.Genre.Contains(genre));

        if (minYear.HasValue)
            query = query.Where(m => m.Year >= minYear.Value);

        if (maxYear.HasValue)
            query = query.Where(m => m.Year <= maxYear.Value);

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        if (minRating.HasValue)
            query = query.Where(m => m.Rating >= minRating.Value);

        if (maxRating.HasValue)
            query = query.Where(m => m.Rating <= maxRating.Value);

        return await query.ToListAsync();
    }

    public async Task<Media?> GetByIdAsync(int id, int userId)
    {
        return await _context.Media
            .Include(m => m.Tags)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
    }

    public async Task AddAsync(Media item)
    {
        try
        {
            Console.WriteLine($"MediaService.AddAsync: Adding '{item.Title}' for UserId={item.UserId}");
            item.DateAdded = DateTime.Now;
            _context.Media.Add(item);
            var result = await _context.SaveChangesAsync();
            Console.WriteLine($"MediaService.AddAsync: SaveChanges returned {result} rows affected");
            
            // Verify it was saved
            var savedItem = await _context.Media
                .FirstOrDefaultAsync(m => m.Id == item.Id);
            Console.WriteLine($"MediaService.AddAsync: Item saved with Id={savedItem?.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MediaService.AddAsync ERROR: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            throw;
        }
    }

    public async Task UpdateAsync(Media item)
    {
        if (item.Status == MediaStatus.Completed && !item.DateCompleted.HasValue)
            item.DateCompleted = DateTime.Now;

        _context.Media.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Media item)
    {
        _context.Media.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetAllGenresAsync(int userId)
    {
        return await _context.Media
            .Where(m => m.UserId == userId && !string.IsNullOrEmpty(m.Genre))
            .Select(m => m.Genre)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<string>> GetAllTagsAsync(int userId)
    {
        return await _context.Tags
            .Where(t => t.Media.UserId == userId)
            .Select(t => t.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetStatisticsAsync(int userId)
    {
        var stats = new Dictionary<string, int>();

        var userMedia = _context.Media.Where(m => m.UserId == userId);

        stats["Total"] = await userMedia.CountAsync();
        stats["Movies"] = await userMedia.CountAsync(m => m.Type == MediaType.Movie);
        stats["TV Shows"] = await userMedia.CountAsync(m => m.Type == MediaType.TVShow);
        stats["Games"] = await userMedia.CountAsync(m => m.Type == MediaType.Game);
        stats["Books"] = await userMedia.CountAsync(m => m.Type == MediaType.Book);
        stats["Music"] = await userMedia.CountAsync(m => m.Type == MediaType.Music);
        stats["Other"] = await userMedia.CountAsync(m => m.Type == MediaType.Other);

        stats["Completed"] = await userMedia.CountAsync(m => m.Status == MediaStatus.Completed);
        stats["In Progress"] = await userMedia.CountAsync(m => m.Status == MediaStatus.InProgress);
        stats["Planned"] = await userMedia.CountAsync(m => m.Status == MediaStatus.Planned);
        stats["On Hold"] = await userMedia.CountAsync(m => m.Status == MediaStatus.OnHold);
        stats["Dropped"] = await userMedia.CountAsync(m => m.Status == MediaStatus.Dropped);

        return stats;
    }

    public async Task<Dictionary<string, int>> GetGenreStatisticsAsync(int userId)
    {
        return await _context.Media
            .Where(m => m.UserId == userId && !string.IsNullOrEmpty(m.Genre))
            .GroupBy(m => m.Genre)
            .Select(g => new { Genre = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Genre, x => x.Count);
    }

    public async Task<List<Media>> GetTopRatedAsync(int userId, int count = 10)
    {
        return await _context.Media
            .Where(m => m.UserId == userId && m.Rating.HasValue)
            .OrderByDescending(m => m.Rating)
            .Take(count)
            .ToListAsync();
    }
}
