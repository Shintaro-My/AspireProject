using Microsoft.AspNetCore.Mvc;
using WebApi.Context;
using System.Net.Sockets;
using System.Net.WebSockets;
using Microsoft.AspNetCore.DataProtection;
using WebApi.Util;
using System.Text;
using Newtonsoft.Json;
using System.Security.Claims;

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
        [Route("default")]
        public async Task Get()
        {
            var userIdBase = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Guid? userId = userIdBase != null
                ? Guid.Parse(userIdBase)
                : null;

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            //Response.Headers.Add("Transfer-Encoding", "chunked");

            Guid id = _sseContext.AddQueue(userId);
            _sseContext.AddMsg(id, new { message = "connect", id = id.ToString() });

            while (true)
            {
                try
                {
                    var text = _sseContext.GetMsg(id);
                    if (text != null)
                    {
                        await Response.WriteAsync("event: message\n");
                        await Response.WriteAsync($"data: {text}");
                        await Response.WriteAsync("\n\n");
                        await Response.Body.FlushAsync();
                    }
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    break;
                }
            }
            await _sseContext.RemoveQueue(id);

        }

        private void Handler(Guid id, string text)
        {
            try
            {
                var data = JsonConvert.DeserializeObject(text);
                Console.WriteLine($"{id}: {text}");
                Console.WriteLine(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
