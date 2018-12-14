using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Model
{
    public class ByCodeModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "站点")]
        public string Station { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "区域")]
        public string Area { get; set; }

        [DataType(DataType.Text)]
        [StringLength(150, ErrorMessage = "请填写正确的条码", MinimumLength = 10)]
        [Display(Name = "条 码")]
        public string BarCode { get; set; }
    }
}