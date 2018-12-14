using System.IO;
using System.Web;
using System.Web.Mvc;

//HTMLConvertPDF
namespace WebApplication1.Models
{
    public class BinaryContentResult : ActionResult
    {
        private readonly string contentType;
        private readonly byte[] contentBytes;

        public BinaryContentResult(byte[] contentBytes, string contentType)
        {
            this.contentBytes = contentBytes;
            this.contentType = contentType;  
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.Clear();
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.ContentType = this.contentType;
             //加上下面这行，则弹出下载 
            //response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("文件名.pdf", System.Text.Encoding.UTF8));
            using (var stream = new MemoryStream(this.contentBytes))
            {
                stream.WriteTo(response.OutputStream);
                stream.Flush();
            }
        }
    }
}