using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResFunctionDto
    {
        public string FunctionId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public string HtmlId { get; set; }
        public string HtmlClass { get; set; }
        public string CheachAuth { get; set; }
        public string SonFionctionId { get; set; }
        public int Index { get; set; }
        public List<SonFunctionDto> SonFunction { get; set; }
    }
}