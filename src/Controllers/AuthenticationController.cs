using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using src.Models.Entities;
using src.Models.RequestModel;
using src.Models.ResponseModel;
using src.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace src.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<UserOne> _userManager;
        private readonly SignInManager<UserOne> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<UserOne> userManager,SignInManager<UserOne> signInManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        string GenerateJWTToken(UserInfoModel userInfo)
        {
            var secret = _configuration["Jwt:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: userInfo.Claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Jwt:expires_token"])),
            signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ActionResult<OneResponse<object>>> Login([FromBody] UserRequest model)
        {

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new OneResponse<object>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = "email atau password kosong"
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {

                return NotFound(new OneResponse<object>()
                {
                    Status = AppConstans.Response_Status_Failed,
                    Message = $"User dengan email {model.Email} tidak ditemukan."
                });
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, true);
            if (result.Succeeded)
            {
                var roleName = await _userManager.GetRolesAsync(user);
                var claimUser = await _userManager.GetClaimsAsync(user);

                claimUser.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserName));
                claimUser.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                foreach (var item in roleName)
                {
                    claimUser.Add(new Claim("role", item));
                }

                var userInfo = new UserInfoModel
                {
                    UserName = user.UserName,
                    Claims = claimUser.ToList()
                };
                var token = GenerateJWTToken(userInfo);

                return Ok(new OneResponse<object>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = "Berhail login",
                    Data = new { Token = token, result }
                });
            }
            else
            {
                return Ok(new OneResponse<object>()
                {
                    Status = AppConstans.Response_Status_Failed,
                    Message = "Gagal Authentication.",
                    Data = new { Token = "", result }
                });
            }


        }
    }
}
