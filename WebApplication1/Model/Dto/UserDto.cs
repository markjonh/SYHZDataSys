using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class UserDto
    {
        public string user_name { get; set; }
        public string password { get; set; }
        public string  userid { get; set; }

        public Guid UserId
        {
            get
            {
                return new Guid(userid);
            }

        }

    }
}