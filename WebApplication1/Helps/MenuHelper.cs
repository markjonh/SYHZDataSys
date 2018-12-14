using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System
{
    public static class MenuHelper
    {
        public static bool CheckMenuPermisson(string MenuId)
        {
            SigninUser user = UserHelper.GetSigninUser;
            if (user.RoleId.Contains(ConfigHelper.SuperRole))
            {
                return true;
            }
            else
            {
                int len = MenuId.Length;
                if (len == 1)
                {
                    var tt = user.Menus.Where(p => p.Substring(0, len).Equals(MenuId)).FirstOrDefault();
                    return !string.IsNullOrEmpty(user.Menus.Where(p => p.Equals(MenuId)).FirstOrDefault());
                }
                else
                {
                    var t = user.Menus.Where(p => p.Equals(MenuId)).FirstOrDefault();
                    return !string.IsNullOrEmpty(user.Menus.Where(p => p.Equals(MenuId)).FirstOrDefault());
                }

            }
        }
    }
}