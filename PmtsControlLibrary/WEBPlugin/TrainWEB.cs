using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PmtsControlLibrary.WEBPlugin
{
    public class TrainWEB
    {
        private MySqlConnection DBCon = null;

        public TrainWEB()
        {
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
                table.Add("tid", (20 + i).ToString());
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
                table.Add("tid", (i + 1).ToString());
                table.Add("tname", nameList[i].ToString());
                table.Add("open", "1");
                table.Add("gateOpen", "20");
                table.Add("gateNum", "15");
                tArr.Add(table);
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
            return hArr;
        }

        /// <summary>
        /// 训练结束后更新五项维度数据到用户属性表
        /// </summary>
        public void OnUpdateTrainDataToUserPara()
        {
        }

        /// <summary>
        /// 训练结束后保存训练结果到历史记录列表
        /// </summary>
        public void OnInsertTrainToHistory(Hashtable trInfo)
        {
        }

        /// <summary>
        /// 取得训练历史记录的ID
        /// </summary>
        /// <returns></returns>
        public int GetTrainRecordID()
        {
            int trid = 0;
            return trid;
        }

        /// <summary>
        /// 更新训练全部完成状态
        /// </summary>
        public void OnUpdateTrainAll()
        {
        }

        /// <summary>
        /// 按照训练的ID取得训练的历史记录
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public ArrayList GetHistoryByTID(int tid)
        {
            ArrayList hList = new ArrayList();
            return hList;
        }

        /// <summary>
        /// 根据训练id和记录id删除一条记录
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="trid"></param>
        public void OnDeleteRecordOne(int tid, int trid)
        {
        }

        /// <summary>
        /// 取得脱敏用的资源列表
        /// </summary>
        /// <returns></returns>
        public ArrayList GetResourcesList()
        {
            ArrayList rList = new ArrayList();
            return rList;
        }
    }
}
