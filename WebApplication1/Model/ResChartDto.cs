using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model
{
    public class ResChartDto
    {
        public string data { get; set; }
        public string datetimeX { get; set; }
        public string output { get; set; }
        public string message { get; set; }
        public string message1 { get; set; }
        //平均产量
        public int AverangeOutPut { get; set; }
    }
}