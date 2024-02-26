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

        [SSEAction]
        [Route("default")]
        public async Task Get(CancellationToken cancellationToken)
        {
            var userIdBase = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = userIdBase != null
                ? Guid.Parse(userIdBase)
                : null;

            Guid id = _sseContext.AddQueue(userId);
            _sseContext.AddMsg(id, new { message = "connect", id = id.ToString() });

            while (true)
            {
                /*
                 JS側の実装
const eventSource = new EventSource(`/sse/default`);
eventSource.addEventListener('message', e => {
    const data = JSON.parse(e.data);
    console.log(data);
});

eventSource.addEventListener('ping', e => {
    const data = JSON.parse(e.data);
    console.log('ping', new Date(data));
}); 
                 */
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
