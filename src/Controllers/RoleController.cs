using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using src.Models.Entities;
using src.Models.ResponseModel;
using src.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<RoleOne> _roleManager;

        public RoleController(RoleManager<RoleOne> roleManager)
        {
            _roleManager = roleManager;
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
                    Description=""
                };
                roleData.Add(role);
            }
            return Ok(new OneResponse<IEnumerable<RoleResponse>>()
            {
                Status = AppConstans.Response_Status_Success,
                Message = "Berhasil mengambil data.",
                Data = roleData
            });
        }
    }
}
