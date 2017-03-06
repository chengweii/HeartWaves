using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PmtsControlLibrary.Common
{
    /// <summary>
    /// 通用类
    /// </summary>
    public class CommonUtils
    {
        #region 用户信息转成HashTable

        /// <summary>
        /// 用户信息转成HashTable
        /// </summary>
        /// <returns></returns>
        public static Hashtable getUserInfoHashTableFromStatic()
        {
            Hashtable info = null;
            info = new Hashtable();
            info["name"] = UserInfoStatic.UserInfo.username;
            info["sex"] = UserInfoStatic.UserInfo.sex;
            info["age"] = UserInfoStatic.UserInfo.birthday;
            info["area"] = UserInfoStatic.UserInfo.workingplace;
            info["pType"] = "未知";
            info["wYear"] = "0";
            info["wArea"] = "未知";
            info["O"] = 0;
            info["R"] = 0;
            info["T"] = 0;
            info["E"] = 0;
            info["W"] = 0;
            info["HRVS"] = 0;
            info["mr"] = "";
            return info;
        }

        #endregion

        /// <summary>
        /// 获取字符串 不返回null值
        /// </summary>
        public static string ToString(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }
            return value.ToString();
        }

        public static string GetAgeFromBirthday(string birthday)
        {
            TimeSpan nowTick = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan birTick = new TimeSpan(Convert.ToDateTime(birthday).Ticks);
            TimeSpan diffTick = nowTick.Subtract(birTick).Duration();
            return Math.Floor((diffTick.TotalDays / 365)).ToString();
        }

        public static string GetSexNameByValue(string sex)
        {
            return "1" == sex ? "男" : "女";
        }
    }
}
