using Newtonsoft.Json;
using System.Text;

namespace WebApi.Util
{
    public class SendRequest
    {
        public Uri _url;
        public SendRequest(Uri url)
        {
            _url = url;
        }
        public SendRequest(string url)
        {
            _url = new Uri(url);
        }
        public async Task<string> Post(Dictionary<string, string> content)
        {
            var param = new FormUrlEncodedContent(content);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(_url, param);
                var bAry = await response.Content.ReadAsByteArrayAsync();
                var text = Encoding.UTF8.GetString(bAry);
                return text;
            }
        }
        public async Task<T?> Post<T>(Dictionary<string, string> content)
        {
            var text = await Post(content);
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
