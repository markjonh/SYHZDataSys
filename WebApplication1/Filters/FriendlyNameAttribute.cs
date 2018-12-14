using System;

namespace System.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class FriendlyNameAttribute : Attribute
    {
        public string Message { get; set; }

        public FriendlyNameAttribute(string message)
        {
            Message = message;
        }
    }
}