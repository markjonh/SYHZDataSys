using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class PartToExcelDto
    {
        //生产线
        public string ProductLine { get; set; }
        //总成条码
        public string SN { get; set; }
        //夹具信息
        public string ClampNumber { get; set; }
        //上线时间
        public string OnLineTime { get; set; }
        //下线时间
        public string UpLineTime { get; set; }
        //部件编号
        public string PART_figure_no { get; set; }
        //车型
        public string cartype { get; set; }

        //质量状态
        public string QualityStatus { get; set; }
        //扭矩编号
        public string NutID { get; set; }
        //质量状态
        public string RealPart { get; set; }
        //扭矩编号
        public string ShouldPart { get; set; }
        //部件安装时间
        public string PartInstallTime { get; set; }
        //所属工厂
        public string Factory { get; set; }
    }
}