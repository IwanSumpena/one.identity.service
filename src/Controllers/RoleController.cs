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
using System.Threading.Tasks;

namespace src.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<RoleOne> _roleManager;
        private readonly UserManager<UserOne> _userManager;

        public RoleController(RoleManager<RoleOne> roleManager,UserManager<UserOne> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("Role")]
        public async Task<ActionResult<OneResponse<IEnumerable<RoleResponse>>>> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleData = new List<RoleResponse>();
            foreach (var item in roles)
            {
                var role = new RoleResponse
                {
                    Id = item.Id,
                    RoleName = item.Name,
                    Description=item.Description
                };
                roleData.Add(role);
            }
            return Ok(new OneResponse<IEnumerable<RoleResponse>>()
            {
                Status = AppConstans.Response_Status_Success,
                Message = AppConstans.Response_Message_Get_Success,
                Data = roleData
            });
        }

        [HttpGet("Role/ById")]
        public async Task<ActionResult<OneResponse<RoleResponse>>> Role(string id)
        {
            var userIdentity = await _roleManager.FindByIdAsync(id);
            if (userIdentity == null)
            {
                return NotFound(new OneResponse<RoleResponse>()
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
                    UserName = userIdentity.Name
                }
            });
        }

        [HttpGet("Role/UsersInRole")]
        public async Task<ActionResult<OneResponse<List<string>>>> GetUsersInRole(string id)
        {
            var roleIdentity = await _roleManager.FindByIdAsync(id);
            if (roleIdentity == null)
            {
                return NotFound(new OneResponse<UserResponse>()
                {
                    Status = AppConstans.Response_Status_Failed,
                    Message = AppConstans.Response_Message_Get_NotFound
                });
            }
            else
            {
                var users = await _userManager.GetUsersInRoleAsync(roleIdentity.Name);

                return Ok(new OneResponse<List<string>>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Get_Success,
                    Data = users.Select(p => p.UserName).ToList()
                });
            }


        }

        [HttpPost("Role")]
        public async Task<ActionResult<OneResponse<RoleResponse>>> Add([FromBody] RoleRequest roleRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new OneResponse<UserResponse>()
                {
                    Status=AppConstans.Response_Status_Failed,
                    Message = Helpers.GetModelStateError(ModelState)
                });
            }

            var roleCreate = new RoleOne { Name = roleRequest.RoleName, Description = roleRequest.Description };
            var identityResult = await _roleManager.CreateAsync(roleCreate);
            if (identityResult.Succeeded)
            {
                return Ok(new OneResponse<RoleResponse>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Post_Success,
                    Data = new RoleResponse
                    {
                        Id = roleCreate.Id,
                        RoleName = roleCreate.Name,
                        Description = roleCreate.Description
                    }
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError,new OneResponse<RoleResponse>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = Helpers.GetIdentityResultError(identityResult)
            });



        }

        [HttpDelete("Role")]
        public async Task<ActionResult<OneResponse<RoleResponse>>> Remove(string roleName)
        {

            var role =await _roleManager.FindByNameAsync(roleName);
            if (role==null)
            {
                return NotFound(new OneResponse<object>()
                {
                    Status=AppConstans.Response_Status_Failed,
                    Message=AppConstans.Response_Message_Get_NotFound
                });
            }
            var identityResult = await _roleManager.DeleteAsync(role);
            if (identityResult.Succeeded)
            {
                return Ok(new OneResponse<RoleResponse>()
                {
                    Status = AppConstans.Response_Status_Success,
                    Message = AppConstans.Response_Message_Post_Success,
                    Data = new RoleResponse
                    {
                        Id = role.Id,
                        RoleName = role.Name,
                        Description = role.Description
                    }
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new OneResponse<RoleResponse>()
            {
                Status = AppConstans.Response_Status_Failed,
                Message = Helpers.GetIdentityResultError(identityResult)
            });



        }
    }
}
