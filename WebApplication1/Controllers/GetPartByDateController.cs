using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.BufferMemory;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class GetPartByDateController : Controller
    {
        // GET: GetPartByDate
      
            public ActionResult Index(PartDataModel model, int? PageIndex, int? PageSize = 30)
            {
                try
                {
                    ViewBag.totalcount = 0;
                    ViewBag.Line = CommonHelp.list;
                    if (PageIndex == null && model.StartTime < model.EndTime && !string.IsNullOrEmpty(model.Line))
                    {

                        string key = Guid.NewGuid().ToString()+ model.Line;
                        CommonHelp.key = key;

                        if (model.EndTime.Month - model.StartTime.Month > 0)
                        {
                            CommonHelp.StarMon = model.StartTime.ToString("yyyy MMMM dd") + "~" +
                                                 model.EndTime.ToString("yyyy MMMM dd");
                            CommonHelp.EndMon = CommonHelp.list.Where(p => p.Area.Equals(model.Line)).FirstOrDefault()
                                .DesCribe;
                        }

                      
                        else
                        {
                            CommonHelp.StarMon = model.StartTime.Month + "月";
                            CommonHelp.EndMon = CommonHelp.list.Where(p => p.Area.Equals(model.Line)).FirstOrDefault()
                                .DesCribe;
                    }

                    if (CommonBuffer.Exist(key))
                        {
                            CommonHelp.list2 = CommonBuffer.Get<List<PartDataDto>>(key);
                            ViewBag.count = CommonHelp.list2.Count;

                            ViewBag.Data = CommonHelp.list2.Take(30).ToList();
                        }
                        else
                        {
                            CommonHelp.list2 = CommonBuffer.FindT(key, DapperService.SqlHelp.PartDataBase, model);
                            ViewBag.count = CommonHelp.list2.Count;
                            ViewBag.Data = CommonHelp.list2.Take(30).ToList(); 
                        }
                    }
                    if (PageIndex != null)
                    {
                        var model1 = new PageInfoModel<PartDataDto>();
                        model1.List = CommonHelp.list2;
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
