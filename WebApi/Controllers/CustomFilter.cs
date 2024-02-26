using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Security.Claims;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// Server-Sent Event送出用の永続レスポンスに変更するためのヘッダーを設定するアクションフィルター
    /// </summary>
    public class SSEActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Response.Headers;

            headers.Append("Content-Type", "text/event-stream");
            headers.Append("Cache-Control", "no-cache");
            headers.Append("Connection", "keep-alive");
        }
    }

    /// <summary>
    /// 一定以上のユーザロール以外からのリクエストを弾くためのアクションフィルター
    /// </summary>
    public class IsHigherThanAttribute : ActionFilterAttribute
    {
        public UserRoles _role;
        public IsHigherThanAttribute(UserRoles role)
        {
            _role = role;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleDesc = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var myUserId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (roleDesc == null)
            {
                context.Result = new UnauthorizedObjectResult("Denied");
            }
            else if (myUserId == null)
            {
                context.Result = new UnauthorizedObjectResult("Denied");
            }
            else
            {
                var role = (int)UserRolesUtil.GetEnum(roleDesc.Value);
                if (role < (int)_role)
                {
                    context.Result = new UnauthorizedObjectResult("Denied");
                }
            }
            base.OnActionExecuting(context);
        }
    }

}
