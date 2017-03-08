using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using PmtsControlLibrary.Common;

namespace PmtsControlLibrary.WEBPlugin
{
    public class GetUserInfoWEB
    {
        private String user = "";
        private Hashtable meg = new Hashtable();

        public GetUserInfoWEB(Hashtable SystemMeg)
        {
            meg = SystemMeg;
            user = SystemMeg["UserID"].ToString();
        }
        public GetUserInfoWEB()
        {
            user = UserInfoStatic.UserInfo.id;
        }

        /// <summary>
        /// 取得用户雷达图相关数据
        /// </summary>
        /// <returns></returns>
        public Hashtable GetUserRadarData()
        {
            Hashtable retHas = new Hashtable();
            retHas["o"] = UserInfoStatic.UserInfo.observe;
            retHas["r"] = UserInfoStatic.UserInfo.rember;
            retHas["t"] = UserInfoStatic.UserInfo.thinking;
            retHas["e"] = UserInfoStatic.UserInfo.emotion;
            retHas["w"] = UserInfoStatic.UserInfo.willpower;
            retHas["hs"] = UserInfoStatic.UserInfo.hrvscore;
            return retHas;
        }

        /// <summary>
        /// 取得用户基本信息
        /// </summary>
        /// <returns></returns>
        public Hashtable GetUserInfoByUID()
        {
            Hashtable info = new Hashtable();
            info["realname"] = UserInfoStatic.UserInfo.realname;
            info["name"] = UserInfoStatic.UserInfo.username;
            info["sex"] = CommonUtils.GetSexNameByValue(UserInfoStatic.UserInfo.sex);
            info["age"] = CommonUtils.GetAgeFromBirthday(UserInfoStatic.UserInfo.birthday);
            info["area"] = UserInfoStatic.UserInfo.workingplace;
            info["pType"] = UserInfoStatic.UserInfo.position;
            info["wYear"] = UserInfoStatic.UserInfo.workingplace;
            info["wArea"] = "";
            info["O"] = UserInfoStatic.UserInfo.observe;
            info["R"] = UserInfoStatic.UserInfo.rember;
            info["T"] = UserInfoStatic.UserInfo.thinking;
            info["E"] = UserInfoStatic.UserInfo.emotion;
            info["W"] = UserInfoStatic.UserInfo.willpower;
            info["HRVS"] = UserInfoStatic.UserInfo.hrvscore;
            info["mr"] = UserInfoStatic.UserInfo.medicalhistory;

            return info;
        }
        /// <summary>
        /// 取得用户属性
        /// </summary>
        /// <returns></returns>
        public Hashtable GetUserMedal()
        {
            Hashtable mInfo = null;
            mInfo = new Hashtable();
            mInfo["ALLC"] = "";
            mInfo["ALLT"] = "";
            mInfo["ALLE"] = "";
            return mInfo;
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userInfo"></param>
        public bool OnUpdateUserInfo(Hashtable userInfo)
        {
            return false;
        }
        /// <summary>
        /// 取得公告
        /// </summary>
        /// <returns></returns>
        public String GetNotice()
        {
            string notice = "";
            try
            {
                var resp = HeartWavesSDK.API.APIClient._GetNoticeboard();

                if (null == resp || null == resp.data)
                {
                    PmtsMessageBox.CustomControl1.Show("网络异常，请稍后重试");
                }
                else if (resp.data.success != "1")
                {
                    PmtsMessageBox.CustomControl1.Show(resp.data.message);
                }
                else
                {
                    if (resp.data.data != null && resp.data.data.Count > 0)
                    {
                        notice = resp.data.data[0].content;
                    }
                }
            }
            catch (Exception ex)
            {
                PmtsMessageBox.CustomControl1.Show(ex.Message);
            }
            return notice;
        }
    }
}
