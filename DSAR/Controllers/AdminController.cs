using DSAR.Data;
using DSAR.Models;
using DSAR.Models.ViewModels;
using DSAR.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DSAR.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _repo;
        private readonly AppDbContext _db;
        private readonly RoleManager<IdentityRole> _roleMgr;
        //private readonly UserManager<User> _userManager;

        public AdminController(
            IAdminRepository repo,
            AppDbContext db,
            RoleManager<IdentityRole> roleMgr)
        {
            _repo = repo;
            _db = db;
            _roleMgr = roleMgr;
        }


        // --- Users ---

        [HttpGet]
        public async Task<IActionResult> ListAllUsers()
        {
            var model = await _repo.GetAllUsersForAdminAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> InsertUser()
        {
            

            var vm = new InsertUserViewModel
            {
                Cities = await _db.City.Select(c => new SelectListItem(c.CityName, c.CityId.ToString())).ToListAsync(),
                Sections = await _db.Section.Select(s => new SelectListItem(s.SectionName, s.SectionId.ToString())).ToListAsync(),
                Departments = await _db.Department.Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString())).ToListAsync(),
                Sectors = await _db.Sector.Select(s => new SelectListItem(s.SectorName, s.SectorId.ToString())).ToListAsync(),
                Roles = await _roleMgr.Roles.Select(r => new SelectListItem(r.Name, r.Name)).ToListAsync()
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertUser(InsertUserViewModel vm)
        {
            vm.Cities = await _db.City.Select(c => new SelectListItem(c.CityName, c.CityId.ToString())).ToListAsync();
            vm.Sections = await _db.Section.Select(s => new SelectListItem(s.SectionName, s.SectionId.ToString())).ToListAsync();
            vm.Departments = await _db.Department.Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString())).ToListAsync();
            vm.Sectors = await _db.Sector.Select(s => new SelectListItem(s.SectorName, s.SectorId.ToString())).ToListAsync();
            vm.Roles = await _roleMgr.Roles.Select(r => new SelectListItem(r.Name, r.Name)).ToListAsync();

            
            if (!ModelState.IsValid)
                return View(vm);

            // map 
            var user = new User
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                UserId = vm.UserId,
                Email = vm.Email,
                UserName = vm.Email,
                CityId = vm.CityId,
                SectionId = vm.SectionId,
                DepartmentId = vm.DepartmentId,
                SectorId = vm.SectorId
            };

            
            var result = await _repo.AddUserAsync(user, vm.Password, vm.Role);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            
            return RedirectToAction(nameof(ListAllUsers));
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _repo.GetUserRolesAsync(user);

            

            var vm = new EditUserViewModel
            {
                IdentityGuid = user.Id,
                NationalId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CityId = user.CityId,
                SectionId = user.SectionId ?? 0,
                DepartmentId = user.DepartmentId ?? 0,
                SectorId = user.SectorId ?? 0,
                Role = roles.FirstOrDefault(),

                Cities = await _db.City.Select(c => new SelectListItem(c.CityName, c.CityId.ToString())).ToListAsync(),
                Sections = await _db.Section.Select(s => new SelectListItem(s.SectionName, s.SectionId.ToString())).ToListAsync(),
                Departments = await _db.Department.Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString())).ToListAsync(),
                Sectors = await _db.Sector.Select(s => new SelectListItem(s.SectorName, s.SectorId.ToString())).ToListAsync(),
                Roles = await _roleMgr.Roles.Select(r => new SelectListItem(r.Name, r.Name)).ToListAsync()
            };


            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel vm)
        {
            vm.Cities = await _db.City.Select(c => new SelectListItem(c.CityName, c.CityId.ToString())).ToListAsync();
            vm.Sections = await _db.Section.Select(s => new SelectListItem(s.SectionName, s.SectionId.ToString())).ToListAsync();
            vm.Departments = await _db.Department.Select(d => new SelectListItem(d.DepartmentName, d.DepartmentId.ToString())).ToListAsync();
            vm.Sectors = await _db.Sector.Select(s => new SelectListItem(s.SectorName, s.SectorId.ToString())).ToListAsync();
            vm.Roles = await _roleMgr.Roles.Select(r => new SelectListItem(r.Name, r.Name)).ToListAsync();

            if (!ModelState.IsValid)
                return View(vm);

            var user = await _repo.GetUserByIdAsync(vm.IdentityGuid);
            if (user == null) return NotFound();

            user.UserId = vm.NationalId;
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.CityId = vm.CityId;
            user.SectionId = vm.SectionId;
            user.DepartmentId = vm.DepartmentId;
            user.SectorId = vm.SectorId;

            var updateResult = await _repo.UpdateUserAsync(user, vm.Role);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(vm);
            }

            return RedirectToAction(nameof(ListAllUsers));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _repo.DeleteUserAsync(id);
            return RedirectToAction(nameof(ListAllUsers));
        }

        [HttpGet]
        public IActionResult ResetPassword(string id)
            => View(new ResetPasswordViewModel { IdentityGuid = id });

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            await _repo.ResetPasswordAsync(vm.IdentityGuid, vm.NewPassword);
            return RedirectToAction(nameof(ListAllUsers));
        }



        [HttpGet]
        public async Task<IActionResult> ListRequests()
            => View(await _repo.GetAllRequestsForAdminAsync());

        [HttpGet]
        public async Task<IActionResult> RequestDetails(int id)
            => View(await _repo.GetRequestDetailsAsync(id));

        [HttpGet]
        public async Task<IActionResult> ChangeLevel(int id)
        {
            var ra = await _db.RequestActions
                .Include(ra => ra.Levels)
                .FirstOrDefaultAsync(ra => ra.RequestId == id);
            if (ra == null) return NotFound();

            var allowedLevels = await _db.Level
                .Where(l => l.LevelId < ra.LevelId)
                .Select(l => new SelectListItem(l.LevelName, l.LevelId.ToString()))
                .ToListAsync();

            var statuses = await _db.Status
                .Select(s => new SelectListItem(s.StatusName, s.StatusId.ToString()))
                .ToListAsync();

            var vm = new ChangeLevelViewModel
            {
                RequestId = id,
                CurrentLevelId = ra.LevelId,
                CurrentLevelName = ra.Levels.LevelName,
                Levels = allowedLevels,
                Statuses = statuses,
                SelectedLevelId = ra.LevelId,
                SelectedStatusId = ra.StatusId
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeLevel(ChangeLevelViewModel vm)
        {
            if (!ModelState.IsValid) { /* repopulate & return */ }
            await _repo.UpdateRequestLevelAsync(vm.RequestId, vm.SelectedLevelId, vm.SelectedStatusId);
            return RedirectToAction(nameof(ListRequests));
        }
    }
}