using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Model.Dto;

namespace WebApplication1.Model
{
    public class ResUsersRoleMode
    {
        public List<UsersDto> Users { get; set; }
        public List<RolesDto> Roles { get; set; }
    }
}