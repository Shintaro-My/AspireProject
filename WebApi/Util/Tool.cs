using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Util
{
    public class Tool
    {
        static public async Task<(UserModel? User, bool isValid)?> GetMyUserInfoFromClaims(WebAPIDbContext dbContext, HttpContext httpContext)
        {
            var claims = httpContext.User.Claims;

            var uId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var uName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (uId == null || uName == null || role == null) return null;

            var user = await dbContext.UserModels.Where(u => u.UserId == Guid.Parse(uId)).FirstOrDefaultAsync();
            if (user == null) return (null, false);

            if (UserRolesUtil.GetEnum(role) != user.Role) return (user, false);
            return (user, true);
        }
    }
}
