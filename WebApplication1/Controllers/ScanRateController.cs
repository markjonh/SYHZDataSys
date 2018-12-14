using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using WebApplication1.BufferMemory;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ScanRateController : Controller
    {
        // GET: ScanRate
        public ActionResult Index(PartDataModel model, int? PageIndex, int? PageSize = 30)
        {
            try
            {
                List<ResScnRateDto> listend=new List<ResScnRateDto>();
          
                ViewBag.totalcount = 0;
                ViewBag.Line = CommonHelp.list;
                if (PageIndex == null && model.StartTime < model.EndTime && !string.IsNullOrEmpty(model.Line))
                {

                    string key = Guid.NewGuid().ToString() + model.Line;
                    CommonHelp.key = key;
                    if (CommonBuffer.Exist(key))
                    {
                        CommonHelp.Listone <ResScnRateDto>.List  = CommonBuffer.Get<List<ResScnRateDto>>(key);
                        ViewBag.count = CommonHelp.Listone<ResScnRateDto>.List.Count;
                        ViewBag.Data = CommonHelp.Listone<ResScnRateDto>.List.Take(30).ToList();
                    }
                    else
                    {
                        CommonHelp.Listone<ResScnRateDto>.List = CommonBuffer.FindT(key, DapperService.SqlHelp.ScanRate, model);
                        ViewBag.count = CommonHelp.Listone<ResScnRateDto>.List.Count;
                        ViewBag.Data = CommonHelp.Listone<ResScnRateDto>.List.Take(30).ToList();
                    }
                }
                if (PageIndex != null)
                {
                    var model1 = new PageInfoModel<ResScnRateDto>();
                    model1.List = CommonHelp.Listone<ResScnRateDto>.List;
                    model1.PageSize = PageSize;
                    model1.PageIndex = PageIndex;
                    var res = CommonHelp.PageList(model1);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
    }
}