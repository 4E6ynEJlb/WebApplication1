using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Users : ControllerBase//Работа с пользователями
    {
        public Users(ILogics logics)
        {
            _Logics = logics;
        }
        private readonly ILogics _Logics;
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddUser(string name, bool isAdmin)
        {            
            return Ok(await _Logics.TryAddUser(name, isAdmin));
        }
        
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchUser(string name)
        {
            var result = await _Logics.TrySearchUser(name);
            if (result == null)
                return NoContent();
            return Ok(result);
        }

        
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
        {
            var result = await _Logics.TryGetUsersList(page, pageSize);
            if (result == null)
                return NoContent();
            return Ok(result);
        }


        [HttpGet]
        [Route("PgCount")]
        public async Task<IActionResult> GetUsersPagesCount(int pageSize = 10)
        {
            return Ok(await _Logics.TryGetUsersPagesCount(pageSize));
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteUser(Guid guid)
        {
            await _Logics.TryDeleteUser(guid);
            return Ok();
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> EditUser(User user)
        {
            await _Logics.TryEditUser(user);
            return Ok();
        }    
    }
}

