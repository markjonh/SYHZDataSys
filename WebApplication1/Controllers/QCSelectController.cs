using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;
using Zhongyu.Data.Extensions;

namespace WebApplication1.Controllers
{
    public class QCSelectController : Controller
    {
        public static List<QCSelectDto> list = new List<QCSelectDto>();
        // GET: QCSelect
        public ActionResult Index(string Area, int? PageIndex, int? PageSize = 30)
        {
            ViewBag.Line = CommonHelp.list;
            // if (!string.IsNullOrEmpty(Request.QueryString["Area"]))
            // Session["Area"] = "123";//Request.QueryString["Area"];
            // string area = Session["Area"].ToString();
            // ViewBag.title= Session["Area"].ToString();
         
            if (!Area.IsNullOrEmpty())
            {
                list = DapperService.SqlHelp.QCSelect(Area);
                foreach (var item in list)
                {
                    item.area = Area;
                }
                ViewBag.list = list.Take(30).ToList();
                ViewBag.totalcount = list.Count;
               // return Json(list, JsonRequestBehavior.AllowGet);
            }
            if (PageIndex != null)
            {
                var model1 = new PageInfoModel<QCSelectDto>();
                model1.List=list;
                model1.PageSize = PageSize;
                model1.PageIndex = PageIndex;
                var res = CommonHelp.PageList(model1);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
         

           
            return View();
        }
    }
}