using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartWavesSDK.Model
{
    public class CommonResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CommonData data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CommonResponse()
        {
            data = new CommonData();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class CommonData
    {
        /// <summary>
        /// 0 1 2 3	返回请求信息
        /// </summary>
        public string success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
    }
}
