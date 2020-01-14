using System.Threading.Tasks;
using JWTToken.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTToken.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController:ControllerBase
    {
        private readonly IUserRepository _repo;
        public UsersController(IUserRepository repo)
        {
            _repo = repo;            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetUser(int id)
        {
            var user = await _repo.GetUser(id);          
            return Ok(user);
        }
    }
}