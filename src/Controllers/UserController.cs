using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src.Models.Entities;
using src.Models.RequestModel;
using src.Models.ResponseModel;
using src.Utilities;
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

        [HttpGet("User")]
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
                Status = AppConstans.Response_Status_Success,
                Message = "Berhasil mengambil data.",
                Data = userData
            });
        }

        [HttpPost("User")]
        public async Task<ActionResult<OneResponse<UserResponse>>> Add([FromBody] UserRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OneResponse<UserResponse>()
                {
                    Message = Helpers.GetModelStateError(ModelState),
                    Data = new UserResponse()
                });
            }

            var userCreate = new UserOne { UserName = userRequest.Email, Email = userRequest.Email };
            var identityResult = await _userManager.CreateAsync(userCreate, userRequest.Password);
            if (!identityResult.Succeeded)
            {
                return BadRequest(new OneResponse<UserResponse>()
                {
                    Message = Helpers.GetIdentityResultError(identityResult),
                    Data = new UserResponse()
                });
            }

            return Ok(new OneResponse<UserResponse>()
            {
                Status = AppConstans.Response_Status_Success,
                Message = "Berhasil mengambil data.",
                Data = new UserResponse
                {
                    Id = userCreate.Id,
                    UserName = userCreate.UserName
                }
            });
            
        }
    }
}
