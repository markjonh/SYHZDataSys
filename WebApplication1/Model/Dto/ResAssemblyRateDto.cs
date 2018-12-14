using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResAssemblyRateDto
    {
        public string Figure_No_up { get; set; }
        public int  sum_up { get; set; }
        public string upstation { get; set; }
        public string Figure_No_down { get; set; }
        public int sum_down { get; set; }
        public string DOWNSTATION { get; set; }
        public double DOWNRATE { get; set; }
    }
}