using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMakler.Controllers;
using System.ComponentModel;
using System.Net.Mime;
using Aspose.Imaging;
namespace MyMakler
{
    public static class Logics
    {
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
        
        public static async Task<Guid> TryAddUser(User user)//Добавление пользователя
        {
            user.Id = Guid.NewGuid();
            using (ApplicationContext context = new ApplicationContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
            return user.Id;
        }
        public static async Task<List<User>> TrySearchUser(string name)//Поиск пользователя по имени (LIKE)
        {
            List<User> result;
            using (ApplicationContext context = new ApplicationContext())
            {
                result = await context.Users.Where(u => EF.Functions.Like(u.Name, $"%{name}%")).ToListAsync();
            }
            return result;
        }
        public static async Task<List<User>> TryGetUsersList(int pageNumber, int pageSize)//Все пользователи
        {
            List<User> usersList;
            using (ApplicationContext context = new ApplicationContext())
            {
                int pagesCount = await context.Users.CountAsync();
                if (pagesCount == 0)
                    pagesCount = 1;
                else pagesCount = pagesCount / pageSize + ((pagesCount % pageSize) == 0 ? 0 : 1);
                if (pageNumber > pagesCount || pageNumber < 1)
                    throw new InvalidPageException();
                usersList = await context.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return usersList;
        }
        public static async Task<int> TryGetUsersPagesCount(int pageSize)
        {
            int pagesCount;
            using (ApplicationContext context = new ApplicationContext())
            {
                pagesCount = await context.Users.CountAsync();
            }
            if (pagesCount == 0)
                return 1;
            return pagesCount / pageSize + ((pagesCount % pageSize) == 0 ? 0 : 1);
        }
        public static async Task TryDeleteUser(Guid guid)//Удаление пользователя по ИД
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if (await context.Users.FirstOrDefaultAsync(u => u.Id == guid) == null)
                    throw new DoesNotExistException(typeof(User));
                User user = new User { Id = guid };
                context.Users.Attach(user);
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }
        public static async Task TryEditUser(User user)//Изменение пользователя
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if (await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id) == null)
                    throw new DoesNotExistException(typeof(User));
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
        }
        public static async Task<Guid> TryAddAdvertisement(Advertisement ad)//Добавление объявления
        {
            Guid adId = Guid.NewGuid();
            using (ApplicationContext context = new ApplicationContext())
            {
                int thisUserAdsCount = await context.Ads.Where(a => a.UserId == ad.UserId).CountAsync();
                bool isThisUserAdmin = (await context.Users.FirstOrDefaultAsync(u => u.Id == ad.UserId)).IsAdmin;
                if (thisUserAdsCount >= AdsMaxCount && !isThisUserAdmin)
                {
                    throw new TooManyAdsException();
                }
                if (await context.Users.FirstOrDefaultAsync(u => u.Id == ad.UserId) == null)
                    throw new DoesNotExistException(typeof(User));
                ad.User = null;
                ad.Id = adId;
                ad.Rating = 0;
                ad.PicLink = "Empty";
                
                ad.CreationDate = DateTime.Now;
                ad.DeletionDate = ad.CreationDate.AddDays(AdLifeDays);
                context.Ads.Add(ad);
                await context.SaveChangesAsync();
            }
            return adId;
        }
        public static async Task TryAttachPic(IFormFile file, Guid adId)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Advertisement ad = await context.Ads.FirstOrDefaultAsync(a => a.Id == adId);
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
                context.Ads.Update(ad);
                await context.SaveChangesAsync();
            }
        }
        public static async Task TryDetachPic(Guid adId)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Advertisement ad = await context.Ads.FirstOrDefaultAsync(a => a.Id == adId);
                if (ad == null)
                    throw new DoesNotExistException(typeof(Advertisement));
                ad.PicLink = "Empty";
                context.Ads.Update(ad);
                await context.SaveChangesAsync();
            }
        }
        public static async Task TryDeleteAdvertisement(Guid guid) //Удаление объявления независимо от его актуальности
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Advertisement ad = new Advertisement { Id = guid };
                if (await context.Ads.FirstOrDefaultAsync(a => a.Id == guid) == null)
                    throw new DoesNotExistException(typeof(Advertisement));
                context.Ads.Attach(ad);
                context.Ads.Remove(ad);
                await context.SaveChangesAsync();
            }
        }
        public static async Task TryEditAdvertisement(Advertisement ad) //Редактирование объявления (с защитой от "нечестного" изменения рейтинга)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Advertisement adInDB = await context.Ads.FirstOrDefaultAsync(a => a.Id == ad.Id);
                if (adInDB == null)
                    throw new DoesNotExistException(typeof(Advertisement));
                ad.Rating = adInDB.Rating;
                ad.PicLink = adInDB.PicLink;
                context.Ads.Update(ad);
                await context.SaveChangesAsync();
            }
        }
        public static async Task<AdsAndPagesCount> TryGetAdsListAndPgCount(SortCriteria criterion, bool isASC, string keyWord, int? ratingLow, int? ratingHigh, int pageNumber, int pageSize) //Сортированный список объявлений с необязательным поиском по тексту и фильтром по рейтингу
        {
            if (ratingHigh.HasValue && ratingLow.HasValue && ratingLow > ratingHigh)
                (ratingLow, ratingHigh) = (ratingHigh, ratingLow);
            IQueryable<Advertisement> ads;
            List<Advertisement> adsList;
            int pagesCount = 0;
            using (ApplicationContext context = new ApplicationContext())
            {
                switch (criterion)
                {
                    case SortCriteria.Rating:
                        ads = (isASC ? context.Ads.OrderBy(a => a.Rating) : context.Ads.OrderByDescending(a => a.Rating));
                        break;
                    default:
                        ads = (isASC ? context.Ads.OrderBy(a => a.CreationDate) : context.Ads.OrderByDescending(a => a.CreationDate));
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
            }
            return new AdsAndPagesCount { Ads = adsList, PagesCount = pagesCount };
        }
        public static async Task<List<Advertisement>> TryGetPersonalAdsList(Guid guid)//Поиск объявлений конкретного пользователя (по его ид)
        {
            List<Advertisement> adsList;

            using (ApplicationContext context = new ApplicationContext())
            {
                if (await context.Users.FirstOrDefaultAsync(u => u.Id == guid) == null)
                    throw new DoesNotExistException(typeof(User));
                adsList = await context.Ads.Where(a => a.UserId == guid).ToListAsync();
            }
            return adsList;
        }
        public static async Task TryChangeRating(Guid guid, RatingChange change)//Изменение (теперь уже "честное") рейтинга на 1 
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                Advertisement? ad = await context.Ads.FirstOrDefaultAsync(a => a.Id == guid);
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
                context.Ads.Update(ad);
                await context.SaveChangesAsync();
            }
        }
        public static async Task<(string, string, string)> TryGetPic(string picName)/////////////////////////////
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if (0 == (await context.Ads.Where(a => a.PicLink == picName).CountAsync()))
                    throw new DoesNotExistException(typeof(File));
            }
            string path = PicsDirectory + "\\" + picName;
            return (path, "image/" + Path.GetExtension(path).Substring(1), picName);
        }
        public static async Task TryResizePic(int height, int width, string picName)/////////////////////////////
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if (0 == (await context.Ads.Where(a => a.PicLink == picName).CountAsync()))
                    throw new DoesNotExistException(typeof(File));
                Image image = Image.Load(PicsDirectory + "\\" + picName);
                image.Resize(height, width);
                image.Save();
            }
        }
    }
}
