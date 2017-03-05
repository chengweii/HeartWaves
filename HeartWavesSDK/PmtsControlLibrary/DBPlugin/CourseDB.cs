using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PmtsControlLibrary.DBPlugin
{
    public class CourseDB
    {
        private MySqlConnection DBCon = null;
        public CourseDB()
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
        /// 取得用户课程列表和详细信息
        /// </summary>
        /// <returns></returns>
        public ArrayList GetCourseInfo()
        {
            ArrayList cInfo = new ArrayList();
            String sqlStr = "SELECT User_ContentMainID,User_ContentMainName FROM users_course WHERE users_UserID = ?uid GROUP BY User_ContentMainID ";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid",MySqlDbType.VarChar).Value=UserInfoStatic.UserID;
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable mainTmp = new Hashtable();
                    mainTmp["mid"] = myRead["User_ContentMainID"];
                    mainTmp["mname"] = myRead["User_ContentMainName"];
                    cInfo.Add(mainTmp);
                }
                myRead.Close();
                myRead.Dispose();
                for (int i = 0; i < cInfo.Count; i++)
                {
                    Hashtable tmp = (Hashtable)cInfo[i];
                    String sqlIsOpenStr = "SELECT * FROM users_course WHERE users_UserID=?uid AND User_ContentMainID=?mid";
                    MySqlCommand openCmd = new MySqlCommand(sqlIsOpenStr, DBCon);
                    openCmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                    openCmd.Parameters.Add("?mid", MySqlDbType.Int32).Value = tmp["mid"];
                    MySqlDataReader openRead = openCmd.ExecuteReader();
                    ArrayList subInfoArr = new ArrayList();
                    while (openRead.Read())
                    {
                        Hashtable subTmp = new Hashtable();
                        subTmp["sid"] = openRead["users_CourseID"];
                        subTmp["sname"] = openRead["users_CourseName"];
                        subTmp["isopen"] = openRead["users_CourseIsOpen"];
                        subInfoArr.Add(subTmp);
                    }
                    tmp["openInfo"] = subInfoArr;
                    openCmd.Dispose();
                    openRead.Close();
                    openRead.Dispose();
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("取得课程开启信息时出错："+ex.Message+"\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return cInfo;
        }
        /// <summary>
        /// 更改课程是否观看
        /// </summary>
        /// <param name="courseInfo"></param>
        public void OnUpCourseLook(Hashtable courseInfo)
        {
            String sqlStr = "UPDATE users_course SET User_CourseLooked=1 WHERE users_UserID=?uid AND users_CourseID=?cid AND User_ContentMainID=?mid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                cmd.Parameters.Add("?cid", MySqlDbType.Int32).Value = courseInfo["cid"];
                cmd.Parameters.Add("?mid", MySqlDbType.Int32).Value = courseInfo["mid"];
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                sqlStr = "SELECT sum(User_CourseLooked) FROM users_course WHERE users_UserID = ?uid";
                cmd = new MySqlCommand(sqlStr, DBCon);
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                int lookedNum = Convert.ToInt32(cmd.ExecuteScalar());
                if (lookedNum >=24)
                {
                    cmd.Dispose();
                    sqlStr = "UPDATE users_property SET    CourseAll=1 WHERE User_ID=?uid";
                    cmd = new MySqlCommand(sqlStr, DBCon);
                    cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("更改课程阅读信息时出错：" + ex.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
        }
    }
}
