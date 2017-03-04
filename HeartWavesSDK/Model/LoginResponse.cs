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
        /*
success	string	0 1 2 3	返回请求信息
id	string	用户id	
realname	string	真实姓名	
weight	string	体重	kg
height	string	身高	cm
workingplace	string	工作地点	
position	string	职位	
medicalhistory	string	病史	
mobile	string	电话	
locked	int	是否锁定	1 锁定 2 未锁定
birthday	string	生日	
sex	int	string	0 女 1 男
username	string	昵称
         */

        /// <summary>
        /// 用户id	
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string realname { get; set; }

        /// <summary>
        /// 体重	kg
        /// </summary>
        public string weight { get; set; }

        /// <summary>
        /// 身高	cm
        /// </summary>
        public string height { get; set; }

        /// <summary>
        /// 工作地点
        /// </summary>
        public string workingplace { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 病史
        /// </summary>
        public string medicalhistory { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 否锁定	1 锁定 2 未锁定
        /// </summary>
        public string locked { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string birthday { get; set; }

        /// <summary>
        /// 0 女 1 男
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string organization { get; set; }
    }  
}
