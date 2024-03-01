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
using WebApi.Util;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly WebAPIDbContext _context;
        private readonly SSEManagerContext _sseContext;

        public MessageController(WebAPIDbContext context, SSEManagerContext sseContext)
        {
            _context = context;
            _sseContext = sseContext;
        }


        /// <summary>
        /// 特定のメッセージ（送信元あるいは送信先が自分のもののみ）
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpGet("{messageId:guid}")]
        public async Task<IActionResult> Get(Guid messageId)
        {
            var myUserInfo = await Tool.GetMyUserInfoFromClaims(_context, HttpContext);
            if (myUserInfo == null) return StatusCode(403, "No Session.");
            var me = myUserInfo.Value.User;
            if (me == null) return StatusCode(403, "Invalid User.");
            // if (!myUserInfo.Value.isValid) return StatusCode(403, "Role Updated.");

            var message = await _context.MessageModels
                .Where(m => m.MessageId == messageId)
                .Where(m => m.ToUserId == me.UserId || m.CreatedBy == me.UserId)
                .FirstOrDefaultAsync();

            if (message == null) return NotFound();
            return Ok(message);
        }

        /// <summary>
        /// 特定のメッセージ群を返却
        /// </summary>
        /// <param name="groupedMessageId"></param>
        /// <returns></returns>
        [HttpGet("group/{groupedMessageId:guid}")]
        public async Task<IActionResult> GetGroup(Guid groupedMessageId)
        {
            var myUserInfo = await Tool.GetMyUserInfoFromClaims(_context, HttpContext);
            if (myUserInfo == null) return StatusCode(403, "No Session.");
            var me = myUserInfo.Value.User;
            if (me == null) return StatusCode(403, "Invalid User.");
            // if (!myUserInfo.Value.isValid) return StatusCode(403, "Role Updated.");

            var messages = await _context.MessageModels
                .Where(m => m.GroupedMessageId == groupedMessageId)
                .ToListAsync();

            var myMessage = messages
                .Where(m => m.ToUserId == me.UserId || m.CreatedBy == me.UserId)
                .FirstOrDefault();

            if (myMessage == null) return NotFound();
            return Ok(messages);
        }

        /// <summary>
        /// 自分宛に届いた全てのメッセージ
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("inbox")]
        public async Task<IActionResult> Inbox()
        {
            var myUserInfo = await Tool.GetMyUserInfoFromClaims(_context, HttpContext);
            if (myUserInfo == null) return StatusCode(403, "No Session.");
            var me = myUserInfo.Value.User;
            if (me == null) return StatusCode(403, "Invalid User.");
            // if (!myUserInfo.Value.isValid) return StatusCode(403, "Role Updated.");

            var messages = await _context.MessageModels
                .Where(m => m.ToUserId == me.UserId)
                .ToListAsync();
            return Ok(messages);
        }

        /// <summary>
        /// あるユーザーから自分宛に届いたメッセージ
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("inbox/{userId:guid}")]
        public async Task<IActionResult> From(Guid userId)
        {
            var myUserInfo = await Tool.GetMyUserInfoFromClaims(_context, HttpContext);
            if (myUserInfo == null) return StatusCode(403, "No Session.");
            var me = myUserInfo.Value.User;
            if (me == null) return StatusCode(403, "Invalid User.");
            // if (!myUserInfo.Value.isValid) return StatusCode(403, "Role Updated.");

            var messages = await _context.MessageModels
                .Where(m => m.ToUserId == me.UserId)
                .Where(m => m.CreatedBy == userId)
                .ToListAsync();
            return Ok(messages);
        }

        /// <summary>
        /// 自分が送った全てのメッセージ
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("sent")]
        public async Task<IActionResult> Sent()
        {
            var myUserInfo = await Tool.GetMyUserInfoFromClaims(_context, HttpContext);
            if (myUserInfo == null) return StatusCode(403, "No Session.");
            var me = myUserInfo.Value.User;
            if (me == null) return StatusCode(403, "Invalid User.");
            // if (!myUserInfo.Value.isValid) return StatusCode(403, "Role Updated.");

            var messages = await _context.MessageModels
                .Where(m => m.CreatedBy == me.UserId)
                .ToListAsync();
            return Ok(messages);
        }

        /// <summary>
        /// あるユーザー宛に自分が送ったメッセージ
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("sent/{userId:guid}")]
        public async Task<IActionResult> To(Guid userId)
        {
            var myUserInfo = await Tool.GetMyUserInfoFromClaims(_context, HttpContext);
            if (myUserInfo == null) return StatusCode(403, "No Session.");
            var me = myUserInfo.Value.User;
            if (me == null) return StatusCode(403, "Invalid User.");
            // if (!myUserInfo.Value.isValid) return StatusCode(403, "Role Updated.");

            var messages = await _context.MessageModels
                .Where(m => m.CreatedBy == me.UserId)
                .Where(m => m.ToUserId == userId)
                .ToListAsync();
            return Ok(messages);
        }

        // POST: api/Auth/Signin
        [HttpPost]
        [IsHigherThan(UserRoles.User)]
        public async Task<IActionResult> Create()
        {
            return NotFound();
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
