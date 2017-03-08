using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using pmts_net.HMRead;
using System.Windows.Threading;
using System.IO;
using Visifire.Charts;
using PmtsHrvChart;
using PmtsControlLibrary.WEBPlugin;

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainPlayerView.xaml 的交互逻辑
    /// </summary>
    public partial class TrainPlayerView : UserControl
    {
        private double[,] EPRange = { { 0, 0.6, 1 }, { 0.6, 1.5, 2 }, { 1.5, 4, 3 }, { 4, 7, 5 }, { 7, 15, 8 }, { 15, 25, 10 }, { 25, 45, 12 }, { 45, 70, 15 }, { 70, 100, 20 }, { 100, 150, 25 }, { 150, 230, 35 }, { 230, 350, 50 }, { 350, 450, 70 }, { 450, 600, 80 }, { 600, 65530, 100 } };//ep档位范围
        private Hashtable tInfo = new Hashtable();
        private UserControl tListView = null;
        private TrainWEB tdb = null;
        private System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        private AxShockwaveFlashObjects.AxShockwaveFlash shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
        private HDRead hd = new HDRead();
        private DispatcherTimer HRVReadTimer;//读取HRV时的Timer
        private bool isStart = false;

        private bool isGameStart = false;
        private ArrayList hrvMarkArr = new ArrayList();//事件标记记录数组（结构：{HashMap,HashMap}）
        private ArrayList HRVData = new ArrayList();//HRV曲线数组
        private ArrayList IBIData = new ArrayList();//IBI柱状图数组
        private ArrayList PPGData = new ArrayList();//PPG折线图
        private ArrayList EPData = new ArrayList();//EP值数组
        private double EPScore = 0;//EP得分
        private DateTime startTime;
        private Grid mainWindow = new Grid();//主窗体中放置控件的层
        private bool hrvPromptFlg = false;//HRV耳夹脱落提示框是否弹出，false没有弹出，true弹出
        private HRVPrompt hrvp = null;//HRV耳夹脱落提示框

        private static HRVControlWEB hrvdb = new HRVControlWEB();
        
        public TrainPlayerView()
        {
            InitializeComponent();

            //            enabledDevice();
            //button声音
            Grid uiButton = this.Content as Grid;
            UIElementCollection Childrens = uiButton.Children;
            foreach (UIElement ui in Childrens)
            {
                //ui转成控件
                if (ui is Button)
                {
                    ui.MouseEnter += new MouseEventHandler(ui_MouseEnter);
                }
            }
        }
        //鼠标在button上
        void ui_MouseEnter(object sender, MouseEventArgs e)
        {
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }
        public TrainPlayerView(Grid Main, Hashtable t, UserControl trainList)
        {
            InitializeComponent();
            tInfo = t;
            tListView = trainList;
            tdb = new TrainWEB();
            mainWindow = Main;
            //            enabledDevice();
            HRVPrompt.Visibility = System.Windows.Visibility.Hidden;


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
        /// 开始训练
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(tInfo["tid"]) == 20 || Convert.ToInt32(tInfo["tid"]) == 21 || Convert.ToInt32(tInfo["tid"]) == 22 || Convert.ToInt32(tInfo["tid"]) == 23 || Convert.ToInt32(tInfo["tid"]) == 24 || Convert.ToInt32(tInfo["tid"]) == 25)
                {
                    HRVPrompt.Visibility = System.Windows.Visibility.Hidden;
                    HrvChartGrid2.Visibility = System.Windows.Visibility.Hidden;
                    xietiaoView.Visibility = System.Windows.Visibility.Hidden;
                    PlayerView.Margin = new Thickness(0, 0, 0, 57);
                    PlayerView.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    PlayerView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    PlayerView.Height = 589;
                    TrainDesensitization des = new TrainDesensitization();

                    this.PlayerView.Children.Add(des);
                }
                else if (Convert.ToInt32(tInfo["tid"]) == 1 || Convert.ToInt32(tInfo["tid"]) == 2 || Convert.ToInt32(tInfo["tid"]) == 3 || Convert.ToInt32(tInfo["tid"]) == 4 || Convert.ToInt32(tInfo["tid"]) == 5 || Convert.ToInt32(tInfo["tid"]) == 6)
                {
                    enabledDevice();
                    HrvChartGrid2.Visibility = System.Windows.Visibility.Visible;
                    xietiaoView.Visibility = System.Windows.Visibility.Visible;
                    PlayerView.Margin = new Thickness(0, 0, 0, 97);
                    PlayerView.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    PlayerView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    PlayerView.Height = 549;
                    host = new System.Windows.Forms.Integration.WindowsFormsHost();
                    shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
                    host.Child = shockwave;

                    this.PlayerView.Children.Add(host);

                    // 设置 .swf 文件相对路径
                    string swfPath = System.Environment.CurrentDirectory;
                    swfPath += @"\Train\" + tInfo["tid"].ToString() + "\\play.swf";
                    shockwave.Movie = swfPath;
                    shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(FromFlashCall);
                    String cmd = "<invoke name=\"c2flash\" returntype=\"xml\"><arguments><object>";
                    cmd += "<property id=\"Level\"><number>" + tInfo["gateOpen"] + "</number></property>";
                    cmd += "<property id=\"O\"><number>" + UserInfoStatic.O + "</number></property>";
                    cmd += "<property id=\"R\"><number>" + UserInfoStatic.R + "</number></property>";
                    cmd += "<property id=\"T\"><number>" + UserInfoStatic.T + "</number></property>";
                    cmd += "<property id=\"E\"><number>" + UserInfoStatic.E + "</number></property>";
                    cmd += "<property id=\"W\"><number>" + UserInfoStatic.W + "</number></property>";
                    cmd += "</object></arguments></invoke>";

                    shockwave.CallFunction(cmd);
                    shockwave.Play();
                }
                //else if (Convert.ToInt32(tInfo["tid"]) == 30 || Convert.ToInt32(tInfo["tid"]) == 31 || Convert.ToInt32(tInfo["tid"]) == 32 )
                //{

                //    enabledDevice();
                //    HrvChartGrid2.Visibility = System.Windows.Visibility.Visible;
                //    xietiaoView.Visibility = System.Windows.Visibility.Visible;
                //    PlayerView.Margin = new Thickness(0, 0, 0, 97);
                //    PlayerView.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                //    PlayerView.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                //    PlayerView.Height = 549;
                //    host = new System.Windows.Forms.Integration.WindowsFormsHost();
                //    shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
                //    host.Child = shockwave;

                //    this.PlayerView.Children.Add(host);

                //    // 设置 .swf 文件相对路径
                //    string swfPath = System.Environment.CurrentDirectory;
                //    swfPath += @"\Train\" + tInfo["tid"].ToString() + "\\play.swf";
                //    shockwave.Movie = swfPath;
                //    shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(FromFlashCall);
                //    String cmd = "<invoke name=\"c2flash\" returntype=\"xml\"><arguments><object>";
                //    cmd += "<property id=\"Level\"><number>" + tInfo["gateOpen"] + "</number></property>";
                //    cmd += "<property id=\"O\"><number>" + UserInfoStatic.O + "</number></property>";
                //    cmd += "<property id=\"R\"><number>" + UserInfoStatic.R + "</number></property>";
                //    cmd += "<property id=\"T\"><number>" + UserInfoStatic.T + "</number></property>";
                //    cmd += "<property id=\"E\"><number>" + UserInfoStatic.E + "</number></property>";
                //    cmd += "<property id=\"W\"><number>" + UserInfoStatic.W + "</number></property>";
                //    cmd += "</object></arguments></invoke>";

                //    shockwave.CallFunction(cmd);
                //    shockwave.Play();
                //}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("载入flash时出错：" + ex.Message + "\n");
            }
        }

        private void GetFiles(string p, TreeViewItem chldNode)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 接受flash传过来的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromFlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            System.Diagnostics.Debug.Write("解析nodeXml：" + e.request.ToString() + "\n");
            Hashtable requestInfo = NodeXmlToHashtable(e.request.ToString());
            //           MessageBox.Show(requestInfo["rType"].ToString());
            if (requestInfo != null)
            {
                if (requestInfo["rType"].ToString() == "test")
                {
                    //                    MessageBox.Show("test");
                    Hashtable historyInfo = new Hashtable();
                    historyInfo["trid"] = tdb.GetTrainRecordID();
                    historyInfo["tid"] = tInfo["tid"];
                    historyInfo["score"] = requestInfo["S"];
                    historyInfo["gate"] = requestInfo["Gate"];
                    historyInfo["diff"] = requestInfo["Diff"];
                    historyInfo["o"] = requestInfo["O"];
                    historyInfo["r"] = requestInfo["R"];
                    historyInfo["t"] = requestInfo["T"];
                    historyInfo["e"] = requestInfo["E"];
                    historyInfo["w"] = requestInfo["W"];
                    historyInfo["bo"] = UserInfoStatic.O;
                    historyInfo["br"] = UserInfoStatic.R;
                    historyInfo["bt"] = UserInfoStatic.T;
                    historyInfo["be"] = UserInfoStatic.E;
                    historyInfo["bw"] = UserInfoStatic.W;
                    tdb.OnInsertTrainToHistory(historyInfo);

                    UserInfoStatic.O += Convert.ToDouble(requestInfo["O"]);
                    UserInfoStatic.R += Convert.ToDouble(requestInfo["R"]);
                    UserInfoStatic.T += Convert.ToDouble(requestInfo["T"]);
                    UserInfoStatic.E += Convert.ToDouble(requestInfo["E"]);
                    UserInfoStatic.W += Convert.ToDouble(requestInfo["W"]);
                    tdb.OnUpdateTrainDataToUserPara();
                    if (UserInfoStatic.O >= 500 && UserInfoStatic.R >= 500 && UserInfoStatic.T >= 500 && UserInfoStatic.E >= 500 && UserInfoStatic.W >= 500)
                    {
                        tdb.OnUpdateTrainAll();
                    }
                    TrainBack tb = tListView as TrainBack;

                    //结束游戏
                    //    MessageBox.Show("结束游戏");

                    isGameStart = false;
                    if (HRVData.Count > 128)
                    {
                        HMMath hdMath = new HMMath(HRVData, EPData);//计算14项数据，调节指数，稳定指数，综合得分和给出评价报告
                        Hashtable HRVDataCalc = hdMath.HRVCalc();//用于存放HRV测量后计算的相关数据
                        HRVDataCalc["HRVScore"] = EPScore;//HRV得分
                        HRVDataCalc["Time"] = (Single)HRVData.Count / 2.0;//测试时间，单位是秒
                        HRVDataCalc["EndTime"] = DateTime.Now;//结束时间，datetime格式
                        HRVDataCalc["StartTime"] = startTime;//开始时间，datetime格式
                        //                HRVDataCalc["TimeType"] = this.SelectComboBox + 1;//HRV检测时间类型
                        HRVDataCalc["TimeType"] = Convert.ToInt32(tInfo["tid"]) + 40;//HRV检测时间类型
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
                    //or (int i = 0; i < tb.TrainButtonGrid.Children.Count; i++)
                    //
                    //rainList tl = tb.TrainButtonGrid.Children[i] as TrainList;
                    //f (tl.HistoryButton.Tag.ToString() == tInfo["tid"].ToString())
                    //
                    //   tl.HistoryButton.IsEnabled = true;
                    //   break;
                    //
                    //
                }
                else if (requestInfo["rType"].ToString() == "start")
                {
                    isGameStart = true;
                    HRVData = new ArrayList();//初始化HRV曲线数组
                    EPData = new ArrayList();//初始化EP数组
                    IBIData = new ArrayList();//初始化IBI数组
                    PPGData = new ArrayList();//初始化PPG数组
                    hrvMarkArr = new ArrayList();//初始化时间标记数组
                    OnChartPaint(HRVData);//初始化曲线图表
                    OnAnimationForEP(0);//协调状态条
                    EPScore = 0;
                    //     MessageBox.Show("开始游戏");
                }
                else if (requestInfo["rType"].ToString() == "enter")
                {

                }
                else if (requestInfo["rType"].ToString() == "cmd")
                {
                    //                   MessageBox.Show("cmd");
                    if (requestInfo["cmd"].ToString() == "1")
                    {
                        OnStartHrvToFlash();
                    }
                    else
                    {
                        OnEndHrvToFlash();
                    }
                }
                else if (requestInfo["rType"].ToString() == "click")
                {
                    //                   MessageBox.Show("click");
                }

            }
        }

        /// <summary>
        /// 绘制HRV曲线图用
        /// </summary>
        /// <param name="points">完整的HRV曲线数组</param>
        private void OnChartPaint(ArrayList points)
        {
            try
            {
                TrainChartsHrv.LineDataArr = points;
                this.HRVChartView.DrawingLine();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write("HRV控件-错误信息为:" + e.Message + "\n");
                System.Diagnostics.Debug.Write("HRV控件-导致错误的控件:" + e.Source + "\n");
                System.Diagnostics.Debug.Write("HRV控件-引发当前异常的方法:" + e.TargetSite + "\n");
            }
        }
        /// <summary>
        /// 解析Flash返回的XML
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Hashtable NodeXmlToHashtable(string s)
        {
            Hashtable trainRequestInfo = new Hashtable();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(s);
            System.Xml.XmlNode typeNode = doc.SelectSingleNode("invoke");
            String requestType = typeNode.Attributes[0].Value;
            trainRequestInfo["rType"] = requestType;
            if (requestType == "test")
            {
                System.Xml.XmlNodeList list = doc.GetElementsByTagName("property");
                //if (list.Count > 0)
                //{
                //    trainRequestInfo = new Hashtable();
                //}
                for (int i = 0; i < list.Count; i++)
                {
                    System.Xml.XmlElement xe = (System.Xml.XmlElement)list[i];
                    String key = xe.GetAttribute("id");
                    String value = xe.FirstChild.InnerText;
                    trainRequestInfo[key] = value;
                }
            }
            else if (requestType == "cmd")
            {
                System.Xml.XmlNodeList list = doc.GetElementsByTagName("number");
                trainRequestInfo["cmd"] = list[0].FirstChild.InnerText;
            }
            return trainRequestInfo;
        }
        /// <summary>
        /// 开始HRV测量
        /// </summary>
        private void OnStartHrvToFlash()
        {
            hd = new HDRead();
            if (hd.StartDriver())
            {

                HRVReadTimer = new DispatcherTimer();
                HRVReadTimer.Tick += new EventHandler(OnTimerHRV);
                HRVReadTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);//20毫秒
                HRVReadTimer.Start();
                isStart = true;
            }
            else
            {
                System.Diagnostics.Debug.Write("开始设备失败\n");
            }
        }
        /// <summary>
        /// 停止HRV测量
        /// </summary>
        private void OnEndHrvToFlash()
        {
            //停止设备
            if (hd.StopDriver())
            {
                isStart = false;
                HRVReadTimer.Stop();//停止TIMER
            }
        }
        /// <summary>
        /// 读取HRV的Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerHRV(Object sender, EventArgs e)
        {
            try
            {
                hd.GetHRV();

                if (isGameStart)
                {
                    ArrayList tmpHRVArr = hd.HRVArr;
                    ArrayList tmpEPArr = hd.EPArr;
                    ArrayList tmpIBIArr = hd.IBIArr;
                    ArrayList tmpPPGArr = hd.PPGArr;

                    for (int i = 0; i < tmpHRVArr.Count; i++)
                    {
                        HRVData.Add(tmpHRVArr[i]);
                        if (HRVData.Count == 1)
                        {
                            startTime = DateTime.Now;
                        }
                        OnChartPaint(HRVData);
                        for (int ep = 0; ep < tmpEPArr.Count; ep++)
                        {
                            EPData.Add(tmpEPArr[ep]);
                            OnAnimationForEP((Single)tmpEPArr[ep]);
                        }
                    }
                }

                ArrayList tempEPArr = hd.EPArr;
                for (int ep = 0; ep < tempEPArr.Count; ep++)
                {
                    double EPValue = (Single)tempEPArr[ep];
                    for (int i = 0; i < 15; i++)
                    {
                        if (EPValue > EPRange[i, 0] && EPValue <= EPRange[i, 1])
                        {
                            String cmd = "<invoke name=\"ep\" returntype=\"xml\"><arguments><number>" + (i + 1);
                            cmd += "</number></arguments></invoke>";
                            shockwave.CallFunction(cmd);
                        }
                    }
                }


                //开始判断是否是伪信号
                if (!hrvPromptFlg)
                {
                    if (!IsSpurious(HRVData))
                    {
                        HRVPrompt.Visibility = System.Windows.Visibility.Visible;


                        //hrvp = new HRVPrompt();
                        //hrvp.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        //hrvp.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                        //hrvp.Margin = new Thickness(0,0, 0, 0);
                        //hrvp.Width = 274;
                        //this.mainWindow.Children.Add(hrvp);
                        hrvPromptFlg = true;
                        System.Diagnostics.Debug.Write("MainWindow中成员个数：" + this.mainWindow.Children.Count + "\n");
                    }
                }
                else
                {
                    if (IsSpurious(HRVData))
                    {
                        //System.Diagnostics.Debug.Write("删除提示框\n");
                        //this.mainWindow.Children.Remove(hrvp);
                        //System.Diagnostics.Debug.Write("MainWindow中成员个数：" + this.mainWindow.Children.Count + "\n");
                        //hrvp = null;
                        HRVPrompt.Visibility = System.Windows.Visibility.Hidden;
                        hrvPromptFlg = false;
                        this.UpdateLayout();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 根据HRV判断是否是伪信号
        /// </summary>
        /// <param name="HrvData">完整的HRV数组</param>
        /// <returns>false：伪信号，true：正常信号</returns>
        private bool IsSpurious(ArrayList HrvData)
        {
            if (HrvData.Count > 2)
            {
                if ((Single)HrvData[HrvData.Count - 1] < 40)
                {
                    return false;//伪信号
                }
                else if (Math.Abs((Single)HrvData[HrvData.Count - 1] - (Single)HrvData[HrvData.Count - 3]) > 50)
                {
                    return false;//伪信号
                }
                else
                {
                    return true;
                }
            }
            else
            {//刚开始，hrv信号只有一个的情况
                return true;
            }
        }
        /// <summary>
        /// 关闭整个播放器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.shockwave.Dispose();
            this.host.Dispose();

            Grid main = (Grid)this.Parent;
            main.Children.Remove(this);
            tListView.Visibility = System.Windows.Visibility.Visible;
            main.Margin = new Thickness(0, 69, 0, 0);
            main.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            main.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }
        /// <summary>
        /// 界面退出时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (isStart)
            {
                OnEndHrvToFlash();
            }
            Grid main = (Grid)mainWindow.Parent;
            main.Margin = new Thickness(0, -20, 0, 0);
            main.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            main.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }


        /// <summary>
        /// EP槽的动画效果
        /// </summary>
        /// <param name="EPValue">及时EP值，非数组形式，float类型</param>
        private void OnAnimationForEP(double EPValue)
        {

            double epMaskValue = 0.0;
            for (int i = 0; i < 15; i++)
            {
                if (EPValue > EPRange[i, 0] && EPValue <= EPRange[i, 1])
                {
                    double epOffice = EPValue * ((698 / 15) / (EPRange[i, 1] - EPRange[i, 0]));
                    epMaskValue = (i * (698 / 15)) + epOffice;
                    EPScore += EPRange[i, 2];
                    string ScoreStr = Convert.ToString(Math.Floor(EPScore)).PadLeft(6, '0');

                }
            }
            epDoubleAnime.To = epMaskValue / 698;
            epStory.Begin();

        }
    }
}
