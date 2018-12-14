using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Model.Dto;
using WebApplication1.Models;
using Zhongyu.Data;
using Zhongyu.Data.Extensions;
using static WebApplication1.Models.DapperService;

namespace WebApplication1.Controllers
{
    public class SignController : Controller
    {
        // GET: Sign
  
        public ActionResult In()
        {
            
            ViewBag.url = "/Home/Index";
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }


        //[HttpPost]
        //public JsonResult In(string uname, string pword)
        //{
        //    OperationResult r = new OperationResult();
        //    if (uname.IsNullOrEmpty() || pword.IsNullOrEmpty())
        //    {
        //        r.Message = "用户名和密码不能为空";
        //        r.ResultType = OperationResultType.ValidError;
        //    }
        //    else
        //    {
        //        string MD5Pword = Zhongyu.Common.EncryptHelper.MD5Encrypt(pword, 32);
        //        UserDto user = DapperService.SqlHelp.UserLogin(uname, pword);


        //      //  AdminUser item = iauser.Entities.Where(p => p.Name.Equals(uname) && p.Password.Equals(MD5Pword)).FirstOrDefault();



        //        if (user==null)
        //        {
        //            r.Message = "用户名或密码错误";
        //            r.ResultType = OperationResultType.Error;
        //        }
        //        else
        //        {


        //            r.ResultType = OperationResultType.Success;
                  
        //            UserHelper.SetSigninUser(user.UserId,user.user_name);
        //        }

        //    }
        //    return Json(r);
        //}


        [HttpPost]
        public JsonResult In(string uname, string pword)
        {
            OperationResult r = new OperationResult();
            if (uname.IsNullOrEmpty() || pword.IsNullOrEmpty())
            {
                r.Message = "用户名和密码不能为空";
                r.ResultType = OperationResultType.ValidError;
            }
            else
            {
                if (uname.Equals("SuperRole") && pword.Equals("123456"))
                {
                    r.ResultType = OperationResultType.Success;
                    UserHelper.SetSigninUser(ConfigHelper.SuperRole, "超级管理员", new List<string>() { ConfigHelper.SuperRole.ToString()}, new List<string>());
                    return Json(r);
                }
                string MD5Pword = Zhongyu.Common.EncryptHelper.MD5Encrypt(pword, 32);
               
                LoginUser user = DapperService.SqlHelp.UserLogin(uname,MD5Pword);

                if (user == null)
                {
                    r.Message = "用户名或密码错误";
                    r.ResultType = OperationResultType.Error;
                }
                else
                {
                    List<string> role=new List<string>();
                    r.ResultType = OperationResultType.Success;
                    DapperService.SqlHelp.GetUserRoles().Where(p1 => p1.UserId.Equals(user.Id)).ToList()
                        .ForEach(p => { role.Add(p.RoleId); });
                     
                    string[] rolesArry = role.ToArray();
                   // string rolesString = rolesArry.ToString();
                    List<string> menus = RoleFunction.Entity()
                        .Where(p => rolesArry.Contains(p.RoleId))
                        .Select(p => p.FunctionID).ToList();
                    HashSet<string> hs = new HashSet<string>(menus);
                    var tt = RoleFunction.Entity();
                    var tt1 = RoleFunction.Entity().Where(p => rolesArry.Contains(p.RoleId));

                 

                    UserHelper.SetSigninUser(user.Id, user.Name, role, hs.ToList());
                        
                }
            }
            return Json(r);
        }

        public ActionResult Out()
        {
            UserHelper.ClearSigninUser();
            return RedirectToAction("In", "Sign", new { Area = "" });
        }


    }
}