using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    //public class SigninUser
    //{
    //    public Guid userId { get; set; }

    //    public string userName { get; set; }
    //}
    public class SigninUser
    {
        public SigninUser()
        {
            RoleId = new List<string>();
            Menus = new List<string>();
        }
        public string userId { get; set; }

        public string userName { get; set; }

        public List<string> RoleId { get; set; }

        public List<string> Menus { get; set; }
    }

    public class UserHelper
    {
        private static string sessionName = "WxvetEnterpriseSiteAdmin";

        /// <summary>
        /// 设置系统用户的session
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRoles"></param>
        public static void SetSigninUser(string userId, string uname, List<string> RoleId, List<string> Menus)
        {
            SigninUser luser = new SigninUser() { userId = userId, userName = uname, RoleId = RoleId, Menus = Menus };

            HttpContext.Current.Session[sessionName] = luser;
        }

        /// <summary>
        /// 清除系统用户session
        /// </summary>
        public static void ClearSigninUser()
        {
            HttpContext.Current.Session[sessionName] = null;
        }

        /// <summary>
        /// 获取登录的系统用户对象
        /// </summary>
        /// <returns></returns>
        public static SigninUser GetSigninUser
        {
            get
            {
                if (HttpContext.Current.Session[sessionName] == null)
                    return null;

                return HttpContext.Current.Session[sessionName] as SigninUser;
            }
        }

    }
}