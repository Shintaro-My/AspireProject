using Microsoft.AspNetCore.Mvc;
using WebApi.Context;
using System.Net.Sockets;
using System.Net.WebSockets;
using Microsoft.AspNetCore.DataProtection;
using WebApi.Util;
using System.Text;
using Newtonsoft.Json;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    public class SSEController : Controller
    {
        private readonly SSEManagerContext _sseContext;

        public SSEController(SSEManagerContext sseContext)
        {
            _sseContext = sseContext;
        }

        /// <summary>
        /// サーバーから動的にクライアントにメッセージを送信するためのアクション
        /// Server-Sent Event実装
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [SSEAction]
        [Route("default")]
        public async Task Get(CancellationToken cancellationToken)
        {
            var userIdBase = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = userIdBase != null
                ? Guid.Parse(userIdBase)
                : null;

            Guid id = _sseContext.AddQueue(userId);
            _sseContext.AddMsg(id, new { type = "connect", message = "Connect!", id = id.ToString() });

            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(200);
                    var message = _sseContext.GetMsg(id);
                    if (message != null)
                    {
                        var check = await SendEvent("message", message);
                        if (!check) break;
                    }
                    else
                    {
                        await SendEvent("ping", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                    }
                }
                catch (OperationCanceledException)
                {
                    await _sseContext.RemoveQueue(id);
                    break;
                }
            }

        }

        /// <summary>
        /// Server-Sent Eventの規格で永続レスポンスに対してメッセージを書き込む
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<bool> SendEvent(string eventName, object message)
        {
            try
            {
                var json = JsonConvert.SerializeObject(message);
                await Response.WriteAsync($"event: {eventName}\n");
                await Response.WriteAsync($"data: {json}");
                await Response.WriteAsync("\n\n");
                await Response.Body.FlushAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }
    }
}
