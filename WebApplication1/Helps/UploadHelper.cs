using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;

namespace System
{
    /// <summary>
    /// 上传文件帮助类
    /// </summary>
    public class UploadHelper
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public class ErrorMessage
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// 错误内容
            /// </summary>
            public string Message { get; set; }
        }


        /// <summary>
        /// 文件的详细信息
        /// </summary>
        public class FileInformation
        {
            /// <summary>
            /// 文件原名称,无后缀
            /// </summary>
            /// 
            public string Format { get; set; }
            public string OldName { get; set; }
            /// <summary>
            /// 文件新名称,无后缀
            /// </summary>
            public string NewName { get; set; }
            /// <summary>
            /// 路径
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// 略缩图名称,无后缀
            /// </summary>
            public string SmallName { get; set; }
            /// <summary>
            /// 类型,后缀名
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 大小
            /// </summary>
            public int Size { get; set; }

        }

        /// <summary>
        /// 上传结果类
        /// </summary>
        public class UploadResult
        {
            public UploadResult()
            {
                Result = false;
                ErrorMessage = new List<ErrorMessage>();
                SuccessFile = new List<FileInformation>();
            }

            /// <summary>
            /// 结果
            /// </summary>
            public bool Result { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            public List<ErrorMessage> ErrorMessage { get; set; }

            /// <summary>
            /// 上传成功的文件
            /// </summary>
            public List<FileInformation> SuccessFile { get; set; }

            /// <summary>
            /// 成功个数
            /// </summary>
            public int SuccessCount { get { return SuccessFile.Count; } }

        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="request">HttpRequestBase</param>
        /// <param name="ExtendPath">保存路径</param>
        /// <returns></returns>
        public static UploadResult Upload(HttpRequestBase request, string ExtendPath)       
        {
            UploadResult result = new UploadResult() { Result = false };

            if (request.Files.Count == 0)
            {
                result.ErrorMessage.Add(new ErrorMessage() { FileName = "", Message = "没有文件" });

                return result;
            }

            try
            {
                foreach (string file in request.Files)
                {

                    HttpPostedFileBase postFile = request.Files[file];
                  
                    try
                    {
                        FileInformation finfo = new FileInformation()
                        {
                            OldName = System.IO.Path.GetFileNameWithoutExtension(postFile.FileName),
                            Size = postFile.ContentLength,
                            Type = System.IO.Path.GetExtension(postFile.FileName).ToLower(),
                            NewName = DateTime.Now.ToString("yyyyMMddHHmmssfffff")
                        };
                        finfo.Path = ExtendPath;
                        finfo.Format = ExtendPath;

                        #region check
                        if (finfo.Size == 0)
                        {
                            result.ErrorMessage.Add(new ErrorMessage() { FileName = finfo.OldName, Message = "文件大小为0" });
                            continue;
                        }
                        #endregion

                        string newFilePath = HttpRuntime.AppDomainAppPath + finfo.Path;

                        if (!Directory.Exists(newFilePath))
                        {
                            Directory.CreateDirectory(newFilePath);
                        }
                        postFile.SaveAs(newFilePath + finfo.NewName + finfo.Type);
                        
                        result.SuccessFile.Add(finfo);
                        result.Result = true;







                    }
                    catch (Exception ex)
                    {
                        result.ErrorMessage.Add(new ErrorMessage() { FileName = postFile.FileName, Message = "系统错误:" + ex.Message });
                    }





                }
            }
            catch (Exception ex) { throw ex; }
            return result;
        }

        private static void RunExe(string arguments, string exePath)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.
ProcessStartInfo(exePath);
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.Arguments = arguments;
            System.Diagnostics.Process pro = System.Diagnostics.Process.Start(startInfo);
            pro.WaitForExit();
        }

        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="path">文件的完整路径</param>
        /// <returns>返回结果</returns>
        public static Boolean IsImage(string path)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}