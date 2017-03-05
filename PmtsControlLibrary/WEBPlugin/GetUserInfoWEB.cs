using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace PmtsControlLibrary.DBPlugin
{
    class GetUserInfoWEB
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
            retHas["o"] = Math.Floor(0.1);
            retHas["r"] = Math.Floor(0.1);
            retHas["t"] = Math.Floor(0.1);
            retHas["e"] = Math.Floor(0.1);
            retHas["w"] = Math.Floor(0.1);
            retHas["hs"] = Math.Floor(0.1);
            return retHas;

            String sqlStr = "SELECT Observe, Rember, Thinking, Emotion, Willpower, HRVScore FROM users_property WHERE User_ID =?User_ID";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            cmd.Parameters.Add("?User_ID", MySqlDbType.VarChar).Value = user;//user;
            if (DBCon.State == System.Data.ConnectionState.Closed)
            {
                DBCon.Open();
            }
            try
            {
                MySqlDataReader read = cmd.ExecuteReader();
                read.Read();
                retHas["o"] = Math.Floor(Convert.ToSingle(read["Observe"]));
                retHas["r"] = Math.Floor(Convert.ToSingle(read["Rember"]));
                retHas["t"] = Math.Floor(Convert.ToSingle(read["Thinking"]));
                retHas["e"] = Math.Floor(Convert.ToSingle(read["Emotion"]));
                retHas["w"] = Math.Floor(Convert.ToSingle(read["Willpower"]));
                retHas["hs"] = Math.Floor(Convert.ToSingle(read["HRVScore"]));
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("用户信息和雷达图读取SQL：" + ex.Message + "\n");
            }
            finally
            {
                DBCon.Clone();
                DBCon.Dispose();
                cmd.Dispose();
            }
            return retHas;
        }
        /// <summary>
        /// 取得用户基本信息
        /// </summary>
        /// <returns></returns>
        public Hashtable GetUserInfoByUID()
        {
            Hashtable info = Common.CommonUtils.getUserInfoHashTableFromStatic();
            return info;

            String sqlStr = "SELECT * FROM users as u LEFT JOIN users_property as up ON u.User_ID=up.User_ID  WHERE u.User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = user;
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                MySqlDataReader myRead = cmd.ExecuteReader();
                if (myRead.Read())
                {
                    info = new Hashtable();
                    info["name"] = myRead["USER_Name"];
                    info["sex"] = myRead["USER_Sex"];
                    info["age"] = myRead["USER_Birthday"];
                    info["area"] = myRead["USER_WorkArea"];
                    info["pType"] = myRead["USER_PoliceType"];
                    info["wYear"] = myRead["USER_WorkYear"];
                    info["wArea"] = myRead["USER_WorkArea"];
                    info["O"] = myRead["Observe"];
                    info["R"] = myRead["Rember"];
                    info["T"] = myRead["Thinking"];
                    info["E"] = myRead["Emotion"];
                    info["W"] = myRead["Willpower"];
                    info["HRVS"] = myRead["HRVScore"];
                    info["mr"] = myRead["USER_MedicalRecord"];
                }
            }
            catch (MySqlException ex)
            {

            }
            finally
            {
                DBCon.Close();
                DBCon.Dispose();
                cmd.Dispose();
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
