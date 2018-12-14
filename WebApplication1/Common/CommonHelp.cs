using System.Collections.Generic;
using System.Linq;
using WebApplication1.Model;
using WebApplication1.Model.Dto;

namespace WebApplication1.Common
{
    public class CommonHelp 
    {
        public static string keyTorq;
        public static string keyPart;
        public static string key;
        public static string StarMon;
        public static string EndMon;
        public static List<AreaDto> list = new List<AreaDto>();
        public static List<PartDataDto> list2 = new List<PartDataDto>();
        public class Listone<T>
        {
            public static List<T> List = new List<T>();
        }
       
        public static ResPageInfoDto<T> PageList<T>(PageInfoModel<T> model)
        {
            var res = new ResPageInfoDto<T>();
            if (model.PageIndex <= model.List.Count / 30)
                res.List = model.List.Skip(model.PageIndex ?? 0 - 1).Take(model.PageSize ?? 30).ToList();
            else
            {
                var last = model.List.Count - (model.PageIndex - 1) * 30;
                res.List = model.List.Skip(model.PageIndex ?? 0 - 1).Take(last??30).ToList();
            }
            res.TotalCount = model.List.Count;
            return res;
        }
    }
}