global using Repos;
using Microsoft.EntityFrameworkCore;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Aspose.Imaging;
using Microsoft.Extensions.Configuration;
using System.Runtime;
namespace LogicsLib
{
    public class Logics : ILogics
    {
        public Logics(ApplicationContext context, IConfiguration configuration)
        {
            Context = context;
            var constsOptions = new ConstsOptions();
            configuration.GetSection(ConstsOptions.ConstsConfiguration).Bind(constsOptions);//   <*> pattern Options (+ check appsettings.json)
            AdsMaxCount = constsOptions.AdsMaxCount;
            AdLifeDays = constsOptions.AdLifeDays;
            TicksCount = constsOptions.TicksCount;
            LinkTemplate = constsOptions.LinkTemplate;//   <*> №8
            PicsDirectory = configuration.GetValue<string>("PicsDirectory");
            EnsureDirectoryCreated();
        }
        private readonly ApplicationContext Context;
        private readonly int AdsMaxCount;//   <*> private
        private readonly int AdLifeDays ;
        private readonly int TicksCount ;
        private readonly string PicsDirectory;
        private readonly string LinkTemplate;
        public enum SortCriteria { Rating, CreationDate }//Критерии сортировки
        public enum RatingChange { up, down }//Возможные изменения рейтинга (+1/-1)
        public void EnsureDirectoryCreated()
        {
            DirectoryInfo drinfo = new DirectoryInfo(PicsDirectory);
            if (!drinfo.Exists)
            {
                drinfo.Create();
            }
        }
        public async Task DeleteDetachedPics(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    DirectoryInfo drinfo = new DirectoryInfo(PicsDirectory);
                    FileInfo[] picsInfo = drinfo.GetFiles();

                    foreach (FileInfo picInfo in picsInfo)
                    {
                        if (await Context.Ads.Where(a => EF.Functions.Like(a.PicLink, $"%/{picInfo.Name}")).CountAsync(token) == 0)//   <*> graceful shutdown
                        {
                            picInfo.Delete();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("info: ");
                            Console.ResetColor();
                            Console.WriteLine("Pic has been deleted");
                        }
                        if (token.IsCancellationRequested)
                            break;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("info: ");
                    Console.ResetColor();
                    Console.WriteLine("End of deletion");

                    await Task.Delay(TicksCount, token);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("info: ");
                    Console.ResetColor();
                    Console.WriteLine("Deletion error");
                }
            }
        }
        public async Task RemoveOldAds(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {

                    var removingAds = await Context.Ads.Where(a => a.DeletionDate <= DateTime.Now).ToListAsync(token);//   <*> gr. SD
                    foreach (Advertisement removingAd in removingAds)
                    {
                        Context.Ads.Remove(removingAd);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("info: ");
                        Console.ResetColor();
                        Console.WriteLine("Ad has been removed");
                        if (token.IsCancellationRequested)
                            break;
                    }
                    await Context.SaveChangesAsync(token);//   <*> savechanges + gr. SD
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("info: ");
                    Console.ResetColor();
                    Console.WriteLine("End of removation");

                    await Task.Delay(TicksCount, token);
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
        public async Task<Guid> TryAddUser(string name, bool isAdmin)
        {
            User user = new User()
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsAdmin = isAdmin
            };
            Context.Users.Add(user);
            await Context.SaveChangesAsync();

            return user.Id;
        }
        public async Task<List<User>> TrySearchUser(string name)
        {
            List<User> result;
            result = await Context.Users.Where(u => EF.Functions.Like(u.Name, $"%{name}%")).ToListAsync();

            return result;
        }
        public async Task<List<User>> TryGetUsersList(int pageNumber, int pageSize)
        {
            List<User> usersList;
            int pagesCount = await Context.Users.CountAsync();
            if (pagesCount == 0)
                pagesCount = 1;
            else pagesCount = pagesCount / pageSize + ((pagesCount % pageSize) == 0 ? 0 : 1);
            if (pageNumber > pagesCount || pageNumber < 1)
                throw new InvalidPageException();
            usersList = await Context.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return usersList;
        }
        public async Task<int> TryGetUsersPagesCount(int pageSize)
        {
            int pagesCount = await Context.Users.CountAsync();

            if (pagesCount == 0)
                return 1;
            return pagesCount / pageSize + ((pagesCount % pageSize) == 0 ? 0 : 1);
        }
        public async Task TryDeleteUser(Guid guid)
        {
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == guid) == null)
                throw new DoesNotExistException(typeof(User));
            User user = new User { Id = guid };
            Context.Users.Attach(user);
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();

        }
        public async Task TryEditUser(User user)
        {
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == user.Id) == null)
                throw new DoesNotExistException(typeof(User));
            Context.Users.Update(user);
            await Context.SaveChangesAsync();

        }
        public async Task<Guid> TryAddAdvertisement(AdvInput adInput)
        {
            Guid adId = Guid.NewGuid();
            int thisUserAdsCount = await Context.Ads.Where(a => a.UserId == adInput.UserId).CountAsync();
            bool isThisUserAdmin = (await Context.Users.FirstOrDefaultAsync(u => u.Id == adInput.UserId)).IsAdmin;
            if (thisUserAdsCount >= AdsMaxCount && !isThisUserAdmin)
            {
                throw new TooManyAdsException();
            }
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == adInput.UserId) == null)
                throw new DoesNotExistException(typeof(User));
            Advertisement ad = new Advertisement()
            {
                User = null,
                Id = adId,
                Rating = 0,
                PicLink = "Empty",
                UserId = adInput.UserId,
                Text = adInput.Text,
                Number = adInput.Number,
                CreationDate = DateTime.Now,
                DeletionDate = DateTime.Now.AddDays(AdLifeDays),
            };
            Context.Ads.Add(ad);
            await Context.SaveChangesAsync();

            return adId;
        }
        public async Task TryAttachPic(IFormFile file, Guid adId)
        {
            Advertisement ad = await Context.Ads.FirstOrDefaultAsync(a => a.Id == adId);
            if (file == null)
                throw new EmptyFileException();
            if (!file.ContentType.Contains("image"))
                throw new InvalidFileFormatException();
            if (ad == null)
                throw new DoesNotExistException(typeof(Advertisement));
            using (FileStream fS = new FileStream(PicsDirectory + "\\" + file.FileName, FileMode.Create))
            {
                await file.CopyToAsync(fS);
            }
            ad.PicLink = LinkTemplate + file.FileName;
            Context.Ads.Update(ad);
            await Context.SaveChangesAsync();

        }
        public async Task TryDetachPic(Guid adId)
        {
            Advertisement ad = await Context.Ads.FirstOrDefaultAsync(a => a.Id == adId);
            if (ad == null)
                throw new DoesNotExistException(typeof(Advertisement));
            ad.PicLink = "Empty";
            Context.Ads.Update(ad);
            await Context.SaveChangesAsync();

        }
        public async Task TryDeleteAdvertisement(Guid guid) 
        {
            Advertisement ad = new Advertisement { Id = guid };
            if (await Context.Ads.FirstOrDefaultAsync(a => a.Id == guid) == null)
                throw new DoesNotExistException(typeof(Advertisement));
            Context.Ads.Attach(ad);
            Context.Ads.Remove(ad);
            await Context.SaveChangesAsync();

        }
        public async Task TryEditAdvertisement(Advertisement ad) 
        {
            Advertisement adInDB = await Context.Ads.FirstOrDefaultAsync(a => a.Id == ad.Id);
            if (adInDB == null)
                throw new DoesNotExistException(typeof(Advertisement));
            ad.Rating = adInDB.Rating;
            ad.PicLink = adInDB.PicLink;
            Context.Ads.Update(ad);
            await Context.SaveChangesAsync();

        }
        public async Task<AdsAndPagesCount> TryGetAdsListAndPgCount(GetAllAdsArgs args) 
        {
            int? ratingHigh = args.RatingHigh;
            int? ratingLow = args.RatingLow;
            string keyWord = args.KeyWord;
            bool isASC = args.IsASC;
            SortCriteria criterion = args.Criterion;
            int pageNumber = args.Page;
            int pageSize = args.PageSize;

            if (ratingHigh.HasValue && ratingLow.HasValue && ratingLow > ratingHigh)
                (ratingLow, ratingHigh) = (ratingHigh, ratingLow);
            IQueryable<Advertisement> ads;
            List<Advertisement> adsList;
            int pagesCount = 0;
            switch (criterion)
            {
                case SortCriteria.Rating:
                    ads = (isASC ? Context.Ads.OrderBy(a => a.Rating) : Context.Ads.OrderByDescending(a => a.Rating));
                    break;
                default:
                    ads = (isASC ? Context.Ads.OrderBy(a => a.CreationDate) : Context.Ads.OrderByDescending(a => a.CreationDate));
                    break;
            }
            if (keyWord != null)
                ads = ads.Where(a => EF.Functions.Like(a.Text, $"%{keyWord}%"));
            if (ratingLow.HasValue)
            {
                ads = ads.Where(a => a.Rating >= ratingLow);
            }
            if (ratingHigh.HasValue)
            {
                ads = ads.Where(a => a.Rating <= ratingHigh);
            }
            pagesCount = await ads.CountAsync();
            if (pagesCount > 0)
                pagesCount = pagesCount / pageSize + ((pagesCount % pageSize) == 0 ? 0 : 1);
            else
                pagesCount = 1;
            if (pageNumber > pagesCount || pageNumber < 1)
                throw new InvalidPageException();
            ads = ads.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            adsList = await ads.ToListAsync();

            return new AdsAndPagesCount { Ads = adsList, PagesCount = pagesCount };
        }
        public async Task<List<Advertisement>> TryGetPersonalAdsList(Guid guid)
        {
            List<Advertisement> adsList;
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == guid) == null)
                throw new DoesNotExistException(typeof(User));
            adsList = await Context.Ads.Where(a => a.UserId == guid).ToListAsync();

            return adsList;
        }
        public async Task TryChangeRating(Guid guid, RatingChange change) 
        {
            Advertisement? ad = await Context.Ads.FirstOrDefaultAsync(a => a.Id == guid);
            if (ad == null)
                throw new DoesNotExistException(typeof(Advertisement));
            switch (change)
            {
                case RatingChange.up:
                    ad.Rating++;
                    break;
                default:
                    ad.Rating--;
                    break;
            }
            Context.Ads.Update(ad);
            await Context.SaveChangesAsync();
        }        
    }
}
