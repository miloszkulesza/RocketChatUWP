using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RocketChatUWP.Core.Services
{
    public class AvatarsService : IAvatarsService
    {
        public async Task<ImageSource> GetAvatar(string address)
        {
            using(var client = new HttpClient())
            {
                var response = await client.GetAsync(address);
                string content = await response.Content.ReadAsStringAsync();
                if(content.Substring(0, 4).Equals("<svg"))
                {
                    var svg = new SvgImageSource(new Uri(address));
                    return svg;
                }
                return new BitmapImage();
            }
        }
    }
}
