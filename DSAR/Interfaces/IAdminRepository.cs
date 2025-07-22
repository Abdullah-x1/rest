using DSAR.Models;
using DSAR.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // ―― Dropdown sources for Users ――
        Task<List<SelectListItem>> GetAllCitiesAsync();
        Task<List<SelectListItem>> GetAllSectionsAsync();
        Task<List<SelectListItem>> GetAllDepartmentsAsync();
        Task<List<SelectListItem>> GetAllSectorsAsync();
        Task<List<SelectListItem>> GetAllRolesAsync();

        // ―― Dropdown sources for ChangeLevel ――
        Task<List<SelectListItem>> GetAllowedLevelsAsync(int currentLevelId);
        Task<List<SelectListItem>> GetAllStatusesAsync();

        //all dropdowns populated
        Task<InsertUserViewModel> BuildInsertUserViewModelAsync();

        Task<EditUserViewModel> BuildEditUserViewModelAsync(string userId);

        Task PopulateUserDropdownsAsync(EditUserViewModel vm);
        Task<ChangeLevelViewModel> BuildChangeLevelViewModelAsync(int requestId);

        Task<List<SelectListItem>> GetSectionsByDepartmentAsync(int departmentId);


    }
}
