using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Model.Dto
{
    public class BeatDto
    {
        public DateTime Date_Time { get; set; }

        public string times
        {
            get { return Date_Time.ToString("H时"); }  //yyyy-mm-dd hh
        }
        public int ProCount { get; set; }
    }
}