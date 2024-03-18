using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace LogicsLib.Services
{
    public class OldAdsService : IHostedService
    {
        public OldAdsService(IServiceScopeFactory serviceScopeFactory)
        {
            _Logics = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogics>();
        }
        private ILogics _Logics;
        private Task DeleteOldAds;
        public async Task StartAsync(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Starting service DOA");
            DeleteOldAds = new Task(() => _Logics.RemoveOldAds(token));
            DeleteOldAds.Start();
            await Task.CompletedTask;
        }
        public async Task StopAsync(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Stopping service DOA");
            await DeleteOldAds;
        }
    }
}
