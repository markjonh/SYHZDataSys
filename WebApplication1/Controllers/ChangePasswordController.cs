using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Model.Dto;
using Zhongyu.Data;
using static WebApplication1.Models.DapperService;
using static WebApplication1.Models.DapperService.Roles;

namespace WebApplication1.Controllers
{
    public class ChangePasswordController : Controller
    {
        // GET: ChangePassword
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Index(string oldp, string newp)
        {
            OperationResult r = new OperationResult();

            SysUserDto u = SysUser.GetById(UserHelper.GetSigninUser.userId);
            if (u.Password.Equals(Zhongyu.Common.EncryptHelper.MD5Encrypt(oldp, 32)))
            {
                SysUser.ChangePassword(UserHelper.GetSigninUser.userId, Zhongyu.Common.EncryptHelper.MD5Encrypt(newp, 32));
                r.ResultType = OperationResultType.Success;
            }
            else
            {
                r.Message = "原始密码不正确。";
                r.ResultType = OperationResultType.ValidError;
            }
            return Json(r);
        }
    }
}