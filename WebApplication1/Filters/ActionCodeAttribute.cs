#region << 版 本 注 释 >>
/*----------------------------------------------------------------
// Copyright (C) 2017 无锡唯易特科技有限公司 版权所有。 
//
// 文件名：ActionCodeAttribute
// 文件功能描述：
//
// 
// 创建者：wxvet.yu
// 时间：2017/9/2 17:37:02
// 版本：V1.0.0
//----------------------------------------------------------------*/
#endregion

using System;

namespace System.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ActionCodeAttribute : Attribute
    {
        public string Message { get; set; }

        public ActionCodeAttribute(string message)
        {
            Message = message;
        }
    }
}