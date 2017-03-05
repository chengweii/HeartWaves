using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Windows;
using PmtsControlLibrary.Common;

namespace PmtsControlLibrary.WEBPlugin
{
    public class HRVControlWEB
    {
        private String user = "";
        private Hashtable meg = new Hashtable();
        /// <summary>
        /// 构造函数
        /// </summary>
        public HRVControlWEB(Hashtable SystemMeg)
        {
            meg = SystemMeg;
            user = SystemMeg["UserID"].ToString();
        }

        /// <summary>
        /// 插入HRV测量和相关数据
        /// </summary>
        /// <param name="HRVData">HRV数据</param>
        /// <param name="EPData">EP数据</param>
        /// <param name="HRVMark">事件标记</param>
        /// <param name="HRVMathData">14项数据相关内容</param>
        public void OnInsertHRVDataAndEpData(ArrayList HRVData, ArrayList EPData, ArrayList HRVMark, Hashtable HRVMathData, ArrayList PPGData, string type)
        {
            try
            {
                var req = new HeartWavesSDK.Model.StopRecordRequest()
                {
                    type = type,
                    id = UserInfoStatic.UserInfo.id,
                    hrvdata = HRVData,
                    epdata = EPData,
                    IBIdata = (ArrayList)HRVMathData["FS"],
                    pulsedata = PPGData,
                    rkind = CommonUtils.ToString(HRVMathData["rkind"]),
                    s_time = CommonUtils.ToString(HRVMathData["StartTime"]),
                    time_length = CommonUtils.ToString(HRVMathData["Time"]),
                    synthesisscore = CommonUtils.ToString(HRVMathData["score"]),
                    deflatingindex = CommonUtils.ToString(HRVMathData["adjust"]),
                    stabilityindex = CommonUtils.ToString(HRVMathData["stable"]),
                    pressureindex = CommonUtils.ToString(HRVMathData["Pressure"]),
                    HRVscore = CommonUtils.ToString(HRVMathData["HRVScore"]),
                    evaluation = CommonUtils.ToString(HRVMathData["evaluation"]),
                    HRVmark = HRVMark,
                    NB = CommonUtils.ToString(HRVMathData["NB"]),
                    fMean = CommonUtils.ToString(HRVMathData["fMean"]),
                    fStdDev = CommonUtils.ToString(HRVMathData["fStdDev"]),
                    fSDNN = CommonUtils.ToString(HRVMathData["fSDNN"]),
                    fRMSSD = CommonUtils.ToString(HRVMathData["fRMSSD"]),
                    fSD = CommonUtils.ToString(HRVMathData["fSD"]),
                    fSDSD = CommonUtils.ToString(HRVMathData["fSDSD"]),
                    fPNN = CommonUtils.ToString(HRVMathData["fPNN"]),
                    tp = CommonUtils.ToString(HRVMathData["tp"]),
                    vlf = CommonUtils.ToString(HRVMathData["vlf"]),
                    lf = CommonUtils.ToString(HRVMathData["lf"]),
                    hf = CommonUtils.ToString(HRVMathData["hf"]),
                    lhr = CommonUtils.ToString(HRVMathData["lhr"]),
                    lfnorm = CommonUtils.ToString(HRVMathData["lfnorm"]),
                    hfnorm = CommonUtils.ToString(HRVMathData["hfnorm"]),
                    left = CommonUtils.ToString(HRVMathData["left"]),
                    right = CommonUtils.ToString(HRVMathData["right"]),
                    lrr = CommonUtils.ToString(HRVMathData["lrr"]),
                    trid = CommonUtils.ToString(HRVMathData["trid"]),
                    tid = CommonUtils.ToString(HRVMathData["tid"]),
                    gate = CommonUtils.ToString(HRVMathData["gate"]),
                    trainscore = CommonUtils.ToString(HRVMathData["trainscore"]),
                    diff = "0",
                    FreData = CommonUtils.ToString(HRVMathData["FreData"]),
                    Level = CommonUtils.ToString(HRVMathData["Level"]),
                    TimeLength = CommonUtils.ToString(HRVMathData["TimeLength"]),
                    RecordType = CommonUtils.ToString(HRVMathData["RecordType"]),
                    Report = CommonUtils.ToString(HRVMathData["report"]),
                    HeartVitalityUpper = CommonUtils.ToString(Convert.ToDouble(HRVMathData["fMean"]) + (Convert.ToSingle(HRVMathData["fStdDev"]) * 1.96)),
                    HeartVitalityDowner = CommonUtils.ToString(Convert.ToDouble(HRVMathData["fMean"]) - (Convert.ToSingle(HRVMathData["fStdDev"]) * 1.96)),
                    TimeType = CommonUtils.ToString(HRVMathData["TimeType"]),
                    EndTime = CommonUtils.ToString(HRVMathData["EndTime"]),
                    Mood = CommonUtils.ToString(HRVMathData["Mood"]),
                };

                var resp = HeartWavesSDK.API.APIClient._StopRecord(req);

                if (null == resp || null == resp.data)
                    MessageBox.Show("网络异常，请稍后重试");
                else
                    MessageBox.Show(resp.data.message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 删除hrv相关数据
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteHrvData(ArrayList ids)
        {
        }

    }
}