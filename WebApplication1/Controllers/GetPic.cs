using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{    
    [ApiController]
    [Route("[controller]")]
    public class GetPic : ControllerBase
    {
        public GetPic(ILogics logics)
        {
            _Logics = logics;
        }
        private readonly ILogics _Logics;
        [HttpGet]
        [Route("{picName}")]
        public async Task<PhysicalFileResult> GetPicture(string picName)
        {
            var fileInfo = await _Logics.TryGetPic(picName);
            return PhysicalFile(fileInfo.Item1, fileInfo.Item2, fileInfo.Item3);
        }
    }
}
