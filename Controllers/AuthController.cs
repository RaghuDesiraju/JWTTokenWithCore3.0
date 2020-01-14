using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTToken.API.Data;
using JWTToken.API.DTO;
using JWTToken.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JWTToken.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config = null;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register(UserForRegisterDTO userForRegisterDTO)//string userName, string password)[FromBody]
        {
            //if we dont use api controller use model state
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            //if(userForRegisterDTO == null || string.IsNullOrWhiteSpace(userForRegisterDTO.UserName) || string.IsNullOrWhiteSpace(userForRegisterDTO.Password))
            //    return BadRequest("User name and password cant be empty");
            //validate request
           userForRegisterDTO.UserName = userForRegisterDTO.UserName.ToLower();

           if(await _repo.UserExists(userForRegisterDTO.UserName))
           {
               return BadRequest("User name already exists");
           }

           var userToCreate = new User
           {
               UserName = userForRegisterDTO.UserName               
           };

           var createdUser = await _repo.Register(userToCreate,userForRegisterDTO.Password);
           //return StatusCode(201);
          return Created("http://example.org/myitem", new { message = userForRegisterDTO.UserName + " has been created" });
        }

         [HttpPost("RequestToken")]
        public async Task<IActionResult>Login(UserForLoginDTO userForLoginDTO)
        {
           //throw new Exception("Raghu test exception");
            var userFromRepo = await _repo.Login(userForLoginDTO.UserName.ToLower(),userForLoginDTO.Password);
            if(userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //use https://jwt.io/
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}