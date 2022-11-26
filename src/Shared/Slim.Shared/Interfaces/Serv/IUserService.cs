using Microsoft.AspNetCore.Identity;
using Slim.Core.Model;
using System.Security.Claims;

namespace Slim.Shared.Interfaces.Serv;

public interface IUserService
{
    Task<bool> UpsertUserClaim(IdentityUser user, string claimType, string claimValue);
    Task<(AddressModel addressModel, List<string> nullOrEmptyProperties)> LoadUserAddressInformationAsync(ClaimsPrincipal principal);

}