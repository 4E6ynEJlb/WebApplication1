using Microsoft.AspNetCore.Mvc;

namespace MyMakler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Users : ControllerBase//Работа с пользователями
    {
        
        [HttpPost]
        [Route("Add")]
        public async Task<bool> AddUser(User user)
        {
            return await Logics.TryAddUser(user);
        }


        [HttpGet]
        [Route("Search")]
        public async Task<List<User>> SearchUser(string name)
        {
            return await Logics.TrySearchUser(name);
        }


        [HttpGet]
        [Route("All")]
        public async Task<List<User>> GetAllUsers()
        {
            return await Logics.TryGetUsersList();
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<bool> DeleteUser(Guid guid)
        {
            return await Logics.TryDeleteUser(guid);
        }


        [HttpPut]
        [Route("Edit")]
        public async Task<bool> EditUser(User user)
        {
            return await Logics.TryEditUser(user);
        }        
    }
}

