using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Model
{
    public class ByDataModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "站点")]
        public string Station { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "区域")]
        public string Area { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "起始日期时间")]
        public DateTime StartDateTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "结束日期时间")]
        public DateTime EndDateTime { get; set; }

    }

}