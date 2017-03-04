using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HeartWavesSDK.Common
{
    /// <summary>
    /// 网络请求类
    /// </summary>
    public class SDKHttpRequest
    {
        #region GET

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="nUrl"></param>
        /// <returns></returns>
        public static string _GET(string nUrl)
        {
            string htmltext = "";
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(nUrl);

            // 头信息
            myRequest.Headers.Add("Cache-Control", "no-cache");
            myRequest.Method = "GET";

            myRequest.ContentType = "text/plain";
            // 发送的数据信息 
            myRequest.Timeout = 15 * 1000;

            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            htmltext = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            responseStream.Close();
            return htmltext;
        }

        #endregion

        #region POST

        /// <summary>
        /// POST
        /// </summary>
        /// <param name="nUrl"></param>
        /// <param name="nPostData"></param>
        /// <returns></returns>
        public static string _POST(string nUrl, object nPostData)
        {
            var htmltext = "";
            Byte[] bSend = null;
            string postData = "";
            if (nPostData is string)
                postData = nPostData.ToString();
            else
                postData = MyJSONHelper.ObjectToJson(nPostData);

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(nUrl);

            // 头信息
            myRequest.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");
            myRequest.Method = "POST";
            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            
            // 发送的数据信息 
            bSend = Encoding.UTF8.GetBytes(postData);
            myRequest.ContentLength = bSend.Length;
            myRequest.Timeout = 30 * 1000;

            // 发送数据 
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(bSend, 0, bSend.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
            htmltext = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            responseStream.Close();
            return htmltext;
        }
        
        #endregion
    }
}
