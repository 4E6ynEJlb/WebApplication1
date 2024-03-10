using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace LogicsLib
{
    public class Services : IHostedService
    {
        public Services(IServiceScopeFactory serviceScopeFactory) 
        { 
            _Logics = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILogics>(); 
        }
        private ILogics _Logics;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken Token;
        public async Task StartAsync(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Starting services");
            
            cancellationTokenSource = new CancellationTokenSource();
            Token = cancellationTokenSource.Token;
            Task rOA = new Task(() => _Logics.RemoveOldAds(Token));
            rOA.Start();
            Task dDP = new Task(() => _Logics.DeleteDetachedPics(Token));
            dDP.Start();
            await Task.CompletedTask;
        }
        public async Task StopAsync(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("info: ");
            Console.ResetColor();
            Console.WriteLine("Stopping services");
            cancellationTokenSource.Cancel();
            await Task.CompletedTask;
        }
    }
}
