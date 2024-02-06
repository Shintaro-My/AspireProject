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
                return BadRequest();
            }

            var roleDesc = UserRolesConverter.GetDescription(userModel.Role);

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

            return Ok();
        }

    }
}
