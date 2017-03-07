using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PmtsControlLibrary.DBPlugin
{
    public class ScaleTableDB
    {
        private MySqlConnection DBCon = null;

        public ScaleTableDB()
        {
            DbConnection();
        }
        private void DbConnection()
        {
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
        /// 取得用户量表列表
        /// </summary>
        /// <returns></returns>
        public ArrayList GetScaleTableList()
        {
            ArrayList stList = new ArrayList();
            String sqlStr = "SELECT * FROM users_scaletable WHERE User_ID=?uid AND TableIsOpen=1";
            //String sqlStr = "SELECT * FROM users_scaletable WHERE User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserInfo.id;
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable stInfo = new Hashtable();
                    stInfo["sacleID"] = myRead["TableID"];
                    stInfo["scaleName"] = myRead["User_ScaleName"];
                    stInfo["resultIsShow"] = myRead["TableIsShow"];
                    stList.Add(stInfo);
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
            return stList;
        }
        /// <summary>
        /// 保存量表测量结果
        /// </summary>
        /// <param name="Xml">返回完整的xml信息</param>
        /// <param name="ScaleTableID">量表ID</param>
        /// <param name="isShow">是否显示结果</param>
        /// <param name="resultID">返回量表结果的ID</param>
        public void OnSaveSacleTableReslut(String Xml,int ScaleTableID,int isShow,int resultID)
        {
            String sqlStr = "INSERT INTO scaletableresult VALUES (?strid, ?stid, ?uid, ?xml, ?isShow, now())";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserInfo.id;
                cmd.Parameters.Add("?strid", MySqlDbType.Int32).Value = resultID;
                cmd.Parameters.Add("?stid", MySqlDbType.Int32).Value = ScaleTableID;
                cmd.Parameters.Add("?isShow", MySqlDbType.Int32).Value = isShow;
                cmd.Parameters.Add("?xml", MySqlDbType.Text).Value = Xml;
                cmd.ExecuteNonQuery();
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
        }

        /// <summary>
        /// 返回量表结果ID
        /// </summary>
        /// <returns></returns>
        public int GetReslutID()
        {
            int id = 0;
            String sqlStr = "SELECT count( * ) +1 AS num FROM scaletableresult WHERE UserID = ?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserInfo.id;
                id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("取得量表结果ID:"+ex.Message+"\n");
            }
            finally
            {
                DBCon.Close();
                DBCon.Dispose();
                cmd.Dispose();
            }
            return id;
        }
        /// <summary>
        /// 取得量表结果列表
        /// </summary>
        /// <param name="scaleTableID"></param>
        /// <returns></returns>
        public ArrayList GetResultList(int scaleTableID)
        {
            ArrayList rList = new ArrayList();
            String sqlStr = "SELECT * FROM scaletableresult WHERE ScaleTableID=?stid AND UserID=?uid AND IsShow=1";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?stid", MySqlDbType.Int32).Value = scaleTableID;
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserInfo.id;
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable info = new Hashtable();
                    info["time"] = myRead["FinishedTime"];
                    info["xml"] = myRead["ResultXML"];
                    rList.Add(info);
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
            return rList;
        }
    }
}
