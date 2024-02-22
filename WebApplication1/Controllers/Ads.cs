using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class Ads : ControllerBase//Просмотр объявлений и изменение их рейтинга
    {
        
        [HttpGet]
        [Route("All")]
        public async Task<List<Advertisement>> GetAllAds(Logics.SortCriteria criterion, bool isASC, string? keyWord, int? ratingLow, int? ratingHigh)
        {
            return await Logics.TryGetAdsList(criterion, isASC, keyWord, ratingLow, ratingHigh);
        }


        [HttpPut]
        [Route("Rating")]
        public async Task<bool> ChangeRating(Guid guid, Logics.RatingChange change)
        {
            return await Logics.TryChangeRating(guid, change);
        }        
    }
}
