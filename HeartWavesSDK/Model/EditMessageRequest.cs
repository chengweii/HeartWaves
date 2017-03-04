using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeartWavesSDK.Model
{
    public class EditMessageRequest
    {
        /*
名字	类型	注释	备注
username	string	用户名	必填
sex	string	性别	选填
height	string	身高	选填
weight	string	体重	选填
birthday	string	生日	选填
mobile	string	电话	选填
workingPlace	string	工作地点	选填
position	string	职位	选填
medicalHistory	string	病史	选填
email	string	email	选填
         */
        public string newPassword { get; set; }
        public string id { get; set; }

        public string username { get; set; }
        public string sex { get; set; }
        public string height { get; set; }
        public string weight { get; set; }
        public string birthday { get; set; }
        public string mobile { get; set; }
        public string workingPlace { get; set; }
        public string position { get; set; }
        public string medicalHistory { get; set; }
        public string email { get; set; }
        public string organization { get; set; }
    }
}
