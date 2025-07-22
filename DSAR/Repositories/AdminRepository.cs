using DSAR.Data;
using DSAR.Models;
using DSAR.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSAR.Repositories
{

    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        public AdminRepository(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        // --- User operations ---

        public async Task<IdentityResult> AddUserAsync(User user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;
            if (await _roleManager.RoleExistsAsync(role))
                result = await _userManager.AddToRoleAsync(user, role);
            return result;
        }

        public async Task<List<ListUserViewModel>> GetAllUsersForAdminAsync()
        {
            var users = await _db.Users
                .Include(u => u.Department)
                .Include(u => u.Section)
                .Include(u => u.City)
                .AsNoTracking()
                .ToListAsync();

            var list = new List<ListUserViewModel>(users.Count);
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                list.Add(new ListUserViewModel
                {
                    NationalId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Roles = roles,
                    DepartmentName = u.Department?.DepartmentName,
                    SectionName = u.Section?.SectionName,
                    SectorId = u.SectorId,
                    CityName = u.City?.CityName,
                    IdentityGuid = u.Id,
                    IsActive = u.Active
                });
            }
            return list;
        }

        public Task<User> GetUserByIdAsync(string id)
            => _userManager.FindByIdAsync(id);

        public Task<IList<string>> GetUserRolesAsync(User user)
            => _userManager.GetRolesAsync(user);

        public async Task<IdentityResult> UpdateUserAsync(User user, string newRole)
        {
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return updateResult;

            // sync roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (await _roleManager.RoleExistsAsync(newRole))
                updateResult = await _userManager.AddToRoleAsync(user, newRole);

            return updateResult;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            user.Active = false;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        // --- Request operations ---

        public async Task<List<RequestListItemViewModel>> GetAllRequestsForAdminAsync()
        {
            var actions = await _db.RequestActions
                .Include(ra => ra.Status)
                .Include(ra => ra.Department)
                .Include(ra => ra.Section)
                .Include(ra => ra.User)
                .AsNoTracking()
                .ToListAsync();

            return actions.Select(ra => new RequestListItemViewModel
            {
                RequestId = ra.RequestId,
                UserId = ra.User.UserId,
                StatusName = ra.Status.StatusName,
                DepartmentName = ra.Department.DepartmentName,
                SectionName = ra.Section.SectionName
            }).ToList();
        }

        public Task<FormData> GetRequestDetailsAsync(int requestId)
            => _db.Forms
                .Include(f => f.User)
                .Include(f => f.RequestActions).ThenInclude(ra => ra.Status)
                .Include(f => f.RequestActions).ThenInclude(ra => ra.Levels)
                .Include(f => f.RequestActions).ThenInclude(ra => ra.Department)
                .Include(f => f.RequestActions).ThenInclude(ra => ra.Section)
                .FirstOrDefaultAsync(f => f.RequestId == requestId);

        public async Task UpdateRequestLevelAsync(int requestId, int levelId, int statusId)
        {
            var ra = await _db.RequestActions.FirstOrDefaultAsync(ra => ra.RequestId == requestId);
            if (ra != null)
            {
                ra.LevelId = levelId;
                ra.StatusId = statusId;
                await _db.SaveChangesAsync();
            }

        }


        //Dropdown sources for Users

        public async Task<List<SelectListItem>> GetAllCitiesAsync() =>
            await _db.City
                     .Select(c => new SelectListItem(c.CityName, c.CityId.ToString()))
                     .ToListAsync();

        public async Task<List<SelectListItem>> GetAllSectionsAsync() =>
            await _db.Section
                     .Select(s => new SelectListItem(s.SectionName, s.SectionId.ToString()))
                     .ToListAsync();

        public async Task<List<SelectListItem>> GetAllDepartmentsAsync() =>
            await _db.Department
                     .Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString()))
                     .ToListAsync();

        public async Task<List<SelectListItem>> GetAllSectorsAsync() =>
            await _db.Sector
                     .Select(s => new SelectListItem(s.SectorName, s.SectorId.ToString()))
                     .ToListAsync();

        public async Task<List<SelectListItem>> GetAllRolesAsync() =>
            await _roleManager.Roles
                              .Select(r => new SelectListItem(r.Name, r.Name))
                              .ToListAsync();

        //for ChangeLevel

        public async Task<List<SelectListItem>> GetAllowedLevelsAsync(int currentLevelId) =>
            await _db.Level
                     .Where(l => l.LevelId < currentLevelId)
                     .Select(l => new SelectListItem(l.LevelName, l.LevelId.ToString()))
                     .ToListAsync();

        public async Task<List<SelectListItem>> GetAllStatusesAsync() =>
            await _db.Status
                     .Select(s => new SelectListItem(s.StatusName, s.StatusId.ToString()))
                     .ToListAsync();


        //collect all dropdowns


        public async Task<InsertUserViewModel> BuildInsertUserViewModelAsync()
        {
            return new InsertUserViewModel
            {
                Cities = await GetAllCitiesAsync(),
                Sectors = await GetAllSectorsAsync(),
                Departments = await GetAllDepartmentsAsync(),
                Sections = new List<SelectListItem>(),       // start empty
                Roles = await GetAllRolesAsync()
            };
        }

        public async Task<EditUserViewModel> BuildEditUserViewModelAsync(string id)
        {
            var user = await GetUserByIdAsync(id);
            var roles = await GetUserRolesAsync(user);

            var vm = new EditUserViewModel
            {
                IdentityGuid = user.Id,
                NationalId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CityId = user.CityId,
                SectionId = user.SectionId,
                DepartmentId = user.DepartmentId,
                SectorId = user.SectorId ?? 0,
                Role = roles.FirstOrDefault()
            };
            await PopulateUserDropdownsAsync(vm);
            return vm;
        }

        public async Task PopulateUserDropdownsAsync(EditUserViewModel vm)
        {
            vm.Cities = await GetAllCitiesAsync();
            vm.Sections = await GetAllSectionsAsync();
            vm.Departments = await GetAllDepartmentsAsync();
            vm.Sectors = await GetAllSectorsAsync();
            vm.Roles = await GetAllRolesAsync();
        }

        public async Task<List<SelectListItem>> GetSectionsByDepartmentAsync(int departmentId)
        {
            return await _db.Section
                .Where(s => s.DepartmentId == departmentId)
                .OrderBy(s => s.SectionName)
                .Select(s => new SelectListItem
                {
                    Value = s.SectionId.ToString(),
                    Text = s.SectionName
                })
                .ToListAsync();
        }


        public async Task<ChangeLevelViewModel> BuildChangeLevelViewModelAsync(int requestId)
        {
            var ra = await _db.RequestActions
                .Include(x => x.Levels)
                .FirstOrDefaultAsync(x => x.RequestId == requestId);
            if (ra == null) return null;

            var vm = new ChangeLevelViewModel
            {
                RequestId = requestId,
                CurrentLevelId = ra.LevelId,
                CurrentLevelName = ra.Levels.LevelName,
                SelectedLevelId = ra.LevelId,
                SelectedStatusId = ra.StatusId,

                Levels = await GetAllowedLevelsAsync(ra.LevelId),
                Statuses = await GetAllStatusesAsync()
            };

            return vm;
        }

    }
}
