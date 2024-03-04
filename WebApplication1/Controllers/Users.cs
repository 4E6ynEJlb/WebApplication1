using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Users : ControllerBase//Работа с пользователями
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
        public async Task<IActionResult> GetAllUsers(int page = 1)
        {
            var result = await Logics.TryGetUsersList(page);
            if (result == null)
                return NoContent();
            return Ok(result);
        }


        [HttpGet]
        [Route("PgCount")]
        public async Task<IActionResult> GetUsersPagesCount()
        {
            return Ok(await Logics.TryGetUsersPagesCount());
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

