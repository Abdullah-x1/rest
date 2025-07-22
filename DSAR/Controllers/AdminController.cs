using DSAR.Data;
using DSAR.Models;
using DSAR.Models.ViewModels;
using DSAR.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DSAR.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminRepository _repo;
        //private readonly AppDbContext _db;
        //private readonly RoleManager<IdentityRole> _roleMgr;
        //private readonly UserManager<User> _userManager;

        public AdminController(
            IAdminRepository repo)
        //AppDbContext db
        //RoleManager<IdentityRole> roleMgr)
        {
            _repo = repo;
            //_db = db;
            //_roleMgr = roleMgr;
        }


        // --- Users ---

        [HttpGet]
        public async Task<IActionResult> ListAllUsers()
        {
            var vm = await _repo.GetAllUsersForAdminAsync();
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> InsertUser()
        {

            var vm = await _repo.BuildInsertUserViewModelAsync();
            if (vm == null) return NotFound();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertUser(InsertUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(await _repo.BuildInsertUserViewModelAsync());

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
                return View(await _repo.BuildInsertUserViewModelAsync());
            }

            return RedirectToAction(nameof(ListAllUsers));
        }

        [HttpGet]
        public async Task<JsonResult> GetSections(int departmentId)
        {
            var sections = await _repo.GetSectionsByDepartmentAsync(departmentId);
            return Json(sections);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var vm = await _repo.BuildEditUserViewModelAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await _repo.PopulateUserDropdownsAsync(vm);
                return View(vm);
            }

            var user = await _repo.GetUserByIdAsync(vm.IdentityGuid);
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
                await _repo.PopulateUserDropdownsAsync(vm);
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
        {
            var Vm = await _repo.GetAllRequestsForAdminAsync();
            if (Vm == null) return NotFound();
            return View(Vm);
        }

        [HttpGet]
        public async Task<IActionResult> RequestDetails(int id)
        {
            var vm = await _repo.GetRequestDetailsAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeLevel(int id)
        {

            var vm = await _repo.BuildChangeLevelViewModelAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeLevel(ChangeLevelViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // repopulate on failure
                vm.Levels = await _repo.GetAllowedLevelsAsync(vm.CurrentLevelId);
                vm.Statuses = await _repo.GetAllStatusesAsync();
                return View(vm);
            }

            await _repo.UpdateRequestLevelAsync(vm.RequestId, vm.SelectedLevelId, vm.SelectedStatusId);
            return RedirectToAction(nameof(ListRequests));
        }

    }
}