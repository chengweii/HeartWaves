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
        private MySqlConnection DBCon = null;

        public GetUserInfoWEB(Hashtable SystemMeg)
        {
            meg = SystemMeg;
            user = SystemMeg["UserID"].ToString();
        }
        public GetUserInfoWEB()
        {
            user = UserInfoStatic.UserID;
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
            if (UserInfoStatic.ipAdd != null) // 非游客
            {
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
            }
            else //游客
            {
            	info["name"] = "游客";
            	info["sex"] = "1";
            	info["age"] = "";
            	info["area"] = "未知";
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
                /*
                medal["ALLC"] = 0;
                medal["ALLT"] = 0;
                medal["ALLE"] = 0;
                */
            }
            
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

            String sqlStr = "SELECT * FROM users_property WHERE User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                MySqlDataReader myRead = cmd.ExecuteReader();
                if (myRead.Read())
                {
                    mInfo = new Hashtable();
                    mInfo["ALLC"] = myRead["CourseAll"];
                    mInfo["ALLT"] = myRead["TrainAll"];
                    mInfo["ALLE"] = myRead["EPAll"];
                }
            }
            catch (MySqlException ex)
            {
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return mInfo;
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userInfo"></param>
        public bool OnUpdateUserInfo(Hashtable userInfo)
        {

            return false;

            String sqlStr = "UPDATE users SET  ";
            sqlStr += "USER_Name=?name,  ";
            if (userInfo["pwd"].ToString() != "NoChange")
            {
                sqlStr += "  USER_PassWrd=?pwd,  ";
            }
            sqlStr += "USER_Sex=?sex,USER_Birthday=?age,USER_WorkYear=?workyear,USER_WorkArea=?workarea,";
            sqlStr += "USER_PoliceType=?worktype,USER_MedicalRecord=?mr WHERE User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                cmd.Parameters.Add("?name", MySqlDbType.VarChar).Value = userInfo["name"];
                if (userInfo["pwd"].ToString() != "NoChange")
                {
                    cmd.Parameters.Add("?pwd", MySqlDbType.VarChar).Value = userInfo["pwd"];
                }
                cmd.Parameters.Add("?sex", MySqlDbType.Int32).Value = userInfo["sex"];
                if (String.IsNullOrEmpty(userInfo["age"].ToString()))
                {
                    cmd.Parameters.Add("?age", MySqlDbType.DateTime).Value = null;
                }
                else
                {
                    cmd.Parameters.Add("?age", MySqlDbType.DateTime).Value = userInfo["age"];
                }


                cmd.Parameters.Add("?workyear", MySqlDbType.Int32).Value = userInfo["workyear"];
                cmd.Parameters.Add("?workarea", MySqlDbType.VarChar).Value = userInfo["workarea"];
                cmd.Parameters.Add("?worktype", MySqlDbType.VarChar).Value = userInfo["worktype"];
                cmd.Parameters.Add("?mr", MySqlDbType.VarChar).Value = userInfo["mr"];
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("用户更新信息时出错：" + ex.Message + "\n");
                return false;
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
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
                    MessageBox.Show("网络异常，请稍后重试");
                }
                else if (resp.data.success != "1")
                {
                    MessageBox.Show(resp.data.message);
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
                MessageBox.Show(ex.Message);
            }
            return notice;
        }
    }
}
