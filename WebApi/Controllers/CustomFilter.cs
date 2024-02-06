using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Security.Claims;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class IsHigherThanAttribute: ActionFilterAttribute
    {
        public UserRoles _role;
        public IsHigherThanAttribute(UserRoles role)
        {
            _role = role;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var roleDesc = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleDesc == null)
            {
                context.Result = new BadRequestObjectResult("Denied");
            }
            else
            {
                var role = (int)UserRolesConverter.GetEnum(roleDesc.Value);
                if (role < (int)_role)
                {
                    context.Result = new BadRequestObjectResult("Denied");
                }
            }
            base.OnActionExecuted(context);
        }
    }
}
