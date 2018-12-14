using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebApplication1.Model;

namespace WebApplication1.BufferMemory
{
    public class CommonBuffer
    {
        static CommonBuffer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    List<string> keyList = new List<string>();
                    foreach (var key in CustomCacheDictionary.Keys)
                    {
                        var timeValue = CustomCacheDictionary[key];
                        if (timeValue.Key <= DateTime.Now)//过期
                        {
                            keyList.Add(key);
                        }
                    }
                    keyList.ForEach(key => CustomCacheDictionary.Remove(key));
                    Thread.Sleep(1000 * 60 * 10);//10分钟执行一次
                }
            });
        }
        //容器存储结果 下次直接用
        private static Dictionary<string, KeyValuePair<DateTime, object>> CustomCacheDictionary = new Dictionary<string, KeyValuePair<DateTime, object>>();//是为了数据安全   为了全局唯一 不被回收  为了多项存储

        public static void Add(string key, object oValue, int second = 1800)
        {
            CustomCacheDictionary.Add(key, new KeyValuePair<DateTime, object>(DateTime.Now.AddSeconds(second), oValue));
          //  CustomCacheDictionary.Add(key, oValue);
        }

        public static T Get<T>(string key)
        {
            return (T)CustomCacheDictionary[key].Value;
        }

        public static bool Exist(string key)
        {
            
            return CustomCacheDictionary.ContainsKey(key);

        }

        public static Result FindT<T, Result>(string key, Func<T, Result> func,T s)
        {
            Result t = default(Result);
            if (CommonBuffer.Exist(key))
            {
               t = CommonBuffer.Get<Result>(key);
            }
            else
            {
                 t = func.Invoke(s);
                CommonBuffer.Add(key, t);
               // CustomCacheDictionary.Clear();
            }
            return t;
        }
    }








}