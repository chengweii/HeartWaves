using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;

namespace PmtsControlLibrary.DBPlugin
{
    class GetUserInfo
    {
        private String user = "";
        private Hashtable meg = new Hashtable();
        private MySqlConnection DBCon = null;

        public GetUserInfo(Hashtable SystemMeg)
        {
            meg = SystemMeg;
            user = SystemMeg["UserID"].ToString();
            DBConnection();
        }
        public GetUserInfo()
        {
            user = UserInfoStatic.UserID;
            DBConnection();
        }
        /// <summary>
        /// 数据库链接
        /// </summary>
        private void DBConnection()
        {
            return;

            if (DBCon == null || DBCon.State == System.Data.ConnectionState.Broken)
            {
                String dbip = UserInfoStatic.ipAdd;
                String dbid = UserInfoStatic.dbUser;
                String dbpwd = UserInfoStatic.dbPwd;
                String dbname = UserInfoStatic.dbName;
                String conStr = string.Format("Database={3};Data Source={0};User Id={1};Password={2}", dbip, dbid, dbpwd, dbname);
                DBCon = new MySqlConnection(conStr);
            }
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
            return "";

            String notice = "";
            String sqlStr = "SELECT * FROM systemnotice";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                if (cmd.ExecuteScalar() != null)
                {
                    notice = cmd.ExecuteScalar().ToString();
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
            return notice;
        }
    }
}
