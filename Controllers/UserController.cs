using MaaranTicketingSystemAPI.Context;
using MaaranTicketingSystemAPI.Helpers;
using MaaranTicketingSystemAPI.Models;
using MaaranTicketingSystemAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MaaranTicketingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;

        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }



            var user = await _authContext.Users.FirstOrDefaultAsync(x => x.username == userObj.username);
            if (user == null)
            {
                return Ok(new { Success = false, Message = "User Not Found!" });
            }
            if (!PasswordHasher.VerifyPassword(userObj.password, user.password))
            {
                return Ok(new { Message = "Password is incorrect" });
            }


            user.Token = CreateJwt(user);
            var newAccessToken = user.Token;
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _authContext.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Message = "Login Success"
            });
        }



        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            if (await CheckUserNameExistAsync(userObj.username))
            {
                return Ok(new { Message = "UserName Already Exists" });

            }


            if (userObj != null)
            {
                userObj.password = PasswordHasher.HashPassword(userObj.password);
                //userObj.Role = " ";
                userObj.Token = "";
                await _authContext.Users.AddAsync(userObj);
                await _authContext.SaveChangesAsync();


                return Ok(new
                {
                    Success = true,
                    Message = "user Register Successfully",
                    userObj
                });
            }
            else
            {
                return Ok(new
                {
                    Success = false,
                    Message = "user Not Register",

                });

            }
        }


        [HttpGet("getAllUsers")]
        public async Task<ActionResult<User>> GetAllUsers()
            //var usersWithDepartments = _context.Users
            //.Include(u => u.Department)
            //.ToList();

        {
            return Ok(await _authContext.Users.ToListAsync());
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private Task<bool> CheckUserNameExistAsync(string username)
       => _authContext.Users.AnyAsync(x => x.username == username);


        [ApiExplorerSettings(IgnoreApi = true)]
        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryveryveryveryveryveryveryverysecret.............");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Name,$"{user.username}"),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        [ApiExplorerSettings(IgnoreApi = true)]

        public string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);
            var tokenInUser = _authContext.Users.Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryveryveryveryveryveryveryverysecret.............");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken != null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("this is invalid Token");
            return principal;
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiDto tokenApiDto)
        {
            if (tokenApiDto is null)
                return BadRequest("Invalid Client Request");
            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;
            var principal = GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.username == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _authContext.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });

        }

        [HttpPost("logout")]
        [Authorize] // Ensure only authenticated users can log out
        public IActionResult Logout()
        {
         
            return Ok(new { message = "Logout successful" });
        }

        ////////////////////////////////TICKET-CREATION/////////////////////////

        [HttpGet("getAllDepartments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _authContext.Department.ToListAsync();
            return Ok(departments);
        }
    }
}