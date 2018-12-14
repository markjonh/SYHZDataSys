using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResScnRateDto
    {
        public string Figure_No_up   { get; set; }
        public double    sum_up            { get; set; }
        public string upstation      { get; set; }
        public string Figure_No_down { get; set; }
        public int    sum_down          { get; set; }
        public string DOWNSTATION    { get; set; }
        public string DOWNRATE       { get; set; }
        public string cartype        { get; set; }
        public string Figure_No      { get; set; }
        public string Partname       { get; set; }
        public double    Part_Sum          { get; set; }
        public string PartFigureNo   { get; set; }
        public string Rate           { get; set; }
    }
}