using AspNetMonsters.Blazor.Geolocation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FarmsToFeedUs.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            if (builder.HostEnvironment.IsDevelopment())
                builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("https://p0zchzjvj9.execute-api.eu-west-1.amazonaws.com/") });
            else
                builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<LocationService>();

            await builder.Build().RunAsync();
        }
    }
}
