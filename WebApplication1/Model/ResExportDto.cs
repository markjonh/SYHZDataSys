using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model
{
    public class ResExportDto
    {
        //序列化后excel
        public string excel { get; set; }
        //返回状态
        public bool success { get; set; }
        //消息
        public string mess { get; set; }
        //导出类型
      
    }
}