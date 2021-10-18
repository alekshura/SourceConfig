using Compentio.SourceConfig.App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Compentio.SourceConfig.App
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            var notesService = serviceScope.ServiceProvider.GetRequiredService<INotesService>();
            var result = notesService.GetNote(1);
            Console.WriteLine($"Note: '{result}'");
            Console.ReadKey();
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services
                    .Configure<AppSettings>(configuration)
                    .AddTransient<INotesService, NotesService>());
        }
    }
}
