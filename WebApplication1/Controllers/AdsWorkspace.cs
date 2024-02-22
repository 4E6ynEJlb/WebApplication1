using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdsWorkspace : ControllerBase//Работа с объявлениями
    {
        [HttpPost]
        [Route("Add")]
        public async Task<bool> AddAdvertisement(Advertisement ad)
        {
            return await Logics.TryAddAdvertisement(ad);
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> DeleteAdvertisement(Guid guid)
        {
            return await Logics.TryDeleteAdvertisement(guid);
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<bool> EditAdvertisement(Advertisement ad)
        {
            return await Logics.TryEditAdvertisement(ad);
        }


        [HttpGet]
        [Route("PersonalAds")]
        public async Task<List<Advertisement>> AdsByUser(Guid guid)
        {
            return await Logics.TryGetPersonalAdsList(guid);
        }
    }
}
