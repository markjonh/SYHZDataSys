using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Model.Dto;
using Zhongyu.Data;
using Zhongyu.Data.Extensions;
using static WebApplication1.Models.DapperService;

namespace WebApplication1.Controllers
{
    public class PowerController : Controller
    {
        // GET: Power
        public ActionResult Index()
        {
            List<FunctionSimpleInfo> list=new List<FunctionSimpleInfo>();
            ViewBag.Role = SqlHelp.GetRoles();
            Function.GetFunction().ForEach(p =>
            {
                if (p.SonFionctionId==null)
                {
                    FunctionSimpleInfo info = new FunctionSimpleInfo();
                    info.Id = p.FunctionId;
                    info.Name = p.Name;
                    info.Content = p.Content;
                    list.Add(info);
                }
            });
            SonFunction.GetSonFunction().ForEach(p =>
            {
                FunctionSimpleInfo info = new FunctionSimpleInfo();
                info.Id = p.SonFunctionId;
                info.Name = p.Name;
                info.Content = p.Content;
                list.Add(info);
            });
            return View(list);
        }
        [HttpPost]

        public ActionResult GetRolePermission(string id)
        {
           //Role r = irole.GetByKey(id);
            
            List<string> fids = RoleFunction.Entity().Where(p => p.RoleId.Equals(id)).Select(p => p.FunctionID).ToList();
            return Json(new { r = fids });
        }
        [HttpPost]

        public ActionResult SetPermission(FormCollection coll)
        {
            OperationResult r = new OperationResult();
            RoleFunctionDto model=new RoleFunctionDto();
            try
            {
                model.RoleId = coll["role"].ToString().Split(',')[0].CastTo<Guid>().ToString();
                model.FunctionID = coll["function"] == null ? "" : coll["function"].ToString();
                RoleFunction.insert(model);
                r.ResultType = OperationResultType.Success;
            }
            catch
            {
                r.Message = "新增失败，请检查所填数据是否正确。";
                r.ResultType = OperationResultType.Error;
            }

            return Json(r);
        }
    }
}