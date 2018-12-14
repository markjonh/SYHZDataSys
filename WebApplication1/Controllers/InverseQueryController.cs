
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Common;
using WebApplication1.Filters;
using WebApplication1.Model.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Signin]
    public class InverseQueryController : Controller
    {
        // GET: InverseQuery
        public ActionResult Index(string code,string area)
        {
            ViewBag.Line = CommonHelp.list;
            ViewBag.count = 0;
            var Successed = true;
            var Message = "";
            List<ResAssemblyBarCode> list=new List<ResAssemblyBarCode>();
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(area))
            {
                try
                {
                    
                    list = DapperService.SqlHelp.GetAssemblyByPartCode(code, area);
                    var checkcount = list.First().COUNT; 
                    if (checkcount>0)
                    {
                        Successed = false;
                        Message = "查询量较大，请输入条码后16位！";
                    }
                    foreach (var item in list)
                    {
                        item.area = area;
                    }
                }
                catch (Exception e)
                {
                    Successed = false;
                    Message = "数据查询有误！";
                }
                return Json(new
                {
                    list1 = list,
                    totalcount = list.Count,
                    success = Successed,
                    mess = Message
                }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }


    }

    
}