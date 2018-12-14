using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace WebApplication1.Model.Dto
{
    public class ThroughRateDto
    {   //工位
        public string STATION { get; set; }
        //状态数量
        public int ALL_SUM { get; set; }
        //单工位扫描状态
        public string SCANSTATUS { get; set; }
        //单工位总扫描量
        public int TOTAL { get; set; }
        //单工位直通率
        public double RATE { get; set; }
    }
}