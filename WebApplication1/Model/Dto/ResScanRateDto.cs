using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResScanRateDto
    {
        public List<ResPartDto> partlist { get; set; }
        public List<ResAssemblyRateDto> assemlist { get; set; }
    }
}