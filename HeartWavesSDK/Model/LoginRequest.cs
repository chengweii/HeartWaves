using HeartWavesSDK.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartWavesSDK.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginRequest : BaseRequest
    {
        /// <summary>
        /// 用户名	必填
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 密码	必填
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var data = "username=" + MyUtils.UrlEncode(username) + "&password=" + password;
            return data;
        }
    }
}
