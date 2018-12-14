using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.BouncyCastle.Asn1;

namespace WebApplication1.Model.Dto
{
    public class PartDataDto
    {
        public string Figure_NO { get; set; }
        public string ZcID { get; set; }
        public string Barcode_zc { get; set; }
        public string PartID { get; set; }
        public string Partname { get; set; }
        public string Barcode_part { get; set; }
        public string ProdDateTime { get; set; }
        //public string ProdDateTimes {
        //    get { return ProdDateTime.ToString(); }
        //}
        public string ZC_NAME { get; set; }
        public string Factory { get; set; }

    }
}