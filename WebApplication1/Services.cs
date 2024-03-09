namespace MyMakler
{
    public class Services : IHostedService
    {
        private void DeleteDetachedPics(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
                {
                try
                {
                    DirectoryInfo drinfo = new DirectoryInfo(Logics.PicsDirectory);
                    FileInfo[] picsInfo = drinfo.GetFiles();
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        foreach (FileInfo picInfo in picsInfo)
                        {
                            if (context.Ads.FirstOrDefault(a => a.PicLink == picInfo.Name) == null)
                            {
                                picInfo.Delete();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("info: ");
                                Console.ResetColor();
                                Console.WriteLine("Pic has been deleted");
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("info: ");
                        Console.ResetColor();
                        Console.WriteLine("End of deletion");
                    }
                    Thread.Sleep(60000);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("info: ");
                    Console.ResetColor();
                    Console.WriteLine("Deletion error");
                }
                if (token.IsCancellationRequested)
                    break;
            }
        }
        private void RemoveOldAds(CancellationToken token)//Удаление устаревших объявлений
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        var removingAds = context.Ads.Where(a => a.DeletionDate <= DateTime.Now);
                        foreach (Advertisement removingAd in removingAds)
                        {
                            context.Ads.Remove(removingAd);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("info: ");
                            Console.ResetColor();
                            Console.WriteLine("Ad has been removed");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("info: ");
                        Console.ResetColor();
                        Console.WriteLine("End of removation");
                    }
                    Thread.Sleep(60000);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("info: ");
                    Console.ResetColor();
                    Console.WriteLine("Removation error");
                }
            }
        }
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
            Task rOA = new Task(() => RemoveOldAds(Token));
            rOA.Start();
            Task dDP = new Task(() => DeleteDetachedPics(Token));
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
