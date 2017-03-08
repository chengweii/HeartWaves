using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;


namespace pmts_net.HMRead
{

    class HDRead
    {
        public ArrayList HRVArr = new ArrayList();
        public ArrayList EPArr = new ArrayList();
        public ArrayList IBIArr =new ArrayList();
        public ArrayList PPGArr = new ArrayList();
        public double NB = 0;
        /*
         * 设备控制部分
         */ 
        //开始设备
        [DllImport("HDRead.DLL", EntryPoint = "?StartDevice_@CHDRead@@QAEHXZ")]
        private static extern bool StartDevice();
        //停止设备
        [DllImport("HDRead.DLL", EntryPoint = "?StopDevice_@CHDRead@@QAEHXZ")]
        private static extern bool StopDevice();
        //读取设备UID
        [DllImport("HDRead.DLL", EntryPoint = "?GetIsRightHD@CHDRead@@QAEHXZ")]
        private static extern int GetIsRightHD();
        //开始读取
        [DllImport("HDRead.DLL", EntryPoint = "?StartRead_@CHDRead@@QAEHXZ")]
        private static extern bool StartRead();
        //得到设备状态
        [DllImport("HDRead.DLL", EntryPoint = "?GetArtifactStatus_@CHDRead@@QAEHXZ")]
        private static extern int GetArtifactStatus();
        //回复设备
        [DllImport("HDRead.DLL", EntryPoint = "?ResumeDriver_@CHDRead@@QAEHXZ")]
        private static extern bool ResumeDriver();
        //设备个数
        [DllImport("HDRead.DLL", EntryPoint = "?GetArtifactCount_@CHDRead@@QAEHXZ")]
        private static extern int GetArtifactCount();
        /*
         * 读取数据部分
         */ 
        //设置阻塞标志
        [DllImport("HDRead.DLL", EntryPoint = "?SetBlockFlag_@CHDRead@@QAEXH@Z")]
        private static extern void SetBlockFlag(bool flg);
        //获取阻塞标志
        [DllImport("HDRead.DLL", EntryPoint = "?GetBlockFlag_@CHDRead@@QAEHXZ")]
        private static extern bool GetBlockFlag();
        //是否有HRV数据可以读取
        [DllImport("HDRead.DLL", EntryPoint = "?GetReadHRV_@CHDRead@@QAEHXZ")]
        private static extern bool GetReadHRV();
        //设置可否读取HRV数据
        [DllImport("HDRead.DLL", EntryPoint = "?SetReadHRV_@CHDRead@@QAEXH@Z")]
        private static extern void SetReadHRV(bool flg);
        //是否有PPG数据可以读取
        [DllImport("HDRead.DLL", EntryPoint = "?GetReadPPG_@CHDRead@@QAEHXZ")]
        private static extern bool GetReadPPG();
        //设置可否读取PPG数据
        [DllImport("HDRead.DLL", EntryPoint = "?SetReadPPG_@CHDRead@@QAEXH@Z")]
        private static extern void SetReadPPG(bool flg);
        //是否有ep值可以读取
        [DllImport("HDRead.DLL", EntryPoint = "?GetReadEP_@CHDRead@@QAEHXZ")]
        private static extern bool GetReadEP();
        //设置是否可以读取EP数据
        [DllImport("HDRead.DLL", EntryPoint = "?SetReadEP_@CHDRead@@QAEXH@Z")]
        private static extern void SetReadEP(bool flg);
        //是否有频谱数据可以读取(方法命名为历史遗留问题，IBI应为原始数据)
        [DllImport("HDRead.DLL", EntryPoint = "?GetReadIBI_@CHDRead@@QAEHXZ")]
        private static extern bool GetReadIBI();
        //设置是否可以读取频谱数据
        [DllImport("HDRead.DLL", EntryPoint = "?SetReadIBI_@CHDRead@@QAEXH@Z")]
        private static extern void SetReadIBI(bool flg);
        //取得原始数据的个数
        [DllImport("HDRead.DLL", EntryPoint = "?GetIBICount@CHDRead@@QAEHXZ")]
        private static extern int GetIBICount();
        /*非安全*/
        //读取HRV数据
        [DllImportAttribute("HDRead.DLL", EntryPoint = "?GetPackHrt_@CHDRead@@QAEPAMAAH@Z")]
        unsafe private static extern float* GetPackHrv(ref int index);
        //读取EP数据
        [DllImportAttribute("HDRead.DLL", EntryPoint = "?GetPackEP_@CHDRead@@QAEPAMAAH@Z")]
        unsafe private static extern float* GetPackEP(ref int index);
        //读取频谱数据
        [DllImportAttribute("HDRead.DLL", EntryPoint = "?GetPackIBI_@CHDRead@@QAEPAMAAH@Z")]
        unsafe private static extern float* GetPackIBI(ref int index);
        //读取PPG数据
        [DllImportAttribute("HDRead.DLL", EntryPoint = "?GetPackPPG_@CHDRead@@QAEPAMAAH@Z")]
        unsafe private static extern float* GetPackPPG(ref int index);
        
        /// <summary>
        /// 读取HRV以及相关值
        /// </summary>
        public void GetHRV()
        {
            HRVArr = new ArrayList();
            EPArr = new ArrayList();
            IBIArr =new ArrayList();
            PPGArr = new ArrayList();
            NB = 0;
            try
            {
                if (!GetBlockFlag())
                {
                    SetBlockFlag(true);
                    //读取HRV
                    if (GetReadHRV())
                    {
                        unsafe
                        {
                            Int32 index = 0;
                            float* hrv; ;
                            hrv = GetPackHrv(ref index);
                            if (index > 0)
                            {
                                if (GetIBICount() >= 10)
                                {
                                    for (int i = 0; i < index; i++)
                                    {
                                        if (hrv[i] == -1)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            HRVArr.Add(hrv[i]);
                                        }
                                    }
                                }
                            }
                        }
                        SetReadHRV(false);
                    }
                    //读取EP
                    if (GetReadEP())
                    {
                        unsafe
                        {
                            Int32 index = 0;
                            float* ep;
                            ep = GetPackEP(ref index);
                            for (int i = 0; i < index; i++)
                            {
                                EPArr.Add(ep[i]);
                            }
                        }
                        SetReadEP(false);
                    }
                    //读取频谱，并计算即时频谱
                    if (GetReadIBI())
                    {
                        unsafe
                        {
                            Int32 index = 0;
                            float* sp;
                            sp = GetPackIBI(ref index);
                            double leftIBI = 0;
                            double rightIBI = 0;
                            for (int i = 0; i < index; i++)
                            {
                                if (sp[i] == -1)
                                    break;
                                IBIArr.Add(sp[i]);
                            }
                            for (int i = 0; i < IBIArr.Count; i++)
                            {
                                if (i < 6)
                                {
                                    leftIBI += (float)IBIArr[i];
                                }
                                if (i > 6 && i < 25)
                                {
                                    rightIBI += (float)IBIArr[i];
                                }
                            }
                            leftIBI += (float)IBIArr[6] / 2;
                            rightIBI += (float)IBIArr[6] / 2;
                            //NB = leftIBI / rightIBI;
                            NB = rightIBI / leftIBI;
                        }
                        SetReadIBI(false);
                    }
                    //读取PPG
                    if (GetReadPPG())
                    {
                        unsafe
                        {
                            Int32 index = 0;
                            float* sp;
                            sp = GetPackPPG(ref index);
/*
                            FileStream fs = new FileStream("ppg.txt", FileMode.OpenOrCreate);
                            fs.Position = fs.Length;
                            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
*/
                            for (int i = 0; i < index; i++)
                            {
                                if (sp[i] == -1)
                                    break;
                                PPGArr.Add(sp[i]);
//                                sw.Write(sp[i].ToString() + "\n");
                            }

//                            sw.Close();
//                            fs.Close();

                        }
                        SetReadPPG(false);
                    }
                    //设置阻塞位
                    SetBlockFlag(false);
                }
                else
                {
                    //System.Diagnostics.Debug.Write("还没有返回结果\n");
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message+"\n");
            }
        }
        /// <summary>
        /// 开始设备
        /// </summary>
        /// <returns></returns>
        public bool StartDriver()
        {
            
            if (StartDevice() && GetArtifactStatus() != -2 && StartRead())
            {
                System.Diagnostics.Debug.Write("取得阻塞值："+GetBlockFlag()+"\n");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 停止设备
        /// </summary>
        /// <returns></returns>
        public bool StopDriver()
        {
            return StopDevice();
        }
      
    }
}
