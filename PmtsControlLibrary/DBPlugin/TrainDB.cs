using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PmtsControlLibrary.DBPlugin
{
    public class TrainDB
    {
        private MySqlConnection DBCon = null;

        public TrainDB()
        {
            DBConnection();
        }
        /// <summary>
        /// 数据库链接
        /// </summary>
        private void DBConnection()
        {
            if (DBCon == null || DBCon.State == System.Data.ConnectionState.Broken)
            {
                String dbip = UserInfoStatic.ipAdd;//meg["DBIP"].ToString();
                String dbid = UserInfoStatic.dbUser;//meg["DBUser"].ToString();
                String dbpwd = UserInfoStatic.dbPwd; //meg["DBPwd"].ToString();
                String dbname = UserInfoStatic.dbName;//meg["DBName"].ToString();
                String conStr = string.Format("Database={3};Data Source={0};User Id={1};Password={2}", dbip, dbid, dbpwd, dbname);
                DBCon = new MySqlConnection(conStr);
            }
        }

        /// <summary>
        /// 取得操作训练中心是否开启
        /// </summary>
        /// <returns></returns>
        public ArrayList GetHandleTrainIsOpen()
        {
            var tArr = new ArrayList();

            var nameList = new List<string>() { 
                "神笔马良",
                "冒险岛",
                "射箭",
               
            };


            for (int i = 0; i < 3; i++)
            {
                var table = new Hashtable();
                table.Add("tid", (30 + i).ToString());
                table.Add("tname", nameList[i].ToString());
                table.Add("open", "1");
                table.Add("gateOpen", "20");
                table.Add("gateNum", "15");
                tArr.Add(table);
            }
            return tArr;
        }

         /// <summary>
        /// 取得仿真训练是否开启
        /// </summary>
        /// <returns></returns>
        public ArrayList GetSimulatioTrainIsOpen()
        {
            var tArr = new ArrayList();

            var nameList = new List<string>() { 
                "亲子教育",
                "情感生活",
                "情绪压力",
                "恐惧",
                "爆刀",
                "暴力",
            };


            for (int i = 0; i < 6; i++)
            {
                var table = new Hashtable();
                table.Add("tid", (20+i).ToString());
                table.Add("tname", nameList[i].ToString());
                table.Add("open", "1");
                table.Add("gateOpen", "20");
                table.Add("gateNum", "15");
                tArr.Add(table);
            }
            return tArr;
        }
        /// <summary>
        /// 取得训练是否开启
        /// </summary>
        /// <returns></returns>
        public ArrayList GetTrainIsOpen()
        {
            var tArr = new ArrayList();

            var nameList = new List<string>() { 
                "挪来移去",
                "看图绘画",
                "边缘视力",
                "多彩球",
                "方向瞬记",
                "以此类推",
            };

            for (int i = 0; i < 6; i++)
            {
                var table = new Hashtable();
                table.Add("tid",(i + 1).ToString());
                table.Add("tname", nameList[i].ToString());
                table.Add("open","1");
                table.Add("gateOpen","20");
                table.Add("gateNum","15");
                tArr.Add(table);
            }

            return tArr;

            String sqlStr = "SELECT * FROM users_trainrecord WHERE User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable tmp = new Hashtable();
                    tmp["tid"] = myRead["User_TrainID"];
                    tmp["tname"] = myRead["User_TrainName"];
                    tmp["open"] = myRead["User_TrainIsOpen"];
                    tmp["gateOpen"] = myRead["User_TrainGateInfo"];
                    tmp["gateNum"] = myRead["User_TrainGateNum"];
                    tArr.Add(tmp);
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("读取用户训练是否开始时出错：" + ex.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return tArr;
        }
        /// <summary>
        /// 取得训练记录的条数
        /// </summary>
        /// <returns></returns>
        public ArrayList GetTrainHistoryNum()
        {
            ArrayList hArr = new ArrayList();
            String sqlStr = "SELECT Train_ID , count( * ) as num FROM train_record WHERE User_ID = ?uid  GROUP BY Train_ID ";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable tmp = new Hashtable();
                    tmp["num"] = myRead["num"];
                    tmp["tid"] = myRead["Train_ID"];
                    hArr.Add(tmp);
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("读取训练记录条数时出错：" + ex.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return hArr;
        }
        /// <summary>
        /// 训练结束后更新五项维度数据到用户属性表
        /// </summary>
        public void OnUpdateTrainDataToUserPara()
        {
            String sqlStr = "UPDATE users_property SET Observe=?o,Rember=?r,Thinking=?t,Emotion=?e,Willpower=?w WHERE User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?o", MySqlDbType.Double).Value = UserInfoStatic.O;
                cmd.Parameters.Add("?r", MySqlDbType.Double).Value = UserInfoStatic.R;
                cmd.Parameters.Add("?t", MySqlDbType.Double).Value = UserInfoStatic.T;
                cmd.Parameters.Add("?e", MySqlDbType.Double).Value = UserInfoStatic.E;
                cmd.Parameters.Add("?w", MySqlDbType.Double).Value = UserInfoStatic.W;
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("更新五项维度数值时出错：" + ex.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
        }
        /// <summary>
        /// 训练结束后保存训练结果到历史记录列表
        /// </summary>
        public void OnInsertTrainToHistory(Hashtable trInfo)
        {
            String sqlStr = "INSERT INTO train_record ";
 //           sqlStr += " (Train_RecordID, Train_ID, User_ID, Train_Gate, Train_Date, Train_Score, Train_DiffNum, Train_Observe, Train_Rember, Train_Thinking, Train_Emotion, Train_Willpower, Base_Observe, Base_Rember, Base_Thinking, Base_Emotion, Base_Willpower) VALUES ";
 //           sqlStr += " (?trid,?tid,?uid,?gate,now(),?score,?diff,?o,?r,?t,?e,?w,?bo,?br,?bt,?be,?bw)";
            sqlStr += " (Train_ID, User_ID, Train_Gate, Train_Date, Train_Score, Train_DiffNum, Train_Observe, Train_Rember, Train_Thinking, Train_Emotion, Train_Willpower, Base_Observe, Base_Rember, Base_Thinking, Base_Emotion, Base_Willpower) VALUES ";
            sqlStr += " (?tid,?uid,?gate,now(),?score,?diff,?o,?r,?t,?e,?w,?bo,?br,?bt,?be,?bw)";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
 //               cmd.Parameters.Add("?trid", MySqlDbType.Int32).Value = trInfo["trid"];
                cmd.Parameters.Add("?tid", MySqlDbType.Int32).Value = trInfo["tid"];
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                cmd.Parameters.Add("?gate", MySqlDbType.Int32).Value = trInfo["gate"];
                cmd.Parameters.Add("?score", MySqlDbType.Double).Value = trInfo["score"];
                cmd.Parameters.Add("?diff", MySqlDbType.Double).Value = trInfo["diff"];
                cmd.Parameters.Add("?o", MySqlDbType.Double).Value = trInfo["o"];
                cmd.Parameters.Add("?r", MySqlDbType.Double).Value = trInfo["r"];
                cmd.Parameters.Add("?t", MySqlDbType.Double).Value = trInfo["t"];
                cmd.Parameters.Add("?e", MySqlDbType.Double).Value = trInfo["e"];
                cmd.Parameters.Add("?w", MySqlDbType.Double).Value = trInfo["w"];
                cmd.Parameters.Add("?bo", MySqlDbType.Double).Value = trInfo["bo"];
                cmd.Parameters.Add("?br", MySqlDbType.Double).Value = trInfo["br"];
                cmd.Parameters.Add("?bt", MySqlDbType.Double).Value = trInfo["bt"];
                cmd.Parameters.Add("?be", MySqlDbType.Double).Value = trInfo["be"];
                cmd.Parameters.Add("?bw", MySqlDbType.Double).Value = trInfo["bw"];
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("插入训练历史记录时出错：" + ex.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
        }
        /// <summary>
        /// 取得训练历史记录的ID
        /// </summary>
        /// <returns></returns>
        public int GetTrainRecordID()
        {
            int trid = 0;
            String sqlStr = "SELECT Train_RecordID FROM train_record  ORDER BY Train_RecordID DESC LIMIT 1 ";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                trid = Convert.ToInt32(cmd.ExecuteScalar())+1;
            }
            catch(MySqlException ex)
            {
                System.Diagnostics.Debug.Write("取得训练记录ID时出错：" + ex.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return trid;
        }

        /// <summary>
        /// 更新训练全部完成状态
        /// </summary>
        public void OnUpdateTrainAll()
        {
            String sqlStr = "UPDATE users_property SET    TrainAll=1 WHERE User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                cmd.ExecuteNonQuery();
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
        }
        /// <summary>
        /// 按照训练的ID取得训练的历史记录
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public ArrayList GetHistoryByTID(int tid)
        {
            ArrayList hList = new ArrayList();
            String sqlStr = "SELECT * FROM train_record JOIN conttrain ON TrainID=Train_ID WHERE Train_ID =?tid AND User_ID = ?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                cmd.Parameters.Add("?tid", MySqlDbType.Int32).Value = tid;
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable tmp = new Hashtable();
                    tmp["tid"] = myRead["Train_ID"];
                    tmp["tname"] = myRead["TrainName"];
                    tmp["totalGate"] = myRead["TrainGateNum"];
                    tmp["nowGate"] = myRead["Train_Gate"];
                    tmp["trid"] = myRead["Train_RecordID"];
                    tmp["time"] = myRead["Train_Date"];
                    tmp["gate"] = myRead["Train_Gate"];
                    tmp["s"] = myRead["Train_Score"];
                    tmp["o"] = myRead["Train_Observe"];
                    tmp["r"] = myRead["Train_Rember"];
                    tmp["t"] = myRead["Train_Thinking"];
                    tmp["e"] = myRead["Train_Emotion"];
                    tmp["w"] = myRead["Train_Willpower"];
                    tmp["to"] = Convert.ToDouble(myRead["Base_Observe"]) + Convert.ToDouble(myRead["Train_Observe"]);
                    tmp["tr"] = Convert.ToDouble(myRead["Base_Rember"]) + Convert.ToDouble(myRead["Train_Rember"]);
                    tmp["tt"] = Convert.ToDouble(myRead["Base_Thinking"]) + Convert.ToDouble(myRead["Train_Thinking"]);
                    tmp["te"] = Convert.ToDouble(myRead["Base_Emotion"]) + Convert.ToDouble(myRead["Train_Emotion"]);
                    tmp["tw"] = Convert.ToDouble(myRead["Base_Willpower"]) + Convert.ToDouble(myRead["Train_Willpower"]);
                    ArrayList ts = new ArrayList();
                    ts.Add(Convert.ToDouble(myRead["Base_Observe"]) + Convert.ToDouble(myRead["Train_Observe"]));
                    ts.Add(Convert.ToDouble(myRead["Base_Rember"]) + Convert.ToDouble(myRead["Train_Rember"]));
                    ts.Add(Convert.ToDouble(myRead["Base_Thinking"]) + Convert.ToDouble(myRead["Train_Thinking"]));
                    ts.Add(Convert.ToDouble(myRead["Base_Emotion"]) + Convert.ToDouble(myRead["Train_Emotion"]));
                    ts.Add(Convert.ToDouble(myRead["Base_Willpower"]) + Convert.ToDouble(myRead["Train_Willpower"]));
                    tmp["totals"] = ts;
                    tmp["diff"] = myRead["Train_DiffNum"];
                    hList.Add(tmp);
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("取得训练历史记录时出错：" + ex.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return hList;
        }
        /// <summary>
        /// 根据训练id和记录id删除一条记录
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="trid"></param>
        public void OnDeleteRecordOne(int tid, int trid)
        {
            String sqlStr = "DELETE FROM train_record WHERE Train_RecordID=?trid AND Train_ID=?tid AND User_ID=?uid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?trid", MySqlDbType.Int32).Value = trid;
                cmd.Parameters.Add("?tid", MySqlDbType.Int32).Value = tid;
                cmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = UserInfoStatic.UserID;
                cmd.ExecuteNonQuery();
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
        }

        /// <summary>
        /// 取得脱敏用的资源列表
        /// </summary>
        /// <returns></returns>
        public ArrayList GetResourcesList()
        {
            ArrayList rList = new ArrayList();
            String sqlStr = "SELECT * FROM train_resources order by resOrder ";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable rInfo = new Hashtable();
                    rInfo["type"] = myRead["type"];
                    rInfo["path"] = myRead["path"];
                    rInfo["order"] = myRead["resOrder"];
                    rList.Add(rInfo);
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("取得脱敏列表时出错："+ex.Message+"\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }

            return rList;
        }
    }
}
