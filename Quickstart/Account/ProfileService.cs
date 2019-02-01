using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Linq;
using System.Threading.Tasks;

namespace sts_tutorials.Quickstart.Account
{
    public class ProfileService : IProfileService
    {
        private readonly string[] _claimTypesToMap = { "name", "role" };

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            foreach (var claimType in _claimTypesToMap)
            {
                var claims = context.Subject.Claims.Where(c => c.Type == claimType);
                context.IssuedClaims.AddRange(claims);
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true; //use some sort of actual validation here!
            return Task.CompletedTask;
        }
    }
}
