using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Slim.Core.Model;
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

        public async Task<(AddressModel addressModel, List<string> nullOrEmptyProperties)> LoadUserAddressInformationAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            var userClaims = await _userManager.GetClaimsAsync(user);
            bool.TryParse(userClaims.FirstOrDefault(x => x.Type == nameof(AddressModel.IsSameAsAddress))?.Value, out var isSame);
            var input = new AddressModel
            {
                FirstName = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty,
                LastName = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsSameAsAddress = isSame,
                Address1 = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.StreetAddress)?.Value ?? string.Empty,
                Address2 = userClaims.FirstOrDefault(x => x.Type == CustomClaims.Address2)?.Value ?? string.Empty,
                ZipCode = userClaims.FirstOrDefault(x => x.Type == CustomClaims.Zipcode)?.Value ?? string.Empty,

                BillingAddress1 = userClaims.FirstOrDefault(x => x.Type == CustomClaims.BillingAddress1)?.Value ?? string.Empty,
                BillingAddress2 = userClaims.FirstOrDefault(x => x.Type == CustomClaims.BillingAddress2)?.Value ?? string.Empty,
                BillingZipCode = userClaims.FirstOrDefault(x => x.Type == CustomClaims.BillingZipCode)?.Value ?? string.Empty,

            };
            var nullOrEmptyProperties =  new List<string> ();

            foreach (var property in input.GetType().GetProperties())
            {
                var value = property.GetValue(input, null);
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    nullOrEmptyProperties.Add(property.Name);
                }
            }   

            return (input, nullOrEmptyProperties);
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
