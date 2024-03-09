using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetPic : ControllerBase
    {
        [HttpGet]
        [Route("{picName}")]
        public async Task<PhysicalFileResult> GetPicture(string picName)
        {
            var fileInfo = await Logics.TryGetPic(picName);
            return PhysicalFile(fileInfo.Item1, fileInfo.Item2, fileInfo.Item3);
        }
    }
}
