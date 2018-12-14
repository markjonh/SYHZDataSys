using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model
{
    public class AreaDto
    {
        [JsonProperty("area")]
        public string Area { get; set; }
        [JsonProperty("describe")]
        public string DesCribe { get; set; }
    }
}