using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using HeartWavesSDK.Model;
using System.Windows.Forms;

namespace PmtsControlLibrary.WEBPlugin
{
    public class WorkMoodWEB
    {
        private String user = "";

        public WorkMoodWEB(String UserID, String hostIp)
        {
            user = UserID;
        }
        /// <summary>
        /// 插入工作心理
        /// </summary>
        /// <param name="MoodData"></param>
        public Boolean OnInsertMood(double moodValue, String moodInfo)
        {
            try
            {
                var request = new MoodRequest()
                {
                    user_id = UserInfoStatic.UserInfo.id,
                    moodsocre = Convert.ToString(moodValue),
                    moodremark = moodInfo
                };
                var resp = HeartWavesSDK.API.APIClient._RecordMood(request);

                if (null == resp || null == resp.data)
                {
                    PmtsMessageBox.CustomControl1.Show("网络异常，请稍后重试");
                }
                else if (resp.data.success == "1")
                {
                    //PmtsMessageBox.CustomControl1.Show(resp.data.message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                PmtsMessageBox.CustomControl1.Show(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 取得工作心理日期
        /// </summary>
        /// <param name="selectDate"></param>
        /// <returns></returns>
        public ArrayList OnGetMoodDate(DateTime selectDate)
        {
            ArrayList dateArr = new ArrayList();
            return dateArr;
        }
        /// <summary>
        /// 取得指定日期的工作心理
        /// </summary>
        /// <param name="selectDate"></param>
        /// <returns></returns>
        public List<MoodDataList> OnGetMoodDetail(DateTime selectDate)
        {
            List<MoodDataList> moodList = new List<MoodDataList>();
            try
            {
                var request = new MoodlistRequest()
                {
                    user_id = UserInfoStatic.UserInfo.id,
                    time = selectDate != null ? selectDate.ToString("yyyy-MM-dd") : null
                };
                var resp = HeartWavesSDK.API.APIClient._GetMoodlist(request);

                if (null == resp || null == resp.data)
                {
                    PmtsMessageBox.CustomControl1.Show("网络异常，请稍后重试");
                }
                else if (resp.data.success == "1")
                {
                    foreach (MoodlistDatum entity in resp.data.result)
                    {
                        MoodDataList MoodDataList = new MoodDataList();
                        MoodDataList.Date = entity.moodtime;
                        MoodDataList.MoodText = entity.moodmark;
                        MoodDataList.MoodValue = entity.moodsocre;
                        moodList.Add(MoodDataList);
                    }
                }
            }
            catch (Exception ex)
            {
                PmtsMessageBox.CustomControl1.Show(ex.Message);
            }
            return moodList;
        }
    }
    public class MoodDataList
    {
        public String Date { set; get; }
        public String MoodValue { set; get; }
        public String MoodText { set; get; }
    }
}
