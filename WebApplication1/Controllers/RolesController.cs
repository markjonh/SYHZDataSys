using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Model.Dto;
using Zhongyu.Data;
using static WebApplication1.Models.DapperService;

namespace WebApplication1.Controllers
{
    public class RolesController : Controller
    {
        // GET: Roles
        public ActionResult Index()
        {
            return View(SqlHelp.GetRoles());
        }
        public ActionResult Create()
        {
            return View();
        }

 
        [HttpPost]
        public JsonResult Create(RolesDto item)
        {
            OperationResult r = new OperationResult();
            try
            {
                item.Id = Guid.NewGuid().ToString();
                Roles.Insert(item);
                r.ResultType = OperationResultType.Success;
            }
            catch(Exception ex)
            {
                r.Message = "新增失败，请检查所填数据是否正确。";
                r.ResultType = OperationResultType.Error;
            }
            return Json(r);
        }


        public ActionResult Edit(string id)
        {
            return View(Roles.GetById(id));
        }


        [HttpPost]
        public JsonResult Edit(RolesDto item)
        {
            OperationResult r = new OperationResult();
            try
            {
                Roles.Update(item);
                r.ResultType = OperationResultType.Success;

            }
            catch
            {
                r.Message = "失败，请检查数据。";
                r.ResultType = OperationResultType.Error;
            }

            return Json(r);
        }


        [HttpPost]
        public JsonResult Delete(string id)
        {
            OperationResult r = new OperationResult();

            try
            {
                Roles.Delete(id);
                r.ResultType = OperationResultType.Success;
            }
            catch
            {
                r.Message = "失败，请检查数据。";
                r.ResultType = OperationResultType.Error;
            }

            return Json(r);
        }




    }
}