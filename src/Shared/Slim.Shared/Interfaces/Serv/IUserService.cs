using Microsoft.AspNetCore.Identity;

namespace Slim.Shared.Interfaces.Serv;

public interface IUserService
{
    Task<bool> UpsertUserClaim(IdentityUser user, string claimType, string claimValue);
}