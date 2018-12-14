using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zhongyu.Data.Extensions;

namespace System
{
    public class ConfigHelper
    {
        public static int WebPageSize
        {
            get { return int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebPageSize"]); }
        }
        public static string  SuperRole
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["SuperRole"].ToString().CastTo<Guid>().ToString(); }
        }

        public static string FactorySiteUrl
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["FactorySiteUrl"].ToString(); }
        }
    }
}