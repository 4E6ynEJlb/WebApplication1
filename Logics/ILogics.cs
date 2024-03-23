using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LogicsLib.Logics;

namespace LogicsLib
{
    public interface ILogics
    {
        Task<Guid> TryAddUser(string name, bool isAdmin);//Добавление пользователя
        Task<List<User>> TrySearchUser(string name);//Поиск пользователя по имени (LIKE)
        Task<List<User>> TryGetUsersList(int pageNumber, int pageSize);//Все пользователи
        Task<int> TryGetUsersPagesCount(int pageSize);
        Task TryDeleteUser(Guid guid);//Удаление пользователя по ИД
        Task TryEditUser(User user);//Изменение пользователя
        Task<Guid> TryAddAdvertisement(AdvInput adInput);//Добавление объявления
        Task TryAttachPic(IFormFile file, Guid adId);
        Task TryDetachPic(Guid adId);
        Task TryDeleteAdvertisement(Guid guid); //Удаление объявления независимо от его актуальности
        Task TryEditAdvertisement(Advertisement ad); //Редактирование объявления (с защитой от "нечестного" изменения рейтинга)
        Task<AdsAndPagesCount> TryGetAdsListAndPgCount(GetAllAdsArgs args); //Сортированный список объявлений с необязательным поиском по тексту и фильтром по рейтингу
        Task<List<Advertisement>> TryGetPersonalAdsList(Guid guid);//Поиск объявлений конкретного пользователя (по его ид)
        Task TryChangeRating(Guid guid, RatingChange change);//Изменение (теперь уже "честное") рейтинга на 1 
        Task DeleteDetachedPics(CancellationToken token);
        Task RemoveOldAds(CancellationToken token);//Удаление устаревших объявлений
    }
}
