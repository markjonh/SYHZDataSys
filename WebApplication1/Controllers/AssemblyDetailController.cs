using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using WebApplication1.BufferMemory;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AssemblyDetailController : Controller
    {
        // GET: AssemblyDetail
        [HttpGet]
        public ActionResult Index(string BarcodeStr,string Area)
        {
            try
            {
               
                ByCodeModel model=new ByCodeModel();
                model.BarCode =  BarcodeStr.Trim();
                ViewBag.title = CommonHelp.list.Where(p => p.Area.Equals(Area)).FirstOrDefault().DesCribe;
              
                model.Area =Area;
               
                model.Station = "All";
                string keyTorq = BarcodeStr + model.Area+ "Torq";
                string keyPart = BarcodeStr + model.Area+ "Part";
                CommonHelp.keyPart = keyPart;
                CommonHelp.keyTorq = keyTorq;
                if (CommonBuffer.Exist(keyTorq)&& CommonBuffer.Exist(keyPart))
                {
                    ViewBag.Torq = CommonBuffer.Get<List<ByCodeDto>>(keyTorq);
                    ViewBag.Part = CommonBuffer.Get<List<ByCodeDto>>(keyPart);
                }
                else
                {
                    ViewBag.Torq = CommonBuffer.FindT(keyTorq,SelectByCodeInfo,model);
                    ViewBag.Part = CommonBuffer.FindT(keyPart, SelectCodePart,model);
                }

            
            }
            catch (Exception ex)
            {
               
            }
        
            return View();
        }

        /// <summary>
        /// 根据条码查询扭矩
        /// </summary>
        public List<ByCodeDto> SelectByCodeInfo(ByCodeModel model)
        {
            List<ByCodeDto> list = new List<ByCodeDto>();
            if (model.BarCode != null)
            {

                list = DapperService.SqlHelp.TorqByCode(model);
            }
            return list;
        }
        /// <summary>
        /// 根据条码查询部件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ByCodeDto> SelectCodePart(ByCodeModel model)
        {
            List<ByCodeDto> list = new List<ByCodeDto>();
            if (model.BarCode != null)
            {

                list = DapperService.SqlHelp.PartByCode(model).OrderBy(p=>p.ProdDateTime).ToList();
            }
            return list;
        }
    }
}