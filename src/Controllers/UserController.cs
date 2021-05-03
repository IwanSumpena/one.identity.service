using Microsoft.AspNetCore.Http;
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
using System.Security.Claims;
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
                return StatusCode(StatusCodes.Status500InternalServerError, new OneResponse<UserResponse>()
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

        [HttpGet("User/UserClaims")]
        public async Task<ActionResult<OneResponse<List<Claim>>>> GetUserClaims(string id)
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
                var claims = await _userManager.GetClaimsAsync(userIdentity);

                return Ok(new OneResponse<List<Claim>>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Get_Success,
                    Data = claims.ToList()
                });
            }


        }

        [HttpGet("User/GetAccessFailedCount")]
        public async Task<ActionResult<OneResponse<int>>> GetAccessFailedCount(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var identityResult = await _userManager.GetAccessFailedCountAsync(user);
                return Ok(new OneResponse<int>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Get_Success,
                    Data = identityResult
                });
            }

            return NotFound(new OneResponse<int>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpPut("User/ResetLockoutEndDate")]
        public async Task<ActionResult<OneResponse<object>>> ResetLockoutEndDate(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {

                var identityResult = await _userManager.SetLockoutEndDateAsync(user, null);
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Put_Success,
                        Data = identityResult
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult),
                        Data = identityResult
                    });
                }

            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Message_Get_NotFound,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpPost("User/AddClaim")]
        public async Task<ActionResult<OneResponse<object>>> AddClaim([FromBody] UserAddClaimRequest claimRequest)
        {

            var user = await _userManager.FindByIdAsync(claimRequest.UserId);
            if (user != null)
            {

                var identityResult = await _userManager.AddClaimAsync(user, new Claim(claimRequest.ClaimType, claimRequest.ClaimValue));
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Post_Success,
                        Data = identityResult
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult)
                    });
                }

            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpPost("User/AddRole")]
        public async Task<ActionResult<OneResponse<object>>> AddRole([FromBody] UserAddRoleRequest roleReequest)
        {

            var user = await _userManager.FindByIdAsync(roleReequest.UserId);
            if (user != null)
            {

                var identityResult = await _userManager.AddToRoleAsync(user, roleReequest.RoleName.ToUpper());
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<IdentityResult>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Post_Success,
                        Data = identityResult
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult)
                    });
                }

            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpDelete("User/RemovRole")]
        public async Task<ActionResult<OneResponse<object>>> RemoveUserFromRole([FromBody] UserAddRoleRequest roleReequest)
        {

            var user = await _userManager.FindByIdAsync(roleReequest.UserId);
            if (user != null)
            {

                var identityResult = await _userManager.RemoveFromRoleAsync(user, roleReequest.RoleName.ToUpper());
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<IdentityResult>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Deleted_Success,
                        Data = identityResult
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult)
                    });
                }

            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpDelete("User/RemoveClaim")]
        public async Task<ActionResult<OneResponse<object>>> RemoveClaimFromUser([FromBody] UserAddClaimRequest claimRequest)
        {

            var user = await _userManager.FindByIdAsync(claimRequest.UserId);
            if (user != null)
            {
                Claim claim = new Claim(claimRequest.ClaimType, claimRequest.ClaimValue);

                var identityResult = await _userManager.RemoveClaimAsync(user, claim);
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<IdentityResult>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Deleted_Success,
                        Data = identityResult
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult)
                    });
                }

            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpDelete("User")]
        public async Task<ActionResult<OneResponse<object>>> RemoveUser(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var identityResult = await _userManager.DeleteAsync(user);
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<IdentityResult>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Deleted_Success,
                        Data = identityResult
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult)
                    });
                }

            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpGet("User/GetLockoutEndDate")]
        public async Task<ActionResult<OneResponse<object>>> GetLockoutEndDate(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var identityResult = await _userManager.GetLockoutEndDateAsync(user);

                return Ok(new OneResponse<string>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Get_Success,
                    Data = identityResult.ToString()
                });


            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpGet("User/GetLockoutEnabledAsync")]
        public async Task<ActionResult<OneResponse<object>>> GetLockoutEnabledAsync(string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var identityResult = await _userManager.GetLockoutEnabledAsync(user);

                return Ok(new OneResponse<string>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Get_Success,
                    Data = identityResult.ToString()
                });


            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpPut("User/SetLockoutEnabledAsync")]
        public async Task<ActionResult<OneResponse<object>>> SetLockoutEnabledAsync([FromBody] UserSetEnableLockoutRequest userRequest)
        {

            bool status = false;
            switch (userRequest.StatusEnableLockout.ToLower())
            {
                case "true":
                    status = true;
                    break;
                case "false":
                    status = false;
                    break;

                default:
                    return BadRequest(new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = "StatusEnable harus 'true' atau 'false' !"
                    });

            }

            var user = await _userManager.FindByIdAsync(userRequest.UserId);
            if (user != null)
            {
                var identityResult = await _userManager.SetLockoutEnabledAsync(user, status);
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Put_Success
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult)
                    });
                }

            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

        [HttpPut("User/SetLockoutEndDateAsync")]
        public async Task<ActionResult<OneResponse<object>>> SetLockoutEndDateAsync([FromBody] UserSetLockoutEndDateRequest userRequest)
        {

            var user = await _userManager.FindByIdAsync(userRequest.UserId);
            if (user != null)
            {
                var identityResult = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddDays(userRequest.UserLockOutEndDateFormDays));
                if (identityResult.Succeeded)
                {
                    return Ok(new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Success,
                        Message = AppConstans.Response_Message_Put_Success
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new OneResponse<object>()
                    {
                        Status = AppConstans.Response_Status_Failed,
                        Message = Helpers.GetIdentityResultError(identityResult)
                    });
                }
            }

            return NotFound(new OneResponse<object>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = AppConstans.Response_Message_Get_NotFound
            });
        }

    }
}
