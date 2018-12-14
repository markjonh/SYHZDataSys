using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace System
{
    public class FunctionHelper
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, string[]>>> GetFunctions()
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string[]>>> result = new Dictionary<string, Dictionary<string, Dictionary<string, string[]>>>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            try
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    bool authorizeClass = false;
                    try
                    {
                        object[] tmp = type.GetCustomAttributes(typeof(UserAuthAttribute), true);
                        if (tmp.Length > 0)
                        {
                            authorizeClass = true;
                        }
                        MethodInfo[] members = type.GetMethods(BindingFlags.Public
                                                               | BindingFlags.Instance |
                                                               BindingFlags.DeclaredOnly);
                        if (members.Length > 0)
                        {
                            foreach (MethodInfo methodInfo in members)
                            {
                                if (authorizeClass)
                                {
                                    //if (methodInfo.ReturnType.Name == "ActionResult")
                                    //{
                                    if (methodInfo.DeclaringType != null)
                                    {
                                        result = AddFunction(result, methodInfo);
                                    }
                                    //}
                                }
                                else if (
                                    methodInfo.GetCustomAttributes(typeof(UserAuthAttribute), true).Length >
                                    0)
                                {
                                    if (methodInfo.DeclaringType != null)
                                    {
                                        result = AddFunction(result, methodInfo);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, string[]>>> AddFunction(Dictionary<string, Dictionary<string, Dictionary<string, string[]>>> result, MethodInfo methodInfo)
        {
            string spaceName = methodInfo.DeclaringType.Namespace;
            string controllerName = methodInfo.DeclaringType.Name;
            string actionName = methodInfo.Name;
            object[] friendlyNames = methodInfo.GetCustomAttributes(typeof(FriendlyNameAttribute), true);
            object[] actionCodes = methodInfo.GetCustomAttributes(typeof(ActionCodeAttribute), true);
            string friendlyName = friendlyNames.Length > 0 ? (friendlyNames[0] as FriendlyNameAttribute).Message : actionName;
            string actionCode = actionCodes.Length > 0 ? (actionCodes[0] as ActionCodeAttribute).Message : Guid.NewGuid().ToString();

            if (!result.ContainsKey(spaceName))
            {
                result[spaceName] = new Dictionary<string, Dictionary<string, string[]>>();
            }
            if (!result[spaceName].ContainsKey(controllerName))
            {
                result[spaceName][controllerName] = new Dictionary<string, string[]>();
            }
            if (!result[spaceName][controllerName].ContainsKey(actionName))
            {
                result[spaceName][controllerName].Add(actionName, new string[] { actionCode, friendlyName });
            }
            return result;
        }

        public static string GetFriendlyName(Type target)
        {
            object[] tmp = target.GetCustomAttributes(typeof(FriendlyNameAttribute), true);
            if (tmp.Length > 0)
            {
                var friendlyNameAttribute = tmp[0] as FriendlyNameAttribute;
                if (friendlyNameAttribute != null)
                {
                    return friendlyNameAttribute.Message;
                }
                else
                {
                    return target.Name;
                }
            }
            else
            {
                return target.Name;
            }
        }

        public static string GetFriendlyName(string namespaceStr, string controllerName)
        {
            try
            {
                var type = Type.GetType(string.Format("{0}.{1}", namespaceStr, controllerName));

                return GetFriendlyName(type);
            }
            catch (Exception)
            {
                return controllerName;
            }
        }


    }
}