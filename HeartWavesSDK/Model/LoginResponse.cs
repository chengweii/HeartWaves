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
    public class LoginResponse : BaseResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoginData data { get; set; }

        public LoginResponse()
        {
            data = new LoginData();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoginData
    {
        /// <summary>
        /// 0 1 2 3	返回请求信息
        /// </summary>
        public string success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserInfo userInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public LoginData()
        {
            userInfo = new UserInfo();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class UserInfo
    {
        public string id { get; set; }
        public string realname { get; set; }
        public string weight { get; set; }
        public string height { get; set; }
        public string workingplace { get; set; }
        public string position { get; set; }
        public string medicalhistory { get; set; }
        public string mobile { get; set; }
        public string locked { get; set; }
        public string birthday { get; set; }
        public string sex { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string pic { get; set; }
        public string organization { get; set; }
        public string hrvscore { get; set; }
        public string observe { get; set; }
        public string rember { get; set; }
        public string emotion { get; set; }
        public string willpower { get; set; }
        public string thinking { get; set; }
        public Mood mood { get; set; } 
    }

    public class Mood {
        public string moodtime { get; set; }
        public string moodmark { get; set; }
        public string moodsocre { get; set; } 
    }
}
