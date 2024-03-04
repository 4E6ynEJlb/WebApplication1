using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdsWorkspace : ControllerBase//Работа с объявлениями
    {
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddAdvertisement(Advertisement ad)
        {
            return Ok(await Logics.TryAddAdvertisement(ad));
        }

        [HttpPost]
        [Route("AttachPic")]
        public async Task<IActionResult> AttachPicToAdvertisement(IFormFile file, Guid adId)
        {
            await Logics.TryAttachPic(file, adId);
            return Ok();
        }


        [HttpDelete]
        [Route("DetachPic")]
        public async Task<IActionResult> AttachPicToAdvertisement(Guid adId)
        {
            await Logics.TryDetachPic(adId);
            return Ok();
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAdvertisement(Guid guid)
        {
            await Logics.TryDeleteAdvertisement(guid);
            return Ok();
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> EditAdvertisement(Advertisement ad)
        {
            await Logics.TryEditAdvertisement(ad);
            return Ok();
        }


        [HttpGet]
        [Route("PersonalAds")]
        public async Task<IActionResult> AdsByUser(Guid guid)
        {
            var result = await Logics.TryGetPersonalAdsList(guid);
            if (result == null)
                return NoContent();
            return Ok(result);
        }
    }
}
