using src.Models.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace src.Models.ResponseModel
{
    public class UserInfoModel :UserOne
    {
        public List<Claim> Claims { get; set; }
    }
}
