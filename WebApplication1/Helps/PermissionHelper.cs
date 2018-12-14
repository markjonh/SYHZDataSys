using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using Wxvet.Ddc.IAdminService;
//using Wxvet.Ddc.Model;

namespace System
{
    internal static class PermissionHelper
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, bool>>>> permissionDic =
            new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, bool>>>>();

    public static bool CheckPermission(string roleId, string namespaceName, string controllerName, string actionName)
    {
        if (roleId.Equals(ConfigHelper.SuperRole))
            return true;
        if (permissionDic == null || permissionDic.Count() == 0)
        {
            permissionDic = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, bool>>>>();
           // RegisterPermissionDic();
        }

        if (permissionDic.ContainsKey(namespaceName) && permissionDic[namespaceName].ContainsKey(controllerName)
            && permissionDic[namespaceName][controllerName].ContainsKey(actionName)
            && permissionDic[namespaceName][controllerName][actionName].ContainsKey(roleId))
        {
            return permissionDic[namespaceName][controllerName][actionName][roleId];
        }
        else
        {
            return false;
        }

    }

    public static bool CheckPermission(string[] roleId, string namespaceName, string controllerName, string actionName)
    {
        if (roleId.Contains(ConfigHelper.SuperRole))
            return true;

        if (permissionDic == null || permissionDic.Count() == 0)
        {
            permissionDic = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, bool>>>>();
           // RegisterPermissionDic();
        }

        if (permissionDic.ContainsKey(namespaceName) && permissionDic[namespaceName].ContainsKey(controllerName)
            && permissionDic[namespaceName][controllerName].ContainsKey(actionName))
        {
            bool result = false;
            foreach (var role in roleId)
            {
                if (!permissionDic[namespaceName][controllerName][actionName].Keys.Contains(role))
                    break;
                result = permissionDic[namespaceName][controllerName][actionName][role] || result;
                if (result)
                    break;
            }
            return result;
        }
        else
        {
            return false;
        }

    }

    //public static void RegisterPermissionDic()
    //{
    //    ClearPermissionDic();

    //    IFunctionPointService iaction = DependencyResolver.Current.GetService<IFunctionPointService>();

    //    foreach (FunctionPoint action in iaction.Entities)
    //    {
    //        if (!permissionDic.ContainsKey(action.Namespace))
    //        {
    //            permissionDic[action.Namespace] = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>();
    //        }
    //        if (!permissionDic[action.Namespace].ContainsKey(action.Controller))
    //        {
    //            permissionDic[action.Namespace][action.Controller] = new Dictionary<string, Dictionary<string, bool>>();
    //        }
    //        if (!permissionDic[action.Namespace][action.Controller].ContainsKey(action.Action))
    //        {
    //            permissionDic[action.Namespace][action.Controller][action.Action] = new Dictionary<string, bool>();
    //        }

    //        foreach (Function fta in action.Function)
    //        {
    //            foreach (Role rtf in fta.Role)
    //            {
    //                permissionDic[action.Namespace][action.Controller][action.Action][rtf.Id] = true;
    //            }
    //        }
    //    }

    //}

    public static void ClearPermissionDic() { permissionDic.Clear(); }
}
}