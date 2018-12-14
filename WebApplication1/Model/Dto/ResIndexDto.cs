using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResIndexDto
    {
        public List<ChartDataDto> data { get; set; }
        public string massage { get; set; }
    }
}