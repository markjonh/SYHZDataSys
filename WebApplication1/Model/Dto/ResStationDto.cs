using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class ResStationDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("station")]
        public string Station { get; set; }
        [JsonProperty("area")]
        public string Area { get; set; }
        [JsonProperty("describe")]
        public string Describe { get; set; }
    }
}