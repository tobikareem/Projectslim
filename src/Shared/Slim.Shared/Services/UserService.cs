using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        
        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<bool> UpsertUserClaim(IdentityUser user, string claimType, string claimValue)
        {
            var claims = await _userManager.GetClaimsAsync(user);

            var claim = claims.FirstOrDefault(c => c.Type == claimType);


            if (claim == null)
            {
                var newClaim = new Claim(claimType, claimValue);
                var added = await _userManager.AddClaimAsync(user, newClaim);
                return added.Succeeded;
            }


            if (claim.Value == claimValue)
            {
                return true;
            }
            
            {
                var newClaim = new Claim(claimType, claimValue);
                var replaced = await _userManager.ReplaceClaimAsync(user, claim, newClaim);
                return replaced.Succeeded;
            }
        }
    }
}
