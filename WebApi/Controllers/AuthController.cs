using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Context;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly WebAPIDbContext _context;

        public AuthController(WebAPIDbContext context)
        {
            _context = context;
        }

        // GET: api/Auth/Signout
        [HttpGet]
        public async Task<IActionResult> Signout(string? redirect_url = null)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect(redirect_url ?? "/");
        }

        // POST: api/Auth/Signin
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserModel>> Signin(UserLoginModel userLoginModel)
        {
            var hash = UserModel.getHashFromPassword(userLoginModel.Password);
            var userModel = await _context.UserModels
                .FirstOrDefaultAsync(u => u.UserName == userLoginModel.UserName && u.PasswordHash == hash);
            if (userModel == null)
            {
                return Unauthorized();
            }

            var roleDesc = UserRolesUtil.GetDescription(userModel.Role);

            Console.WriteLine(roleDesc);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userModel.UserId.ToString()),
                new Claim(ClaimTypes.Name, userModel.UserName),
                new Claim(ClaimTypes.Role, roleDesc),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                }
            );

            return RedirectToAction(nameof(CheckSession));
        }

        [HttpGet("session")]
        public ActionResult CheckSession()
        {

            var claims = HttpContext.User.Claims;
            var uId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var uName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(new
            {
                UserId = uId,
                UserName = uName,
                Role = role != null ? (int)UserRolesUtil.GetEnum(role) : -1,
            });
        }
        [HttpGet("roles")]
        public ActionResult GetRoles()
        {
            var obj = new Dictionary<int, string>();
            foreach(UserRoles role in Enum.GetValues(typeof(UserRoles)))
            {
                obj.Add((int)role, UserRolesUtil.GetDescription(role));
            }
            return Ok(obj);
        }

    }
}
