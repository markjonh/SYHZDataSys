using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class AssembleSearchDto
    {
        public DateTime end_time { get; set; }

        public string end_times
        {
            get { return end_time.ToString(); }
        }
        public string barcode_zc { get; set; }
        public string sort_order { get; set; }
        public string csn { get; set; }
        public string area{ get; set; }
        public string STATUS { get; set; }
        public int COUNT { get; set; }

    }
}