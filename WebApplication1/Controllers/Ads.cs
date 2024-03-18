using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.Swagger.Annotations;
namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]    
    public class Ads : ControllerBase//Просмотр объявлений и изменение их рейтинга
    {
        public Ads(ILogics logics)
        {
            _Logics = logics;
        }
        private readonly ILogics _Logics;
        [HttpOptions]
        [Route("All")]
        public async Task<IActionResult> GetAllAds(GetAllAdsArgs args)
        {
            var result = await _Logics.TryGetAdsListAndPgCount(args);
            if (result.Ads == null)
                return StatusCode((int)HttpStatusCode.NoContent, result.PagesCount);
            return Ok(result);
        }


        [HttpPut]
        [Route("Rating")]
        public async Task<IActionResult> ChangeRating(Guid guid, Logics.RatingChange change)
        {
                await _Logics.TryChangeRating(guid, change);
                return Ok();
        }

    }
}
