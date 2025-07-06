using DSAR.Models;
using DSAR.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DSAR.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountRepository(UserManager<User> userManager,SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model, string role)
        {
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                UserId = model.UserId.ToString(), // Convert int to string to match the UserId property type
                CityId = model.CityId,
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Create the role if it does not exist
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                // Assign the user to the role
                await _userManager.AddToRoleAsync(user, role);
            }
            return result;
        }
    }
}
