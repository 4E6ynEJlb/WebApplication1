using Microsoft.AspNetCore.Mvc;
namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdsWorkspace : ControllerBase//Работа с объявлениями
    {
        public AdsWorkspace(ILogics logics) 
        {
            _Logics = logics;
        }
        private readonly ILogics _Logics;
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddAdvertisement(Advertisement ad)
        {
            return Ok(await _Logics.TryAddAdvertisement(ad));
        }

        [HttpPost]
        [Route("AttachPic")]
        public async Task<IActionResult> AttachPicToAdvertisement(IFormFile? file, Guid adId)
        {
            await _Logics.TryAttachPic(file, adId);
            return Ok();
        }


        [HttpDelete]
        [Route("DetachPic")]
        public async Task<IActionResult> DetachPicFromAdvertisement(Guid adId)
        {
            await _Logics.TryDetachPic(adId);
            return Ok();
        }

        [HttpPut]
        [Route("ResizePic")]/////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> ResizePic(ResizePicArgs args)
        {
            await _Logics.TryResizePic(args);
            return Ok();
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAdvertisement(Guid guid)
        {
            await _Logics.TryDeleteAdvertisement(guid);
            return Ok();
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> EditAdvertisement(Advertisement ad)
        {
            await _Logics.TryEditAdvertisement(ad);
            return Ok();
        }


        [HttpGet]
        [Route("PersonalAds")]
        public async Task<IActionResult> AdsByUser(Guid guid)
        {
            var result = await _Logics.TryGetPersonalAdsList(guid);
            if (result == null)
                return NoContent();
            return Ok(result);
        }
    }
}
