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
using PmtsControlLibrary.DBPlugin;
using System.IO;
using pmts_net.HMRead;
using System.Windows.Threading;
using PmtsControlLibrary.WEBPlugin;

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainCenter.xaml 的交互逻辑
    /// </summary>
    public partial class TrainCenter : UserControl
    {
        //private AxShockwaveFlashObjects.AxShockwaveFlash shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
        //private HrvFlashWinForm.MyShockwaveFlash shockwave;
        //private System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        private Grid mainWindow = new Grid();//主窗体中放置控件的层
        private TrainDB tdb = null;
        private Hashtable tInfo = new Hashtable();

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

  //      public TrainHandleList tl = null;
        public int oldGameType = 70;
        
        private HRVControlWEB hrvdb = null;

        public TrainCenter(Grid Main)
        {
            InitializeComponent();
            mainWindow = Main;
            mainWindow.Margin = new Thickness(0, 69, 0, 0);
            mainWindow.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            mainWindow.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tdb = new TrainDB();
            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/Train/adjustbg.png", UriKind.Relative));
            trainCenterbgdImage.Source = ima;
            adjustbutton.IsEnabled = false;//调节
            cognitivebutton.IsEnabled = true;//认知
            simulationbutton.IsEnabled = true;//仿真
            handlebutton.IsEnabled = true;//操作
            TrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;//认知
            simulatioTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;//仿真
            handleTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;//操作
            //调节
            this.circlebgd.Width = 876;
            this.circlebgd.Height = 400;
            this.circlebgd.Margin = new Thickness(136, 182, 188, 0);
            this.circlebgd.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.circlebgd.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.TrainButtonGrid.Width = 876;
            this.TrainButtonGrid.Height = 400;
            this.TrainButtonGrid.Margin = new Thickness(136, 182, 188, 0);
            this.TrainButtonGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.TrainButtonGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            gamePanel_Loaded();
            
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Children.Remove(this);
        }

        private void gamePanel_Loaded() {
            this.GameButton1.Tag = "../Image/Train/game2.png";//荷韵
            this.GameButton2.Tag = "../Image/Train/game2.png";//梅花
            this.GameButton3.Tag = "../Image/Train/game2.png";//丝绸之路
            this.GameButton4.Tag = "../Image/Train/game2.png";//菩提树
            this.GameButton5.Tag = "../Image/Train/game2.png";//生命之泉
            this.GameButton6.Tag = "../Image/Train/game2.png";//星空

            if (UserInfoStatic.hasAuth("荷韵"))
            {
                this.GameButton1.Tag = "../Image/Train/game4.png";
                this.GameButton1.Click += OnOpenTrain1;
            }

            if (UserInfoStatic.hasAuth("梅花"))
            {
                this.GameButton2.Tag = "../Image/Train/game3.png";
                this.GameButton2.Click += OnOpenTrain2;
            }

            if (UserInfoStatic.hasAuth("丝路"))
            {
                this.GameButton3.Tag = "../Image/Train/game5.png";
                this.GameButton3.Click += OnOpenTrain3;
            }

            if (UserInfoStatic.hasAuth("菩提树"))
            {
                this.GameButton4.Tag = "../Image/Train/game6.png";
                this.GameButton4.Click += OnOpenTrain4;
            }

            if (UserInfoStatic.hasAuth("生命之泉"))
            {
                this.GameButton5.Tag = "../Image/Train/game8.png";
                this.GameButton5.Click += OnOpenTrain5;
            }

            if (UserInfoStatic.hasAuth("星空"))
            {
                this.GameButton6.Tag = "../Image/Train/game7.png";
                this.GameButton6.Click += OnOpenTrain6;
            }
            
        }

        /// <summary>
        /// 载入认知训练图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ArrayList tList = tdb.GetTrainIsOpen();
            ArrayList tNumList = tdb.GetTrainHistoryNum();
            if (tNumList.Count > 0)
            {
                int tNumIndex = 0;
                Hashtable tNumTmp = (Hashtable)tNumList[tNumIndex];

                for (int i = 0; i < tList.Count; i++)
                {
                    Hashtable tmp = (Hashtable)tList[i];
                    TrainList tl = new TrainList(mainWindow, tmp, this);
                    if (UserInfoStatic.hasAuth(tmp["tname"].ToString()))
                    {
                        tl.Margin = new Thickness(30, 30, 0, 0);
                        if (tmp["tid"].ToString() == tNumTmp["tid"].ToString())
                        {
                            tNumIndex += 1;
                            tmp["historyNum"] = tNumTmp["num"];
                            if (tNumIndex < tNumList.Count)
                            {
                                tNumTmp = (Hashtable)tNumList[tNumIndex];
                            }
                        }
                        this.TrainButtonGrid.Children.Add(tl);
                    }
                }
            }
            else
            {
                for (int i = 0; i < tList.Count; i++)
                {
                    Hashtable tmp = (Hashtable)tList[i];
                    if (UserInfoStatic.hasAuth(tmp["tname"].ToString()))
                    {
                        TrainList tl = new TrainList(mainWindow, tmp, this);
                        tl.Margin = new Thickness(30, 30, 0, 0);
                        this.TrainButtonGrid.Children.Add(tl);
                    }
                }
            }

        }

        /// <summary>
        /// 载入仿真训练图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simulatioWrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ArrayList tList = tdb.GetSimulatioTrainIsOpen();
            for (int i = 0; i < tList.Count; i++)
            {
                Hashtable tmp = (Hashtable)tList[i];
                if (UserInfoStatic.hasAuth(tmp["tname"].ToString()))
                {
                    TrainList tl = new TrainList(mainWindow, tmp, this);
                    tl.Margin = new Thickness(30, 30, 0, 0);
                    this.simulatioTrainButtonGrid.Children.Add(tl);
                }
            }
        }

        /// <summary>
        /// 载入操作训练图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleWrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ArrayList tList = tdb.GetHandleTrainIsOpen();
            ArrayList tNumList = tdb.GetTrainHistoryNum();
            if (tNumList.Count > 0)
            {
                int tNumIndex = 0;
                Hashtable tNumTmp = (Hashtable)tNumList[tNumIndex];

                for (int i = 0; i < tList.Count; i++)
                {
                    Hashtable tmp = (Hashtable)tList[i];
                    if (UserInfoStatic.hasAuth(tmp["tname"].ToString()))
                    {
                        TrainHandleList tl = new TrainHandleList(mainWindow, tmp, this);
                        //                   tl = new TrainHandleList(mainWindow, tmp, this);
                        tl.Margin = new Thickness(30, 50, 0, 0);
                        if (tmp["tid"].ToString() == tNumTmp["tid"].ToString())
                        {
                            tNumIndex += 1;
                            tmp["historyNum"] = tNumTmp["num"];
                            if (tNumIndex < tNumList.Count)
                            {
                                tNumTmp = (Hashtable)tNumList[tNumIndex];
                            }
                        }
                        this.handleTrainButtonGrid.Children.Add(tl);
                    }
                }
            }
            else
            {
                for (int i = 0; i < tList.Count; i++)
                {
                    Hashtable tmp = (Hashtable)tList[i];
                    if (UserInfoStatic.hasAuth(tmp["tname"].ToString()))
                    {
                        TrainHandleList tl = new TrainHandleList(mainWindow, tmp, this);
                        //                   tl = new TrainHandleList(mainWindow, tmp, this);
                        tl.Margin = new Thickness(30, 50, 0, 0);
                        this.handleTrainButtonGrid.Children.Add(tl);
                    }
                }
            }
        }

        /// <summary>
        /// 认知显示或是隐藏是发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainButtonGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (this.TrainButtonGrid.Children.Count != 0)
                {
                    for (int i = 0; i < this.TrainButtonGrid.Children.Count; i++)
                    {
                        TrainList tl = this.TrainButtonGrid.Children[i] as TrainList;
                    }
                }
            }
        }

        /// <summary>
        /// 仿真显示或是隐藏是发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simulatioTrainButtonGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (this.simulatioTrainButtonGrid.Children.Count != 0)
                {
                    for (int i = 0; i < this.simulatioTrainButtonGrid.Children.Count; i++)
                    {
                        TrainList tl = this.simulatioTrainButtonGrid.Children[i] as TrainList;
                    }
                }
            }
        }

        /// <summary>
        /// 操作显示或是隐藏是发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleTrainButtonGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (this.handleTrainButtonGrid.Children.Count != 0)
                {
                    for (int i = 0; i < this.handleTrainButtonGrid.Children.Count; i++)
                    {
                        TrainHandleList tl = this.handleTrainButtonGrid.Children[i] as TrainHandleList;
                    }
                }
            }
        }

        //调节
        private void adjust_Click(object sender, RoutedEventArgs e)
        {
            circlebgd.Visibility = System.Windows.Visibility.Visible;
            TrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            simulatioTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            handleTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/Train/adjustbg.png", UriKind.Relative));
            trainCenterbgdImage.Source = ima;
            adjustbutton.IsEnabled = false;
            cognitivebutton.IsEnabled = true;
            simulationbutton.IsEnabled = true;
            handlebutton.IsEnabled = true;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }
        //认知
        private void cognitive_Click(object sender, RoutedEventArgs e)
        {
            circlebgd.Visibility = System.Windows.Visibility.Hidden;
            TrainButtonGrid.Visibility = System.Windows.Visibility.Visible;
            simulatioTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            handleTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/Train/cognitivebg.png", UriKind.Relative));
            trainCenterbgdImage.Source = ima;
            adjustbutton.IsEnabled = true;
            cognitivebutton.IsEnabled = false;
            simulationbutton.IsEnabled = true;
            handlebutton.IsEnabled = true;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }


        //仿真
        private void simulationbutton_Click(object sender, RoutedEventArgs e)
        {
            circlebgd.Visibility = System.Windows.Visibility.Hidden;
            TrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            simulatioTrainButtonGrid.Visibility = System.Windows.Visibility.Visible;
            handleTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/Train/simulationbg.png", UriKind.Relative));
            trainCenterbgdImage.Source = ima;
            adjustbutton.IsEnabled = true;
            cognitivebutton.IsEnabled = true;
            simulationbutton.IsEnabled = false;
            handlebutton.IsEnabled = true;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }

        //操作
        private void handlebutton_Click(object sender, RoutedEventArgs e)
        {
            circlebgd.Visibility = System.Windows.Visibility.Hidden;
            TrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            simulatioTrainButtonGrid.Visibility = System.Windows.Visibility.Hidden;
            handleTrainButtonGrid.Visibility = System.Windows.Visibility.Visible;
            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/Train/handlegb.png", UriKind.Relative));
            trainCenterbgdImage.Source = ima;
            adjustbutton.IsEnabled = true;
            cognitivebutton.IsEnabled = true;
            simulationbutton.IsEnabled = true;
            handlebutton.IsEnabled = false;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }

        //梅花
        private void OnOpenTrain1(object sender, RoutedEventArgs e)
        {
            var table = new Hashtable();
            table.Add("tid", (61).ToString());
            tInfo = table;

            FlashWindow flashWindow = new FlashWindow(tInfo);
            flashWindow.Show();
        }

        //荷韵
        private void OnOpenTrain2(object sender, RoutedEventArgs e)
        {
            var table = new Hashtable();
            table.Add("tid", (62).ToString());
            tInfo = table;
            FlashWindow flashWindow = new FlashWindow(tInfo);
            flashWindow.Show();
        }

        //丝绸之路
        private void OnOpenTrain3(object sender, RoutedEventArgs e)
        {
            var table = new Hashtable();
            table.Add("tid", (63).ToString());
            tInfo = table;
            FlashWindow flashWindow = new FlashWindow(tInfo);
            flashWindow.Show();
        }

        //菩提树
        private void OnOpenTrain4(object sender, RoutedEventArgs e)
        {
            var table = new Hashtable();
            table.Add("tid", (64).ToString());
            tInfo = table;
            FlashWindow flashWindow = new FlashWindow(tInfo);
            flashWindow.Show();
        }

        //星空
        private void OnOpenTrain5(object sender, RoutedEventArgs e)
        {
            var table = new Hashtable();
            table.Add("tid", (65).ToString());
            tInfo = table;
            FlashWindow flashWindow = new FlashWindow(tInfo);
            flashWindow.Show();
        }

        //生命之泉
        private void OnOpenTrain6(object sender, RoutedEventArgs e)
        {
            var table = new Hashtable();
            table.Add("tid", (66).ToString());
            tInfo = table;
            FlashWindow flashWindow = new FlashWindow(tInfo);
            flashWindow.Show();
        }


        //public static FlashWindow f2;


        //private void Form2_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Escape) //“Esc” 按键退出全频
        //    {
        //        f2.Close(); // 关闭 Form2 （或者还原窗口也行）
        //        f2 = null;
        //    }
        //}




        public bool enabledDevice()
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

                //               PmtsMessageBox.CustomControl1.Show("停止设备。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                //EPData = new ArrayList();
            }
            //if (timeTimer.IsEnabled == true)
            //{
            //    timeTimer.Stop();
            //    //timeTimer = new System.Windows.Threading.DispatcherTimer();
            //}
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
                //HRVDataCalc["TimeType"] = Convert.ToInt16(tInfo["tid"]) + 40;
                HRVDataCalc["TimeType"] = oldGameType;
//                PmtsMessageBox.CustomControl1.Show(Convert.ToString(tInfo["tid"]), PmtsMessageBox.ServerMessageBoxButtonType.OK);
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
                /*
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
                 */
            }
            catch (Exception ex)
            {
            }
        }

        private void GameButton1_MouseEnter(object sender, MouseEventArgs e)
        {
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }
    }
}
