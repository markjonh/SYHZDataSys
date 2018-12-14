using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class LoginUser
    {
        public string Id { get; set; }


        public string LoginName { get; set; }


        public string Password { get; set; }


        public string Name { get; set; }


        public string Phone { get; set; }


        public bool Enable { get; set; }
    }
}