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
    public class ThroughRateController : Controller
    {
        // GET: ThroughRate
        public ActionResult Index()
        {
            ViewBag.Line = CommonHelp.list;
            return View();
        }
        public JsonResult ThroughRate(ThroughRateModel model )
        {
            System.Web.Script.Serialization.JavaScriptSerializer sj = new System.Web.Script.Serialization.JavaScriptSerializer();
            ResThroughDataDto res = new ResThroughDataDto();
            List<double> StationRate=new List<double>();
            List<string> Station=new List<string>();
            List<int> OutPut=new List<int>();
            var TotalCount = 0;
            double AllRate = 1.00;
            try
            {
                List<ThroughRateDto> list = DapperService.SqlHelp.ThroughRateData(model);
                foreach (var item in list)
                {
                    OutPut.Add(item.TOTAL);
                    Station.Add(item.STATION);
                    StationRate.Add(item.RATE);
                    AllRate = AllRate *(item.RATE / 100);
                    TotalCount = TotalCount + item.TOTAL;
                }
                List<ResAllRateDto> listallrate=new List<ResAllRateDto>(){new ResAllRateDto(){value =Convert.ToInt32(TotalCount * AllRate),name = "直通率"},
                    new ResAllRateDto(){ value =TotalCount - Convert.ToInt32(TotalCount * AllRate),name = "其他" } };
                res.Data = sj.Serialize(listallrate);       
                res.OutPut= sj.Serialize(OutPut);
                res.Station = sj.Serialize(Station);
                res.StationThroughRate = sj.Serialize(StationRate);
            }
            catch (Exception ex)
            {

            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}