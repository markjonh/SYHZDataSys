using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class SysUserDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
       
        public string LoginName { get; set; }

        /// <summary>
        /// 用户登录密码
        /// </summary>
 
        public string Password { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
    
        public string Name { get; set; }

      
        public string Phone { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public bool Enable { get; set; }

    }
}