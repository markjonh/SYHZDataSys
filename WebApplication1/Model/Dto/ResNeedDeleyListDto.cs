using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResNeedDeleyListDto
    {
        public List<ResNeedDeleyDto> ListNeed { get; set; }
        public List<ResNeedDeleyDto> ListDeley { get; set; }
    }
}