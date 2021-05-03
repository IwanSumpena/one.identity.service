using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src.Models.Entities;
using src.Models.RequestModel;
using src.Models.ResponseModel;
using src.Utilities;
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
                Message = AppConstans.Response_Message_Get_Success,
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
                Message = AppConstans.Response_Message_Post_Success,
                Data = new UserResponse
                {
                    Id = userCreate.Id,
                    UserName = userCreate.UserName
                }
            });
            
        }

        [HttpGet("User/ById")]
        public async Task<ActionResult<OneResponse<UserResponse>>> ById(string id)
        {
            var userIdentity = await _userManager.FindByIdAsync(id);
            if (userIdentity == null)
            {
                return NotFound(new OneResponse<UserResponse>()
                {
                    Status = AppConstans.Response_Status_Failed,
                    Message = AppConstans.Response_Message_Get_NotFound
                });
            }

            return Ok(new OneResponse<UserResponse>()
            {
                Status = AppConstans.Response_Status_Success,
                Message = AppConstans.Response_Message_Get_Success,
                Data = new UserResponse
                {
                    Id = userIdentity.Id,
                    UserName = userIdentity.UserName
                }
            });
        }

        [HttpGet("User/UserRoles")]
        public async Task<ActionResult<OneResponse<List<string>>>> GetUserRoles(string id)
        {
            var userIdentity = await _userManager.FindByIdAsync(id);
            if (userIdentity == null)
            {
                return NotFound(new OneResponse<UserResponse>()
                {
                    Status = AppConstans.Response_Status_Failed,
                    Message = AppConstans.Response_Message_Get_NotFound,
                    Data = new UserResponse()
                });
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(userIdentity);

                return Ok(new OneResponse<List<string>>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Get_Success,
                    Data = roles.ToList()
                });
            }
        }
    }
}
