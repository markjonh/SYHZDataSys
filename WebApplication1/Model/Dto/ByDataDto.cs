using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ByDataDto
    {
        [JsonProperty("barCode_zc")]
        public string BarCode_zc { get; set; }
        [JsonProperty("prodDateTime")]
        public DateTime ProdDateTime { get; set; }
        [JsonProperty("barCodePart_part")]
        public string BarCode_part { get; set; }

        [JsonProperty("scanStatus")]
        public string ScanStatus { get; set; }
        [JsonProperty("station")]
        public string Station { get; set; }
        //螺栓名称
        [JsonProperty("nutname")]
        public string Nutname { get; set; }
       // 螺栓编号
        [JsonProperty("nutid")]
        public string NutID { get; set; }
        [JsonProperty("torque")]
        public string Torque { get; set; }

        [JsonProperty("angle")]
        public string Angle { get; set; }






    }
}