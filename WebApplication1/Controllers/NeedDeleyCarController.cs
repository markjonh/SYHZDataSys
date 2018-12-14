using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class NeedDeleyCarController : Controller
    {
        // GET: NeedDeleyCar
        public ActionResult Index()
        {
            ViewBag.Line = CommonHelp.list;
            return View();
        }

        public JsonResult NeedDeley(ThroughRateModel model)
        {
            var Successed = true;
            var Message = "";
            ResNeedDeleyListDto list = new ResNeedDeleyListDto();
            try
            {
                list=DapperService.SqlHelp.NeedDeley(model);
                Message = "成功!";
            }
            catch (Exception e)
            {
                Successed = false;
                Message = "失败！";
            }

            return Json(new {listnd = list, Success = Successed, Mess = Message}, JsonRequestBehavior.AllowGet);
        }
    }
}