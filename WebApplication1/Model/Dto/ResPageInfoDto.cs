using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResPageInfoDto<T>
    {
        //当前页数据
        // public List<AssembleSearchDto> List { get; set; }
        public List<T> List { get; set; }
        //总页数
        public int TotalCount { get; set; }
    }
}