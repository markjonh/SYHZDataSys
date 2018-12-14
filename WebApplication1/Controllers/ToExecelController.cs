using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using WebApplication1.BufferMemory;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ToExecelController : Controller
    {
        /// <summary>
        /// 详情导出excel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TorqPartToExcel()
        {
            ResDto res = new ResDto();
            res.Successed = true;
            try
            {
                string type = Request.QueryString["type"];
          
                List<ByCodeDto> list=new List<ByCodeDto>();
                List<ByCodeDto> list2 =new List<ByCodeDto>();
                if (CommonBuffer.Exist(CommonHelp.keyTorq) && CommonBuffer.Exist(CommonHelp.keyPart))
                {
                    list = CommonBuffer.Get<List<ByCodeDto>>(CommonHelp.keyTorq);
                    list2 = CommonBuffer.Get<List<ByCodeDto>>(CommonHelp.keyPart);
                }
             
                if (type=="Torq"&& list.Count != 0)
                {    
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("ProdDateTime", "开始日期");
                    dic.Add("BarCode_zc", "总成条码");
                    dic.Add("Station", "工位");
                    dic.Add("Cartype", "车型");
                    dic.Add("Nutname", "扭矩名称");
                    dic.Add("NutID", "扭矩编号");
                    dic.Add("Torque", "扭矩");
                    dic.Add("Angle", "角度");
                    dic.Add("ScanStatus", "扫描状态");//根据总成条码获得扭矩信息
                    ExExcel(list, dic, "扭距时间段筛选表");
                    res.Successed = true;
                }
                else if  (type == "Part"&&list2.Count != 0)
                {
                    Dictionary<string, string> dic2 = new Dictionary<string, string>();
                    dic2.Add("BarCode_zc", "总成条码");
                    dic2.Add("Barcode_part", "部件条码");
                    dic2.Add("PartName", "部件名称");
                    dic2.Add("Part_figure_no", "部件图号");
                    dic2.Add("ProdDateTime", "开始时间");
                    dic2.Add("ScanStatus", "扫描状态");
                    dic2.Add("Station", "站点");
                    dic2.Add("Part_Signs", "精追/批追");


                    dic2.Add("cartype", "车型");
          
                    ExExcel(list2, dic2, "部件时间段筛选表");
                   
                }
                
            }
            catch (Exception e)
            {
                res.Successed = false;
                Console.WriteLine(e); 
            }
            return Json(data:res, behavior:JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 扭矩信息导出excel
        /// </summary>
        /// <returns></returns>
       //导出零件

        public string GetPartByDateToExcel()
        {
            var Successed = true;
            var Message = "";
            string s = "";
            List<PartDataDto> list = new List<PartDataDto>();
           
            try
            {
                if (CommonBuffer.Exist(CommonHelp.key))
                {
                    list = CommonBuffer.Get<List<PartDataDto>>(CommonHelp.key);
                    if (list.Count != 0)
                    {
                        Message = "成功！";
                
                    
                        JavaScriptSerializer json = new JavaScriptSerializer();
                        json.MaxJsonLength = Int32.MaxValue;
                        s = json.Serialize(list);
                        //Dictionary<string, string> dic = new Dictionary<string, string>();
                        //dic.Add("Figure_NO",     "总成零件号");
                        //dic.Add("ZcID",          "总成ID");
                        //dic.Add("Barcode_zc",    "总成条码");
                        //dic.Add("PartID",        "分零件ID");
                        //dic.Add("Partname",      "分零件名称");
                        //dic.Add("Barcode_part",  "分零件条形码");
                        //dic.Add("ProdDateTimes", "装配时间");
                        //dic.Add("Factory",       "所属厂商");
                        //  ExExcel(list, dic, CommonHelp.StarMon+"零件数据"+ CommonHelp.EndMon);

                    }
                }
            }
            catch (Exception e)
            {
                 Successed = false;
                 Message = "失败！";
            }
            return s;
        }


        //导出扫描率
        public ActionResult ExportScanRate()
        {
            List<ResScnRateDto> list = new List<ResScnRateDto>();
            if (CommonBuffer.Exist(CommonHelp.key))
            {
                list = CommonBuffer.Get<List<ResScnRateDto>>(CommonHelp.key);
                if (list.Count != 0)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();    
                    dic.Add("Figure_No_up", "上线图号");
                    dic.Add("sum_up", "上线总数");
                    dic.Add("upstation", "上线工位");
                    dic.Add("Figure_No_down", "下线总数");
                    dic.Add("sum_down", "下线数量");
                    dic.Add("DOWNSTATION", "下线工位");
                    dic.Add("DOWNRATE", "下线扫描率");
                    dic.Add("cartype", "车型");
                    dic.Add("Figure_No", "部件图号");
                    dic.Add("Part_Sum", "部件总数");
                    dic.Add("PartFigureNo", "部件图号");
                    dic.Add("Rate", "部件扫描率");
                    ExExcel(list, dic, CommonHelp.StarMon + CommonHelp.EndMon+ "扫描率");
                }
            }
            return Content("<script>alert('请稍后再试！');history.go(-1);</script>");
        }



        //导出总成
        [HttpGet]
        public string TorqPartToExcelAll(ToExcelModel Model)
        {
            var Successed = true;
            var Message = "";
            string excel = "";
            try
              {
                    AssembleSearchModel model = new AssembleSearchModel();
                    model.Area = Model.line;
                    model.EndTime = Model.endtime;
                    model.StarTime = Model.starttime;
                    if (Model.type=="Torq")
                    {
                       
                        List<TorqToExcelDto> list = DapperService.SqlHelp.TorqByDataToExcel(model);
                        if (list.Count != 0)
                        {
                            Message = "成功！";
                            JavaScriptSerializer json = new JavaScriptSerializer();
                            json.MaxJsonLength = Int32.MaxValue;
                            excel = json.Serialize(list);
                        //Dictionary<string, string> dic = new Dictionary<string, string>();
                        //dic.Add("ProductLine", "生产线");
                        //dic.Add("SN", "总成条码");
                        //dic.Add("OnLineTime", "上线时间");
                        //dic.Add("UpLineTime", "下线时间");
                        //dic.Add("ClampNumber", "夹具信息");
                        //dic.Add("QualityStatus", "质量状态");
                        //dic.Add("NutID", "扭矩编号");
                        //dic.Add("Torque", "扭矩");
                        //dic.Add("Nutname", "螺栓名称");
                        //dic.Add("Station", "工位");
                        //dic.Add("Angle", "角度");
                        //dic.Add("PartInstallTime", "部件安装时间");
                        //dic.Add("Factory", "所属工厂");
                        //ExExcel(list, dic, "扭矩参数追溯信息表");
                    }
                    }
                    else if(Model.type == "Part")
                    {
                        List<PartToExcelDto> list = DapperService.SqlHelp.PartyByDataToExcel(model);
                        if (list.Count != 0)
                        {
                            Message = "成功！";
                            JavaScriptSerializer json = new JavaScriptSerializer();
                            json.MaxJsonLength = Int32.MaxValue;
                            excel = json.Serialize(list);
                        //Dictionary<string, string> dic = new Dictionary<string, string>();
                        //dic.Add("ProductLine", "生产线");
                        //dic.Add("SN", "总成条码");
                        //dic.Add("OnLineTime", "上线时间");
                        //dic.Add("UpLineTime", "下线时间");
                        //dic.Add("ClampNumber", "夹具信息");
                        //dic.Add("QualityStatus", "质量状态");
                        //dic.Add("NutID", "部件编号");
                        //dic.Add("RealPart", "实装部件");
                        //dic.Add("ShouldPart", "应装部件");
                        //dic.Add("PartInstallTime", "部件安装时间");
                        //dic.Add("Factory", "所属工厂");
                        //ExExcel(list, dic, "部件参数追溯信息表");
                    }
                    }
            }
            catch(Exception ex)
            {
                Successed = false;
                Message = "失败！";
            }
            return excel;

        }

        #region 扭矩条码查询结果表

        // [HttpGet]
        //public ActionResult ByCodeToExcel()
        //{
        //    try
        //    {
        //        if (Session["bycode"] != null)
        //        {
        //            ByCodeModel model = Session["bycode"] as ByCodeModel;
        //            List<ByCodeDto> list = SelectByCodeInfo(model).ToList();
        //            if (list.Count != 0)
        //            {
        //                Dictionary<string, string> dic = new Dictionary<string, string>();
        //                dic.Add("ProdDateTime", "开始时间");
        //                dic.Add("Torque", "扭距");
        //                dic.Add("Angle", "角度");
        //                dic.Add("Station", "站点");
        //                dic.Add("NutID", "Nut");
        //                dic.Add("ScanStatus", "扫描状态");
        //                ExExcel(list, dic, "扭矩条码查询结果表");


        //              //return Content("<script>alert('导出成功');history.go(-1);</script>");
        //            }
        //            else
        //            {
        //                return Content("<script>alert('没有要导出的数据！');history.go(-1);</script>");

        //            }
        //              //



        //        }
        //        else
        //        {
        //            return Content("<script>alert('没有要导出的数据！');history.go(-1);</script>");
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return Json("导出成功！");
        //}

        #endregion

        #region 部件条码查询结果表

        //[HttpGet]
        //public ActionResult ByCodeParyToExcel()
        //{
        //    try
        //    {
        //        if (Session["bycodeparty"] != null)
        //        {
        //            ByCodeModel model = Session["bycodeparty"] as ByCodeModel;
        //            List<ByCodeDto> list = SelectCodePart(model).ToList();
        //            if (list.Count!=0)
        //            {
        //                Dictionary<string, string> dic = new Dictionary<string, string>();
        //                dic.Add("ProdDateTime", "开始时间");
        //                dic.Add("BarCode_zc", "总成条码");
        //                dic.Add("Station", "站点");
        //                dic.Add("Barcode_part", "部件条码");
        //                dic.Add("ScanStatus", "扫描状态");
        //                ExExcel(list, dic, "部件条码查询结果表");
        //            }
        //            else
        //            {
        //                return Content("<script>alert('没有可导出的数据');history.go(-1);</script>");
        //            }
        //        }
        //        else
        //        {
        //            return Content("<script>alert('没有可导出的数据');history.go(-1);</script>");
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return View();
        //}

        #endregion


        [HttpGet]
        public JsonResult ByDataParyToExcel(ByDataModel model)
        {
               var Successed = true;
               var Message = "";
               string s = "";
               List<ByDataDto> list = SelectrDataPart(model).ToList();
               try
               {
                if (list.Count != 0)
                {
                    Message = "导出成功！";
                    JavaScriptSerializer json = new JavaScriptSerializer();
                    json.MaxJsonLength = Int32.MaxValue;
                    s = json.Serialize(list);
                    //Dictionary<string, string> dic = new Dictionary<string, string>();
                    //dic.Add("ProdDateTime", "开始时间");
                    //dic.Add("BarCode_zc", "总成条码");
                    //dic.Add("Station", "站点");
                    //dic.Add("Barcode_part", "部件条码");
                    //dic.Add("PartName", "部件名称");
                    //dic.Add("Part_figure_no", "部件图号");
                    //dic.Add("cartype", "车型");
                    //dic.Add("ScanStatus", "扫描状态");
                    //ExExcel(list, dic, "部件时间查询结果表");
                }
               }
            catch (Exception e)
            {
                 Successed = false;
                 Message = "导出失败！";
            }
            return Json(new ResExportDto() { excel = s, mess = Message, success = Successed },JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///根据集合导出Excel
        /// </summary>    
        public void ExExcel<T>(List<T> objList, Dictionary<string, string> columnInfo, string FileName)
        {
            if (columnInfo.Count == 0) { return; }
            if (objList.Count == 0) { return; }
            //生成EXCEL的HTML 
            string excelStr = "";
            Type myType = objList[0].GetType();
            //根据反射从传递进来的属性名信息得到要显示的属性 
            List<System.Reflection.PropertyInfo> myPro = new List<System.Reflection.PropertyInfo>();
            foreach (string cName in columnInfo.Keys)
            {
                System.Reflection.PropertyInfo p = myType.GetProperty(cName);
                if (p != null)
                {
                    myPro.Add(p);
                    excelStr += columnInfo[cName] + "\t'";
                }
            }
            //如果没有找到可用的属性则结束 
            if (myPro.Count == 0) { return; }
            excelStr += "\r'";
            foreach (T obj in objList)
            {
                foreach (System.Reflection.PropertyInfo p in myPro)
                {
                    excelStr += p.GetValue(obj, null) + "\t'";
                }
                excelStr += "\r'";
            }


            HttpResponse rs = System.Web.HttpContext.Current.Response;

            rs.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
          // rs.Flush();
            rs.AppendHeader("content-disposition","attachment;filename=" + HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8) + ".xls");
       
            rs.ContentType = "application/ms-excel";
            rs.Write(excelStr);
            //  rs.Clear();

            rs.End();

        }
        public List<ByDataDto> SelectInfo(ByDataModel model)
        {
            List<ByDataDto> list = new List<ByDataDto>();
            if (model.StartDateTime != null || model.EndDateTime != null)
            {

                list = DapperService.SqlHelp.TorqByData(model);
            }
            return list;
        }
        public List<ByCodeDto> SelectByCodeInfo(ByCodeModel model)
        {
            List<ByCodeDto> list = new List<ByCodeDto>();
            if (model.BarCode != null)
            {

                list = DapperService.SqlHelp.TorqByCode(model);
            }
            return list;
        }
        public List<PartDetailToExcelDto> SelectCodePart(ByCodeModel model)
        {
            List<PartDetailToExcelDto> list = new List<PartDetailToExcelDto>();
            if (model.BarCode != null)
            {

                list = DapperService.SqlHelp.PartToExcel(model);
            }
            return list;
        }
        public List<ByDataDto> SelectrDataPart(ByDataModel model)
        {
            List<ByDataDto> list = new List<ByDataDto>();
            if (model.StartDateTime != null || model.EndDateTime != null)
            {

                list = DapperService.SqlHelp.PartByData(model);
            }
            return list;
        }

    }
    
}