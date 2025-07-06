using System.Collections.Generic;
using System.Threading.Tasks;
using DSAR.Models;
using DSAR.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DSAR.Repositories
{
    public interface IAdminRepository
    {
        // --- User operations ---
        Task<IdentityResult> AddUserAsync(User user, string password, string role);
        Task<List<ListUserViewModel>> GetAllUsersForAdminAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<IdentityResult> UpdateUserAsync(User user, string newRole);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<IdentityResult> ResetPasswordAsync(string id, string newPassword);

        // --- Request operations ---
        Task<List<RequestListItemViewModel>> GetAllRequestsForAdminAsync();
        Task<FormData> GetRequestDetailsAsync(int requestId);
        Task UpdateRequestLevelAsync(int requestId, int levelId, int statusId);
    }
}
