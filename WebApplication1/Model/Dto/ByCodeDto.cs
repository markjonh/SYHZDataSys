using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Protobuf.WellKnownTypes;

namespace WebApplication1.Model.Dto
{
    public class ByCodeDto
    {

        public string BarCode_zc { get; set; }
    
        public DateTime ProdDateTime { get; set; }

  
        public string  Barcode_part { get; set; }

        public string PartName { get; set; }
        public string Part_Sign
        {
            get { return Part_Signs; }


            set
            {
                if (value == "1")
                {
                    Part_Signs = "精追";
                }

                else if (value == "2")
                {
                    Part_Signs = "批追";
                }
                else
                {
                    Part_Signs = "空";
                }

            }
        }

        public string  Part_Signs { get; set; }
        
        public string Part_figure_no { get; set; }
        /// <summary>
        ///  扭矩编号
        /// </summary>
      
        public string NutID { get; set; }
        /// <summary>
        /// 扭矩
        /// </summary>
     
        public string Torque { get; set; }
        /// <summary>
        /// 角度
        /// </summary>
      
        public string Angle { get; set; }

        public string ScanStatus { get; set; }

        [JsonProperty("cartype")]
        public string cartype { get; set; }

        [JsonProperty("cartype")]
        public string Cartype { get; set; }

        [JsonProperty("nut_name")]
        public string Nutname { get; set; }

        [JsonProperty("station")]
        public string Station { get; set; }



    }
}