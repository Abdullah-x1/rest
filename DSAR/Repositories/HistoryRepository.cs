using DSAR.Data;
using DSAR.Interfaces;
using DSAR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DSAR.Repositories
{
    public class AppHistoryRepository : IAppHistoryRepository
    {
        private readonly AppDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppHistoryRepository(
            AppDbContext db,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task CreateHistoryAsync(User user, FormData request, int statusId, int levelId, string information)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? string.Empty;
            var role = await _roleManager.FindByNameAsync(roleName);

            var history = new History
            {
                RequestId = request.RequestId,
                UserId = user.Id,
                RoleId = role?.Id,
                StatusId = statusId,
                LevelId = levelId,
                Information = information,
                CreationDate = DateTime.UtcNow
            };

            _db.Histories.Add(history);
            await _db.SaveChangesAsync();
        }

        public async Task<List<History>> GetHistroyRequestsByUserId(string UserId)
        {
            return await _db.Histories // Fixed '_context' to '_db' to match the existing field name
                           .Include(r => r.User)
                           .Include(r => r.Levels)
                           .Include(r => r.FormData)
                           .Where(r => r.UserId == UserId && r.LevelId != 8 && r.LevelId != 9)
                           .ToListAsync();
        }

        public async Task<List<History>> GetAllHistroyRequestsByUserId(string UserId)
        {
            return await _db.Histories // Fixed '_context' to '_db' to match the existing field name
                           .Include(r => r.User)
                           .Include(r => r.Levels)
                           .Include(r => r.FormData)
                           .Where(r => r.UserId == UserId)
                           .ToListAsync();
        }
    }
}
