using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src.Models.Entities;
using src.Models.RequestModel;
using src.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<UserOne> _userManager;

        public UserController(UserManager<UserOne> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("User/Users")]
        public async Task<ActionResult<OneResponse<IEnumerable<UserResponse>>>> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userData = new List<UserResponse>();
            foreach (var item in users)
            {
                var user = new UserResponse
                {
                    Id = item.Id,
                    UserName = item.UserName
                };
                userData.Add(user);
            }
            return Ok(new OneResponse<IEnumerable<UserResponse>>()
            {
                Status = "SUCCESS",
                Message = "Berhasil mengambil data.",
                Data = userData
            });
        }

        [HttpPost("User/Add")]
        public async Task<ActionResult<OneResponse<UserResponse>>> Add([FromBody] UserRequest userRequest)
        {
            if (string.IsNullOrEmpty(userRequest.Email) || string.IsNullOrEmpty(userRequest.Password))
            {
                return BadRequest(new OneResponse<UserResponse>()
                {
                    Status = "failed",
                    Message = "Data ada yang kosong.",
                    Data = new UserResponse()
                });
            }

            var userCreate = new UserOne { UserName = userRequest.Email, Email = userRequest.Email };
            var identityResult = await _userManager.CreateAsync(userCreate, userRequest.Password);
            if (identityResult.Succeeded)
            {
                return Ok(new OneResponse<UserResponse>()
                {
                    Status = "SUCCESS",
                    Message = "Berhasil mengambil data.",
                    Data = new UserResponse
                    {
                        Id = userCreate.Id,
                        UserName = userCreate.UserName
                    }
                });
            }

            var error = String.Join(" ", identityResult.Errors.Select(p => p.Description).ToArray());

            return BadRequest(new OneResponse<UserResponse>()
            {
                Status = "FIELD",
                Message = error,
                Data = new UserResponse()
            });



        }
    }
}
