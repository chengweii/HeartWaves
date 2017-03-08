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
using System.IO;
using System.Runtime.InteropServices;
using pmts_net.HMRead;
using System.Windows.Threading;

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainList.xaml 的交互逻辑
    /// </summary>
    public partial class TrainHandleList : UserControl
    {
        [Flags]
        public enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(
            IntPtr hwnd,
            string lpOperation,
            string lpFile,
            string lpParameters,
            string lpDirectory,
            ShowCommands nShowCmd);

        private Hashtable tInfo = new Hashtable();
 //       private UserControl PUI = new UserControl();
        private TrainCenter PUI = null;
        private Grid mainWindow = new Grid();//主窗体中放置控件的层
/*
        private HDRead hd = new HDRead();
        private DispatcherTimer HRVReadTimer;//读取HRV时的Timer
        private bool isStart = false;
        private bool isGameStart = false;

        private ArrayList hrvMarkArr = new ArrayList();//事件标记记录数组（结构：{HashMap,HashMap}）
        private ArrayList HRVData = new ArrayList();//HRV曲线数组
        private ArrayList IBIData = new ArrayList();//IBI柱状图数组
        private ArrayList PPGData = new ArrayList();//PPG折线图
        private ArrayList EPData = new ArrayList();//EP值数组
        private DateTime startTime;
*/
        public TrainHandleList()
        {
            InitializeComponent();

        }
        public TrainHandleList(Grid Main, Hashtable trainInfo, UserControl parentUI)
        {
            InitializeComponent();
            tInfo = trainInfo;
            PUI = (TrainCenter)parentUI;
            mainWindow = Main;
            if (!UserInfoStatic.hasAuth(tInfo["tname"].ToString()))
            {
                tInfo["open"] = 0;
            }
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            if (Convert.ToInt32(tInfo["open"]) == 0)
            {
                this.TrainHandleButton.Tag = tInfo["lockPic"].ToString();
                this.TrainHandleButton.IsEnabled = false;
            }
            else
            {
                this.TrainHandleButton.Tag = "../Image/Train/" + tInfo["tid"].ToString() + ".png";
                this.TrainHandleButton.Click += OnOpenTrain;

            }
            //this.TrainName.Text = tInfo["tname"].ToString();
            if (Convert.ToInt32(tInfo["historyNum"]) > 0)
            {
                //his.HistoryButton.Tag = tInfo["tid"];
            }
            else
            {
                //his.HistoryButton.Tag = tInfo["tid"];
                //his.HistoryButton.IsEnabled = false;
            }
            if (Convert.ToDouble(tInfo["gateNum"].ToString()) == 0f)
            {
                //his.GetNumShow.ScaleX = 1;
            }
            else
            {
                double gatePer = Convert.ToDouble(tInfo["gateOpen"].ToString()) / Convert.ToDouble(tInfo["gateNum"].ToString());
                //is.GetNumShow.ScaleX = gatePer;
            }

        }

        /// <summary>
        /// 训练按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenTrain(object sender, RoutedEventArgs e)
        {
            //PUI.Visibility = System.Windows.Visibility.Hidden;
            //Grid mainGrid = (Grid)PUI.Parent;
            //TrainPlayerView tpv = new TrainPlayerView(mainWindow, tInfo, PUI);
            //mainGrid.Children.Add(tpv);
            //mainGrid.Margin = new Thickness(0, 89, 0, 20);
            //mainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //mainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            //假的数据 
            //tInfo为游戏资源序号
/*

            var table = new Hashtable();
            table.Add("tid", (61).ToString());
            tInfo = table;
            TrainHandleWindow flashWindow = new TrainHandleWindow(tInfo);
            flashWindow.Show();
*/
 //           MessageBox.Show( Convert.ToString(tInfo["tid"]));
            if (Convert.ToInt16(tInfo["tid"]) == 30)
            {
                StartUpGame("ColorSense");
                PUI.oldGameType = 70;
            }
            else if (Convert.ToInt16(tInfo["tid"]) == 31)
            {
                StartUpGame("AgileBall");
                PUI.oldGameType = 71;
            }
            else if (Convert.ToInt16(tInfo["tid"]) == 32)
            {
                StartUpGame("Toxophily");
                PUI.oldGameType = 72;
            }
            //
        }




        /// <summary>
        /// 打开记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenHistory(object sender, RoutedEventArgs e)
        {
            TrainHistory th = new TrainHistory(Convert.ToInt32(tInfo["tid"]));
            th.ShowDialog();

        }
/*
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
        /// 停止所有timer和硬件设备
        /// </summary>
        private void OnStopAllTimerAndHD()
        {
            if (isStart)
            {
                isStart = false;
                //isImage = false;
                hd.StopDriver();
                HRVReadTimer.Stop();
                HRVReadTimer = null;// new System.Windows.Threading.DispatcherTimer();

            }

        }

        public void startGame()
        {
            isGameStart = true;
            HRVData = new ArrayList();//初始化HRV曲线数组
            EPData = new ArrayList();//初始化EP数组
            IBIData = new ArrayList();//初始化IBI数组
            PPGData = new ArrayList();//初始化PPG数组
            hrvMarkArr = new ArrayList();//初始化时间标记数组
        }

        public void stopGame()
        {
            
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
                //                 HRVDataCalc["TimeType"] = Convert.ToInt32(tInfo["tid"]);//HRV检测时间类型
                HRVDataCalc["TimeType"] = Convert.ToInt16(tInfo["tid"])+40;
                PmtsMessageBox.CustomControl1.Show(Convert.ToString(tInfo["tid"]), PmtsMessageBox.ServerMessageBoxButtonType.OK);
                //                HRVDataCalc["Mood"] = this.systemMeg["Mood"];//测量时心情状态
                HRVDataCalc["Mood"] = 101;
                HRVDataCalc["HRVMark"] = hrvMarkArr;//事件标记

                //开始数据库操作
                //lich
                if (UserInfoStatic.ipAdd != null)
                    //                    hrvdb.OnInsertHRVDataAndEpData(HRVData, EPData, hrvMarkArr, HRVDataCalc);
                    ;
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

        public void exitGame()
        {
            OnStopAllTimerAndHD();
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

                        for (int ep = 0; ep < tmpEPArr.Count; ep++)
                        {
                            EPData.Add(tmpEPArr[ep]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
*/      
        public bool StartUpGame(string strGame)
        {
            MessageExchange.RefreshState();


            if (MessageExchange.IsGameRunning() || MessageExchange.IsEnableStartUpGame())
            {
                // 有游戏正在运行
                MessageBox.Show("已经有训练项目在进行，不能开启新的训练项目！", "错误信息");
                return false;
            }

            string strPath = Directory.GetCurrentDirectory();
            string strGameFile = "";
            strGameFile = string.Format("{0}.exe", strGame);

            if ((int)ShellExecute(IntPtr.Zero, "open", strGameFile, "", strPath, ShowCommands.SW_SHOW) <= 32)
            {
                MessageBox.Show("运行训练程序失败！", "错误信息");
                return false;
            }

            MessageExchange.EnableStartUpGame();

            if (!PUI.enabledDevice())
            {
                MessageBox.Show("启动监测失败！", "错误信息");
                return false;
            }

            return true;
        }
    }
    public class MessageExchange
    {
        [DllImport("MsgExchange.dll")]
        public static extern void CreateMsgExchge(bool bSystem);

        [DllImport("MsgExchange.dll")]
        public static extern void StartUpSystemMsg();

        [DllImport("MsgExchange.dll")]
        public static extern void ShutDownSystemMsg();

        [DllImport("MsgExchange.dll")]
        public static extern void RefreshState();

        [DllImport("MsgExchange.dll")]
        public static extern bool IsGameRunning();

        [DllImport("MsgExchange.dll")]
        public static extern void EnableStartUpGame();

        [DllImport("MsgExchange.dll")]
        public static extern bool IsEnableStartUpGame();

        [DllImport("MsgExchange.dll")]
        public static extern void SetTrainingUserName(string strName);

        /*       private MessageExchange() { }
               public static readonly MessageExchange Instance = new MessageExchange();

               public void Create(bool bSystem)
               {
                   CreateMsgExchge(bSystem);
               }

               public void StartUpSystem()
               {
                   StartUpSystemMsg();
               }

               public void ShutDownSystem()
               {
                   ShutDownSystemMsg();
               }

               public void RefreshMsgState()
               {
                   RefreshState();
               }

               public bool GetGameRunState()
               {
                   return IsGameRunning();
               }

               public void EnableStartGame()
               {
                   EnableStartUpGame();
               }

               public bool IsEnableStartGame()
               {
                   return IsEnableStartUpGame();
               }

               public void SetUserName(string strName)
               {
                   SetTrainingUserName(strName);
               }*/
    }
}
