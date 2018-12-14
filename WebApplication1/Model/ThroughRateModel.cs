using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf.parser;

namespace WebApplication1.Model
{
    public class ThroughRateModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string  Line { get; set; }
    }
}