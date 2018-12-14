using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using WebApplication1.Common;

namespace WebApplication1.Controllers
{
    public class ChartShowController : Controller
    {
        /// <summary>
        /// 页面初次加载显示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string area)
        {
            ViewBag.Line = CommonHelp.list;
            ViewBag.title = "视图化数据展示";
      
            ViewBag.title1 = "默认曲线";// Session["area"].ToString();
            //List<SelectListItem> opxxSelect = new List<SelectListItem>();
            //opxxSelect.Add(new SelectListItem { Value = "1", Text = "按天查询"});
            return View();
        }
        /// <summary>
        /// 用于返回查询数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns> 
        public JsonResult Json( ChartModel model)
        {
       
            System.Web.Script.Serialization.JavaScriptSerializer sj = new System.Web.Script.Serialization.JavaScriptSerializer();
            ResChartDto res = new ResChartDto();
            try
            {
                if (model.QueryKind==1|| model.QueryKind == 2)
                {
                    //ViewBag.title1 = "123";// Session["area"].ToString();

                    string title = "";
                    string title1 = "";
                    if (model.QueryKind == 1)
                    {

                        title = model.DateTime.Year + "年" + model.DateTime.Month + "月份数据";
                        title1 = CommonHelp.list.Where(p => p.Area.Equals(model.Area)).FirstOrDefault().DesCribe;

                    }
                    if (model.QueryKind == 2)
                    {

                        title = model.DateTime.Year + "年全年数据";
                        title1 = CommonHelp.list.Where(p => p.Area.Equals(model.Area)).FirstOrDefault().DesCribe;
                    }
                    //if (!string.IsNullOrEmpty(Session["area"].ToString()))  
            
                  
                    
                    List<ChartDataDto> list = DapperService.SqlHelp.GetChart(model);
                    string data = sj.Serialize(list);
                    res.data = data;
                    res.message = title;
                    res.message1 = title1;
                }
                if(model.QueryKind==3)
                {
                    List<BeatDto> list = DapperService.SqlHelp.BeatChart(model);
                    List<string> datetimeX=new List<string>();
                    List<int> output = new List<int>();
                    int days = 0;
                    foreach (var item in list)
                    {
                        datetimeX.Add(item.times);
                        output.Add(item.ProCount);
                        if (item.ProCount!=0)
                        {
                            days++;
                        }
                    }
                    //string data = sj.Serialize(list);
                    // res.data = data;
                    //foreach (var item in list)
                    //{
                    //    if (item.ProCount == 0)
                    //    {
                    //        list.Remove(item);
                    //    }
                    //}
                    res.AverangeOutPut = list.Sum(p => p.ProCount)/days;
                    res.datetimeX = sj.Serialize(datetimeX);
                    res.output = sj.Serialize(output);
                    res.message ="";
                }

               
            }
            catch(Exception ex) {

            }
            return Json(res,JsonRequestBehavior.AllowGet);
        }


        public JsonResult JsonHome(ChartModel model)
        {
            ResChartDto res = new ResChartDto();
            try
            {   model.DateTime=DateTime.Now;
        
                string title = "";
                string title1 = "";
                if (model.QueryKind == 1)
                {

                    title = model.DateTime.Year + "年" + model.DateTime.Month + "月份数据";
                    title1 = CommonHelp.list.Where(p => p.Area.Equals(model.Area)).FirstOrDefault().DesCribe;
                }
                if (model.QueryKind == 2)
                {

                    title = model.DateTime.Year + "年全年数据";
                    title1 = CommonHelp.list.Where(p => p.Area.Equals(model.Area)).FirstOrDefault().DesCribe;
                }
            
                System.Web.Script.Serialization.JavaScriptSerializer sj = new System.Web.Script.Serialization.JavaScriptSerializer();
                List<object> list1 = new List<object>();
                List<ChartDataDto> list = DapperService.SqlHelp.GetChart(model);
                string data = sj.Serialize(list);
                res.data = data;
                res.message = title;
                res.message1 = title1;
            }
            catch (Exception ex)
            {


            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }




    }
}