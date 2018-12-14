using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class PartDetailToExcelDto
    {
        public string BarCode_zc { get; set; }

        public DateTime ProdDateTime { get; set; }


        public string Barcode_part { get; set; }

        public string PartName { get; set; }
        public string part_Sign { get; set; }
        public string Part_figure_no { get; set; }



        public string ScanStatus { get; set; }

        public string cartype { get; set; }

        public string Station { get; set; }
    }
}