using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly WebAPIDbContext _context;
        private readonly SSEManagerContext _wsContext;

        public UserController(WebAPIDbContext context, SSEManagerContext wsContext)
        {
            _context = context;
            _wsContext = wsContext;
        }

        // GET: api/User
        [HttpGet]
        [IsHigherThan(UserRoles.User)]
        public async Task<ActionResult<IEnumerable<UserModelDto>>> GetUserModels()
        {
            var users = await _context.UserModels.ToListAsync();
            var dto = users.Select(u => new UserModelDto(u));
            return Ok(dto);
        }

        // GET: api/User/5
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<UserModel>> GetUserModel(Guid id)
        {
            var userModel = await _context.UserModels.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }

            return Ok( new UserModelDto(userModel) );
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> PutUserModel(Guid id, UserRequestModel userRequestModel)
        {
            var isAdmin = CompareRole(UserRoles.Moderator);
            if (!CheckMySelf(id) && !isAdmin)
            {
                return Unauthorized("自分のデータではないか、権限不足です");
            }
            var userModel = await _context.UserModels.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }
            if (userModel.UserName != userRequestModel.UserName && UserModelExists(userRequestModel.UserName))
            {
                return BadRequest("同名のユーザーがいます");
            }

            userModel = userModel.Merge(id, userRequestModel, true);
            _context.Entry(userModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserModel>> PostUserModel([FromBody] UserRequestModel userRequestModel)
        {
            var newUser = new UserModel().Merge(Guid.Empty, userRequestModel);

            if (UserModelExists(newUser.UserName))
            {
                return BadRequest("同名のユーザーがいます");

            }
            _context.UserModels.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserModel), new { id = newUser.UserId }, new UserModelDto(newUser));
        }

        // DELETE: api/User/5
        [HttpDelete("{id:Guid}")]
        [IsHigherThan(UserRoles.Moderator)]
        public async Task<IActionResult> DeleteUserModel(Guid id)
        {
            var userModel = await _context.UserModels.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }

            _context.UserModels.Remove(userModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserModelExists(Guid id)
        {
            return _context.UserModels.Any(e => e.UserId == id);
        }
        private bool UserModelExists(string name)
        {
            return _context.UserModels.Any(e => e.UserName == name);
        }
        private bool CompareRole(UserRoles target)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == null)
            {
                return false;
            }
            return target <= UserRolesUtil.GetEnum(role);
        }
        private bool CheckMySelf(Guid targetUserId)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return false;
            }
            return targetUserId.ToString() == userId;
        }
    }
}
