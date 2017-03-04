using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

namespace HeartWavesSDK.Common
{
    public class MyJSONHelper
    {
        /// <summary>
        /// 将object转换为json字符串,obj为方法实体类
        /// </summary>
        /// <param name="obj">方法实体类</param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
#if DEBUG
            //var o = new List<string>();
            //o.Add("2");
            //o.Add("3");
            //o.Add("4");
            //obj = o;
#endif
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, obj);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                    var result = sb.ToString();
                    return result;
                }
            }
            catch { }
            return "";
            
        }

        /// <summary>
        /// 将json字符串转化为方法实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T jsonObject = (T)ser.ReadObject(ms);
            ms.Close();
            return jsonObject;
        }
    }
}
