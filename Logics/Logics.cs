global using Repos;
using Microsoft.EntityFrameworkCore;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Aspose.Imaging;
namespace LogicsLib
{
    public class Logics : ILogics
    {
        public Logics(ApplicationContext context)
        {
            Context = context;
            EnsureDirectoryCreated();
        }
        private readonly ApplicationContext Context;
        public const int AdsMaxCount = 5;//Максимальное кол-во объявлений для пользователя (игнорируется у админов (это не баг, а фича))
        public const int AdLifeDays = 10;//Время жизни объявления в бд, устаревшие удаляются из бд каждую минуту (60000мс)
        public enum SortCriteria { Rating, CreationDate }//Критерии сортировки
        public enum RatingChange { up, down }//Возможные изменения рейтинга (+1/-1)
        public const string PicsDirectory = "D:\\TestTaskDex\\Pics";
        public static void EnsureDirectoryCreated()
        {
            DirectoryInfo drinfo = new DirectoryInfo(PicsDirectory);
            if (!drinfo.Exists)
            {
                drinfo.Create();
            }
        }
        public void DeleteDetachedPics(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    DirectoryInfo drinfo = new DirectoryInfo(Logics.PicsDirectory);
                    FileInfo[] picsInfo = drinfo.GetFiles();

                    foreach (FileInfo picInfo in picsInfo)
                    {
                        if (Context.Ads.FirstOrDefault(a => a.PicLink == picInfo.Name) == null)
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
        public void RemoveOldAds(CancellationToken token)//Удаление устаревших объявлений
        {
            while (!token.IsCancellationRequested)
            {
                try
                {

                    var removingAds = Context.Ads.Where(a => a.DeletionDate <= DateTime.Now);
                    foreach (Advertisement removingAd in removingAds)
                    {
                        Context.Ads.Remove(removingAd);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("info: ");
                        Console.ResetColor();
                        Console.WriteLine("Ad has been removed");
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("info: ");
                    Console.ResetColor();
                    Console.WriteLine("End of removation");

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
        public async Task<Guid> TryAddUser(User user)//Добавление пользователя
        {
            user.Id = Guid.NewGuid();
            Context.Users.Add(user);
            await Context.SaveChangesAsync();

            return user.Id;
        }
        public async Task<List<User>> TrySearchUser(string name)//Поиск пользователя по имени (LIKE)
        {
            List<User> result;
            result = await Context.Users.Where(u => EF.Functions.Like(u.Name, $"%{name}%")).ToListAsync();

            return result;
        }
        public async Task<List<User>> TryGetUsersList(int pageNumber, int pageSize)//Все пользователи
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
        public async Task TryDeleteUser(Guid guid)//Удаление пользователя по ИД
        {
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == guid) == null)
                throw new DoesNotExistException(typeof(User));
            User user = new User { Id = guid };
            Context.Users.Attach(user);
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();

        }
        public async Task TryEditUser(User user)//Изменение пользователя
        {
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == user.Id) == null)
                throw new DoesNotExistException(typeof(User));
            Context.Users.Update(user);
            await Context.SaveChangesAsync();

        }
        public async Task<Guid> TryAddAdvertisement(Advertisement ad)//Добавление объявления
        {
            Guid adId = Guid.NewGuid();
            int thisUserAdsCount = await Context.Ads.Where(a => a.UserId == ad.UserId).CountAsync();
            bool isThisUserAdmin = (await Context.Users.FirstOrDefaultAsync(u => u.Id == ad.UserId)).IsAdmin;
            if (thisUserAdsCount >= AdsMaxCount && !isThisUserAdmin)
            {
                throw new TooManyAdsException();
            }
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == ad.UserId) == null)
                throw new DoesNotExistException(typeof(User));
            ad.User = null;
            ad.Id = adId;
            ad.Rating = 0;
            ad.PicLink = "Empty";

            ad.CreationDate = DateTime.Now;
            ad.DeletionDate = ad.CreationDate.AddDays(AdLifeDays);
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
            ad.PicLink = file.FileName;
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
        public async Task TryDeleteAdvertisement(Guid guid) //Удаление объявления независимо от его актуальности
        {
            Advertisement ad = new Advertisement { Id = guid };
            if (await Context.Ads.FirstOrDefaultAsync(a => a.Id == guid) == null)
                throw new DoesNotExistException(typeof(Advertisement));
            Context.Ads.Attach(ad);
            Context.Ads.Remove(ad);
            await Context.SaveChangesAsync();

        }
        public async Task TryEditAdvertisement(Advertisement ad) //Редактирование объявления (с защитой от "нечестного" изменения рейтинга)
        {
            Advertisement adInDB = await Context.Ads.FirstOrDefaultAsync(a => a.Id == ad.Id);
            if (adInDB == null)
                throw new DoesNotExistException(typeof(Advertisement));
            ad.Rating = adInDB.Rating;
            ad.PicLink = adInDB.PicLink;
            Context.Ads.Update(ad);
            await Context.SaveChangesAsync();

        }
        public async Task<AdsAndPagesCount> TryGetAdsListAndPgCount(GetAllAdsArgs args) //Сортированный список объявлений с необязательным поиском по тексту и фильтром по рейтингу
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
        public async Task<List<Advertisement>> TryGetPersonalAdsList(Guid guid)//Поиск объявлений конкретного пользователя (по его ид)
        {
            List<Advertisement> adsList;
            if (await Context.Users.FirstOrDefaultAsync(u => u.Id == guid) == null)
                throw new DoesNotExistException(typeof(User));
            adsList = await Context.Ads.Where(a => a.UserId == guid).ToListAsync();

            return adsList;
        }
        public async Task TryChangeRating(Guid guid, RatingChange change)//Изменение (теперь уже "честное") рейтинга на 1 
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

        public async Task<(string, string, string)> TryGetPic(string picName)/////////////////////////////
        {
            if (0 == (await Context.Ads.Where(a => a.PicLink == picName).CountAsync()))
                throw new DoesNotExistException(typeof(File));

            string path = PicsDirectory + "\\" + picName;
            return (path, "image/" + Path.GetExtension(path).Substring(1), picName);
        }
        public async Task TryResizePic(ResizePicArgs args)/////////////////////////////
        {
            if (0 == (await Context.Ads.Where(a => a.PicLink == args.PicName).CountAsync()))
                throw new DoesNotExistException(typeof(File));
            Image image = Image.Load(PicsDirectory + "\\" + args.PicName);
            image.Resize(args.Width, args.Height);
            image.Save();

        }
    }
}
