
using DSAR.Data;
using DSAR.Interfaces;
using DSAR.Models;
using DSAR.Repositories;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace DSAR.Controllers
{
    public class AccountController : Controller
    {

        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        private readonly IAccountRepository _accountRepository;
        private readonly ICityRepository _cityRepository;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IAccountRepository accountRepository, ICityRepository cityRepository)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _accountRepository = accountRepository;
            _cityRepository = cityRepository;
        }


        // GET: UserController

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.LoginAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("Main", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect.");
                }
            }

            return View(model);
        }
        public ActionResult Register()
        {
            var model = new RegisterViewModel
            {
                Cities = _cityRepository.GetAll()
        .Select(c => new SelectListItem
        {
            Value = c.CityId.ToString(),
            Text = c.CityName
        }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var result = await _accountRepository.RegisterAsync(model, "User"); // or "Admin", etc.

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [Authorize]
        public IActionResult Main()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


    }
}
