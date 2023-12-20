﻿using MaaranTicketingSystemAPI.Context;
using MaaranTicketingSystemAPI.Helpers;
using MaaranTicketingSystemAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                return Ok(new {Success=false, Message = "User Not Found!" });
            }
           if(!PasswordHasher.VerifyPassword(userObj.password,user.password))
            {
                return Ok(new { Message = "Password is incorrect" });
            }
            
            
            user.Token = CreateJwt(user);
            return Ok(new
            {
                Token = user.Token,
                Success = true,Message = "Login Success!",user
            });
        }



        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            if(await CheckUserNameExistAsync(userObj.username)) 
            {
                return Ok(new { Message = "UserName Already Exists" });
            
            }
            
            
            if (userObj!=null)
            {

         

                userObj.password = PasswordHasher.HashPassword(userObj.password);
                userObj.Role = "user";
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
            else {
                return Ok(new
                {
                    Success = false,
                    Message = "user Not Register",
                   
                });

            }
        }


        [HttpGet("getAllUsers")]
        public async Task <ActionResult<User>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }
        private Task<bool> CheckUserNameExistAsync(string username)
       => _authContext.Users.AnyAsync(x=>x.username == username);



        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryveryveryveryveryveryveryverysecret.............");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role,user.Role),
                new Claim(ClaimTypes.Name,$"{user.firstname}{user.lastname}"),
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
    }
}
