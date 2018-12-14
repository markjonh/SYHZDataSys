using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Model
{
    public class ToExecelModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "区域")]
        public string Area { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "起始日期时间")]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "结束日期时间")]
        public DateTime EndTime { get; set; }
    }
}