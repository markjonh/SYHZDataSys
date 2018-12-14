using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.BouncyCastle.Asn1.Mozilla;

namespace WebApplication1.Model.Dto
{
    public class ResPartDto
    {
        public string cartype { get; set; }
        public string Figure_No { get; set; }
        public string Partname { get; set; }
        public int  Part_Sum { get; set; }
        public string PartFigureNo { get; set; }

    }
}