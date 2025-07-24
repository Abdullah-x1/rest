
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

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        private readonly IAccountRepository _accountRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IUserRepository _userRepository;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IAccountRepository accountRepository, ICityRepository cityRepository, IUserRepository userRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountRepository = accountRepository;
            _cityRepository = cityRepository;
            _userRepository = userRepository;
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
                    // Get the logged-in user
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // Get roles for the user
                    var roles = await _userManager.GetRolesAsync(user);

                    // Assuming a user only has one role, you can use FirstOrDefault
                    var role = roles.FirstOrDefault();

                    // Redirect based on role
                    switch (role)
                    {
                        case "Analyzer":
                            return RedirectToAction("orderpage", "Request");
                        case "ApplicationManager":
                            return RedirectToAction("orderpage", "Request");
                        case "ITManager":
                            return RedirectToAction("orderpage", "Request");
                        default:
                            return RedirectToAction("Main", "Account");
                    }
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
        public async Task<IActionResult> MainAsync()
        {  // Check if the user is authenticated
            var currentUser = await _userManager.GetUserAsync(User);

            var viewModel = new UserView
           {
               Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
               Email = User.FindFirstValue(ClaimTypes.Email),
               FirstName = User.FindFirstValue("FirstName"),
               LastName = User.FindFirstValue("LastName"),
               UserName = User.Identity.Name,
               CityId = int.Parse(User.FindFirstValue("CityId") ?? "0"),
               TermsAccepted = currentUser.TermsAccepted,
               terms = User.FindFirstValue("terms") ?? ""
           };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Terms(string terms, string termsAccepted)
        {
            var user = await _userManager.GetUserAsync(User);
            // Check if the user is authenticated
            if (terms == "Accept" && termsAccepted != "on")
            {
                TempData["Error"] = "ÌÃ» «·„Ê«›ﬁ… ⁄·Ï «·‘—Êÿ Ê«·√Õﬂ«„ ﬁ»· «·„ «»⁄….";
                await Logout();
                return RedirectToAction("Login", "Account");
            }

            if (terms == "Accept")
            {
                 _userRepository.UpdateTerms(user);
                return RedirectToAction("Main", "Account");
            }
            else if (terms == "Decline")
            {
                await Logout();
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


    }
}
