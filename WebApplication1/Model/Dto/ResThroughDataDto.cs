using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResThroughDataDto
    {
        //工位
        public string Station { get; set; }
        //工位直通率
        public string StationThroughRate { get; set; }
        //工位产量
        public string OutPut { get; set; }
        //总直通率
        public string AllRate { get; set; }
        //总量
        public string Data { get; set; }
    }
}