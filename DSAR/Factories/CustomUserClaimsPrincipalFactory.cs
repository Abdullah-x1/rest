using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using DSAR.Models; // Replace with your User class namespace

namespace DSAR.Factories // Replace with your project's namespace
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
    {
        public CustomUserClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Add custom claims
            identity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
            identity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
            identity.AddClaim(new Claim("Id", user.Id ?? string.Empty));
            // Add more claims if needed (e.g., Email, UserId)
            // identity.AddClaim(new Claim("Email", user.Email));

            return identity;
        }
    }
}