using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class Ads : ControllerBase
    {
        
        [HttpGet]
        [Route("All")]
        public async Task<List<Advertisement>> GetAllAds(Logics.SortCriteria criterion, bool isASC)
        {
            return await Logics.TryGetAdsList(criterion, isASC);
        }


        [HttpPut]
        [Route("Rating")]
        public async Task<bool> ChangeRating(Guid guid, Logics.RatingChange change)
        {
            return await Logics.TryChangeRating(guid, change);
        }
    }
}
