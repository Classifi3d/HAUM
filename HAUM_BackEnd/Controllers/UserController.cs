using HAUM_BackEnd.Entities;
using HAUM_BackEnd.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace HAUM_BackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        private string CreateToken(Guid userId)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,userId.ToString())
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddSeconds(6),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            if (users != null)
            {
                return Ok(users);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{userId:guid}")]
        [ActionName("GetUserById")]
        public async Task<IActionResult> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user != null)
            { 
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync([FromBody] UserDTO userDTO)
        {
            var isAdded = await _userRepository.AddUserAsync(userDTO);
            if (isAdded)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserDTO userDTO)
        {
            var isUpdated = await _userRepository.UpdateUserAsync(userDTO);
            if(isUpdated)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{userId:guid}")]
        [ActionName("DeleteById")]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            var isDeleted = await _userRepository.DeleteUserAsync(userId);
            if (isDeleted)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("login")]
        [ActionName("UserLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] UserDTO userDTO)
        {
            Guid userGUID = await _userRepository.LoginAsync(userDTO);
            if(userGUID != Guid.Empty) {
                return Ok(userGUID);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("login_token")]
        [ActionName("UserLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsyncToken([FromBody] UserDTO userDTO)
        {
            Guid userGUID = await _userRepository.LoginAsync(userDTO);
            if (userGUID != Guid.Empty)
            {
                string token = CreateToken(userGUID).ToString();
                return Ok(token);
            }
            else
            {
                return NotFound();
            }
        }
    } 
}
