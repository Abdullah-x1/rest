using DSAR.ViewModels;
using DSAR.Models;
using Microsoft.AspNetCore.Identity;

namespace DSAR.Repositories
{
    public interface IAccountRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model, string role);
        Task<SignInResult> LoginAsync(LoginViewModel model);


    }
}
