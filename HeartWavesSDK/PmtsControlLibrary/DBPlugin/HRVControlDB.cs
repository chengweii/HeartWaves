using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;

namespace PmtsControlLibrary.DBPlugin
{
    class HRVControlDB
    {
        private MySqlConnection DBCon = null;
        private String user = "";
        private Hashtable meg = new Hashtable();
        /// <summary>
        /// 构造函数
        /// </summary>
        public HRVControlDB(Hashtable SystemMeg)
        {
            meg = SystemMeg;
            user = SystemMeg["UserID"].ToString();
            DBConnection();
        }
        /// <summary>
        /// 数据库链接
        /// </summary>
        private void DBConnection()
        {
            if (DBCon == null || DBCon.State == System.Data.ConnectionState.Broken)
            {
                String dbip = meg["DBIP"].ToString();
                String dbid = meg["DBUser"].ToString();
                String dbpwd = meg["DBPwd"].ToString();
                String dbname = meg["DBName"].ToString();
                String conStr = string.Format("Database={3};Data Source={0};User Id={1};Password={2}", dbip, dbid, dbpwd, dbname);
                DBCon = new MySqlConnection(conStr);
            }
        }
        /// <summary>
        /// 插入HRV测量和相关数据
        /// </summary>
        /// <param name="HRVData">HRV数据</param>
        /// <param name="EPData">EP数据</param>
        /// <param name="HRVMark">事件标记</param>
        /// <param name="HRVMathData">14项数据相关内容</param>
        public void OnInsertHRVDataAndEpData(ArrayList HRVData, ArrayList EPData, ArrayList HRVMark, Hashtable HRVMathData)
        {
            ///插入14项数据，数据准备中，start
   //         int scaleID = this.GetTableNum("scale", "Scale_ID");
            int scaleID;
   //         String hrvMathStr = " INSERT INTO  scale (Scale_ID , Scale_UserID ,Scale_MHRT ,Scale_SDHRT ,Scale_SDNN ,Scale_RMSSD ,Scale_SD ,Scale_SDSD ,Scale_PNN50 ,Scale_TP ,";
            String hrvMathStr = " INSERT INTO  scale ( Scale_UserID ,Scale_MHRT ,Scale_SDHRT ,Scale_SDNN ,Scale_RMSSD ,Scale_SD ,Scale_SDSD ,Scale_PNN50 ,Scale_TP ,";
            hrvMathStr += "Scale_VLF ,Scale_LF ,Scale_HF ,Scale_LFHF ,Scale_LFnorm ,Scale_HFnorm ,Scale_Diffic ,Scale_Report ,Scale_Pressure ,Scale_Adjust ,Scale_Stable ,";
            hrvMathStr += "Scale_Score ,Scale_NerveActivity ,Scale_HeartVitalityUpper ,Scale_HeartVitalityDowner ,Scale_TotalTime ,Scale_EndTime ,Scale_HRVScore,Scale_TimeType,Scale_Mood,Scale_StartTime )VALUES(";
  //          hrvMathStr += scaleID + ",?Scale_UserID,?Scale_MHRT,?Scale_SDHRT,?Scale_SDNN,?Scale_RMSSD,?Scale_SD,";
            hrvMathStr += "?Scale_UserID,?Scale_MHRT,?Scale_SDHRT,?Scale_SDNN,?Scale_RMSSD,?Scale_SD,";
            hrvMathStr += "?Scale_SDSD,?Scale_PNN50,?Scale_TP,?Scale_VLF,?Scale_LF,?Scale_HF,?Scale_LFHF,";
            hrvMathStr += "?Scale_LFnorm,?Scale_HFnorm,?Scale_Diffic,?Scale_Report,?Scale_Pressure,?Scale_Adjust,?Scale_Stable,";
            hrvMathStr += "?Scale_Score,?Scale_NerveActivity,?Scale_HeartVitalityUpper,?Scale_HeartVitalityDowner,?Scale_TotalTime,?Scale_EndTime,?Scale_HRVScore,";
            hrvMathStr += "?Scale_TimeType,?Scale_Mood,?Scale_StartTime);"; 
            hrvMathStr += "Select @@Identity";
            MySqlParameter[] sqlHrvMathParams = new MySqlParameter[]
            {
                    new MySqlParameter("?Scale_UserID", MySqlDbType.VarChar),//用户ID
                    new MySqlParameter("?Scale_MHRT", MySqlDbType.Double)   ,
                    new MySqlParameter("?Scale_SDHRT", MySqlDbType.Double)  ,
                    new MySqlParameter("?Scale_SDNN", MySqlDbType.Double)   ,
                    new MySqlParameter("?Scale_RMSSD", MySqlDbType.Double)  ,
                    new MySqlParameter("?Scale_SD", MySqlDbType.Double)     ,
                    new MySqlParameter("?Scale_SDSD", MySqlDbType.Double)   ,
                    new MySqlParameter("?Scale_PNN50", MySqlDbType.Double)  ,
                    new MySqlParameter("?Scale_TP", MySqlDbType.Double)     ,
                    new MySqlParameter("?Scale_VLF", MySqlDbType.Double)    ,
                    new MySqlParameter("?Scale_LF", MySqlDbType.Double)     ,
                    new MySqlParameter("?Scale_HF", MySqlDbType.Double)     ,
                    new MySqlParameter("?Scale_LFHF", MySqlDbType.Double)   ,
                    new MySqlParameter("?Scale_LFnorm", MySqlDbType.Double) ,
                    new MySqlParameter("?Scale_HFnorm", MySqlDbType.Double) ,
                    new MySqlParameter("?Scale_Diffic", MySqlDbType.Int16)  ,
                    new MySqlParameter("?Scale_Report", MySqlDbType.Text)   ,
                    new MySqlParameter("?Scale_Pressure", MySqlDbType.Double),     
                    new MySqlParameter("?Scale_Adjust", MySqlDbType.Double)   ,    
                    new MySqlParameter("?Scale_Stable", MySqlDbType.Double)    ,   
                    new MySqlParameter("?Scale_Score", MySqlDbType.Double)      ,
                    new MySqlParameter("?Scale_NerveActivity", MySqlDbType.Double),
                    new MySqlParameter("?Scale_HeartVitalityUpper", MySqlDbType.Double) ,
                    new MySqlParameter("?Scale_HeartVitalityDowner", MySqlDbType.Double),
                    new MySqlParameter("?Scale_HRVScore", MySqlDbType.Double)       ,
                    new MySqlParameter("?Scale_TotalTime", MySqlDbType.Double)      ,
                    new MySqlParameter("?Scale_EndTime", MySqlDbType.DateTime)      ,
                    new MySqlParameter("?Scale_TimeType",MySqlDbType.Int32),
                    new MySqlParameter("?Scale_Mood",MySqlDbType.Int32),
                    new MySqlParameter("?Scale_StartTime",MySqlDbType.DateTime)
            };
            sqlHrvMathParams[0].Value = user;//用户ID
            sqlHrvMathParams[1].Value = HRVMathData["fMean"];//平均心率
            sqlHrvMathParams[2].Value = HRVMathData["fStdDev"];//即时心率的标准差
            sqlHrvMathParams[3].Value = HRVMathData["fSDNN"];//SDNN心跳间隔的标准差
            sqlHrvMathParams[4].Value = HRVMathData["fRMSSD"];//
            sqlHrvMathParams[5].Value = HRVMathData["fSD"];
            sqlHrvMathParams[6].Value = HRVMathData["fSDSD"];
            sqlHrvMathParams[7].Value = HRVMathData["fPNN"];
            sqlHrvMathParams[8].Value = HRVMathData["tp"];
            sqlHrvMathParams[9].Value = HRVMathData["vlf"];
            sqlHrvMathParams[10].Value = HRVMathData["lf"];
            sqlHrvMathParams[11].Value = HRVMathData["hf"];
            sqlHrvMathParams[12].Value = HRVMathData["lhr"];
            sqlHrvMathParams[13].Value = HRVMathData["lfnorm"];
            sqlHrvMathParams[14].Value = HRVMathData["hfnorm"];
            sqlHrvMathParams[15].Value = 0;//难度
            sqlHrvMathParams[16].Value = HRVMathData["report"];//评价报告
            sqlHrvMathParams[17].Value = HRVMathData["Pressure"];//压力
            sqlHrvMathParams[18].Value = HRVMathData["adjust"];//调节
            sqlHrvMathParams[19].Value = HRVMathData["stable"];//稳定
            sqlHrvMathParams[20].Value = HRVMathData["score"];//综合得分
            sqlHrvMathParams[21].Value = HRVMathData["NB"];//神经兴奋性
            sqlHrvMathParams[22].Value = Convert.ToDouble(HRVMathData["fMean"]) + (Convert.ToSingle(HRVMathData["fStdDev"]) * 1.96);
            sqlHrvMathParams[23].Value = Convert.ToDouble(HRVMathData["fMean"]) - (Convert.ToSingle(HRVMathData["fStdDev"]) * 1.96);
            sqlHrvMathParams[24].Value = HRVMathData["HRVScore"];//根据EP计算的得分
            sqlHrvMathParams[25].Value = HRVMathData["Time"];//总共测量时间
            sqlHrvMathParams[26].Value = HRVMathData["EndTime"];//结束时间
            sqlHrvMathParams[27].Value = HRVMathData["TimeType"];//HRV检测时间类型
            sqlHrvMathParams[28].Value = HRVMathData["Mood"];//工作心理
            sqlHrvMathParams[29].Value = HRVMathData["StartTime"];//开始时间
            MySqlCommand sqlCmd = new MySqlCommand();
            MySqlTransaction sqlTra;
            if (DBCon.State == System.Data.ConnectionState.Closed)
            {
                DBCon.Open();
            }
            sqlTra = DBCon.BeginTransaction();
            try
            {
                sqlCmd.Connection = DBCon;
                sqlCmd.Transaction = sqlTra;
                //发送SqlCmmmand
                sqlCmd.CommandText = hrvMathStr;
                sqlCmd.Parameters.AddRange(sqlHrvMathParams);
 //               sqlCmd.ExecuteNonQuery();
                scaleID = Convert.ToInt32(sqlCmd.ExecuteScalar());
                //-----------------------------------------------------HRV数据---------------------------------------------------------------------///
                for (int h = 0; h < HRVData.Count; h++)
                {
                    String sqlHrvStr = "INSERT INTO scalehrv (Scale_ID, Scale_HRV, HRV_Availability, HRV_time, HRV_index) VALUES  (";
                    sqlHrvStr += scaleID + ",?hrvData" + h + ",0,null,?hrvIndex" + h + ")";
                    sqlCmd.CommandText = sqlHrvStr;
                    MySqlParameter[] sqlHrvParams = new MySqlParameter[]
                    {
                         new MySqlParameter("?hrvData"+h, MySqlDbType.Double),
                         new MySqlParameter("?hrvIndex"+h, MySqlDbType.Int32)
                    };
                    sqlHrvParams[0].Value = HRVData[h];
                    sqlHrvParams[1].Value = h;
                    sqlCmd.Parameters.AddRange(sqlHrvParams);
                    sqlCmd.ExecuteNonQuery();
                }
                //-----------------------------------------------------EP数据---------------------------------------------------------------------///
                for (int e = 0; e < EPData.Count; e++)
                {
                    String sqlEpStr = "INSERT INTO scaleep(`Scale_ID`, `Scale_EP`, `EP_index`) VALUES (";
                    sqlEpStr += scaleID + ",?epData" + e + ",?epIndex" + e + ")";
                    sqlCmd.CommandText = sqlEpStr;
                    sqlCmd.Parameters.Add("?epData" + e, MySqlDbType.Double).Value = EPData[e];
                    sqlCmd.Parameters.Add("?epIndex" + e, MySqlDbType.Int32).Value = e;
                    sqlCmd.ExecuteNonQuery();
                }
                //-----------------------------------------------------HRV事件标记---------------------------------------------------------------------///
                if (HRVMark.Count > 0)
                {
                    for (int m = 0; m < HRVMark.Count; m++)
                    {
                        Hashtable tmpHash = (Hashtable)HRVMark[m];
                        String sqlMarkStr = "INSERT INTO `scalemark`(`Scale_ID`, `Mark_Index`, `Mark_Remark`,`Mark_Datetime`) VALUES ( " + scaleID + " , ";
                        sqlMarkStr += "?MarkIndex" + m + ",?Remark" + m + ",?MarkDateTime" + m + ")";
                        sqlCmd.CommandText = sqlMarkStr;
                        sqlCmd.Parameters.Add("?MarkIndex" + m, MySqlDbType.Int32).Value = Convert.ToInt32(tmpHash["Time"]) * 2;
                        sqlCmd.Parameters.Add("?Remark" + m, MySqlDbType.Text).Value = tmpHash["Content"];
                        sqlCmd.Parameters.Add("?MarkDateTime" + m, MySqlDbType.DateTime).Value = tmpHash["DateTime"];
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                //-----------------------------------------------------频谱数据---------------------------------------------------------------------///
                ArrayList tmpIBIData = (ArrayList)HRVMathData["FS"];
                for (int ibi = 0; ibi < tmpIBIData.Count; ibi++)
                {
                    String sqlIBIStr = "INSERT INTO `scaleibi`(`Scale_ID`, `Scale_IBI`, `IBI_index`) VALUES (" + scaleID + " ,";
                    sqlIBIStr += "?IBIData" + ibi + ",?IBIindex" + ibi + ")";
                    sqlCmd.CommandText = sqlIBIStr;
                    sqlCmd.Parameters.Add("?IBIData" + ibi, MySqlDbType.Double).Value = tmpIBIData[ibi];
                    sqlCmd.Parameters.Add("?IBIindex" + ibi, MySqlDbType.Int32).Value = ibi;
                    sqlCmd.ExecuteNonQuery();
                }
                sqlTra.Commit();
                sqlCmd.Dispose();

                //--------------------------------------------------更新HRV得分
                sqlCmd.Dispose();
                String oldHRVSql = "SELECT HRVScore FROM users_property WHERE User_ID=?uid";
                sqlCmd = new MySqlCommand(oldHRVSql, DBCon);
                sqlCmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = user;
                int newHrvScore = Convert.ToInt32(sqlCmd.ExecuteScalar()) + Convert.ToInt32(HRVMathData["HRVScore"]);
                String newHRVSql = "UPDATE users_property SET  HRVScore=?newScore WHERE User_ID=?uid";
                sqlCmd.Dispose();
                sqlCmd = new MySqlCommand(newHRVSql, DBCon);
                sqlCmd.Parameters.Add("?uid", MySqlDbType.VarChar).Value = user;
                sqlCmd.Parameters.Add("?newScore", MySqlDbType.Int32).Value = newHrvScore;
                sqlCmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                System.Diagnostics.Debug.Write("插入HRV以及HRV相关数据，数据库错误：" + e.Message + "\n");
                sqlTra.Rollback();
            }
            finally
            {
                DBCon.Close();
                System.Diagnostics.Debug.Write("数据库当前状态：" + DBCon.State + "\n");
                DBCon.Dispose();
                System.Diagnostics.Debug.Write("数据库操作结束\n");
                sqlCmd.Dispose();
            }
        }
        /// <summary>
        /// 常量列表和历史记录列表查询
        /// </summary>
        /// <param name="where">常量列表的条件</param>
        public ArrayList GetConstAndHistoryListData(int timeType = 0, int mood = 0)
        {
            ArrayList retArr = new ArrayList();
            String sqlStr = "select Scale_ID,Scale_MHRT,Scale_HRVScore,Scale_Score,Scale_Pressure,Scale_Adjust,Scale_Stable,Scale_StartTime,Scale_TotalTime from Scale   ";
            sqlStr += "where Scale_UserID=?user ";
            if (timeType > 0)
            {
                sqlStr += "  AND Scale_TimeType=?TimeType  ";
            }
            if (mood > 0)
            {
                sqlStr += "    AND( Scale_Mood>=?Mood1  AND  Scale_Mood<=?Mood2)  ";
            }
            sqlStr += "  order by   Scale_EndTime";
            System.Diagnostics.Debug.Write("查询用SQL语句：" + sqlStr + "\n");
            if (DBCon.State == System.Data.ConnectionState.Closed)
            {
                DBCon.Open();
            }
            MySqlCommand myCmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                myCmd.Parameters.Add("?user", MySqlDbType.VarChar).Value = user;
                if (timeType > 0)
                {
                    myCmd.Parameters.Add("?TimeType", MySqlDbType.Int32).Value = timeType;
                }
                if (mood > 0)
                {
                    switch (mood)
                    {
                        case 1:
                            myCmd.Parameters.Add("?Mood1", MySqlDbType.Int32).Value = 1;
                            myCmd.Parameters.Add("?Mood2", MySqlDbType.Int32).Value = 10;
                            break;
                        case 2:
                            myCmd.Parameters.Add("?Mood1", MySqlDbType.Int32).Value = 11;
                            myCmd.Parameters.Add("?Mood2", MySqlDbType.Int32).Value = 25;
                            break;
                        case 3:
                            myCmd.Parameters.Add("?Mood1", MySqlDbType.Int32).Value = 26;
                            myCmd.Parameters.Add("?Mood2", MySqlDbType.Int32).Value = 40;
                            break;
                        case 4:
                            myCmd.Parameters.Add("?Mood1", MySqlDbType.Int32).Value = 41;
                            myCmd.Parameters.Add("?Mood2", MySqlDbType.Int32).Value = 60;
                            break;
                        case 5:
                            myCmd.Parameters.Add("?Mood1", MySqlDbType.Int32).Value = 61;
                            myCmd.Parameters.Add("?Mood2", MySqlDbType.Int32).Value = 75;
                            break;
                        case 6:
                            myCmd.Parameters.Add("?Mood1", MySqlDbType.Int32).Value = 76;
                            myCmd.Parameters.Add("?Mood2", MySqlDbType.Int32).Value = 90;
                            break;
                        case 7:
                            myCmd.Parameters.Add("?Mood1", MySqlDbType.Int32).Value = 91;
                            myCmd.Parameters.Add("?Mood2", MySqlDbType.Int32).Value = 100;
                            break;
                    }
                }
                MySqlDataReader myRead = myCmd.ExecuteReader();
                int i = 0;
                while (myRead.Read())
                {
                    Hashtable tmp = new Hashtable();
                    tmp["arrayIndex"] = i;//
                    tmp["mhrt"] = myRead["Scale_MHRT"];//平均心率
                    tmp["hrvScore"] = myRead["Scale_HRVScore"];//hrv得分
                    tmp["totalScore"] = myRead["Scale_Score"];//综合得分
                    tmp["pressure"] = myRead["Scale_Pressure"];//压力
                    tmp["adjust"] = myRead["Scale_Adjust"];//调节
                    tmp["stable"] = myRead["Scale_Stable"];//稳定
                    tmp["id"] = myRead["Scale_ID"];//检测的ID
                    tmp["startTime"] = myRead["Scale_StartTime"];//开始时间
                    tmp["totalTime"] = myRead["Scale_TotalTime"];//总共测量时间
                    retArr.Add(tmp);
                    i++;
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("常量列表查询:" + ex.Message + "\n");
            }
            finally
            {
                DBCon.Clone();
                DBCon.Dispose();
                myCmd.Dispose();
            }
            return retArr;
        }
        /// <summary>
        /// 删除hrv相关数据
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteHrvData(ArrayList ids)
        {
            if (DBCon.State == System.Data.ConnectionState.Closed)
            {
                DBCon.Open();
            }
            MySqlCommand myCmd = new MySqlCommand();
            MySqlTransaction myTra;
            myTra = DBCon.BeginTransaction();
            myCmd.Connection = DBCon;
            myCmd.Transaction = myTra;
            try
            {
                for (int i = 0; i < ids.Count; i++)
                {
                    int id = Convert.ToInt32(ids[i]);
                    String scaleStr = "DELETE FROM scale WHERE Scale_ID = ?Scale_ID";
                    myCmd.CommandText = scaleStr;
                    myCmd.Parameters.Add("?Scale_ID", MySqlDbType.Int32).Value = id;
                    myCmd.ExecuteNonQuery();
                    String hrvStr = "DELETE FROM scalehrv WHERE Scale_ID = ?Scale_ID";
                    myCmd.CommandText = hrvStr;
                    //myCmd.Parameters.Add("?Scale_ID", MySqlDbType.Int32).Value = id;
                    myCmd.ExecuteNonQuery();
                    String epStr = "DELETE FROM scaleep WHERE Scale_ID = ?Scale_ID";
                    myCmd.CommandText = epStr;
                    //myCmd.Parameters.Add("?Scale_ID", MySqlDbType.Int32).Value = id;
                    myCmd.ExecuteNonQuery();
                    String markStr = "DELETE FROM scalemark WHERE Scale_ID=?Scale_ID";
                    myCmd.CommandText = markStr;
                    //myCmd.Parameters.Add("?Scale_ID", MySqlDbType.Int32).Value = id;
                    myCmd.ExecuteNonQuery();
                    String ibiStr = "DELETE FROM scaleibi WHERE Scale_ID=?Scale_ID";
                    myCmd.CommandText = ibiStr;
                    //myCmd.Parameters.Add("?Scale_ID", MySqlDbType.Int32).Value = id;
                    myCmd.ExecuteNonQuery();
                    myCmd.Parameters.Clear();
                }
                myTra.Commit();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.Write("" + ex.Message + "\n");
            }
            finally
            {
                DBCon.Close();
                DBCon.Dispose();
                myCmd.Dispose();
                myTra.Dispose();
            }
        }
        /// <summary>
        /// 更新EP完成状态
        /// </summary>
        public void UpEpLevel()
        {
            String sqlStr = "UPDATE users_property SET    EPAll=1 WHERE User_ID=?uid";
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
        /// 取得表的ID
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columName"></param>
        /// <returns></returns>
        private int GetTableNum(string tableName, string columName)
        {
            int num = 0;
            String sql = "select " + columName + " from " + tableName + " order by " + columName + " desc limit 1;";
            System.Diagnostics.Debug.Write("SQL：" + sql + "\n");
            MySqlCommand cmd = new MySqlCommand(sql, DBCon);
            if (DBCon.State == System.Data.ConnectionState.Closed)
            {
                DBCon.Open();
            }
            try
            {
                num = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                System.Diagnostics.Debug.Write("获取table的ID：" + num + "\n");
            }
            catch (MySqlException e)
            {
                System.Diagnostics.Debug.Write("获取table的ID，数据库错误：" + e.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return num;
        }

        /// <summary>
        /// 根据历史记录ID取得详细信息
        /// </summary>
        /// <returns></returns>
        public Hashtable GetHistoryByID(int SID)
        {
            Hashtable hInfo = new Hashtable();
            String sqlStr = "SELECT * FROM scale  Where  Scale_ID=?SID";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?SID", MySqlDbType.Int32).Value = SID;
                MySqlDataReader myRead = cmd.ExecuteReader();
                if (myRead.Read())
                {

                    hInfo["fMean"] = myRead["Scale_MHRT"];//平均心率
                    hInfo["HRVScore"] = myRead["Scale_HRVScore"];//hrv得分
                    hInfo["score"] = myRead["Scale_Score"];//综合得分
                    hInfo["Pressure"] = myRead["Scale_Pressure"];//压力
                    hInfo["adjust"] = myRead["Scale_Adjust"];//调节
                    hInfo["stable"] = myRead["Scale_Stable"];//稳定
                    hInfo["report"] = myRead["Scale_Report"];//评价报告
                    hInfo["NB"] = myRead["Scale_NerveActivity"];//神经兴奋性
                    hInfo["StartTime"] = myRead["Scale_StartTime"];//开始时间
                }
                myRead.Close();
                String hrvStr = "SELECT * FROM scalehrv Where Scale_ID=?SID order by HRV_index";
                cmd.CommandText = hrvStr;
                myRead = cmd.ExecuteReader();
                ArrayList hrvData = new ArrayList();
                while (myRead.Read())
                {
                    hrvData.Add(myRead["Scale_HRV"]);
                }
                hInfo["Time"] = hrvData.Count / 2.0;
                hInfo["hrvData"] = hrvData;
            }
            catch (MySqlException e)
            {
                System.Diagnostics.Debug.Write("取得历史记录详情时出错:" + e.Message + "\n");
            }
            finally
            {
                cmd.Dispose();
                DBCon.Close();
                DBCon.Dispose();
            }
            return hInfo;
        }

        /// <summary>
        /// 按照SID取得事件标记
        /// </summary>
        /// <param name="SID"></param>
        /// <returns></returns>
        public ArrayList GetMarkByID(int SID)
        {
            ArrayList markList = new ArrayList();
            String sqlStr = "SELECT * FROM scalemark WHERE Scale_ID=?sid";
            MySqlCommand cmd = new MySqlCommand(sqlStr, DBCon);
            try
            {
                if (DBCon.State == System.Data.ConnectionState.Closed)
                {
                    DBCon.Open();
                }
                cmd.Parameters.Add("?sid", MySqlDbType.Int32).Value = SID;
                MySqlDataReader myRead = cmd.ExecuteReader();
                while (myRead.Read())
                {
                    Hashtable markInfo = new Hashtable();
                    markInfo["Time"] = myRead["Mark_Index"];
                    markInfo["Content"] = myRead["Mark_Remark"];
                    markInfo["DateTime"] = myRead["Mark_Datetime"];
                    markList.Add(markInfo);
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

            return markList;

        }
    }
}