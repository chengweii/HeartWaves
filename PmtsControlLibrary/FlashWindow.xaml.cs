using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using pmts_net.HMRead;
using System.Windows.Threading;
using System.Collections;
using PmtsControlLibrary.WEBPlugin;


namespace PmtsControlLibrary
{
    /// <summary>
    /// FlashWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FlashWindow : System.Windows.Window
    {
        private AxShockwaveFlashObjects.AxShockwaveFlash shockwave;
        private System.Windows.Forms.Integration.WindowsFormsHost host;
        private HDRead hd = new HDRead();
        private DispatcherTimer HRVReadTimer;//读取HRV时的Timer
        private bool isStart = false;
        private bool isGameStart = false;
        private Hashtable tInfo = new Hashtable();
        
        private ArrayList hrvMarkArr = new ArrayList();//事件标记记录数组（结构：{HashMap,HashMap}）
        private ArrayList HRVData = new ArrayList();//HRV曲线数组
        private ArrayList IBIData = new ArrayList();//IBI柱状图数组
        private ArrayList PPGData = new ArrayList();//PPG折线图
        private ArrayList EPData = new ArrayList();//EP值数组
        private DateTime startTime;

        private HRVControlWEB hrvdb = null;
        
        public FlashWindow(Hashtable t)
        {
            InitializeComponent();

            //ChangeResolution ChangeRes = new ChangeResolution(1280, 720);

            enabledDevice();

            fullScreen();

            host = new System.Windows.Forms.Integration.WindowsFormsHost();
            shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
            ((System.ComponentModel.ISupportInitialize)(shockwave)).BeginInit();
            host.Child = shockwave;
            this.LayoutRoot.Children.Add(host);
            ((System.ComponentModel.ISupportInitialize)(shockwave)).EndInit();

            tInfo = t;
            if (Convert.ToInt32(tInfo["tid"]) == 61)
            {
                string swfPath = System.Environment.CurrentDirectory;
                swfPath += "\\Train\\game\\heyun.swf";
                // 设置 .swf 文件相对路径
                shockwave.Movie = swfPath;
            }
            else if (Convert.ToInt32(tInfo["tid"]) == 62)
            {
                string swfPath = System.Environment.CurrentDirectory;
                swfPath += "\\Train\\game\\Meihua.swf";
                // 设置 .swf 文件相对路径
                shockwave.Movie = swfPath;
            }
            else if (Convert.ToInt32(tInfo["tid"]) == 63)
            {
                string swfPath = System.Environment.CurrentDirectory;
                swfPath += "\\Train\\game\\sichouzhilu.swf";
                // 设置 .swf 文件相对路径
                shockwave.Movie = swfPath;
            }
            else if (Convert.ToInt32(tInfo["tid"]) == 64)
            {
                string swfPath = System.Environment.CurrentDirectory;
                swfPath += "\\Train\\game\\putishu.swf";
                // 设置 .swf 文件相对路径
                shockwave.Movie = swfPath;
            }
            else if (Convert.ToInt32(tInfo["tid"]) == 65)
            {
                string swfPath = System.Environment.CurrentDirectory;
                swfPath += "\\Train\\game\\quanshui.swf";
                // 设置 .swf 文件相对路径
                shockwave.Movie = swfPath;
            }
            else if (Convert.ToInt32(tInfo["tid"]) == 66)
            {
                string swfPath = System.Environment.CurrentDirectory;
                swfPath += "\\Train\\game\\Starry.swf";
                // 设置 .swf 文件相对路径
                shockwave.Movie = swfPath;
            }
            
            shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(flash_FlashCall);

            //String cmd = "<invoke name=\"detected\" returntype=\"xml\"><arguments><number>1</number></arguments></invoke>";
            //shockwave.CallFunction(cmd);
            shockwave.Menu = false;
            shockwave.ScaleMode = 0;
            shockwave.Play();
        }

        private bool enabledDevice()
        {
            hd = new HDRead();
            if (hd.StartDriver())
            {
                HRVReadTimer = new DispatcherTimer();
                HRVReadTimer.Tick += new EventHandler(OnTimerHRV);
                HRVReadTimer.Interval = new TimeSpan(0, 0, 0, 0, 125);
                HRVReadTimer.Start();
                isStart = true;
                return true;
            }
            System.Diagnostics.Debug.Write("开始设备失败\n");
            return false;
        }

        /// <summary>
        /// HRV读取用Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerHRV(object sender, EventArgs e)
        {
            try
            {
                hd.GetHRV();

                if (isGameStart)
                {
                    ArrayList tmpHRVArr = hd.HRVArr;
                    ArrayList tmpEPArr  = hd.EPArr;
                    ArrayList tmpIBIArr = hd.IBIArr;
                    ArrayList tmpPPGArr = hd.PPGArr;

                    for (int i = 0; i < tmpHRVArr.Count; i++)
                    {
                        HRVData.Add(tmpHRVArr[i]);
                        if (HRVData.Count == 1)
                        {
                            startTime = DateTime.Now;
                        }

                        for (int ep = 0; ep < tmpEPArr.Count; ep++)
                        {
                            EPData.Add(tmpEPArr[ep]);
                         }
                    }
                }

                ArrayList tempHRVArr = hd.HRVArr;
                for (int hrv = 0; hrv < tempHRVArr.Count; hrv++)
                {
                    String cmd = "<invoke name=\"setHrv\" returntype=\"xml\"><arguments><number>" +
                       (Single)tempHRVArr[hrv] + "</number></arguments></invoke>";
                    shockwave.CallFunction(cmd);
                }

                ArrayList tempEPArr = hd.EPArr;
                for (int ep = 0; ep < tempEPArr.Count; ep++)
                {
                    //OnAnimationForEP((Single)tempEPArr[ep]);
                    Console.WriteLine(tempEPArr[ep]);
                    String cmd = "<invoke name=\"writeEp\" returntype=\"xml\"><arguments><number>" +
                       (Single)tempEPArr[ep] + "</number></arguments></invoke>";
                    shockwave.CallFunction(cmd);
                }
            }
            catch (Exception ex)
            {
            }
        }

        ///// <summary>
        ///// 退出游戏
        ///// </summary> 
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void quietGame(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        //{
        //    OnStopAllTimerAndHD();
        //    host = null;
        //    shockwave = null;
        //    this.Close();
        //}

        public void flash_FlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            ArrayList paraList = GetParaList(e.request);
            if (paraList[0].Equals("startGame"))
            {
                //开始游戏
                XmlDocument document = new XmlDocument();
                document.LoadXml(e.request);
                // get attributes to see which command flash is trying to call
                XmlAttributeCollection attributes = document.FirstChild.Attributes;

                // get function
                String command = attributes.Item(0).InnerText;

                // get parameters
                XmlNodeList list = document.GetElementsByTagName("arguments");

                isGameStart = true;
                HRVData = new ArrayList();//初始化HRV曲线数组
                EPData = new ArrayList();//初始化EP数组
                IBIData = new ArrayList();//初始化IBI数组
                PPGData = new ArrayList();//初始化PPG数组
                hrvMarkArr = new ArrayList();//初始化时间标记数组

 //               MessageBox.Show("开始游戏");

            }
            else if (paraList[0].Equals("endGame"))
            {
                //结束游戏
//                MessageBox.Show("结束游戏");

                isGameStart = false;
                if (HRVData.Count > 128)
                {
                    HMMath hdMath = new HMMath(HRVData, EPData);//计算14项数据，调节指数，稳定指数，综合得分和给出评价报告
                    Hashtable HRVDataCalc = hdMath.HRVCalc();//用于存放HRV测量后计算的相关数据
                    //                HRVDataCalc["HRVScore"] = EPScore;//HRV得分
                    HRVDataCalc["HRVScore"] = 0;
                    HRVDataCalc["Time"] = (Single)HRVData.Count / 2.0;//测试时间，单位是秒
                    HRVDataCalc["EndTime"] = DateTime.Now;//结束时间，datetime格式
                    HRVDataCalc["StartTime"] = startTime;//开始时间，datetime格式
                    //                HRVDataCalc["TimeType"] = this.SelectComboBox + 1;//HRV检测时间类型
                    HRVDataCalc["TimeType"] = Convert.ToInt32(tInfo["tid"]);//HRV检测时间类型
                    //                HRVDataCalc["Mood"] = this.systemMeg["Mood"];//测量时心情状态
                    HRVDataCalc["Mood"] = 101;
                    HRVDataCalc["HRVMark"] = hrvMarkArr;//事件标记

                    //开始数据库操作
                    //lich
                    if (UserInfoStatic.ipAdd != null)
                    	hrvdb.OnInsertHRVDataAndEpData(HRVData, EPData, hrvMarkArr, HRVDataCalc, PPGData,"2");
                    else
                    {
                        UserHrvRecord hrvRecord = new UserHrvRecord();
                        hrvRecord.HRVData = HRVData;
                        hrvRecord.EPData = EPData;
                        hrvRecord.MarkData = hrvMarkArr;
                        hrvRecord.PWRData = (ArrayList)HRVDataCalc["FS"];
                        hrvRecord.TimeData.Add(HRVDataCalc["fMean"]);
                        hrvRecord.TimeData.Add(HRVDataCalc["fStdDev"]);
                        hrvRecord.TimeData.Add(HRVDataCalc["fSDNN"]);
                        hrvRecord.TimeData.Add(HRVDataCalc["fRMSSD"]);
                        hrvRecord.TimeData.Add(HRVDataCalc["fSD"]);
                        hrvRecord.TimeData.Add(HRVDataCalc["fSDSD"]);
                        hrvRecord.TimeData.Add(HRVDataCalc["fPNN"]);
                        hrvRecord.FreData.Add(HRVDataCalc["tp"]);
                        hrvRecord.FreData.Add(HRVDataCalc["vlf"]);
                        hrvRecord.FreData.Add(HRVDataCalc["lf"]);
                        hrvRecord.FreData.Add(HRVDataCalc["hf"]);
                        hrvRecord.FreData.Add(HRVDataCalc["lhr"]);
                        hrvRecord.FreData.Add(HRVDataCalc["lfnorm"]);
                        hrvRecord.FreData.Add(HRVDataCalc["hfnorm"]);
                        hrvRecord.AnsBalance = Convert.ToDouble(HRVDataCalc["NB"]);
                        hrvRecord.Pressure = Convert.ToDouble(HRVDataCalc["Pressure"]);
                        hrvRecord.Adjust = Convert.ToDouble(HRVDataCalc["adjust"]);
                        hrvRecord.Stable = Convert.ToDouble(HRVDataCalc["stable"]);
                        hrvRecord.Score = Convert.ToDouble(HRVDataCalc["score"]);
                        hrvRecord.HrvScore = Convert.ToDouble(HRVDataCalc["HRVScore"]);
                        hrvRecord.TimeLength = Convert.ToDouble(HRVData.Count / 2.0);
                        hrvRecord.StartTime = Convert.ToDateTime(HRVDataCalc["StartTime"]);
                        hrvRecord.EndTime = Convert.ToDateTime(HRVDataCalc["EndTime"]);
                        hrvRecord.RecordType = Convert.ToInt32(HRVDataCalc["TimeType"]);
                        hrvRecord.Mood = Convert.ToInt32(HRVDataCalc["Mood"]);
                        hrvRecord.Report = Convert.ToString(HRVDataCalc["report"]);

                        MainRightPerson.TmpHrvRecord.Add(hrvRecord);
                    }
                }
            }
            else if (paraList[0].Equals("quietGame"))
            {
                //退出游戏
                OnStopAllTimerAndHD();
                host = null;
                shockwave = null;
                this.Close();
            }

        }

        public ArrayList GetParaList(string ParaListXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(ParaListXml);

            XmlNodeList pareNodeList = doc.GetElementsByTagName("string");

            ArrayList paraList = new ArrayList();
            foreach (XmlNode node in pareNodeList)
            {
                paraList.Add(node.InnerText);
            }
            return paraList;
        }

        /// <summary>
        /// 停止所有timer和硬件设备
        /// </summary>
        private void OnStopAllTimerAndHD()
        {
            //if (startTimer.IsEnabled == true)
            //{
            //    startTimer.Stop();
            //    startTimer = new System.Windows.Threading.DispatcherTimer();
            //    this.playGrid.Children.Clear();
            //}
            if (isStart)
            {
                isStart = false;
                //isImage = false;
                hd.StopDriver();
                HRVReadTimer.Stop();
                HRVReadTimer = null;// new System.Windows.Threading.DispatcherTimer();
                //EPData = new ArrayList();
            }
            //if (timeTimer.IsEnabled == true)
            //{
            //    timeTimer.Stop();
            //    //timeTimer = new System.Windows.Threading.DispatcherTimer();
            //}
        }

        //public void startGame(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        //{
        //    XmlDocument document = new XmlDocument();
        //    document.LoadXml(e.request);
        //    // get attributes to see which command flash is trying to call
        //    XmlAttributeCollection attributes = document.FirstChild.Attributes;

        //    // get function
        //    String command = attributes.Item(0).InnerText;

        //    // get parameters
        //    XmlNodeList list = document.GetElementsByTagName("arguments");

        //    isGameStart = true;
        //    HRVData = new ArrayList();//初始化HRV曲线数组
        //    EPData = new ArrayList();//初始化EP数组
        //    IBIData = new ArrayList();//初始化IBI数组
        //    PPGData = new ArrayList();//初始化PPG数组
        //    hrvMarkArr = new ArrayList();//初始化时间标记数组

        ////    MessageBox.Show("开始游戏");
        //    //switch (command)
        //    //{
        //    //    case "callCSharp":
        //    //        callCSharp();
        //    //        break;
             
        //    //}
        //}

        public void endGame(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(e.request);
            // get attributes to see which command flash is trying to call
            XmlAttributeCollection attributes = document.FirstChild.Attributes;

            // get function
            String command = attributes.Item(0).InnerText;

            // get parameters
            XmlNodeList list = document.GetElementsByTagName("arguments");

            // arguments paramters
            XmlNodeList pList = list[0].ChildNodes;

//            MessageBox.Show("结束游戏");

            isGameStart = false;
            if (HRVData.Count > 128)
            {
                HMMath hdMath = new HMMath(HRVData, EPData);//计算14项数据，调节指数，稳定指数，综合得分和给出评价报告
                Hashtable HRVDataCalc = hdMath.HRVCalc();//用于存放HRV测量后计算的相关数据
//                HRVDataCalc["HRVScore"] = EPScore;//HRV得分
                HRVDataCalc["HRVScore"] = 0;
                HRVDataCalc["Time"] = (Single)HRVData.Count / 2.0;//测试时间，单位是秒
                HRVDataCalc["EndTime"] = DateTime.Now;//结束时间，datetime格式
                HRVDataCalc["StartTime"] = startTime;//开始时间，datetime格式
//                HRVDataCalc["TimeType"] = this.SelectComboBox + 1;//HRV检测时间类型
                HRVDataCalc["TimeType"] = Convert.ToInt32(tInfo["tid"]);//HRV检测时间类型
//                HRVDataCalc["Mood"] = this.systemMeg["Mood"];//测量时心情状态
                HRVDataCalc["Mood"] = 101;
                HRVDataCalc["HRVMark"] = hrvMarkArr;//事件标记
               
                //开始数据库操作
                //lich
                if (UserInfoStatic.ipAdd != null)
                	hrvdb.OnInsertHRVDataAndEpData(HRVData, EPData, hrvMarkArr, HRVDataCalc, PPGData,"2");
                else
                {
                    UserHrvRecord hrvRecord = new UserHrvRecord();
                    hrvRecord.HRVData = HRVData;
                    hrvRecord.EPData = EPData;
                    hrvRecord.MarkData = hrvMarkArr;
                    hrvRecord.PWRData = (ArrayList)HRVDataCalc["FS"];
                    hrvRecord.TimeData.Add(HRVDataCalc["fMean"]);
                    hrvRecord.TimeData.Add(HRVDataCalc["fStdDev"]);
                    hrvRecord.TimeData.Add(HRVDataCalc["fSDNN"]);
                    hrvRecord.TimeData.Add(HRVDataCalc["fRMSSD"]);
                    hrvRecord.TimeData.Add(HRVDataCalc["fSD"]);
                    hrvRecord.TimeData.Add(HRVDataCalc["fSDSD"]);
                    hrvRecord.TimeData.Add(HRVDataCalc["fPNN"]);
                    hrvRecord.FreData.Add(HRVDataCalc["tp"]);
                    hrvRecord.FreData.Add(HRVDataCalc["vlf"]);
                    hrvRecord.FreData.Add(HRVDataCalc["lf"]);
                    hrvRecord.FreData.Add(HRVDataCalc["hf"]);
                    hrvRecord.FreData.Add(HRVDataCalc["lhr"]);
                    hrvRecord.FreData.Add(HRVDataCalc["lfnorm"]);
                    hrvRecord.FreData.Add(HRVDataCalc["hfnorm"]);
                    hrvRecord.AnsBalance = Convert.ToDouble(HRVDataCalc["NB"]);
                    hrvRecord.Pressure = Convert.ToDouble(HRVDataCalc["Pressure"]);
                    hrvRecord.Adjust = Convert.ToDouble(HRVDataCalc["adjust"]);
                    hrvRecord.Stable = Convert.ToDouble(HRVDataCalc["stable"]);
                    hrvRecord.Score = Convert.ToDouble(HRVDataCalc["score"]);
                    hrvRecord.HrvScore = Convert.ToDouble(HRVDataCalc["HRVScore"]); 
                    hrvRecord.TimeLength = Convert.ToDouble(HRVData.Count / 2.0);
                    hrvRecord.StartTime = Convert.ToDateTime(HRVDataCalc["StartTime"]);
                    hrvRecord.EndTime = Convert.ToDateTime(HRVDataCalc["EndTime"]);
                    hrvRecord.RecordType = Convert.ToInt32(HRVDataCalc["TimeType"]);
                    hrvRecord.Mood = Convert.ToInt32(HRVDataCalc["Mood"]);
                    hrvRecord.Report = Convert.ToString(HRVDataCalc["report"]);

                    MainRightPerson.TmpHrvRecord.Add(hrvRecord);
                }
            }
            //Console.WriteLine("游戏结果：" + pList[0].InnerText);
           // Console.WriteLine("游戏得分：" + pList[1].InnerText);
           // Console.WriteLine("剩余秒数（游戏总长300s）：" + pList[2].InnerText);

             //标记当前节点
            //XmlNode currentNode;
            //遍历所有二级节点
            //for (int i = 0; i < list[0].ChildNodes.Count; i++)
            //{
                //二级
               // currentNode = list[0].ChildNodes[i];
                //currentNode.child
           // }
        }

        /// <summary>
        /// 全屏
        /// </summary>
        public void fullScreen()
        {
            if (this.WindowStyle == WindowStyle.None)//全屏  
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }
            else//非全屏  
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// 退出全屏
        /// </summary>
        public void exitFullScreen()
        {
            if (this.WindowStyle == WindowStyle.None)//全屏  
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }
    }
}
