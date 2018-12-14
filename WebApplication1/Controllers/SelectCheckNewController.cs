using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using iTextSharp.text.pdf.parser;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    public class SelectInfoNewController : Controller
    {
        /// <summary>
        /// 根据日期查询扭矩
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexNew(string Area, ByDataModel model)
        {
            try {
                if (Area != null)
                {
                    Session["area"] = Area;
                    model.StartDateTime = DateTime.Now.AddHours(-12);
                    model.EndDateTime = DateTime.Now;
                    model.Station = "All";
                }
                ViewBag.area = Session["area"].ToString();
               
                Area = Session["area"].ToString();
                List<SelectListItem> list= GetStation(Area);
                list.Add(new SelectListItem { Text = "All", Value = "All" });
                ViewBag.opxxSelect = list;
                model.Area = Area;
                ViewBag.table = null;
                if (model.Station != null)
                {
                   
                    if (model.StartDateTime < model.EndDateTime)
                    {
                        TimeSpan ts = Convert.ToDateTime(model.EndDateTime) - Convert.ToDateTime(model.StartDateTime);
                        if (ts.Days < 45)
                        {
                            Session["indexnew"] = model;
                            ViewBag.count = SelectInfo(model).Count();
                            ViewBag.table = SelectInfo(model).ToList();
                          
                        }
                        else
                        {
                            ViewBag.count = 0;
                            return Content("<script>alert('时间必须小于45天！');history.go(-1);</script>");
                            // return Content("<script>alert('时间必须小于45天！');</script>");
                        }
                    }



                }
                else
                {

                    ViewBag.count = 0;
                }
            }
            catch { }
            return View();
        }
        /// <summary>
        /// 根据条码查询扭矩
        /// </summary>
        /// <param name="Area">区域</param>
        /// <param name="model">查询请求实体</param>
        /// <returns></returns>
        public ActionResult ByCode(string Area, ByCodeModel model)
        {
            List<SelectListItem> list = GetStation(Area);
            if (Area != null)
            Session["area"] = Area;
            Area = Session["area"].ToString();
            ViewBag.opxxSelect = GetStation(Area);
            model.Area = Area;
            ViewBag.title = Session["area"].ToString();
            ViewBag.table = null;
            list.Add(new SelectListItem { Text = "All", Value = "All" });
            ViewBag.opxxSelect = list;
            if (model.BarCode != null)
            {
                Session["bycode"] = model;
                ViewBag.count = SelectByCodeInfo(model).Count();
                ViewBag.table = SelectByCodeInfo(model);
            }
            else
            { ViewBag.count = 0; }
            return View();
        }
        /// <summary>
        /// 根据条码查部件
        /// </summary>
        /// <returns></returns>
        public ActionResult ByCodePary(string Area, ByCodeModel model)
        {
            if (Area != null)
                Session["area"] = Area;
            Area = Session["area"].ToString();
            ViewBag.opxxSelect = GetStation(Area);
            model.Area = Area;
            ViewBag.title = Session["area"].ToString();
            if (model.Station != null)
            {
                Session["bycodeparty"] = model;
                ViewBag.count = SelectCodePart(model).Count();
                ViewBag.table = SelectCodePart(model);
            }
            else
            {
                ViewBag.count = 0;
            }
            return View();
        }
        /// <summary>
        /// 根据时间段查询部件
        /// </summary>
        /// <param name="Area"></param>
        /// <param name="model"></param>
        /// <param name="Page">分页</param>
        /// <returns></returns>
        public ActionResult ByDataPary(string Area, ByDataModel model)
        {
            try
            {
                if (Area != null)
                    if (Area != null)
                    {
                        Session["area"] = Area;
                        model.StartDateTime = DateTime.Now.AddHours(-12);
                        model.EndDateTime = DateTime.Now;
                        model.Station = "All";
                    }
                ViewBag.title = Session["area"].ToString();
                Area = Session["area"].ToString();
                List<SelectListItem> list = GetStation(Area);
                list.Add(new SelectListItem { Text = "All", Value = "All" });
                ViewBag.opxxSelect = list;
                model.Area = Area;
                ViewBag.table = Session["area"].ToString();
                if (model.Station != null)
                {
                    Session["bydataparty"] = model;
                    if (model.StartDateTime < model.EndDateTime)
                    {
                        TimeSpan ts = Convert.ToDateTime(model.EndDateTime) - Convert.ToDateTime(model.StartDateTime);
                        if (ts.Days < 45)
                        {
                            Session["bydataparty"] = model;
                            ViewBag.count = SelectrDataPart(model).Count();
                            ViewBag.table = SelectrDataPart(model);
                        }
                        else
                        {
                            ViewBag.count = 0;
                            return Content("<script>alert('时间必须小于45天！');history.go(-1);</script>");
                            // return Content("<script>alert('时间必须小于45天！');</script>");
                        }
                    }
                }
                else
                {
                    ViewBag.count = 0;
                }
            }
            catch
            {
            }

            return View();

        }

        #region 方法代码
        /// <summary>
        /// 根据时间段查询扭矩
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // [HttpGet]

        public List<ByDataDto> SelectInfo(ByDataModel model)
        {
            List<ByDataDto> list = new List<ByDataDto>();
            if (model.StartDateTime != null || model.EndDateTime != null)
            {

                list = DapperService.SqlHelp.TorqByData(model);
            }
            return list;
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
        /// 根据区域获取站点
        /// </summary>
        /// <param name="Area"></param>
        /// <returns></returns>
        private List<SelectListItem> GetStation(string Area)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            using (IDbConnection conn = DapperService.MySqlConnection())
            {

                var p = new DynamicParameters();
                p.Add("@_Area", Area);
                List<ResStationDto> list = conn.Query<ResStationDto>("SP_Station_QueryByArea", p, commandType: CommandType.StoredProcedure).ToList();
                foreach (var item in list)
                {
                    listItem.Add(new SelectListItem { Text = item.Station, Value = item.Station });
                }

            }

            return listItem;
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
                list = DapperService.SqlHelp.PartByCode(model);
            }
            return list;
        }
        /// <summary>
        /// 根据时间段查询部件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ByDataDto> SelectrDataPart(ByDataModel model)
        {
            List<ByDataDto> list = new List<ByDataDto>();
            if (model.StartDateTime != null || model.EndDateTime != null)
            {

                list = DapperService.SqlHelp.PartByData(model);
            }
            return list;
        }
        #endregion

        #region Pdf 方法

        private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf");//arial unicode MS是完整的unicode字型。
        private static readonly string Path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "SIMHEI.TTF");//標楷體
        //可用Arial或標楷體，自己選一個
        static BaseFont baseFont = BaseFont.CreateFont(Path1, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

        /// <summary>
        ///下载PDF文档
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadPdf()
        {
           
            string BarCode = Server.UrlDecode(Request.QueryString["BarcodeStr"].Trim());
            string Area = Request.QueryString["Area"]; //Session["area"].ToString();
            byte[] pdfFile = this.ConvertHtmlTextToPDF(BarCode, Area);
            return new BinaryContentResult(pdfFile, "application/pdf");
        }
        /// <summary>
        /// 输出到PDF
        /// </summary>
        /// <param name="BarcodeStr"></param>
        /// <returns></returns>
        public byte[] ConvertHtmlTextToPDF(string BarcodeStr, string Area)
        {
            if (string.IsNullOrEmpty(BarcodeStr))
            {
                return null;
            }

            MemoryStream outputStream = new MemoryStream(); //要把PDF寫到哪個串流
            byte[] data = Encoding.UTF8.GetBytes(BarcodeStr); //字串轉成byte[]
            MemoryStream msInput = new MemoryStream(data);

            Document doc = new Document(); //要寫PDF的文件，建構子沒填的話預設直式A4
            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            //指定文件預設開檔時的縮放為100%
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //開啟Document文件 
            doc.Open();

            Font font = new Font(baseFont, 15);
            var table1 = new PdfPTable(12);     //创建表格实例4列
            int[] a = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };        //设置列宽比例
            table1.SetWidths(a);
            table1.TotalWidth = 500;
            table1.LockedWidth = true;
            //电子流程卡标题
            Paragraph para = new Paragraph("电子流程卡查询", font);
            PdfPCell cell = new PdfPCell(para);
            cell.MinimumHeight = 30;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 12;
            cell.BackgroundColor = BaseColor.GRAY;
            table1.AddCell(cell);

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            MySqlParameter[] parameters = new MySqlParameter[2] {
                                                new MySqlParameter("_Barcode", MySqlDbType.VarChar),
                                                new MySqlParameter("_Area", MySqlDbType.VarChar) };
            MySqlParameter[] parameters1 = new MySqlParameter[1] { new MySqlParameter("@area", MySqlDbType.VarChar) };
            parameters[0].Value = BarcodeStr;
            parameters[1].Value = Area;
            parameters1[0].Value = Area;
            DataTable dt1= sql.ExecuteDataTable("SELECT list_area.describe FROM `list_area` WHERE AREA=@area", CommandType.Text,
                parameters1);
            //var p = new DynamicParameters();
           
            //p.Add("@_Barcode", BarcodeStr);
            //p.Add("@_Area", Area);

          //  DataTable dt= DapperService.SqlHelp.StoredProcedure("SP_Web_AssemblyInfo", p);
            DataTable dt = sql.ExecuteDataTable("SP_Web_AssemblyInfo",CommandType.StoredProcedure,parameters);
  
            if (dt.Rows.Count > 0&& dt.Columns.Count>0)
            {
                #region 总成信息
                string figure, zc, sn, code, status, car, group;

                figure = dt.Rows[0]["FigureNo"].ToString();
                status = dt.Rows[0]["Status"].ToString();
                car = dt.Rows[0]["CarType"].ToString();
                char[] ch = new char[1] { '&' };
                string[] bar = BarcodeStr.Split(ch);
                sn = bar[0];
                code = bar[0]; //bar[1];
                group = car + dt1.Rows[0]["Describe"].ToString();
                zc = group + figure.Substring(figure.Count() - 4);

               // 总成信息标题
                font = new Font(baseFont, 8);
                para = new Paragraph("总成信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 15;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("图号/零件号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(figure, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                table1.AddCell(cell);

                //para = new Paragraph("物料编号", font);
                //cell = new PdfPCell(para);
                //cell.MinimumHeight = 17;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //table1.AddCell(cell);

                //para = new Paragraph("/", font);
                //cell = new PdfPCell(para);
                //cell.MinimumHeight = 17;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //cell.Colspan = 3;
                //table1.AddCell(cell);

                para = new Paragraph("总成名称", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(zc, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("SN", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(sn, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                table1.AddCell(cell);

                para = new Paragraph("质量状态", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);
                
                para = new Paragraph(status, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

             

                para = new Paragraph("出厂编号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(code, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                table1.AddCell(cell);

                para = new Paragraph("物料系列", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(car, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("当前群组", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(group, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                table1.AddCell(cell);

                para = new Paragraph("生产方式", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("正常", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);
                #endregion

                #region 扫描信息
                font = new Font(baseFont, 8);
                para = new Paragraph("扫描信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("序号", font);//1
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("时间", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;                       //2
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("扫描点", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                table1.AddCell(cell);

                //para = new Paragraph("员工", font);
                //cell = new PdfPCell(para);
                //cell.MinimumHeight = 17;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //table1.AddCell(cell);

                para = new Paragraph("质量", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
              
                table1.AddCell(cell);

                para = new Paragraph("生产状态", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                 dt = sql.ExecuteDataTable("SP_Web_Sannerinfo", CommandType.StoredProcedure, parameters);
                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    for (int index = 0; index < dt.Rows.Count; index++)
                    {
                        para = new Paragraph((index + 1).ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["ProdDateTime"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["Station"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 6;
                        table1.AddCell(cell);

                        //para = new Paragraph("/", font);
                        //cell = new PdfPCell(para);
                        //cell.MinimumHeight = 17;
                        //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //table1.AddCell(cell);

                        string quality = "";
                        if (dt.Rows[index]["ScanStatus"].ToString() == "1" || dt.Rows[index]["ScanStatus"].ToString() == "OK") quality = "正常"; else quality = "异常";

                        para = new Paragraph(quality, font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 1;
                        table1.AddCell(cell);

                        para = new Paragraph("正常", font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);
                    }
                }
                else
                {
                    font = new Font(baseFont, 8);
                    para = new Paragraph("未查询到该信息", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 30;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 12;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table1.AddCell(cell);
                }

                #endregion

                #region 装配信息
                //扫描信息标题
                font = new Font(baseFont, 8);
                para = new Paragraph("装配信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("序号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("应装部件", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("实装部件", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("部件追溯码", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("追溯", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("差异件", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("拆散", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("装配时间", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                dt = sql.ExecuteDataTable("SP_Web_PartsInfo", CommandType.StoredProcedure, parameters);
                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                   
                    for (int index = 0; index < dt.Rows.Count; index++)
                    {
                        para = new Paragraph((index + 1).ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);
                        
                        string partkey = dt.Rows[index]["Barcode_part"].ToString();
                        string pk = "";
                        if (partkey.Count() > 4) pk = partkey.Substring(partkey.Count() - 4); else pk = partkey;

                        para = new Paragraph(dt.Rows[index]["CarType"].ToString() + dt.Rows[index]["Name"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["CarType"].ToString() + dt.Rows[index]["Name"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);


                        para = new Paragraph(dt.Rows[index]["Barcode_part"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);


                        string infostatus = "";
                        if (dt.Rows[index]["Sign"].ToString() == "1") { infostatus = "精追"; }
                        else if (dt.Rows[index]["Sign"].ToString() == "2") { infostatus = "批追"; }
                        else { infostatus = "无"; }

                        para = new Paragraph(infostatus, font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph("否", font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph("无", font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["ProdDateTime"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);
                    }
                }
                else
                {
                    font = new Font(baseFont, 8);
                    para = new Paragraph("未查询到该信息", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 30;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 12;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table1.AddCell(cell);
                }

                

               
                #endregion

                #region 制造参数信息
                //制造参数信息
                font = new Font(baseFont, 8);
                para = new Paragraph("制造参数信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("序号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("应采集制造参数", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("编号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("实采集制造参数", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("拧紧值", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("角度值", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("结果", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("拆散", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("时间", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);
                dt = sql.ExecuteDataTable("SP_Web_ManufacturerInfo", CommandType.StoredProcedure, parameters);
                if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
                {
                    for (int index = 0; index < dt.Rows.Count; index++)
                    {
                        para = new Paragraph((index + 1).ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);
                        para = new Paragraph(dt.Rows[index]["CarType"].ToString() + dt.Rows[index]["Order_Nut_Name"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["Code_No"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["CarType"].ToString() + dt.Rows[index]["Act_Nut_Name"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["Torque"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["Angle"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        string ok_nok = "";
                        if (dt.Rows[index]["ScanStatus"].ToString() == "1") { ok_nok = "OK"; } else { ok_nok = "NOK"; }

                        para = new Paragraph(ok_nok, font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph("无", font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table1.AddCell(cell);

                        para = new Paragraph(dt.Rows[index]["ProdDateTime"].ToString(), font);
                        cell = new PdfPCell(para);
                        cell.MinimumHeight = 17;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 2;
                        table1.AddCell(cell);
                    }
                }
                else
                {
                    font = new Font(baseFont, 8);
                    para = new Paragraph("未查询到该信息", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 30;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 12;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table1.AddCell(cell);
                }


                #endregion
            }
            else
            {
                font = new Font(baseFont, 8);
                para = new Paragraph("未查询到该信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 30;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
            }
            doc.Add(table1);

            doc.Close();
            msInput.Close();
            outputStream.Close();
            // 
            return outputStream.ToArray();

        }
        #region
        public class SqlHelper
        {
            public string SQLName = "MySql";
            private string connectionString;

            /// <summary>  
            /// 设定数据库访问字符串  
            /// </summary>  
            public string ConnectionString
            {
                set { connectionString = value; }
            }

            /// <summary>  
            /// 构造函数  
            /// </summary>  
            /// <param name="connectionString">数据库访问字符串</param>  
            public SqlHelper(string connectionString)
            {
                this.connectionString = connectionString;
            }

            /// <summary>
            /// 判断数据库是否可以联接
            /// </summary>
            /// <returns>返回联接结果</returns>
            public bool isConnectted()
            {
                MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
                try
                {
                    mySqlConnection.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }

            /// <summary>  
            /// 执行一个查询，并返回查询结果  
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandType">要执行的查询语句的类型，如存储过程或者sql文本命令</param>  
            /// <returns>返回查询结果集</returns>  
            public DataTable ExecuteDataTable(string sql, CommandType commandType)
            {
                return ExecuteDataTable(sql, commandType, null);
            }

            /// <summary>  
            /// 执行一个查询，并返回结果集  
            /// </summary>  
            /// <param name="sql">要执行的sql文本命令</param>  
            /// <returns>返回查询的结果集</returns>  
            public DataTable ExecuteDataTable(string sql)
            {
                return ExecuteDataTable(sql, CommandType.Text, null);
            }


            /// <summary>  
            /// 执行一个查询，并返回查询结果  
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandtype">要执行查询语句的类型，如存储过程或者sql文本命令</param>  
            /// <param name="parameters">Transact-SQL语句或者存储过程参数数组</param>  
            /// <returns></returns>  
            public DataTable ExecuteDataTable(string sql, CommandType commandtype, MySqlParameter[] parameters)
            {
                DataTable data = new DataTable(); //实例化datatable，用于装载查询结果集  
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.CommandType = commandtype;//设置command的commandType为指定的Commandtype  
                        cmd.CommandTimeout = 240;                    //如果同时传入了参数，则添加这些参数  
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        //通过包含查询sql的sqlcommand实例来实例化sqldataadapter  
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(data);//填充datatable  

                    }
                }
                return data;
            }

            /// <summary>  
            /// 返回一个SqlDataReader对象的实例  
            /// </summary>  
            /// <param name="sql">要执行的SQl查询命令</param>  
            /// <returns></returns>  
            public MySqlDataReader ExecuteReader(string sql)
            {
                return ExecuteReader(sql, CommandType.Text, null);
            }

            /// <summary>  
            ///   
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandType">要执行查询语句的类型，如存储过程或者SQl文本命令</param>  
            /// <returns></returns>  
            public MySqlDataReader ExecuteReader(string sql, CommandType commandType)
            {
                return ExecuteReader(sql, commandType, null);
            }

            /// <summary>  
            /// 返回一个sqldatareader对象的实例  
            /// </summary>  
            /// <param name="sql"></param>  
            /// <param name="commandType"></param>  
            /// <param name="parameters"></param>  
            /// <returns></returns>  
            public MySqlDataReader ExecuteReader(string sql, CommandType commandType, MySqlParameter[] parameters)
            {
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(sql, con);

                if (parameters != null)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameters);
                    }
                }
                con.Open();
                //CommandBehavior.CloseConnection参数指示关闭reader对象时关闭与其关联的Connection对象  
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }

            /// <summary>  
            /// 执行一个查询，返回结果集的首行首列。忽略其他行，其他列  
            /// </summary>  
            /// <param name="sql">要执行的SQl命令</param>  
            /// <returns></returns>  
            public Object ExecuteScalar(string sql)
            {
                return ExecuteScalar(sql, CommandType.Text, null);
            }

            /// <summary>  
            ///   
            /// </summary>  
            /// <param name="sql"></param>  
            /// <param name="commandType"></param>  
            /// <returns></returns>  
            public Object ExecuteScalar(string sql, CommandType commandType)
            {
                return ExecuteScalar(sql, commandType, null);
            }


            /// <summary>  
            ///   
            /// </summary>  
            /// <param name="sql"></param>  
            /// <param name="commandType">参数类型</param>  
            /// <param name="parameters"></param>  
            /// <returns></returns>  
            public Object ExecuteScalar(string sql, CommandType commandType, MySqlParameter[] parameters)
            {
                Object result = null;
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (MySqlParameter parapmeter in parameters)
                    {
                        cmd.Parameters.Add(parapmeter);
                    }
                }

                con.Open();
                result = cmd.ExecuteScalar();
                con.Close();
                return result;
            }

            /// <summary>  
            /// 对数据库进行增删改的操作  
            /// </summary>  
            /// <param name="sql">要执行的sql命令</param>  
            /// <returns></returns>  
            public int ExecuteNonQuery(string sql)
            {
                return ExecuteNonQuery(sql, CommandType.Text, null);
            }

            /// <summary>  
            /// 数据库进行增删改的操作  
            /// </summary>  
            /// <param name="sql">对数据库进行操作的sql命令</param>  
            /// <param name="commandType">要执行查询语句的类型，如存储过程或者sql文本命令</param>  
            /// <returns></returns>  
            public int ExecuteNonQuery(string sql, CommandType commandType)
            {
                return ExecuteNonQuery(sql, commandType, null);
            }

            /// <summary>  
            /// 对数据库进行增删改的操作  
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandType">要执行的查询语句类型，如存储过程或者sql文本命令</param>  
            /// <param name="parameters">Transact-SQL语句或者存储过程的参数数组</param>  
            /// <returns></returns>  
            public int ExecuteNonQuery(string sql, CommandType commandType, MySqlParameter[] parameters)
            {
                int count = 0;
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.CommandTimeout = 0;
                cmd.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                con.Open();
                count = cmd.ExecuteNonQuery();
                con.Close();
                return count;
            }

            /// <summary>  
            /// 返回当前连接的数据库中所有用户创建的数据库  
            /// </summary>  
            /// <returns></returns>  
            public DataTable GetTables()
            {
                DataTable table = null;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    table = con.GetSchema("Tables");

                }
                return table;
            }
        }
    }

    #endregion
    #endregion

}
