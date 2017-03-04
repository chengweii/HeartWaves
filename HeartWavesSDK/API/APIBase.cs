using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HeartWavesSDK.Common;

namespace HeartWavesSDK.API
{
    public class APIBase
    {
        /// <summary>
        /// 用户接口地址
        /// </summary>
        public static string m_APIUrl = "http://120.27.98.52/heartwaves/index.php?m=Home&c=Apiuserr&a=";

        public static T Post<T>(string url, Object requestModel)
        {
            var requestData = requestModel == null ? "" : getUrlParams(requestModel);
            var uri = m_APIUrl + url;
            var json = SDKHttpRequest._POST(uri, requestData);
            return MyJSONHelper.JsonToObject<T>(json);
        }
        public static T Get<T>(string url, Object requestModel)
        {
            var requestData = requestModel == null ? "" : getUrlParams(requestModel);
            var uri = m_APIUrl + url + requestData;
            var json = SDKHttpRequest._GET(uri);
            return MyJSONHelper.JsonToObject<T>(json);
        }

        private static string getUrlParams(Object obj)
        {
            Type type = obj.GetType();
            System.Reflection.PropertyInfo[] properties = type.GetProperties();
            StringBuilder urlParams = new StringBuilder();
            string value = null;
            object orign = null;
            foreach (PropertyInfo item in properties)
            {
                value = null;
                orign = item.GetValue(obj, null);
                if (orign != null)
                {
                    if (orign is List<String>)
                    {
                        value = listToString((List<String>)orign);
                    }
                    else
                    {
                        value = orign.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(value))
                    urlParams.Append("&").Append(item.Name).Append("=").Append(value);
            }
            return urlParams.ToString();
        }

        private static string listToString(List<String> list)
        {
            return "[" + string.Join(",", ((List<String>)list).ToArray()) + "]";
        }
    }
}
