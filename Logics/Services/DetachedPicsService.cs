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
        public async Task StartAsync(CancellationToken token)//   <*> №11
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Starting service DPS");
            _Logics.DeleteDetachedPics(token);
        }
        public async Task StopAsync(CancellationToken token)//   <*> №11
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Stopping service DPS");
        }
    }
}
