using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Users : ControllerBase//������ � ��������������
    {
        
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddUser(User user)
        {
            return Ok(await Logics.TryAddUser(user));
        }

        
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchUser(string name)
        {
            var result = await Logics.TrySearchUser(name);
            if (result == null)
                return NoContent();
            return Ok(result);
        }

        
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
        {
            var result = await Logics.TryGetUsersList(page, pageSize);
            if (result == null)
                return NoContent();
            return Ok(result);
        }


        [HttpGet]
        [Route("PgCount")]
        public async Task<IActionResult> GetUsersPagesCount(int pageSize = 10)
        {
            return Ok(await Logics.TryGetUsersPagesCount(pageSize));
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteUser(Guid guid)
        {
            await Logics.TryDeleteUser(guid);
            return Ok();
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> EditUser(User user)
        {
            await Logics.TryEditUser(user);
            return Ok();
        }    
    }
}

