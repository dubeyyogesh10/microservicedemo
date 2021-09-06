using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.DTOs;

namespace PlatformService.SyncMessages.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration config)
        {
            this.httpClient = httpClient;
            this.config = config;
        }
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat), Encoding.UTF8, "application/json"
            );

            var result = await httpClient.PostAsync($"{this.config["CommandServiceUri"]}", httpContent).ConfigureAwait(false);

            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine("Send SYNC post successfull");
            }
            else
            {
                Console.WriteLine("Send SYNC post Failed");
            }

        }
    }
}