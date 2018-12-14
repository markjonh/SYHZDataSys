using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Model
{
    public class ChartModel
    {
      
        [Display(Name = "时间")]
        public DateTime DateTime { get; set; }
      
        [Display(Name = "查询类型")]
        public int QueryKind { get; set; }

        [Display(Name = "地区")]
        public string Area { get; set; }
    }
}