using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QRCode.Models.CommonModels;
using System.Net.Http.Json;

namespace QRCode
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(async p =>
            {
                var httpClient = p.GetRequiredService<HttpClient>();
                return await httpClient.GetFromJsonAsync<AppSettings>("AppSettings.json")
                    .ConfigureAwait(false);
            });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:51770") });

            await builder.Build().RunAsync();
        }
    }
}
