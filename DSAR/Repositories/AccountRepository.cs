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


        public AccountRepository(UserManager<User> userManager,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new User
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email,
                UserId = model.UserId
            };

            return await _userManager.CreateAsync(user, model.Password);
        }
    }
}
