using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace LogicsLib.Services
{
    public class DetachedPicsService : IHostedService
    {
        public DetachedPicsService(IServiceScopeFactory serviceScopeFactory)
        {
            _Logics = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogics>();
        }
        private ILogics _Logics;
        private Task DeleteDetachedPics;
        public async Task StartAsync(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Starting service DPS");
            DeleteDetachedPics = new Task(() => _Logics.DeleteDetachedPics(token));
            DeleteDetachedPics.Start();
            await Task.CompletedTask;
        }
        public async Task StopAsync(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Stopping service DPS");
            await DeleteDetachedPics;
        }
    }
}
