using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Net.Mime;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace MyMakler
{
    public static class Logics
    {
        public const int AdsMaxCount = 5;//Максимальное кол-во объявлений для пользователя (игнорируется у админов (это не баг, а фича))
        public const int AdLifeDays = 10;//Время жизни объявления в бд, устаревшие удаляются из бд каждую минуту (60000мс)
        public enum SortCriteria { Rating, CreationDate }//Критерии сортировки
        public enum RatingChange { up, down }//Возможные изменения рейтинга (+1/-1)
        public static void RemoveOldAds()//Удаление устаревших объявлений
        {
            while (true)
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        var removingAds = context.Ads.Where(a => a.DeletionDate <= DateTime.Now);
                        foreach (Advertisement removingAd in removingAds)
                        {
                            context.Ads.Remove(removingAd);
                            Console.WriteLine("Ad has been removed");
                        }
                        Console.WriteLine("End of removation");
                    }
                    Thread.Sleep(60000);
                }
                catch
                {
                    Console.WriteLine("Removation error");
                }
            }
        }
        public static async Task<bool> TryAddUser(User user)//Добавление пользователя
        {
            bool isSuccessful = true;
            Task task = new Task(() =>
            {
                try
                {
                    user.Id = Guid.NewGuid();
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                }
                catch
                {
                    isSuccessful = false;
                }
            });    
            task.Start();
            await task;
            return isSuccessful;
        }
        public static async Task<List<User>> TrySearchUser(string name)//Поиск пользователя по имени (LIKE)
        {
            List<User> usersList = new List<User>();
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        var result = context.Users.Where(u =>  EF.Functions.Like(u.Name, $"%{name}%"));                        
                        foreach (User user in result)
                        {
                            usersList.Add(user);
                        }                        
                    }
                }
                catch
                {
                    usersList = null;
                }
            });
            task.Start();
            await task;
            return usersList;
        }
        public static async Task<List<User>> TryGetUsersList()//Все пользователи
        {
            List<User> usersList = new List<User>();
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        usersList = context.Users.ToList();
                    }
                }
                catch
                {
                    usersList = null;
                }
            });
            task.Start();
            await task;
            return usersList;
        }
        public static async Task<bool> TryDeleteUser(Guid guid)//Удаление пользователя по ИД
        {
            bool isSuccessful = true;
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        User user = new User { Id = guid };
                        context.Users.Attach(user);
                        context.Users.Remove(user);
                        context.SaveChanges();
                    }
                }
                catch
                {
                    isSuccessful = false;
                }
            });
            task.Start();
            await task;
            return isSuccessful;
        }
        public static async Task<bool> TryEditUser(User user)//Изменение пользователя
        {
            bool isSuccessful = true;
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        context.Users.Update(user);
                        context.SaveChanges();
                    }
                }
                catch
                {
                    isSuccessful = false;
                }
            });
            task.Start();
            await task;
            return isSuccessful;
        }
        public static async Task<bool> TryAddAdvertisement(Advertisement ad)//Добавление объявления
        {
            bool isSuccessful = true;
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        var thisUserAds = context.Ads.Where(a => a.UserId == ad.UserId);
                        bool isThisUserAdmin = context.Users.FirstOrDefault(u => u.Id == ad.UserId).IsAdmin;
                        if (thisUserAds.Count<Advertisement>() >= AdsMaxCount && !isThisUserAdmin)
                        {
                            isSuccessful = false;
                        }
                        else
                        {
                            ad.User = null;
                            ad.Id = Guid.NewGuid();
                            ad.Rating = 0;
                            ad.CreationDate = DateTime.Now;
                            ad.DeletionDate = ad.CreationDate.AddDays(AdLifeDays);
                            context.Ads.Add(ad);
                            context.SaveChanges();
                        }
                    }
                }
                catch
                {
                    isSuccessful = false;
                }
            });
            task.Start();
            await task;
            return isSuccessful;
        }
        public static async Task<bool> TryDeleteAdvertisement(Guid guid) //Удаление объявления независимо от его актуальности
        {
            bool isSuccessful = true;
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        Advertisement ad = new Advertisement { Id = guid };
                        context.Ads.Attach(ad);
                        context.Ads.Remove(ad);
                        context.SaveChanges();
                    }
                }
                catch
                {
                    isSuccessful = false;
                }
            });
            task.Start();
            await task;
            return isSuccessful;
        }
        public static async Task<bool> TryEditAdvertisement(Advertisement ad) //Редактирование объявления (с защитой от "нечестного" изменения рейтинга)
        {
            bool isSuccessful = true;
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        Advertisement adInDB = context.Ads.FirstOrDefault(a => a.Id == ad.Id);
                        ad.Rating = adInDB.Rating;
                        context.Ads.Update(ad);
                        context.SaveChanges();
                    }
                }
                catch
                {
                    isSuccessful = false;
                }
            });
            task.Start();
            await task;
            return isSuccessful;
        }
        public static async Task<List<Advertisement>> TryGetAdsList(SortCriteria criterion, bool isASC, string keyWord, int? ratingLow, int? ratingHigh) //Сортированный список объявлений с необязательным поиском по тексту и фильтром по рейтингу
        {
            if (ratingHigh.HasValue && ratingLow.HasValue && ratingLow > ratingHigh)
                (ratingLow, ratingHigh) = (ratingHigh, ratingLow);
            List<Advertisement> adsList = new List<Advertisement>();
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        switch (criterion)
                        {
                            case SortCriteria.Rating:
                                adsList = (isASC? context.Ads.OrderBy(a => a.Rating): context.Ads.OrderByDescending(a => a.Rating)).ToList();
                                break;
                            default: 
                                adsList = (isASC ? context.Ads.OrderBy(a => a.CreationDate) : context.Ads.OrderByDescending(a => a.CreationDate)).ToList();
                                break;
                        }
                    }
                    if (keyWord != null)
                        adsList = adsList.Where(a => EF.Functions.Like(a.Text, $"%{keyWord}%")).ToList();
                    if (ratingLow.HasValue)
                    {
                        adsList = adsList.Where(a => a.Rating>=ratingLow).ToList();
                    }
                    if (ratingHigh.HasValue)
                    {
                        adsList = adsList.Where(a => a.Rating <= ratingHigh).ToList();
                    }
                }
                catch
                {
                    adsList = null;
                }
            });
            task.Start();
            await task;
            return adsList;
        }
        public static async Task<List<Advertisement>> TryGetPersonalAdsList(Guid guid)//Поиск объявлений конкретного пользователя (по его ид)
        {
            List<Advertisement> adsList = new List<Advertisement>();
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        adsList = context.Ads.Where(a => a.UserId == guid).ToList();
                    }
                }
                catch
                {
                    adsList = null;
                }
            });
            task.Start();
            await task;
            return adsList;
        }
        public static async Task<bool> TryChangeRating(Guid guid, RatingChange change)//Изменение (теперь уже "честное") рейтинга на 1 
        {
            bool isSuccessful = true;
            Task task = new Task(() =>
            {
                try
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        Advertisement ad = context.Ads.FirstOrDefault(a => a.Id == guid);
                        switch(change)
                        {
                            case RatingChange.up:
                                ad.Rating++;
                                break;
                            default:
                                ad.Rating--;
                                break;
                        }
                        context.Ads.Update(ad);
                        context.SaveChanges();
                    }
                }
                catch
                {
                    isSuccessful = false;
                }
            });
            task.Start();
            await task;
            return isSuccessful;
        }        
    }
}
