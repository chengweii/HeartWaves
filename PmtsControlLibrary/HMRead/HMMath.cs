using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Resources;
/*
 * 2012-2-13 李孜博
 * HRV测量结束后结算14项数据、几种得分和生成评价报告
 */ 
namespace pmts_net.HMRead
{
    class HMMath
    {
        private ArrayList vecTHR = new ArrayList();//
        private ArrayList vecFS = new ArrayList();//频谱数据(显示)
        private Hashtable vecFSData = new Hashtable();//14项数据相关内容（频谱相关）（显示）
        private Hashtable vecHRTData = new Hashtable();//14项数相关内容（心率相关）（显示）
        private ArrayList vecIBI = new ArrayList();//原始数据
        private Hashtable vecAppraiseData = new Hashtable();//调节指数和稳定指数,0为压力指数，1为调节指数，2为稳定指数,3为综合得分（显示）
        private ArrayList vecZeroScore = new ArrayList();//0等级时EP的得分
        private string vecReport = "";//数据报告

        //读取频谱数据
        [DllImportAttribute("HDRead.DLL", EntryPoint = "?BuildFSFromHRV_@CHDRead@@QAEPAMPAMAAHH@Z")]
        unsafe private static extern float* BuildData(float* Y,ref int size,int hSize);
        
        public HMMath()
        {
        }
        public HMMath(ArrayList HRVData, ArrayList EPData)
        {
            BuildIBIFromHRV(HRVData);//把HRV还原成为原始数据，并且过滤伪信号
            BuildFSFromHRV(HRVData);//计算频谱
            BuildTHRFromEP(EPData);//计算EP的变化，得到THR
            BuildFSDataFromFS(vecFS);//计算14项数据中部分
            BuildHRTDataFromIBI(vecIBI);//计算14项数据中部分
            CalcAdjust();//调节指数
            CalcStable();//稳定指数
            CalcScore();//综合得分
            GetReport();//生成评价报告
        }
        public Hashtable HRVCalc()
        {
            Hashtable calcs = new Hashtable();
            calcs["FS"] = vecFS;//频谱,arraylist类型
            calcs["report"] = vecReport;//评价报告，string类型
            calcs["Pressure"] = vecAppraiseData[0];//压力指数,float类型
            calcs["adjust"] = vecAppraiseData[1];//调节指数，float类型
            calcs["stable"] = vecAppraiseData[2];//稳定指数，float类型
            calcs["score"] = vecAppraiseData[3];//综合指数，float类型
            calcs["fMean"] = vecHRTData["fMean"];//平均心率,float类型
            calcs["fStdDev"] = vecHRTData["fStdDev"];	//标准差,float类型
            calcs["fSDNN"] = vecHRTData["fSDNN"];	//总域值,float类型
            calcs["fRMSSD"] = vecHRTData["fRMSSD"];	//极低频分量,float类型
            calcs["fSD"] = vecHRTData["fSD"];		//低频分量,float类型
            calcs["fSDSD"] = vecHRTData["fSDSD"];	//高频分量,float类型
            calcs["fPNN"] = vecHRTData["fPNN"];		//低频高频比,float类型
            calcs["NB"] = vecFSData["lrr"];//神经兴奋性,float类型
            calcs["tp"] = vecFSData["tp"];
            calcs["vlf"] = vecFSData["vlf"];
            calcs["lf"] = vecFSData["lf"];
            calcs["hf"] = vecFSData["hf"];
            calcs["lhr"] = vecFSData["lhr"];
            calcs["lfnorm"] = vecFSData["lfnorm"];
            calcs["hfnorm"] = vecFSData["hfnorm"];
            return calcs;
        }
        /// <summary>
        /// 从hrv计算原始数据并且滤波
        /// </summary>
        /// <param name="HRVArray">HRV数组</param>
        /// <returns></returns>
        private bool BuildIBIFromHRV(ArrayList HRVArray)
        {
            for (int i = 0; i < HRVArray.Count; i++)
            {
                if ((Single)HRVArray[i] > 40)
                {
                    vecIBI.Add(60000.0f / (Single)HRVArray[i]);
                }
            }
            return true;
        }

        /// <summary>
        /// 从EP计算调整指数用的基数
        /// </summary>
        /// <param name="EPArray">EP的数组</param>
        /// <returns>vecTHR中写入以0难度为基础计算的数据</returns>
        private bool BuildTHRFromEP(ArrayList EPArray)
        {
            if (EPArray.Count == 0)
            {
                return false;
            }
            else
            {
                vecTHR.Clear();
                int nState = 0;
                int PreState = 0;
                float AScore = 0;
                for (int i = 0; i < EPArray.Count; i++)
                {
                    if ((Single)EPArray[i] < 0.6)
                    {
                        nState = 0;
                    }
                    else if ((Single)EPArray[i] < 1.5)
                    {
                        nState = 1;
                    }
                    else
                    {
                        nState = 2;
                    }
                    if (nState == 2 && PreState == 0) AScore += 1;//加一
                    else if (nState == 1 && PreState == 0) AScore += 1;//加一
                    else if (nState == 0 && PreState == 0) AScore += -2;//减二
                    else if (nState == 2 && PreState == 1) AScore += 1;
                    else if (nState == 1 && PreState == 1) AScore += 1;
                    else if (nState == 0 && PreState == 1) AScore += -1;
                    else if (nState == 2 && PreState == 2) AScore += 2;
                    else if (nState == 1 && PreState == 2) AScore += 1;
                    else if (nState == 0 && PreState == 2) AScore += -2;
                    if (AScore < 0)
                        AScore = 0;
                    PreState = nState;
                    vecTHR.Add(nState);
                    vecZeroScore.Add(AScore);
                }
                return true;
            }
        }
        /// <summary>
        /// 从HRV计算频谱
        /// </summary>
        /// <param name="HRVArray">存储HRV数据的数组</param>
        /// <returns>向vecFS中写入频谱数据</returns>
        private bool BuildFSFromHRV(ArrayList HRVArray)
        {
            ////////////计算原始数据部分，从HRV还原成原始数据
            ArrayList m_vecGenIBI = new ArrayList();
            for (int i = 0; i < HRVArray.Count; ++i)
            {
                m_vecGenIBI.Add(60000.0f / (Single)HRVArray[i]);
            }
            ////////////开始计算频谱
            if (m_vecGenIBI.Count < 128)
            {
                return false;
            }
            else
            {
                try
                {
                    vecFS.Clear();//清空存储频谱用的数组
                    int hSize = m_vecGenIBI.Count;
                    unsafe
                    {
                        float* Y = stackalloc float[hSize];//= new float[m_vecGenIBI.Count];
                        for (int i = 0; i < m_vecGenIBI.Count; i++)
                        {
                            Y[i] = (Single)m_vecGenIBI[i];
                        }
                        float* FS;
                        int size = 0;
                        FS = BuildData(Y, ref size, hSize);
                        for (int i = 0; i < size; i++)
                        {
                            vecFS.Add(FS[i]);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write("频谱错误信息为:" + ex.Message + "\n");
                    System.Diagnostics.Debug.Write("频谱导致错误的控件:" + ex.Source + "\n");
                    System.Diagnostics.Debug.Write("频谱引发当前异常的方法:" + ex.TargetSite + "\n");
                    return false;
                }
            }
        }
        /// <summary>
        /// 从频谱数据中计算相关数据（14项数据中的部分内容）
        /// </summary>
        /// <param name="FSData">计算后的频谱</param>
        private bool BuildFSDataFromFS(ArrayList FSData)
        {
            int NF = 128 / 2;
            int t = 6;
            int t1 = 2;
            int t2 = 9;
            int t3 = 25;

            float tp = 0;
            float vlf = 0;	//极低频
            float lf = 0;	//低频
            float hf = 0;	//高频
            float lhr = 0;	//高低频比
            float lfnorm = 0;//归一化低频分量 
            float hfnorm = 0;//归一化高频分量
            float left = 0;//频谱左侧部分
            float right = 0;//频谱右侧部分
            float lrr = 0;//频谱左右比例
            if (FSData.Count < NF)
            {
                vecFSData["tp"] = 0;
                vecFSData["vlf"] = 0;
                vecFSData["lf"] = 0;
                vecFSData["hf"] = 0;
                vecFSData["lhr"] = 0;
                vecFSData["lfnorm"] = 0;
                vecFSData["hfnorm"] = 0;
                vecFSData["lrr"] = 0;
                //m_arrAppraiseData[0] = 0.0f;//压力指数
                return false;
            }

            for (int i = 0; i < NF; i++)
            {
                //tp += PWR[i];
                tp += (Single)FSData[i];
                if (i < t1)
                    vlf += (Single)FSData[i];
                if (i > t1 && i < t2)
                    lf += (Single)FSData[i];
                if (i > t2 && i < t3)
                    hf += (Single)FSData[i];
                if (i < t)
                    left += (Single)FSData[i];
                if (i > t && i < t3)
                    right += (Single)FSData[i];
            }

            vlf += (Single)FSData[t1] / 2;
            lf += (Single)FSData[t1] / 2;
            lf += (Single)FSData[t2] / 2;
            hf += (Single)FSData[t2] / 2;
            hf += (Single)FSData[t3] / 2;
            left += (Single)FSData[t] / 2;
            right += (Single)FSData[t] / 2;

            tp = vlf + lf + hf;

            if (hf > 0)
                lhr = lf / hf;
            else if (lf > 0)
                lhr = 10000;
            if (tp - vlf > 0)
            {
                lfnorm = lf * 100.0f / (tp - vlf);
                hfnorm = hf * 100.0f / (tp - vlf);
            }
            else
            {
                lfnorm = 0;
                hfnorm = 0;
            }
            lrr = left / right;
            vecFSData["tp"] = tp;
            vecFSData["vlf"] = vlf;
            vecFSData["lf"] = lf;
            vecFSData["hf"] = hf;
            vecFSData["lhr"] = lhr;
            vecFSData["lfnorm"] = lfnorm;
            vecFSData["hfnorm"] = hfnorm;
            vecFSData["lrr"] = lrr;
            vecAppraiseData[0] = ((vlf / tp + lf / tp / 3.0f) * 100.0f);//压力指数
            if ((Single)vecAppraiseData[0] <= 0.0f)
            {
                vecAppraiseData[0] = 0.0f;
            }
            if ((Single)vecAppraiseData[0] >= 100.0f)
            {
                vecAppraiseData[0] = 100.0f;
            }
            if (!((Single)vecAppraiseData[0] >= 0 && (Single)vecAppraiseData[0] <= 100))
            {
                vecAppraiseData[0] = 0.0f;
            }
            return true;
        }
        /// <summary>
        /// 计算心率相关数据
        /// </summary>
        /// <param name="IBIData">原始数据数组</param>
        /// <returns></returns>
        private bool BuildHRTDataFromIBI(ArrayList IBIData)
        {
            float fMin = 1000;		//最小心率
            float fMax = 0;			//最大心率
            float fVariance = 0;	//方差
            float fMean = 0;		//平均心率
            float fStdDev = 0;		//标准差

            float fSDNN=0;			//总域值
            float fRMSSD=0;			//极低频分量
            float fSD=0;				//低频分量
            float fSDSD=0;			//高频分量
            float fPNN=0;				//低频高频比

            int i;
            int shift = 0;
            int start = 5;
            int end = IBIData.Count - shift - 1;

            float average = 0;
            float fValue = 0;
            float fIBI=0, fMeanIBI=0;
            float averageIBI = 0;
            float fCount = 0;

            for (i = start; i <= end; i++)
            {
                fValue = 60000.0f / (Single)IBIData[i];
                fIBI = (Single)IBIData[i + shift];
                if (fValue < fMin)
                    fMin = fValue;
                if (fValue > fMax)
                    fMax = fValue;

                average += fValue;
                averageIBI += fIBI;
                fCount++;
            }

            fMean = average / fCount;
            fMeanIBI = averageIBI / fCount;

            float fVar = 0;
            float fVarIBI = 0;
            for (i = start; i <= end; i++)
            {
                fValue = 60000.0f / (Single)IBIData[i];
                fIBI = (Single)IBIData[i + shift];

                fVar += (fValue - fMean) * (fValue - fMean);
                fVarIBI += (fIBI - fMeanIBI) * (fIBI - fMeanIBI);
            }

            fVariance = fVar / fCount;
            fStdDev = (Single)Math.Sqrt(fVariance);
            fSDNN = (Single)Math.Sqrt(fVarIBI / fCount);

            float fVarSub = 0;
            float fSub = 0;
            float fSDSub = 0;
            float averageSub = 0;
            float fNNCount = 0;

            for (i = start; i < end; i++)
            {
                fSub = (Single)IBIData[i + shift + 1] - (Single)IBIData[i + shift];
                fSDSub += fSub * fSub;
                averageSub += Math.Abs(fSub);
                if (Math.Abs(fSub) > 50)
                    fNNCount++;
            }
            fSD = averageSub / fCount;
            fRMSSD = (Single)Math.Sqrt(fSDSub / fCount);
            fPNN = fNNCount * 100.0f / fCount;
            for (i = start; i < end; i++)
            {
                fSub = Math.Abs((Single)IBIData[i + shift + 1] - (Single)IBIData[i + shift]);
                if (fSub > 0)
                    fVarSub += (fSub - fSD) * (fSub - fSD);
            }
            fSDSD = (Single)Math.Sqrt(fVarSub / fCount);

            vecHRTData["fMean"] = Single.IsNaN(fMean) ? 0 : fMean;	//平均心率
            vecHRTData["fStdDev"] = Single.IsNaN(fStdDev) ? 0 : fStdDev;	//标准差
            vecHRTData["fSDNN"] = Single.IsNaN(fSDNN) ? 0 : fSDNN;	//总域值
            vecHRTData["fRMSSD"] = Single.IsNaN(fRMSSD) ? 0 : fRMSSD;	//极低频分量
            vecHRTData["fSD"] = Single.IsNaN(fSD) ? 0 : fSD;		//低频分量
            vecHRTData["fSDSD"] = Single.IsNaN(fSDSD) ? 0 : fSDSD;	//高频分量
            vecHRTData["fPNN"] = Single.IsNaN(fPNN) ? 0 : fPNN;		//低频高频比
            return true;
        }
        /// <summary>
        /// 计算调节指数
        /// </summary>
        private void CalcAdjust()
        {
            int i;
	        float pure_score,high_score;
	        int count = 0;
	        for( i = 0; i < vecTHR.Count; i ++ )
	        {
		        if((int)vecTHR[i]>= 1 )
			        count ++;
	        }
	        pure_score = count * 1.0f / (vecTHR.Count ) * 60.0f;

	        count = 0;
	        for( i = 0; i < vecTHR.Count; i ++ )
	        {
		        if((int)vecTHR[i] == 2 )
			        count ++;
	        }
	        high_score = count*1.0f/(vecTHR.Count) * 40.0f;
	        vecAppraiseData[1] = pure_score + high_score;
	        if ((Single) vecAppraiseData[1]<=0.0f )
	        {
		        vecAppraiseData[1] = 0.0f;
	        }
	        if ( (Single)vecAppraiseData[1]>=100.0f )
	        {
		        vecAppraiseData[1]=100.0f;
	        }
	        if (!((Single)vecAppraiseData[1]>=0&&(Single)vecAppraiseData[1]<=100) )
	        {
		        vecAppraiseData[1]=0;
	        }
        }
        /// <summary>
        /// 稳定指数
        /// </summary>
        private void CalcStable()
        {
            int count = 0;
            int time;
            time = vecZeroScore.Count * 5;
            int mid_score;
            mid_score = vecZeroScore.Count;
            int flag = 0;
            int reverse_count = 0;
            for (int i = 1; i < vecZeroScore.Count; i++)
            {
                if ((Single)vecZeroScore[i] > (Single)vecZeroScore[i - 1])
                {
                    count++;
                    flag = 1;
                }
                else if ((Single)vecZeroScore[i] < (Single)vecZeroScore[i - 1])
                {
                    if (flag == 1)
                    {
                        reverse_count++;
                        flag = 0;
                    }
                }
            }
            vecAppraiseData[2] = count * 1.0f / (mid_score - 1) * 100.0f;
            vecAppraiseData[2] = (Single)vecAppraiseData[2] * (mid_score / 2.0f - reverse_count) * 1.0f / (mid_score / 2.0f);
            if ((Single)vecAppraiseData[2] <= 0.0f)
            {
                vecAppraiseData[2] = 0.0f;
            }
            if ((Single)vecAppraiseData[2] >= 100.0f)
            {
                vecAppraiseData[2] = 100.0f;
            }
            if (!((Single)vecAppraiseData[2] >= 0 && (Single)vecAppraiseData[2] <= 100))
            {
                vecAppraiseData[2] = 0;
            }
        }
        /// <summary>
        /// 综合得分
        /// </summary>
        private void CalcScore()
        {
            float fStressPart;
            float fAdjustPart;
            float fStablePart;

            int stressStardard = 40;
            if ((Single)vecAppraiseData[0] >= stressStardard)
                fStressPart = (100.0f - ((Single)vecAppraiseData[0] - stressStardard) * 1.5f) * 0.4f;
            else
                fStressPart = (100.0f - (stressStardard - (Single)vecAppraiseData[0]) * 2.0f) * 0.4f;

            fAdjustPart = (Single)vecAppraiseData[1] * 0.4f;
            fStablePart = (Single)vecAppraiseData[2] * 0.2f;
            vecAppraiseData[3] = fStressPart + fAdjustPart + fStablePart;
            if ((Single)vecAppraiseData[3] <= 0.0f)
            {
                vecAppraiseData[3] = 0.0f;
            }
            if ((Single)vecAppraiseData[3] >= 100.0f)
            {
                vecAppraiseData[3] = 100.0f;
            }
        }
        private void GetReport()
        {
            float adjust = (Single)vecAppraiseData[1];
            float stable = (Single)vecAppraiseData[2];
            System.Diagnostics.Debug.Write("调节指数："+adjust+"\n");
            System.Diagnostics.Debug.Write("稳定指数："+stable+"\n");
            int adjustNum = 0;
            int stableNum = 0;
            //调节指数分类
            if (adjust >= 0 && adjust < 20)
            {
                adjustNum = 1;
            }else if (adjust >= 20 && adjust < 40)
            {
                adjustNum = 2;
            }else if (adjust >= 40 && adjust < 60)
            {
                adjustNum = 3;
            }else if (adjust >= 60 && adjust < 80)
            {
                adjustNum = 4;
            }else if (adjust >= 80 && adjust <= 100)
            {
                adjustNum = 5;
            }
            //稳定指数分类
            if (stable >= 0 && stable < 20)
            {
                stableNum = 1;
            }
            else if (stable >= 20 && stable < 30)
            {
                stableNum = 2;
            }
            else if (stable >= 30 && stable < 60)
            {
                stableNum = 3;
            }
            else if (stable >= 60 && stable < 90)
            {
                stableNum = 4;
            }
            else if (stable >= 90 && stable <= 100)
            {
                stableNum = 5;
            }
            System.Diagnostics.Debug.Write("T:" + adjustNum + "\n");
            System.Diagnostics.Debug.Write("W:"+stableNum+"\n");

            ////生成评价报告
            if (adjustNum == 5 && stableNum == 5)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T5W5Z7;
            }//T5W4
            else if (adjustNum == 5 && stableNum == 4)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T5W4Z6;
            }//T4W4
            else if (adjustNum == 4 && stableNum == 4)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T4W4Z5;
            }//T3W5
            else if (adjustNum == 3 && stableNum == 5)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T3W5Z5;
            }//T5W3
            else if (adjustNum == 5 && stableNum == 3)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T5W3Z4;
            }//T4W3
            else if (adjustNum == 4 && stableNum == 3)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T4W3Z4;
            }//T3W4
            else if (adjustNum == 3 && stableNum == 4)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T3W4Z4;
            }//T2W5
            else if (adjustNum == 2 && stableNum == 5)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T2W5Z4;
            }//T3W3
            else if (adjustNum == 3 && stableNum == 3)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T3W3Z3;
            }//T2W4
            else if (adjustNum == 2 && stableNum == 4)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T2W4Z3;
            }//T4W2
            else if (adjustNum == 4 && stableNum == 2)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T4W2Z3;
            }//T1W5
            else if (adjustNum == 1 && stableNum == 5)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T1W5Z3;
            }//T5W1
            else if (adjustNum == 5 && stableNum == 1)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T5W1Z3;
            }//T5W2
            else if (adjustNum == 5 && stableNum == 2)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T5W2Z3;
            }//T3W2
            else if (adjustNum == 3 && stableNum == 2)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T3W2Z2;
            }//T2W3
            else if (adjustNum == 2 && stableNum == 3)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T2W3Z2;
            }//T4W1
            else if (adjustNum == 4 && stableNum == 1)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T4W1Z2;
            }//T1W4
            else if (adjustNum == 1 && stableNum == 4)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T1W4Z2;
            }//T1W1
            else if (adjustNum == 1 && stableNum == 1)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T1W1Z1;
            }//T1W2
            else if (adjustNum == 1 && stableNum == 2)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T1W2Z1;
            }//T1W3
            else if (adjustNum == 1 && stableNum == 3)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T1W3Z1;
            }//T2W1
            else if (adjustNum == 2 && stableNum == 1)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T2W1Z1;
            }//T2W2
            else if (adjustNum == 2 && stableNum == 2)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T2W2Z1;
            }//T3W1
            else if (adjustNum == 3 && stableNum == 1)
            {
                vecReport = PmtsControlLibrary.Properties.Resources.T3W1Z1;
            }

        }
    }
}
