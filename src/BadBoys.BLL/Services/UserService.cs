using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BadBoys.DAL;
using BadBoys.DAL.Entities;

namespace BadBoys.BLL.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"Login attempt: {username}");
                
                // First, ensure database is ready
                await EnsureDatabaseReady();
                
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null || !VerifyPassword(password, user.PasswordHash))
                {
                    Console.WriteLine($"Login failed for: {username}");
                    return null;
                }

                Console.WriteLine($"Login successful: {username}");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"Register attempt: {username}");
                
                // Ensure database is ready
                await EnsureDatabaseReady();
                
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    Console.WriteLine($"Username already exists: {username}");
                    return false;
                }

                var user = new User
                {
                    Username = username,
                    PasswordHash = HashPassword(password),
                    Role = "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Registration successful: {username}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                throw;
            }
        }

        private async Task EnsureDatabaseReady()
        {
            try
            {
                // Try to query Users table
                await _context.Users.FirstOrDefaultAsync();
            }
            catch
            {
                Console.WriteLine("Database not ready. Creating...");
                await _context.Database.EnsureCreatedAsync();
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}
