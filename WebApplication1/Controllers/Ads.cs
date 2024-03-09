using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class Ads : ControllerBase//Просмотр объявлений и изменение их рейтинга
    {
        
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAllAds(Logics.SortCriteria criterion, bool isASC, string? keyWord, int? ratingLow, int? ratingHigh, int page = 1, int pageSize = 10)
        {
            var result = await Logics.TryGetAdsListAndPgCount(criterion, isASC, keyWord, ratingLow, ratingHigh, page, pageSize);
            if (result.Ads == null)
                return StatusCode((int)HttpStatusCode.NoContent, result.PagesCount);
            return Ok(result);
        }


        [HttpPut]
        [Route("Rating")]
        public async Task<IActionResult> ChangeRating(Guid guid, Logics.RatingChange change)
        {
                await Logics.TryChangeRating(guid, change);
                return Ok();
        }
    }
    public class AdsAndPagesCount
    {
        public List<Advertisement> Ads { get; set; }
        public int PagesCount { get; set; }
    }
}
