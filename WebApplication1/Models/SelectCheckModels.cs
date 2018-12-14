using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class SelectCheckModels
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "线体代号")]
        public string LineCode { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "起始日期时间")]
        public string StartDateTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "结束日期时间")]
        public string EndDateTime { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "查询工位")]
        public string  opxx { get; set; }

        [DataType(DataType.Text)]
        [StringLength(150, ErrorMessage = "请填写正确的条码", MinimumLength = 10)]
        [Display(Name = "条 码")]
        public string BarCode { get; set; }

        [Required]
        [Display(Name = "条数/页")]
        public int CountPerPage { get; set; }

        [Required]
        [Display(Name = "当前页")]
        public int CurrentPage { get; set; }

    }

    public class SelectCheckSubPartModels
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LineCode")]
        public string LineCode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(150, ErrorMessage = "", MinimumLength = 10)]
        [Display(Name = "BarCode")]
        public string BarCode { get; set; }

        [Required]
        [Display(Name = "CountPerPage")]
        public int CountPerPage { get; set; }

        [Required]
        [Display(Name = "CurrentPage")]
        public int CurrentPage { get; set; }

        [Required]
        [Display(Name = "TotalCount")]
        public int TotalCount { get; set; }
    }

}