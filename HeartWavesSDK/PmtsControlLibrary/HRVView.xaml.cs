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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using PmtsHrvChart;
using System.Threading;
using System.Windows.Media.Animation;
using pmts_net.HMRead;
using System.Windows.Threading;
using PmtsControlLibrary.DBPlugin;
using Visifire.Charts;

namespace PmtsControlLibrary
{
    /// <summary>
    /// HRVView.xaml 的交互逻辑
    /// </summary>

    public partial class HRVView : UserControl
    {
        private double[,] EPRange = { { 0, 0.6, 1 }, { 0.6, 1.5, 2 }, { 1.5, 4, 3 }, { 4, 7, 5 }, { 7, 15, 8 }, { 15, 25, 10 }, { 25, 45, 12 }, { 45, 70, 15 }, { 70, 100, 20 }, { 100, 150, 25 }, { 150, 230, 35 }, { 230, 350, 50 }, { 350, 450, 70 }, { 450, 600, 80 }, { 600, 65530, 100 } };
        private int selectedIndex = 1;//记录下拉框的index，1：基线测试，2：5分钟测试，3：10分钟测试
        private HRVMark hrvMark = null;//HRV事件标记
        private ArrayList hrvMarkArr = new ArrayList();//事件标记记录数组（结构：{HashMap,HashMap}）
        private ArrayList HRVData = new ArrayList();//HRV曲线数组
        private ArrayList IBIData = new ArrayList();//IBI柱状图数组
        private ArrayList PPGData = new ArrayList();//PPG折线图
        private ArrayList EPData = new ArrayList();//EP值数组
        private double EPScore = 0;//EP得分
        private bool isStart = false;//HRV测量是否开始的FLG
        private Grid mainWindow = new Grid();//主窗体中放置控件的层
        private DispatcherTimer HRVReadTimer;//读取HRV时的Timer
        private HDRead hd = new HDRead();//读取设备的类
        private bool hrvPromptFlg = false;//HRV耳夹脱落提示框是否弹出，false没有弹出，true弹出
        private HRVPrompt hrvp = null;//HRV耳夹脱落提示框
        private HRVDetaile hrvd = null;//HRV结束后弹出的详细信息框
        private HRVRight hRight = null;//HRV监测时，右侧信息栏 指针
        private HRVConstant hrvc = null;//常量列表窗口
        private HRVHistory hrvh = null;//历史记录窗口
        private HRVControlDB hrvdb = null;
        private Hashtable systemMeg = new Hashtable();
        private DateTime startTime;
        private DispatcherTimer HRVScaleTimer;//5分，10分，基线测试用timer
        private Rectangle mark = new Rectangle();//屏蔽层
        private AxShockwaveFlashObjects.AxShockwaveFlash shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
        private System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        private Button mainHelpBut = new Button();

        private MediaPlayer MusicPlayer = null;//音乐播放
        private HRVSettingView hrvSeting = null;//设置
        private bool MusicPlayerIsMuted = false;
        private double SelectComboBox = 0;
        private string MusicPlayerOpen =null;
        private bool MuscePlayerdefaultCheckBox = true;//默认音乐

        private Breathing breath = null;//呼吸助手

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Main"></param>
        public HRVView(Grid Main, Hashtable Meg, Button hBut)
        {
            InitializeComponent();
            this.SelectComboBox = 0;
            EPScore = 0;
            isStart = false;
            mainHelpBut = hBut;
            mainWindow = Main;
            systemMeg = Meg;
            MusicPlayer = new MediaPlayer(); ;
//lich
            if(UserInfoStatic.ipAdd != null)
                hrvdb = new HRVControlDB(systemMeg);
            if (ppgChart != null)
            {
                ppgChart.Visibility = System.Windows.Visibility.Hidden;
            }

            if (ibiChart != null)
            {
                ibiChart.Visibility = System.Windows.Visibility.Hidden;
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

        /// <summary>
        /// 载入flash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadedBall(object sender, RoutedEventArgs e)
        {
            //开始播放Flash
            this.host = new System.Windows.Forms.Integration.WindowsFormsHost();
            shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
            host.Child = shockwave;
            this.BallGrid.Children.Add(host);

            string swfPath = System.Environment.CurrentDirectory;
            swfPath += "\\S\\Ball.swf";//556x128
            // 设置 .swf 文件相对路径
            shockwave.Movie = swfPath;
            String cmd = "<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>1</number></arguments></invoke>";
            shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(FromFlashData);
            shockwave.CallFunction(cmd);
            shockwave.Play();
            //调整ep槽
            OnAnimationForEP(0);

        }
        private void FromFlashData(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            //System.Diagnostics.Debug.Write("HRV小球动画返回值为：" + e.request + "\n");
            System.Diagnostics.Trace.Write("HRV小球动画返回值为：" + e.request + "\n");
        }
        /// <summary>
        /// 功能上相当于构造函数，实现图表等元素的初始化
        /// </summary>
        public void HRVViewStart()
        {
            hrvMarkArr = new ArrayList();//清空事件标记数组
            HRVData = new ArrayList();//清空HRV曲线数组
            EPData = new ArrayList();//清空EP数组
            OnChartPaint(HRVData);//初始化曲线图表
            EPScore = 0;
            isStart = false;
            this.SelectComboBox = 0;
            OnAnimationForEP(0);
            System.Diagnostics.Debug.Write("初始化HRV面板\n");

        }

        /// <summary>
        /// 绘制HRV曲线图用
        /// </summary>
        /// <param name="points">完整的HRV曲线数组</param>
        private void OnChartPaint(ArrayList points)
        {
            try
            {
                //ChartsHrv.tempBottom = this.HRVChartView.BottomTickNum;
                //ChartsHrv.tempLeft = this.HRVChartView.LeftTickNum;
                //System.Diagnostics.Debug.Write("points的长度" + points.Count + "\n");
                ChartsHrv.LineDataArr = points;
                this.HRVChartView.DrawingLine();
                if (ChartsHrv.LineDataArr.Count == 4)
                {
                    this.HrvMarkButton.IsEnabled = true;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write("HRV控件-错误信息为:" + e.Message + "\n");
                System.Diagnostics.Debug.Write("HRV控件-导致错误的控件:" + e.Source + "\n");
                System.Diagnostics.Debug.Write("HRV控件-引发当前异常的方法:" + e.TargetSite + "\n");
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
                    int ballNum = this.MathBallNum(i + 1);
                    String cmd = "<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>" + ballNum + "</number></arguments></invoke>";
                    shockwave.CallFunction(cmd);
                    double epOffice = EPValue * ((698 / 15) / (EPRange[i, 1] - EPRange[i, 0]));
                    epMaskValue = (i * (698 / 15)) + epOffice;
                    EPScore += EPRange[i, 2];
                    string ScoreStr = Convert.ToString(Math.Floor(EPScore)).PadLeft(6, '0');
                    this.EPScoreText.Text = ScoreStr;
                }
            }
            epDoubleAnime.To = epMaskValue / 698;
            epStory.Begin();
//lich
            if (EPValue >= 599 && UserInfoStatic.ipAdd != null)
            {
                hrvdb.UpEpLevel();
            }
        }
        /// <summary>
        /// 计算HRV球的加减数量
        /// </summary>
        /// <param name="epRange"></param>
        /// <returns></returns>
        private int MathBallNum(int epRange)
        {
            int ball = 1;
            if (epRange == 1)
            {
                ball = -3;
            }
            else if (epRange >= 2 && epRange <= 3)
            {
                ball = -2;
            }
            else if (epRange == 4)
            {
                ball = -1;
            }
            else if (epRange >= 5 && epRange <= 9)
            {
                ball = 1;
            }
            else if (epRange >= 10 && epRange <= 12)
            {
                ball = 2;
            }
            else if (epRange >= 13 && epRange <= 15)
            {
                ball = 3;
            }
            return ball;
        }
        /// <summary>
        /// 取得基线测试改变值以后的index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox tempComboBox = (ComboBox)sender;
            selectedIndex = tempComboBox.SelectedIndex + 1;
        }
        /// <summary>
        /// 事件标记按钮点击动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //屏蔽事件标记按钮
            HrvMarkButton.IsEnabled = false;
            //计算点击时的时间
            double titleSecond = ChartsHrv.LineDataArr.Count / 2;
            string second = "00";
            string minute = "00";
            if (titleSecond % 60 < 10)
            {
                second = "0" + Convert.ToString(Math.Floor(titleSecond % 60));
            }
            else
            {
                second = Convert.ToString(Math.Floor(titleSecond % 60));
            }
            if (Math.Floor(titleSecond / 60) < 10)
            {
                minute = "0" + Math.Floor(titleSecond / 60);
            }
            else
            {
                minute = Math.Floor(titleSecond / 60).ToString();
            }
            DateTime markDateTime = DateTime.Now;
            TimeSpan startTick = new TimeSpan(startTime.Ticks);
            TimeSpan nowTick = new TimeSpan(markDateTime.Ticks);
            TimeSpan diffTick = new TimeSpan();
            diffTick = nowTick.Subtract(startTick).Duration();
            //弹出事件标记框
            hrvMark = new HRVMark();
            hrvMark.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            hrvMark.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            hrvMark.Margin = new Thickness(this.Width / 2 - hrvMark.Width / 2, this.HrvChartGrid.Height / 2 - hrvMark.Height / 2 + 50, 0, 0);
            hrvMark.markTime.Text = diffTick.Minutes + "分" + diffTick.Seconds.ToString().PadLeft(2, '0') + "秒";
            hrvMark.CloseButton.MouseLeftButtonUp += OnCloseMark;
            hrvMark.SaveButton.Click += OnSaveMark;
            hrvMark.SaveTime = titleSecond;
            hrvMark.MarkDateTime = markDateTime;
            this.HrvChartGrid.Children.Add(hrvMark);

//            int returnvalue = this.HrvChartGrid.Children.Add(hrvMark);
//            PmtsMessageBox.CustomControl1.Show("suoyin"+returnvalue, PmtsMessageBox.ServerMessageBoxButtonType.OK);
        }
        /// <summary>
        /// 关闭事件标记框的叉子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseMark(object sender, MouseEventArgs e)
        {
            HrvMarkButton.IsEnabled = true;
           this.HrvChartGrid.Children.Remove(hrvMark);

            hrvMark = null;
        }
        /// <summary>
        /// 点击事件标记框的保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveMark(object sender, EventArgs e)
        {
            if (hrvMark.markContent.Text == "")
            {
                PmtsMessageBox.CustomControl1.Show("请填写内容", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }
            else
            {
                Hashtable tmp = new Hashtable();
                tmp.Add("Time", hrvMark.SaveTime);
                tmp.Add("Content", hrvMark.markContent.Text);
                tmp.Add("DateTime", hrvMark.MarkDateTime);
                hrvMarkArr.Add(tmp);
                this.HRVChartView.OnAddMarkLable(Convert.ToInt32(Math.Floor(hrvMark.SaveTime * 2)), hrvMark.markContent.Text);

                HrvMarkButton.IsEnabled = true;
               this.HrvChartGrid.Children.Remove(hrvMark);

                hrvMark = null;
            }
        }
        /// <summary>
        /// HRV监测开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HrvStartButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            if (isStart)
            {
                /*
                 *停止测量HRV的逻辑 
                 */
                System.Diagnostics.Debug.Write("Timer状态:" + HRVReadTimer.IsEnabled + "\n");
                OnStopDriver();//停止设备
                //关闭HRV部分
                //this.MainWindow.Children.Remove(hv);
                //this.MainWindow.Children.Remove(hvRight);
                //hvRight = null;
                //hv = null;
                hRight.StopAnime();
                
                MusicPlayer.Stop();
            }
            else
            {
                /*
                *开始测量HRV的逻辑 
                 */
                //初始化设备
                hd = new HDRead();
                HRVData = new ArrayList();//初始化HRV曲线数组
                EPData = new ArrayList();//初始化EP数组
                IBIData = new ArrayList();//初始化IBI数组
                PPGData = new ArrayList();//初始化PPG数组
                hrvMarkArr = new ArrayList();//初始化时间标记数组
                OnChartPaint(HRVData);//清空绘图区域的曲线

                this.HRVChartView.BottomTickInit(0);//时间轴清零
                this.HRVChartView.ClearMarkLable();
                String cmd = "<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>-30</number></arguments></invoke>";
                shockwave.CallFunction(cmd);
                OnAnimationForEP(0);
                EPScore = 0;
                string ScoreStr = Convert.ToString(Math.Floor(EPScore)).PadLeft(6, '0');
                this.EPScoreText.Text = ScoreStr;
                try
                {
                    if (hd.StartDriver())
                    {
 //                       MessageBox.Show("开始成功");
                        //hRight.constantButton.IsEnabled = false;
                        //hRight.historyButton.IsEnabled = false;
                        hRight.HRText.Text = "";
                        hRight.OnAnime(0);
                        HRVReadTimer = new DispatcherTimer();
                        HRVReadTimer.Tick += new EventHandler(OnTimerHRV);
                        //HRVReadTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);//20毫秒
                        HRVReadTimer.Interval = new TimeSpan(0, 0, 0, 0, 125);
                        HRVReadTimer.Start();
                        isStart = true;
                        this.HrvStartButton.Content = "停 止";
                        //this.SelectComboBox.IsEnabled = false;
                         setButton.IsEnabled = false;
                        ///startTime = DateTime.Now;
                        if (this.SelectComboBox > 0)
                        {
                            HRVScaleTimer = new DispatcherTimer();
                            HRVScaleTimer.Tick += OnStopHrvScale;
                            if (this.SelectComboBox == 1)
                           {
                                HRVScaleTimer.Interval = new TimeSpan(0, 5, 20);
                            }
                            else if (this.SelectComboBox == 2)
                            {
                                HRVScaleTimer.Interval = new TimeSpan(0, 10, 20);
                            }
                            this.HrvStartButton.IsEnabled = false;
                            HRVScaleTimer.Start();
                        }
                        //YINYUE
                        
                        //MusicPlayer.Open(new Uri(@"Resources/PureMusic.mp3", UriKind.Relative));

                        if (MusicPlayerOpen == null || MuscePlayerdefaultCheckBox == true)
                        {
                            MusicPlayerOpen = @"Resources/PureMusic.mp3";
                        }
                        MusicPlayer.Open(new Uri(MusicPlayerOpen, UriKind.Relative));
                        MusicPlayer.Play();
                        if (MusicPlayerIsMuted == false)
                        {
                            MusicPlayer.IsMuted = false;
                        }
                        else { 
                            MusicPlayer.IsMuted = true;
                        }
                    }
                    else
                    {
                        PmtsMessageBox.CustomControl1.Show("打开采集仪失败,请连接采集仪", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                        System.Diagnostics.Debug.Write("开始设备失败\n");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex.Message);
                }

            }
        }
        /// <summary>
        /// 定时测量timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStopHrvScale(object sender, EventArgs e)
        {
            DispatcherTimer tmp = sender as DispatcherTimer;
            tmp.Stop();
            OnStopDriver();
        }
        /// <summary>
        /// 停止设备
        /// </summary>
        private void OnStopDriver()
        {
            if (HRVReadTimer.IsEnabled == true)
            {
                this.HrvMarkButton.IsEnabled = false;
                //首先删除耳夹脱落提示框
                if (hrvPromptFlg)
                {
                    mainWindow.Children.Remove(hrvp);
                }
               // this.SelectComboBox.IsEnabled = true;
                setButton.IsEnabled = true;
                HRVReadTimer.Stop();//停止TIMER
                //停止设备
                if (hd.StopDriver())
                {
                    isStart = false;
                    this.HrvStartButton.Content = "开 始";//按钮状态
                    this.HrvStartButton.IsEnabled = true;
                    //hRight.constantButton.IsEnabled = true;
                    //hRight.historyButton.IsEnabled = true;
                    //弹出HRV详细对话框
                    if (HRVData.Count > 128)
                    {
                        this.Visibility = System.Windows.Visibility.Hidden;//暂时隐藏hrv测量主界面
                        //hRight.Visibility = System.Windows.Visibility.Hidden;//暂时隐藏右侧信息面板
                        HMMath hdMath = new HMMath(HRVData, EPData);//计算14项数据，调节指数，稳定指数，综合得分和给出评价报告
                        Hashtable HRVDataCalc = hdMath.HRVCalc();//用于存放HRV测量后计算的相关数据
                        HRVDataCalc["HRVScore"] = EPScore;//HRV得分
                        HRVDataCalc["Time"] = (Single)HRVData.Count / 2.0;//测试时间，单位是秒
                        HRVDataCalc["EndTime"] = DateTime.Now;//结束时间，datetime格式
                        HRVDataCalc["StartTime"] = startTime;//开始时间，datetime格式
                        HRVDataCalc["TimeType"] = this.SelectComboBox + 1;//HRV检测时间类型
                        HRVDataCalc["Mood"] = this.systemMeg["Mood"];//测量时心情状态
                        HRVDataCalc["HRVMark"] = hrvMarkArr;//事件标记
                        mark.Fill = Brushes.Transparent;
                        mark.Margin = new Thickness(0, 60, 0, 0);
                        mainWindow.Children.Add(mark);//添加屏蔽层
                        hrvd = new HRVDetaile(HRVData, HRVDataCalc, mainWindow);//实例化HRV详情面板，HRVData：hrv曲线数据，HRVDataCalc：计算后获得的和hrv相关的数据
                        hrvd.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        hrvd.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        System.Diagnostics.Debug.Write("添加侦听\n");
                        hrvd.closeButton.Click += OnCloseHrvD;
                        mainWindow.Children.Add(hrvd);
                        //开始数据库操作
                        //lich
                        if (UserInfoStatic.ipAdd != null)
                            hrvdb.OnInsertHRVDataAndEpData(HRVData, EPData, hrvMarkArr, HRVDataCalc);
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
                            hrvRecord.HrvScore = EPScore;
                            hrvRecord.TimeLength = Convert.ToDouble(HRVData.Count / 2.0);
                            hrvRecord.StartTime = Convert.ToDateTime(HRVDataCalc["StartTime"]);
                            hrvRecord.EndTime = Convert.ToDateTime(HRVDataCalc["EndTime"]);
                            hrvRecord.RecordType = Convert.ToInt32(HRVDataCalc["TimeType"]);
                            hrvRecord.Mood = Convert.ToInt32(HRVDataCalc["Mood"]);
                            hrvRecord.Report = Convert.ToString(HRVDataCalc["report"]);

                            MainRightPerson.TmpHrvRecord.Add(hrvRecord);
                        }
                        
                        MusicPlayer.Stop();
                    }
                    //startTime = new DateTime();
                }
                else
                {
                    System.Diagnostics.Debug.Write("停止设备失败\n");
                }
            }
            System.Diagnostics.Debug.Write("Timer状态:" + HRVReadTimer.IsEnabled + "\n");
        }

        public void InsertTmpHRVData(ArrayList HRVData, ArrayList EPData, ArrayList HRVMark, Hashtable HRVMathData)
        {
        }

        /// <summary>
        /// 读取HRV，IBI ,PPG,EP，频谱时 的timer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimerHRV(object source, EventArgs e)
        {
            try
            {
                if (MusicPlayer.Position == MusicPlayer.NaturalDuration)
                {
                    MusicPlayer.Position = TimeSpan.Zero;
                    MusicPlayer.Play();
                }
                hd.GetHRV();
                //hd.GetReadIBI();
                ArrayList tempHrvArr = hd.HRVArr;
                ArrayList tempEPArr = hd.EPArr;
                ArrayList tempIBIArr = hd.IBIArr;
                ArrayList tempPPGArr = hd.PPGArr;
                double tempNB = hd.NB;
                //HRV
                for (int i = 0; i < tempHrvArr.Count; i++)
                {
                    HRVData.Add(tempHrvArr[i]);
                    if (HRVData.Count == 1)
                    {
                        startTime = DateTime.Now;
                    }
                    OnChartPaint(HRVData);
                    if (HRVData.Count % 10 == 0)
                    {
                        if (IsSpurious(HRVData))
                        {
                            hRight.HRText.Text = Convert.ToString(Math.Floor((Single)HRVData[HRVData.Count - 1]));
                        }
                        else
                        {
                            hRight.HRText.Text = "";
                        }
                    }
                }
                for (int ep = 0; ep < tempEPArr.Count; ep++)
                {
                    EPData.Add(tempEPArr[ep]);
                    OnAnimationForEP((Single)tempEPArr[ep]);
                }
                if (tempNB > 0)
                {
                    hRight.OnAnime(tempNB);
                }
                IBIData.Clear();
                //IBI
                for (int i = 0; i < tempIBIArr.Count; i++)
                {
                    if (IBIData.Count >= 32)
                    {
                        break;
                    }
                    IBIData.Add(tempIBIArr[i]);
           
                    
                }
                List<string> listx = new List<string>();
                List<string> listy = new List<string>();
                for (int i = 0; i < IBIData.Count; i++)
                {
                    Single single = (Single)IBIData[i];
                    string str = ("" + single);
                    listx.Add(i.ToString());
                    listy.Add(str);
                }
                CreateChartColumn("IBI", listx, listy);


                //PPG
                for (int i = 0; i < tempPPGArr.Count; i++)
                {
                    PPGData.Add(tempPPGArr[i]);
                    if (PPGData.Count == 1)
                    {
                        startTime = DateTime.Now;
                    }
                    //OnChartPaint(PPGData);
                    //if (PPGData.Count % 10 == 0)
                    //{
                    //    if (IsSpurious(PPGData))
                    //    {
                    //        hRight.HRText.Text = Convert.ToString(Math.Floor((Single)PPGData[PPGData.Count - 1]));
                    //    }
                    //    else
                    //    {
                     //       hRight.HRText.Text = "";
                    //    }
                   // }
                }
                if (PPGData.Count > 256)
                {
                    PPGData.RemoveRange(0, PPGData.Count - 256);
                }
                List<string> listx2 = new List<string>();
                List<string> listy2 = new List<string>();
                for (int i = 0; i < PPGData.Count; i++)
                {
                    Single single = (Single)PPGData[i];
                    string str = ("" + single);
                    listx2.Add(i.ToString());
                    listy2.Add(str);
                }
                CreateChartSpline("PPG", listx2, listy2);


                //开始判断是否是伪信号
                if (!hrvPromptFlg)
                {
                    if (!IsSpurious(HRVData))
                    {
                        hrvp = new HRVPrompt();
                        hrvp.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        hrvp.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                        hrvp.Margin = new Thickness(0, 0, 5, 5);
                        this.mainWindow.Children.Add(hrvp);
                        hrvPromptFlg = true;
                        System.Diagnostics.Debug.Write("MainWindow中成员个数：" + this.mainWindow.Children.Count + "\n");
                    }
                }
                else
                {
                    if (IsSpurious(HRVData))
                    {
                        System.Diagnostics.Debug.Write("删除提示框\n");
                        this.mainWindow.Children.Remove(hrvp);
                        System.Diagnostics.Debug.Write("MainWindow中成员个数：" + this.mainWindow.Children.Count + "\n");
                        hrvp = null;
                        hrvPromptFlg = false;
                        this.UpdateLayout();
                    }
                }
                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("错误信息为:" + ex.Message + "\n");
                System.Diagnostics.Debug.Write("导致错误的控件:" + ex.Source + "\n");
                System.Diagnostics.Debug.Write("引发当前异常的方法:" + ex.TargetSite + "\n");
            }

        }

        /// <summary>
        /// 关闭HRV详情窗口，并回复HRV监测界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseHrvD(object sender, EventArgs e)
        {
            mainWindow.Children.Remove(hrvd);
            mainWindow.Children.Remove(mark);
            hrvd = null;
            this.Visibility = System.Windows.Visibility.Visible;
            hRight.Visibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// 打开历史记录窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenHRVHistory(object sender, RoutedEventArgs e)
        {
            mainHelpBut.Tag = 5;
            hrvh = new HRVHistory(systemMeg);
            //hRight.historyButton.IsEnabled = false;
            //hRight.constantButton.IsEnabled = false;
            hrvh.HistoryCloseButton.Click += OnCloseHistory;
            this.mainWindow.Children.Add(hrvh);
            this.Visibility = System.Windows.Visibility.Hidden;
            hRight.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// 关闭HRV历史详情窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseHistory(object sender, RoutedEventArgs e)
        {
            mainHelpBut.Tag = 3;
            this.Visibility = System.Windows.Visibility.Visible;
            hRight.Visibility = System.Windows.Visibility.Visible;
            this.mainWindow.Children.Remove(hrvh);
            //hRight.constantButton.IsEnabled = true;
            //hRight.historyButton.IsEnabled = true;
        }
        /// <summary>
        /// 打开常量列表窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenConstant(object sender, RoutedEventArgs e)
        {
            mainHelpBut.Tag = 6;
            hrvc = new HRVConstant(systemMeg);
            //hRight.constantButton.IsEnabled = false;
            //hRight.historyButton.IsEnabled = false;
            hrvc.CloseButton.Click += OnCloseConstant;
            this.mainWindow.Children.Add(hrvc);
            this.Visibility = System.Windows.Visibility.Hidden;
            hRight.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// 关闭常量列表窗口 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseConstant(object sender, RoutedEventArgs e)
        {
            mainHelpBut.Tag = 3;
            this.Visibility = System.Windows.Visibility.Visible;
            hRight.Visibility = System.Windows.Visibility.Visible;
            this.mainWindow.Children.Remove(hrvc);
           // hRight.constantButton.IsEnabled = true;
           // hRight.historyButton.IsEnabled = true;
        }
        /// <summary>
        /// 退出窗口时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (isStart)
            {
                if (HRVReadTimer.IsEnabled == true)
                {
                    HRVReadTimer.Stop();
                    if (hd.StopDriver())
                    {
                        isStart = false;
                        this.HrvStartButton.Content = "开 始";
                        System.Diagnostics.Debug.Write("停止设备成功\n");
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write("停止设备失败\n");
                    }
                }
            }
            MusicPlayer.Stop();
        }

        /// <summary>
        /// 界面载入时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            hrvbutton.IsEnabled = false;
            ibibutton.IsEnabled = true;
            ppgbutton.IsEnabled = true;
            if (hRight == null)
            {
                hRight = new HRVRight();
                hRight.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                hRight.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
 //lich  
               // hRight.Margin = new Thickness(0, 100, 10, 0);
                hRight.Margin = new Thickness(0, 100, 30, 0);
               //hRight.constantButton.Click += OnOpenConstant;
                //hRight.historyButton.Click += OnOpenHRVHistory;
                hRight.BreathingButton.Click += breathingButton_Click;
            }
            else
            {
                hRight.HRText.Text = "";
                hRight.OnAnime(1.15);
            }
           // hRight.historyButton.IsEnabled = true;
           // hRight.constantButton.IsEnabled = true;
            if (!this.mainWindow.Children.Contains(hRight))
            {
                this.mainWindow.Children.Add(hRight);
            }

            //hrvibippg.historyButton.IsEnabled = true;
            //hrvibippg.constantButton.IsEnabled = true;

            System.Diagnostics.Debug.Write("HRV曲线界面载入\n");
        }

        /// <summary>
        /// 呼吸助手的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void breathingButton_Click(object sender, RoutedEventArgs e)
        {

            breath = new Breathing(this.LayoutRoot, hRight.BreathingButton);
            breath.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            breath.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            breath.Margin = new Thickness(0, 0, -160,-60);
            this.LayoutRoot.Children.Add(breath);
            hRight.BreathingButton.IsEnabled = false;
        }

        /// <summary>
        /// 载入横向滑动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadedHorBar(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.ScrollBar tmp = sender as System.Windows.Controls.Primitives.ScrollBar;
            if (null != tmp.Track)
            {
                System.Windows.Controls.Primitives.Track myTrack = tmp.Track;
                System.Windows.Controls.Primitives.Thumb myThumb = myTrack.Thumb;
                myThumb.DragDelta += onDragDelta;
            }

        }
        /// <summary>
        /// 禁止鼠标动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void onDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            e.Handled = true;
        }
        /// <summary>
        /// 载入竖向滑动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadedVetBar(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.ScrollBar tmp = sender as System.Windows.Controls.Primitives.ScrollBar;
            System.Windows.Controls.Primitives.Track myTrack = tmp.Track;
            System.Windows.Controls.Primitives.Thumb myThumb = myTrack.Thumb;
            myThumb.DragDelta += onDragDelta;
        }

        //点击HRV
        private void hrvbutton_Click(object sender, RoutedEventArgs e)
        {
            hrvbutton.IsEnabled = false;
            ibibutton.IsEnabled = true;
            ppgbutton.IsEnabled = true;
            HrvImage.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/hrvbgd.png", UriKind.Relative)); 
            HRVChartView.Visibility = System.Windows.Visibility.Visible;
            VerScrollBar.Visibility = System.Windows.Visibility.Visible;
            HorSrollBar.Visibility = System.Windows.Visibility.Visible;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            if (ppgChart != null)
            {
                ppgChart.Visibility = System.Windows.Visibility.Hidden;
            }

            if (ibiChart != null)
            {
                ibiChart.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        // IBI柱状图
        private void ibibutton_click(object sender, RoutedEventArgs e)
        {
            hrvbutton.IsEnabled = true;
            ibibutton.IsEnabled = false;
            ppgbutton.IsEnabled = true;
            HrvImage.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/ibibgd.png", UriKind.Relative)); 
            HRVChartView.Visibility = System.Windows.Visibility.Hidden;
            VerScrollBar.Visibility = System.Windows.Visibility.Hidden;
            HorSrollBar.Visibility = System.Windows.Visibility.Hidden;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            if (ppgChart != null)
            {
                ppgChart.Visibility = System.Windows.Visibility.Hidden;
            }
            if (ibiChart == null)
            {
                List<string> listx = new List<string>();
                List<string> listy = new List<string>();

                CreateChartColumn("", listx, listy);

            }

            ibiChart.Visibility = System.Windows.Visibility.Visible;

            if (ppgChart != null)
            {
                ppgChart.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //创建一个图标
        private Chart ibiChart = null;
        #region ibiChart柱状图
        public void CreateChartColumn(string name, List<string> valuex, List<string> valuey)
        {
            if (ibiChart == null)
            {
                //创建一个图标
                ibiChart = new Chart();
                if (ibibutton.IsEnabled == false)
                {
                    ibiChart.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    ibiChart.Visibility = System.Windows.Visibility.Hidden;
                }
//                ibiChart.Background = new SolidColorBrush(Color.FromArgb(255, 27, 47, 71));// 
//                ibiChart.BorderThickness = new Thickness(0);

                // Create an ImageBrush
                ImageBrush brush = new ImageBrush();

                // Set image source
                brush.ImageSource = new BitmapImage(new Uri("UI/powerback.png", UriKind.Relative));

                // Set ImageBrush's Stretch property
                brush.Stretch = Stretch.Fill;

                // Set image brush to Chart Background
                ibiChart.Background = brush;

                //}
                //设置图标的宽度和高度
                ibiChart.Width = 810;
                ibiChart.Height = 260;
                ibiChart.Margin = new Thickness(0, 0, 0, -20);
                //是否启用打印和保持图片
                ibiChart.ToolBarEnabled = false;
                ibiChart.HideIndicator();
                //设置图标的属性
                ibiChart.ScrollingEnabled = false;//是否启用或禁用滚动
                ibiChart.View3D = false;//3D效果显示
                ibiChart.ShadowEnabled = false;
                //创建一个标题的对象
                // Title title = new Title();

                //设置标题的名称
                // title.Text = Name;
                // title.Padding = new Thickness(0, 10, 5, 0);

                //向图标添加标题
                //ibiChart.Titles.Add(title);

               // Axis yAxis = new Axis();
                //设置图标中Y轴的最小值永远为0           
               // yAxis.AxisMinimum = 0;
                //设置图表中Y轴的后缀          
              //  yAxis.Suffix = "斤";
              //  ibiChart.AxesY.Add(yAxis);
                AxisLabels xLabel = new AxisLabels();
                xLabel.FontColor = new SolidColorBrush(Colors.Transparent);

                Ticks xTick = new Ticks();
                xTick.Enabled = false;

                Axis xAxis = new Axis();
                xAxis.AxisLabels = xLabel;
                xAxis.Ticks.Add(xTick);
                ibiChart.AxesX.Add(xAxis);

                AxisLabels yLabel = new AxisLabels();
                yLabel.FontColor = new SolidColorBrush(Colors.White);
                ChartGrid xGrid = new ChartGrid();
                xGrid.Enabled = false;
                Axis yAxis = new Axis();
                yAxis.AxisLabels = yLabel;
                yAxis.Grids.Add(xGrid);
                ibiChart.AxesY.Add(yAxis);

                PlotArea plot = new PlotArea();
                plot.ShadowEnabled = false;

                ImageBrush brushF = new ImageBrush();

                // Set image source
                brushF.ImageSource = new BitmapImage(new Uri("UI/colorpower.png", UriKind.Relative));

                // Set ImageBrush's Stretch property
                brushF.Stretch = Stretch.Fill;

                // Set image brush to Chart Background
                plot.Background = brushF;

   //             plot.Background = new SolidColorBrush(Colors.Transparent);
                ibiChart.PlotArea = plot;

                HrvChartGrid2.Children.Add(ibiChart);
                
            }
            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();

            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.StackedColumn;//柱状Stacked
 
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                //dataPoint.AxisXLabel = valuex[i];
                dataPoint.XValue = valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
                dataPoint.Color = new SolidColorBrush(Color.FromArgb(150, 33, 155, 240));
                //添加一个点击事件        
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }
            if (dataSeries.DataPoints.Count == 0)
            {
                return;
            }
            // 添加数据线到数据序列。   
            ibiChart.Series.Add(dataSeries);
            if(ibiChart.Series.Count > 1)
            {
                ibiChart.Series.RemoveAt(0);
            }

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            

            // Image simpleImage = new Image();
            // simpleImage.Width = 400;
            // simpleImage.Height = 200;
            // simpleImage.Margin = new Thickness(0, 0, 0, 0);

            // Create source.
            // BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            //bi.BeginInit();
            //bi.UriSource = new Uri(@"/images/t0151320b1d0fc50be8.png", UriKind.RelativeOrAbsolute);
            ////bi.EndInit();
            // Set the image source.
            // simpleImage.Source = bi;

            // gr.Children.Add(simpleImage);
            //}
            //gr.Background = System.Windows.Media.Brush;
//            Grid gr = new Grid();
//            gr.Children.Add(ibiChart);
//            HrvChartGrid2.Children.Add(gr);
   
            
            // }

        }
        #endregion

        #region ibiChart点击事件
        //点击事件
        void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DataPoint dp = sender as DataPoint;
            //MessageBox.Show(dp.YValue.ToString());
        }
        #endregion

        private Chart ppgChart = null;
        #region ppgChart曲线图
        public void CreateChartSpline(string name, List<string> lsTime, List<string> cherry)
        {
            //设置图标的宽度和高度
            if (ppgChart == null)
            {
                ppgChart = new Chart();
                if (ppgbutton.IsEnabled == false)
                {
                    ppgChart.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    ppgChart.Visibility = System.Windows.Visibility.Hidden;
                }
                ppgChart.Background = new SolidColorBrush(Color.FromArgb(255, 27, 47, 71));//
                ppgChart.Width = 810;
                ppgChart.Height = 260;
                ppgChart.Margin = new Thickness(0, 0, 0, -20);
                
                //是否启用打印和保持图片
                ppgChart.ToolBarEnabled = false;

                //设置图标的属性
                ppgChart.ScrollingEnabled = false;//是否启用或禁用滚动
                ppgChart.View3D = false;//3D效果显示

                //创建一个标题的对象
/*                Title title = new Title();

                //设置标题的名称
                title.Text = name;
                title.Padding = new Thickness(0, 5, 10, 0);

                //向图标添加标题
                ppgChart.Titles.Add(title);
*/
                //初始化一个新的Axis
             //   Axis xaxis = new Axis();
              //  xaxis.AxisMinimum = 0;
                //设置图表中Y轴的后缀          
             //   xaxis.Suffix = " ";
                //给图标添加Axis            
              //  ppgChart.AxesX.Add(xaxis);

                Axis yAxis = new Axis();
                //设置图标中Y轴的最小值永远为0           
                yAxis.AxisMinimum = 0;
                yAxis.AxisMaximum = 100;
                //设置图表中Y轴的后缀          
                yAxis.Suffix = " ";
                yAxis.Enabled = false;
                ppgChart.AxesY.Add(yAxis);

                Axis xAxis = new Axis();
                xAxis.Enabled = false;
                ppgChart.AxesX.Add(xAxis);

                PlotArea plot = new PlotArea();
                plot.ShadowEnabled = false;
                plot.Background = new SolidColorBrush(Color.FromArgb(255, 17, 24, 35));
                ppgChart.PlotArea = plot;

                HrvChartGrid2.Children.Add(ppgChart);
            }
            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式。               
            dataSeries.LegendText = "樱桃";

            dataSeries.RenderAs = RenderAs.Spline;//折线图
            dataSeries.Color = new SolidColorBrush(Color.FromArgb(150, 62, 233, 253));
//            dataSeries.ShadowEnabled = false;
            
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < lsTime.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.XValue = double.Parse(lsTime[i]);
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(cherry[i]);
                dataPoint.MarkerSize = 0;
                //dataPoint.Tag = tableName.Split('(')[0];
                //设置数据点颜色                  
                // dataPoint.Color = new SolidColorBrush(Colors.LightGray);  
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            ppgChart.Series.Add(dataSeries);
            if(ppgChart.Series.Count > 1)
            {
                ppgChart.Series.RemoveAt(0);
            }

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
/*            Grid gr = new Grid();
            gr.Children.Add(ppgChart);

            HrvChartGrid2.Children.Add(gr);
*/
//            Grid gr = new Grid();
//            gr.Children.Add(ppgChart);

              

        }
        #endregion

        #region ppgChart点击事件
        //点击事件
        private void ppgbutton_Click(object sender, RoutedEventArgs e)
        {
            hrvbutton.IsEnabled = true;
            ibibutton.IsEnabled = true;
            ppgbutton.IsEnabled = false;
            HrvImage.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/ppgbgd.png", UriKind.Relative));
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            if (ppgChart != null)
            {
                ppgChart.Visibility = System.Windows.Visibility.Hidden;
            }
            if (null == ppgChart)
            {
                List<string> listx2 = new List<string>();
                List<string> listy2 = new List<string>();

                CreateChartSpline("PPG", listx2, listy2);
            }
            else
            {
                ppgChart.Visibility = System.Windows.Visibility.Visible;
            }

            if (ibiChart != null)
            {
                ibiChart.Visibility = System.Windows.Visibility.Hidden;
            }


            HRVChartView.Visibility = System.Windows.Visibility.Hidden;
            VerScrollBar.Visibility = System.Windows.Visibility.Hidden;
            HorSrollBar.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion

        //设置
        private void setButton_Click(object sender, RoutedEventArgs e)
        {
            setButton.IsEnabled = false;
                //弹出事件标记框
            hrvSeting = new HRVSettingView();
            hrvSeting.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            hrvSeting.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            hrvSeting.Margin = new Thickness(220,220, 0, 0);

            hrvSeting.closeSettingButton.Click += OnCloseSetting;
            hrvSeting.saveSettingButton.Click += OnSaveSetting;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            this.mainWindow.Children.Add(hrvSeting);
            //mark.Fill = Brushes.Transparent;
            //mark.Margin = new Thickness(0, 60, 0, 0);
            //mainWindow.Children.Add(mark);//添加屏蔽层
        }

        /// <summary>
        /// 取消设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseSetting(object sender, EventArgs e)
        {
            setButton.IsEnabled = true;
            hrvSeting.onCheckBox.IsChecked = true;
            hrvSeting.offCheckBox.IsChecked = false;
            hrvSeting.basisCheckBox.IsChecked = true;
            hrvSeting.fiveminCheckBox.IsChecked = false;
            hrvSeting.tenminCheckBox.IsChecked = false;
            hrvSeting.defaultCheckBox.IsChecked = true;
            this.mainWindow.Children.Remove(hrvSeting);
            hrvSeting = null;
        }
        /// <summary>
        /// 点击设置确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSaveSetting(object sender, EventArgs e)
        {
            setButton.IsEnabled = true;
            if (hrvSeting.onCheckBox.IsChecked == false && hrvSeting.offCheckBox.IsChecked == false)
            {
                PmtsMessageBox.CustomControl1.Show("请选择是否开启背景音乐", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }
            else if (hrvSeting.basisCheckBox.IsChecked == false && hrvSeting.fiveminCheckBox.IsChecked == false && hrvSeting.tenminCheckBox.IsChecked == false) {
                PmtsMessageBox.CustomControl1.Show("请选择监测类型", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }else if(hrvSeting.defaultCheckBox.IsChecked ==false && hrvSeting.stringstring == null && hrvSeting.onCheckBox.IsChecked == true){
                PmtsMessageBox.CustomControl1.Show("请选择自选音乐地址", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }
            else
            {
                //是否开启背景音乐
                if (hrvSeting.onCheckBox.IsChecked == false)
                {
                    MusicPlayerIsMuted = true;
                    MusicPlayer.IsMuted = true;
                }
                else
                {
                    MusicPlayerIsMuted = false;
                    MusicPlayer.IsMuted = false;
                }

                if (hrvSeting.basisCheckBox.IsChecked == true)
                {
                    SelectComboBox = 0;
                }
                if (hrvSeting.fiveminCheckBox.IsChecked == true)
                {
                    SelectComboBox = 1;
                }
                if (hrvSeting.tenminCheckBox.IsChecked == true)
                {
                    SelectComboBox = 2;
                }

                if (hrvSeting.stringstring != null || hrvSeting.defaultCheckBox.IsChecked == false)
                {
                    MusicPlayerOpen = hrvSeting.stringstring;
                    MuscePlayerdefaultCheckBox = false;
                }
                if(hrvSeting.defaultCheckBox.IsChecked ==true){
                    MuscePlayerdefaultCheckBox = true;
                    MusicPlayerOpen = @"Resources/PureMusic.mp3";
                }
                //Hashtable tmp = new Hashtable();
                //tmp.Add("Time", hrvMark.SaveTime);
                //tmp.Add("Content", hrvMark.markContent.Text);
                //tmp.Add("DateTime", hrvMark.MarkDateTime);
                //hrvMarkArr.Add(tmp);
                //this.HRVChartView.OnAddMarkLable(Convert.ToInt32(Math.Floor(hrvMark.SaveTime * 2)), hrvMark.markContent.Text);

                //HrvMarkButton.IsEnabled = true;
                this.mainWindow.Children.Remove(hrvSeting);
                hrvSeting = null;
            }
        }
    }
}
