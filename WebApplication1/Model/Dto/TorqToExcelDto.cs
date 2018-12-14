using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class TorqToExcelDto
    {  
        //生产线
        public string ProductLine { get; set; }
        //总成条码
        public string SN { get; set; }
        //上线时间
        public string OnLineTime { get; set; }

        //下线时间
        public string UpLineTime { get; set; }
        //夹具信息
        public string ClampNumber { get; set; }

        //质量状态
        public string QualityStatus { get; set; }
        //扭矩编号
        public string NutID { get; set; }
     
        //螺栓名称
        public string Nutname { get; set; }
        //工位
        public string Station { get; set; }

        //扭矩
        public string Torque { get; set; }

        //部件安装时间
        public string PartInstallTime { get; set; }
     
      
        //所属工厂
        public string Factory { get; set; }
    }
}