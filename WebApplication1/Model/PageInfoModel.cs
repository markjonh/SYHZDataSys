using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Model.Dto;

namespace WebApplication1.Model
{
    public class PageInfoModel<T> 
    {
        //分页集合
        public List<T> List { get; set; }
        // public ArrayList List { get; set; }
       // public List<AssembleSearchDto> List { get; set; }
        //
       //  public List<QCSelectDto> QCList { get; set; }
        //每页多少条数据
        public int? PageSize { get; set; }
        //页码
        public int? PageIndex { get; set; }

    }
}