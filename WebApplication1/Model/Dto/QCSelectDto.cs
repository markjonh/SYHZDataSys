using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class QCSelectDto
    {
        public int ID { set; get; }
        public DateTime datetime { set; get; }
        public string datetimes {
            get { return datetime.ToString(); }
        }
        public string area { set; get; }
        public string  barcode_zc { set; get; }
        public string  figure { set; get; }
        public string  content { set; get; }
        public string  sign { set; get; }
        public DateTime end_time { get ; set ; }

        public string end_times ;


    }
}