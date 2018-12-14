using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResAssemblyBarCode
    {
        public string area { get; set; }
        public string ID { get; set; }
        public string BarCode_zc { get; set; }
        public string BarCode_part { get; set; }
        public string ProdDateTime { get; set;}
        //public string StartTime
        //{
        //    get { return ProdDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } 
        //}
        public string  ScanStatus { get; set; }
        public string PartName { get; set; }
        public string Partfigureno { get; set; }
        public int COUNT { get; set; }
    }
}