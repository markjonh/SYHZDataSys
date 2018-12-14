using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using WebApplication1.Common;
using WebApplication1.Filters;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using WebApplication1.Models;
using Zhongyu.Data.Extensions;
using static WebApplication1.Models.DapperService;

namespace WebApplication1.Controllers
{
    [Signin]
    public class HomeController : PdfViewController
    {
      

        private static readonly string arialFontPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
            "arialuni.ttf"); 

        private static readonly string Path1 = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
            "SIMHEI.TTF"); 


        private static readonly BaseFont baseFont =
            BaseFont.CreateFont(Path1, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

       
        public ActionResult Index()
        {
            using (IDbConnection conn = DapperService.MySqlConnection())
            {
                var list = conn.Query<AreaDto>("SP_Area_QueryList", commandType: CommandType.StoredProcedure).ToList();
                CommonHelp.list = list;
              
                ViewBag.date = DateTime.Now;
          
            }
            return View();
        }

       
        public PartialViewResult Head()
        {
            return PartialView();
        }
        public PartialViewResult Menu()
        {
            List<ResFunctionDto> function = new List<ResFunctionDto>();
            function = DapperService.Function.GetFunction().OrderBy(p=>p.Index).ToList();
            function.ForEach(p =>
                {
                    List<SonFunctionDto> son = new List<SonFunctionDto>();
                    if (!p.SonFionctionId.IsNullOrEmpty())
                    {
                        foreach (var sonid in p.SonFionctionId.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(sonid))
                            {
                                son.Add(SonFunction.GetById(sonid));
                            }
                        }
                    }
                    p.SonFunction = son;
                }
            );
            return PartialView(function);
        }
        /// <summary>
        ///     下载pdf
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadPdf()
        {
            var person = new People("左成");
            //string htmlText = this.ViewPdf("Preview", person);
            var htmlText = "";
            var pdfFile = ConvertHtmlTextToPDF(htmlText);
            return new BinaryContentResult(pdfFile, "application/pdf");
        }
        /// <summary>
        ///     <param name="htmlText"></param>
        ///     將Html文字輸出到PDF檔裡  
        /// </summary>
        /// <returns></returns>
        public byte[] ConvertHtmlTextToPDF(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText)) return null;
            //避免當htmlText無任何html tag標籤的純文字時，轉PDF時會掛掉，所以一律加上<p>標籤
            //htmlText = "<p>" + htmlText + "</p>";

            var outputStream = new MemoryStream(); //要把PDF寫到哪個串流
            var data = Encoding.UTF8.GetBytes(htmlText); //字串轉成byte[]
            var msInput = new MemoryStream(data);

            var doc = new Document(); //要寫PDF的文件，建構子沒填的話預設直式A4
            var writer = PdfWriter.GetInstance(doc, outputStream);
            //指定文件預設開檔時的縮放為100%
            var pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //開啟Document文件 
            doc.Open();
            //使用XMLWorkerHelper把Html parse到PDF檔裡
            //try
            //{
            //XMLWorkerHelper.GetInstance()
            //    .ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());
            ////}
            //catch(Exception e)
            //{ }
            ////將pdfDest設定的資料寫到PDF檔
            //PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
            //writer.SetOpenAction(action);

            var font = new Font(baseFont, 15);
            var table1 = new PdfPTable(12); //创建表格实例4列
            int[] a = {2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 2, 2}; //设置列宽比例
            table1.SetWidths(a);
            table1.TotalWidth = 500;
            table1.LockedWidth = true;
            //电子流程卡标题
            var para = new Paragraph("电子流程卡查询", font);
            var cell = new PdfPCell(para);
            cell.MinimumHeight = 30;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 12;
            cell.BackgroundColor = BaseColor.GRAY;
            table1.AddCell(cell);

            //总成信息标题
            font = new Font(baseFont, 10);
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

            para = new Paragraph("图号/零件号内容", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 6;
            table1.AddCell(cell);

            para = new Paragraph("物料编号", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table1.AddCell(cell);

            para = new Paragraph("物料编号内容", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 3;
            table1.AddCell(cell);

            para = new Paragraph("总成名称", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 2;
            table1.AddCell(cell);

            para = new Paragraph("总成名称内容", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 7;
            table1.AddCell(cell);

            para = new Paragraph("质量状态", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table1.AddCell(cell);

            para = new Paragraph("质量状态内容", font);
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
            para = new Paragraph("SN内容", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 10;
            table1.AddCell(cell);
            para = new Paragraph("出厂编号", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 2;
            table1.AddCell(cell);

            para = new Paragraph("出厂编号内容", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 7;
            table1.AddCell(cell);

            para = new Paragraph("物料系列", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table1.AddCell(cell);

            para = new Paragraph("物料系列内容", font);
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

            para = new Paragraph("当前群组内容", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 7;
            table1.AddCell(cell);

            para = new Paragraph("生产方式", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table1.AddCell(cell);

            para = new Paragraph("生产方式内容", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 2;
            table1.AddCell(cell);

            //扫描信息标题
            font = new Font(baseFont, 10);
            para = new Paragraph("扫描信息", font);
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

            para = new Paragraph("时间", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
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

            para = new Paragraph("员工", font);
            cell = new PdfPCell(para);
            cell.MinimumHeight = 17;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table1.AddCell(cell);

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
            table1.AddCell(cell);

            for (var index = 0; index < 5; index++)
            {
                para = new Paragraph((index + 1).ToString(), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("时间" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("扫描点" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                table1.AddCell(cell);

                para = new Paragraph("员工" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("质量" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("生产状态" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);
            }

            //扫描信息标题
            font = new Font(baseFont, 10);
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

            for (var index = 0; index < 5; index++)
            {
                para = new Paragraph((index + 1).ToString(), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("应装部件" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("实装部件" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("部件追溯码" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("追溯" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("差异件" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("拆散" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("装配时间" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);
            }

            //制造参数信息
            font = new Font(baseFont, 10);
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

            for (var index = 0; index < 5; index++)
            {
                para = new Paragraph((index + 1).ToString(), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("应采集制造参数" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("编号" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("实采集制造参数" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("拧紧值" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("角度值" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("结果" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("拆散" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("时间" + (index + 1), font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);
            }

            doc.Add(table1);

            doc.Close();
            msInput.Close();
            outputStream.Close();
            //回傳PDF檔案 
            return outputStream.ToArray();
        }
        private ResIndexDto ChartData(ChartModel model)
        {
            var res = new ResIndexDto();


            var title = "";
            if (model.QueryKind == 1) title = model.DateTime.Year + "年" + model.DateTime.Month + "月份数据";
            if (model.QueryKind == 2) title = model.DateTime.Year + "年全年数据";


            var sj = new JavaScriptSerializer();
            var list2 = DapperService.SqlHelp.GetChart(model);
            //var data = sj.Serialize(list2);
            res.data = list2;
            res.massage = title;

            return res;
        }
    }
}