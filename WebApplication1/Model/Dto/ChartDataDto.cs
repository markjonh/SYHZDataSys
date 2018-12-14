using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ChartDataDto
    {
        /// <summary>
        ///时间 
        /// </summary>
        [JsonProperty("value")]
        public int Num { get; set; }
        [JsonProperty("date")]
        public string ChartDate { get; set; }
        [JsonProperty("la")]
        public int Light { get; set; }
        [JsonProperty("u")]
        public int Hight { get; set; }
      

    }
}