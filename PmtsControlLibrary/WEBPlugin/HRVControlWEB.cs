using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Windows;
using PmtsControlLibrary.Common;
using HeartWavesSDK.Model;
using HeartWavesSDK.Common;

namespace PmtsControlLibrary.WEBPlugin
{
    public class HRVControlWEB
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public HRVControlWEB()
        {
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
            try
            {
                var request = new DeleteRecordRequest()
                {
                    user_id = UserInfoStatic.UserInfo.id,
                    r_id = string.Join(",", ids.ToArray())

                };
                var resp = HeartWavesSDK.API.APIClient._DeleteRecord(request);

                if (null == resp || null == resp.data)
                {
                    MessageBox.Show("网络异常，请稍后重试");
                }
                else if (resp.data.success == "1")
                {
                    MessageBox.Show(resp.data.message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 根据历史记录ID取得详细信息
        /// </summary>
        /// <returns></returns>
        public Hashtable GetHistoryByID(int SID)
        {
            Hashtable hInfo = new Hashtable();

            try
            {
                var request = new RecordDetailRequest()
                {
                    user_id = UserInfoStatic.UserInfo.id,
                    r_id = Convert.ToString(SID)
                };
                var resp = HeartWavesSDK.API.APIClient._GetRecordDetail(request);

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
                    if (resp.data.datas != null)
                    {
                        hInfo["fMean"] = resp.data.datas.fmean;
                        hInfo["HRVScore"] = resp.data.datas.hrvscore;
                        hInfo["score"] = resp.data.datas.synthesisscore;
                        hInfo["Pressure"] = resp.data.datas.pressureindex;
                        hInfo["adjust"] = resp.data.datas.deflatingindex;
                        hInfo["stable"] = resp.data.datas.stabilityindex;
                        hInfo["report"] = resp.data.datas.report;
                        hInfo["NB"] = resp.data.datas.nb;
                        hInfo["StartTime"] = resp.data.datas.s_time;
                        var hrvdata = CommonUtils.getArrayListFromJson(resp.data.datas.hrvdata);
                        hInfo["Time"] = hrvdata.Count / 2.0;
                        hInfo["hrvData"] = resp.data.datas.hrvdata;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            try
            {
                var request = new RecordDetailRequest()
                {
                    user_id = UserInfoStatic.UserInfo.id,
                    r_id = Convert.ToString(SID)
                };
                var resp = HeartWavesSDK.API.APIClient._GetRecordDetail(request);

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
                    if (resp.data.datas != null)
                    {
                        foreach (var entity in resp.data.datas.hrvdata)
                        {
                            Hashtable markInfo = new Hashtable();
                            markInfo["Time"] = resp.data.datas.s_time;
                            markInfo["Content"] = entity;
                            markInfo["DateTime"] = resp.data.datas.s_time;
                            markList.Add(markInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return markList;
        }

        /// <summary>
        /// 常量列表和历史记录列表查询
        /// </summary>
        /// <param name="where">常量列表的条件</param>
        public ArrayList GetConstAndHistoryListData(int timeType = 0, int mood = 0, string type = "1", string pageNum = "0")
        {
            ArrayList retArr = new ArrayList();
            try
            {
                var request = new GetRecordRequest()
                {
                    user_id = UserInfoStatic.UserInfo.id,
                    type = type,
                    pageNum = pageNum
                };
                var resp = HeartWavesSDK.API.APIClient._GetRecord(request);

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
                    if (resp.data.data != null)
                    {
                        foreach (var entity in resp.data.data)
                        {
                            UserHrvRecord hrvRecord = new UserHrvRecord();
                            hrvRecord.HRVData = CommonUtils.getArrayListFromJson(entity.hrvdata);
                            hrvRecord.EPData = CommonUtils.getArrayListFromJson(entity.epdata);
                            hrvRecord.MarkData = CommonUtils.getArrayListFromJson(entity.hrvmark);
                            hrvRecord.PWRData = CommonUtils.getArrayListFromJson(entity.ibidata);
                            hrvRecord.TimeData.Add(entity.fmean);
                            hrvRecord.TimeData.Add(entity.fstddev);
                            hrvRecord.TimeData.Add(entity.fsdnn);
                            hrvRecord.TimeData.Add(entity.frmssd);
                            hrvRecord.TimeData.Add(entity.fsd);
                            hrvRecord.TimeData.Add(entity.fsdsd);
                            hrvRecord.TimeData.Add(entity.fpnn);
                            hrvRecord.FreData.Add(entity.tp);
                            hrvRecord.FreData.Add(entity.vlf);
                            hrvRecord.FreData.Add(entity.lf);
                            hrvRecord.FreData.Add(entity.hf);
                            hrvRecord.FreData.Add(entity.lhr);
                            hrvRecord.FreData.Add(entity.lfnorm);
                            hrvRecord.FreData.Add(entity.hfnorm);
                            hrvRecord.AnsBalance = Convert.ToDouble(entity.nb);
                            hrvRecord.Pressure = Convert.ToDouble(entity.pressureindex);
                            hrvRecord.Adjust = Convert.ToDouble(entity.deflatingindex);
                            hrvRecord.Stable = Convert.ToDouble(entity.stabilityindex);
                            hrvRecord.Score = Convert.ToDouble(entity.synthesisscore);
                            hrvRecord.HrvScore = Convert.ToDouble(entity.hrvscore);
                            var hrvdata = CommonUtils.getArrayListFromJson(entity.hrvdata);
                            hrvRecord.TimeLength = Convert.ToDouble(hrvdata.Count / 2.0);
                            hrvRecord.StartTime = CommonUtils.getDateTime(entity.s_time);
                            hrvRecord.EndTime = CommonUtils.getDateTime(entity.endtime);
                            hrvRecord.RecordType = Convert.ToInt32(entity.recordtype);
                            hrvRecord.Mood = string.IsNullOrWhiteSpace(entity.mood) ? 0 : Convert.ToInt32(entity.mood);
                            hrvRecord.Report = Convert.ToString(entity.report);
                            retArr.Add(hrvRecord);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return retArr;
        }

        /// <summary>
        /// 更新EP完成状态
        /// </summary>
        public void UpEpLevel()
        {
        }

    }
}