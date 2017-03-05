using System;
using System.Collections;
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
using PmtsControlLibrary.DBPlugin;
using pmts_net.HMRead;

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainDesensitization.xaml 的交互逻辑
    /// </summary>
    public partial class TrainDesensitization : UserControl
    {
        private double[,] EPRange = { { 0, 0.6, 1 }, { 0.6, 1.5, 2 }, { 1.5, 4, 3 }, { 4, 7, 5 }, { 7, 15, 8 }, { 15, 25, 10 }, { 25, 45, 12 }, { 45, 70, 15 }, { 70, 100, 20 }, { 100, 150, 25 }, { 150, 230, 35 }, { 230, 350, 50 }, { 350, 450, 70 }, { 450, 600, 80 }, { 600, 65530, 100 } };
        private TrainDB tdb = new TrainDB();
        private System.Windows.Threading.DispatcherTimer startTimer = new System.Windows.Threading.DispatcherTimer();
        private ArrayList playList = new ArrayList();
        private int playIndex = 0;
        private HDRead hd = new HDRead();
        private System.Windows.Threading.DispatcherTimer HRVReadTimer = new System.Windows.Threading.DispatcherTimer();
        private System.Windows.Threading.DispatcherTimer timeTimer = new System.Windows.Threading.DispatcherTimer();
        
        private bool isStart = false;
        private bool isImage = false;
        private Double epOver500 = 0;//EP得分累加
        private DateTime startTime;//开始时间
        private DateTime endTime;//结束时间

        private bool isGameStart = false;
        private ArrayList hrvMarkArr = new ArrayList();//事件标记记录数组（结构：{HashMap,HashMap}）
        private ArrayList HRVData = new ArrayList();//HRV曲线数组
        private ArrayList IBIData = new ArrayList();//IBI柱状图数组
        private ArrayList PPGData = new ArrayList();//PPG折线图
        private ArrayList EPData = new ArrayList();//EP值数组

        public TrainDesensitization()
        {
            InitializeComponent();
            tdb = new TrainDB();
            this.Unloaded += OnCloseTrain;
            playIndex = 0;

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
        /// 初始化脱敏训练
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInitPlayer(object sender, RoutedEventArgs e)
        {
            playList = new ArrayList();
            //playList = tdb.GetResourcesList();
            //if (playList.Count > 0)
            //{
            //    for (int i = 0; i < playList.Count; i++)
            //    {
            //        Hashtable info = playList[i] as Hashtable;
            //        info["path"] = "http://" + UserInfoStatic.ipAdd + ":8081/" + info["path"];
            //    }

                startTimer.Interval = new TimeSpan(0, 0, 5);
                startTimer.Tick += OnStartTrain;
                startTimer.Start();
/*
                timeTimer = new System.Windows.Threading.DispatcherTimer();
                timeTimer.Interval = new TimeSpan(0, 0, 0, 0, 125);
                timeTimer.Tick += OnTimeTimer;
                timeTimer.Start();
 */ 
            //}
        }
        /// <summary>
        /// 开始脱敏训练
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartTrain(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            startTimer.Stop();
            startTimer = new System.Windows.Threading.DispatcherTimer();
            startTimer.Interval = new TimeSpan(0, 0, 1);
            startTimer.Tick += OnShowRe;
            playIndex = 0;//计数器归零
            this.epGrid.Opacity = 1;//显示ep条
            //this.timeGrid.Opacity = 1;//显示进度条
            this.scoreGrid.Opacity = 0;
            OnAnimationForEP(0);//ep条归零
            OnStartHD();//开始测量HRV
            //this.numText.Text = (playIndex + 1) + "/" + playList.Count.ToString();
            this.RootGrid.Children.Remove(this.WraingImage);
            startTimer.Start();

            isGameStart = true;
            HRVData = new ArrayList();//初始化HRV曲线数组
            EPData = new ArrayList();//初始化EP数组
            IBIData = new ArrayList();//初始化IBI数组
            PPGData = new ArrayList();//初始化PPG数组
            hrvMarkArr = new ArrayList();//初始化时间标记数组

       //     MessageBox.Show("开始游戏");
 
        }
        /// <summary>
        /// 开始硬件
        /// </summary>
        private void OnStartHD()
        {
            hd = new HDRead();
            if (hd.StartDriver())
            {
                isStart = true;
                HRVReadTimer = new System.Windows.Threading.DispatcherTimer();
                HRVReadTimer.Tick += new EventHandler(OnTimerHRV);
                HRVReadTimer.Interval = new TimeSpan(0, 0, 0, 0, 125);
                HRVReadTimer.Start();
            }
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

                ArrayList tempEPArr = hd.EPArr;
                for (int ep = 0; ep < tempEPArr.Count; ep++)
                {
                    OnAnimationForEP((Single)tempEPArr[ep]);
                }

            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 进度条Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeTimer(object sender, EventArgs e)
        {
            if (isImage)
            {
                //this.timeMask.ScaleX = this.timeMask.ScaleX - (125.0 / 30000);
            }
        }
        /// <summary>
        /// 开始播放设置好的内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowRe(object sender, EventArgs e)
        {
            startTimer.Stop();
            //this.numText.Text = (playIndex + 1) + "/" + playList.Count.ToString();//题号替换
            //this.timeMask.ScaleX = 1;//进度条归1
            this.playGrid.Children.Clear();
            //Hashtable playInfo = playList[playIndex] as Hashtable;
            //if (Convert.ToInt32(playInfo["type"]) == 1)
            //{
            //    isImage = true;
            //    //播放图片
            //    startTimer = new System.Windows.Threading.DispatcherTimer();
            //    startTimer.Interval = new TimeSpan(0, 0, 30);
            //    startTimer.Tick += OnShowRe;
            //    Image im = new Image();
            //    im.Source = new BitmapImage(new Uri(playInfo["path"].ToString(), UriKind.Absolute));
            //    this.playGrid.Children.Add(im);
            //    startTimer.Start();
            //}
            //else
            //{
                isImage = false;
                //播放视频
                MediaElement me = new MediaElement();
                me.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Resources/1.mp4", UriKind.RelativeOrAbsolute);
                me.Margin = new Thickness(180, 20, 120,80);
                me.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                me.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                //me.Width = 800;
                //me.Height = 968;
                //me.MediaOpened += OnStartPlayVideo;
                me.MediaEnded += OnEndPlayVideo;//播放结束
                me.BufferingStarted += OnStartBuff;
                me.BufferingEnded += OnEndBuff;

                me.LoadedBehavior = MediaState.Play;
                me.UnloadedBehavior = MediaState.Close;

                this.RootGrid.Children.Add(me);
            //}
            playIndex += 1;
            //if (playIndex > playList.Count - 1)
            //{
                playIndex = 0;
            //}
        }
        /// <summary>
        /// 开始缓冲BUFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartBuff(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.Write("开始缓冲视频\n");
            //MessageBox.Show("开始缓冲视频\n");
        }
        /// <summary>
        /// 结束缓冲BUFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndBuff(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.Write("结束缓冲视频\n");
        }
        /// <summary>
        /// 视频载入成功后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartPlayVideo(object sender, EventArgs e)
        {
            try
            {
                MediaElement me = sender as MediaElement;
                me.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("视频开始载入时出现问题" + ex.Message + "\n");
            }
        }
        /// <summary>
        /// 视频结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndPlayVideo(object sender, EventArgs e)
        {
/*            startTimer = new System.Windows.Threading.DispatcherTimer();
            startTimer.Interval = new TimeSpan(0, 0, 1);
            startTimer.Tick += OnShowRe;
            startTimer.Start();
 */
            this.RootGrid.Children.Clear();
            this.RootGrid.Children.Add(this.WraingImage);

            endTime = DateTime.Now;
            TimeSpan diffTime = new TimeSpan();
            TimeSpan startTick = new TimeSpan(startTime.Ticks);
            TimeSpan endTick = new TimeSpan(endTime.Ticks);
            diffTime = endTick.Subtract(startTick).Duration();
            System.Diagnostics.Debug.Write("开始和结束的时间差:" + diffTime.TotalSeconds + "\n");
            OnStopAllTimerAndHD();//停止所有timer
            this.playGrid.Children.Clear();
            //this.timeGrid.Opacity = 0;
            this.epGrid.Opacity = 0;
            this.scoreGrid.Opacity = 1;
            this.playGrid.Opacity = 0;
            if (diffTime.TotalSeconds < 60)
            {
                this.totalTimeText.Text = Math.Floor(diffTime.TotalSeconds) + "秒";
            }
            else
            {
                this.totalTimeText.Text = diffTime.Minutes + "分" + diffTime.Seconds + "秒";
            }
            Double totalScore = Math.Floor((epOver500 / diffTime.TotalSeconds) * 100.0);
            this.totalScoreText.Text = totalScore.ToString();
            //-----------------------------对数据库操作
            Hashtable historyInfo = new Hashtable();
            historyInfo["trid"] = tdb.GetTrainRecordID();
            historyInfo["tid"] = 12;
            historyInfo["score"] = totalScore;
            historyInfo["gate"] = 1;
            historyInfo["diff"] = 2;
            historyInfo["o"] = 0;
            historyInfo["r"] = 0;
            historyInfo["t"] = 0;
            historyInfo["e"] = totalScore * 0.2;
            historyInfo["w"] = totalScore * 0.05;
            historyInfo["bo"] = UserInfoStatic.O;
            historyInfo["br"] = UserInfoStatic.R;
            historyInfo["bt"] = UserInfoStatic.T;
            historyInfo["be"] = UserInfoStatic.E;
            historyInfo["bw"] = UserInfoStatic.W;
            tdb.OnInsertTrainToHistory(historyInfo);

            UserInfoStatic.E += totalScore * 0.2;
            UserInfoStatic.W += totalScore * 0.05;
            tdb.OnUpdateTrainDataToUserPara();
            if (UserInfoStatic.O >= 500 && UserInfoStatic.R >= 500 && UserInfoStatic.T >= 500 && UserInfoStatic.E >= 500 && UserInfoStatic.W >= 500)
            {
                tdb.OnUpdateTrainAll();
            }
            //--------------------------------绘制雷达图
            DrawRadar(UserInfoStatic.O, UserInfoStatic.W, UserInfoStatic.E, UserInfoStatic.T, UserInfoStatic.R);
            //--------------------------------基本属性归零
            epOver500 = 0.0;

            

 //new           OnStopAllTimerAndHD();
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
                    epOver500 += EPRange[i, 2];
                    System.Diagnostics.Debug.Write("ep得分累加：" + epOver500 + "\n");
                    if (epOver500 >= 500)
                    {
                        endTime = DateTime.Now;
                        TimeSpan diffTime = new TimeSpan();
                        TimeSpan startTick = new TimeSpan(startTime.Ticks);
                        TimeSpan endTick = new TimeSpan(endTime.Ticks);
                        diffTime = endTick.Subtract(startTick).Duration();
                        System.Diagnostics.Debug.Write("开始和结束的时间差:" + diffTime.TotalSeconds + "\n");
                        OnStopAllTimerAndHD();//停止所有timer
                        this.playGrid.Children.Clear();
                        //this.timeGrid.Opacity = 0;
                        this.epGrid.Opacity = 0;
                        this.scoreGrid.Opacity = 1;
                        this.playGrid.Opacity = 0;
                        if (diffTime.TotalSeconds < 60)
                        {
                            this.totalTimeText.Text = Math.Floor(diffTime.TotalSeconds) + "秒";
                        }
                        else
                        {
                            this.totalTimeText.Text = diffTime.Minutes + "分" + diffTime.Seconds + "秒";
                        }
                        Double totalScore = Math.Floor((epOver500 / diffTime.TotalSeconds) * 100.0);
                        this.totalScoreText.Text = totalScore.ToString();
                        //-----------------------------对数据库操作
                        Hashtable historyInfo = new Hashtable();
                        historyInfo["trid"] = tdb.GetTrainRecordID();
                        historyInfo["tid"] = 12;
                        historyInfo["score"] = totalScore;
                        historyInfo["gate"] = 1;
                        historyInfo["diff"] = 2;
                        historyInfo["o"] = 0;
                        historyInfo["r"] = 0;
                        historyInfo["t"] = 0;
                        historyInfo["e"] = totalScore * 0.2;
                        historyInfo["w"] = totalScore * 0.05;
                        historyInfo["bo"] = UserInfoStatic.O;
                        historyInfo["br"] = UserInfoStatic.R;
                        historyInfo["bt"] = UserInfoStatic.T;
                        historyInfo["be"] = UserInfoStatic.E;
                        historyInfo["bw"] = UserInfoStatic.W;
                        tdb.OnInsertTrainToHistory(historyInfo);

                        UserInfoStatic.E += totalScore * 0.2;
                        UserInfoStatic.W += totalScore * 0.05;
                        tdb.OnUpdateTrainDataToUserPara();
                        if (UserInfoStatic.O >= 500 && UserInfoStatic.R >= 500 && UserInfoStatic.T >= 500 && UserInfoStatic.E >= 500 && UserInfoStatic.W >= 500)
                        {
                            tdb.OnUpdateTrainAll();
                        }
                        //--------------------------------绘制雷达图
                        DrawRadar(UserInfoStatic.O, UserInfoStatic.W, UserInfoStatic.E, UserInfoStatic.T, UserInfoStatic.R);   
                        //--------------------------------基本属性归零
                        epOver500 = 0.0;
                    }
                }
            }
            epDoubleAnime.To = epMaskValue / 698;
            epStory.Begin();
        }
        /// <summary>
        /// 绘制雷达图
        /// </summary>
        private void DrawRadar(Double o,Double w,Double e,Double t,Double r)
        {
            //--------------------------------------------------------------绘制正五边形--------------------------------------------------------
            Point p1 = new Point(120, 0);//第一个点
            Point p2 = new Point(120 - (Math.Cos(18 * Math.PI / 180) * 120), 120 - (Math.Sin(18 * Math.PI / 180) * 120));//逆时针。第二个点坐标
            Point p3 = new Point(120 - (Math.Cos(54 * Math.PI / 180) * 120), 120 + (Math.Sin(54 * Math.PI / 180) * 120));//逆时针第三个点坐标
            Point p4 = new Point(120 + (Math.Cos(54 * Math.PI / 180) * 120), 120 + (Math.Sin(54 * Math.PI / 180) * 120));//逆时针第四个点坐标
            Point p5 = new Point(120 + (Math.Cos(18 * Math.PI / 180) * 120), 120 - (Math.Sin(18 * Math.PI / 180) * 120));//逆时针第五个点坐标
            PathFigure pf = new PathFigure();
            pf.StartPoint = p1;
            pf.IsClosed = true;
            LineSegment ls1 = new LineSegment();
            LineSegment ls2 = new LineSegment();
            LineSegment ls3 = new LineSegment();
            LineSegment ls4 = new LineSegment();
            ls1.Point = p2;
            ls2.Point = p3;
            ls3.Point = p4;
            ls4.Point = p5;
            pf.Segments.Add(ls1);
            pf.Segments.Add(ls2);
            pf.Segments.Add(ls3);
            pf.Segments.Add(ls4);
            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);
            this.RadarPF.Data = pg;
            //--------------------------------------------------------------绘制正五边形结束------------------------------------------------------
            //--------------------------------------------------------------绘制正五边形中的连线---------------------------------------------------
            LineGeometry l1 = new LineGeometry(new Point(120, 120), p1);
            LineGeometry l2 = new LineGeometry(new Point(120, 120), p2);
            LineGeometry l3 = new LineGeometry(new Point(120, 120), p3);
            LineGeometry l4 = new LineGeometry(new Point(120, 120), p4);
            LineGeometry l5 = new LineGeometry(new Point(120, 120), p5);
            //--------------------------------------------------------------绘制正五边形中的连线结束---------------------------------------------------
            GeometryGroup gg = new GeometryGroup();
            gg.Children.Add(l1);
            gg.Children.Add(l2);
            gg.Children.Add(l3);
            gg.Children.Add(l4);
            gg.Children.Add(l5);
            this.Midpoint.Data = gg;
            //--------------------------------------------------------------绘制雷达图中的数据显示区域---------------------------------------------------
            Point rp1 = new Point(120, 120 - MathLong(o));
            Point rp2 = new Point(120 - (Math.Cos(18 * Math.PI / 180) * MathLong(w)), 120 - (Math.Sin(18 * Math.PI / 180) * MathLong(w)));
            Point rp3 = new Point(120 - (Math.Cos(54 * Math.PI / 180) * MathLong(e)), 120 + (Math.Sin(54 * Math.PI / 180) * MathLong(e)));
            Point rp4 = new Point(120 + (Math.Cos(54 * Math.PI / 180) * MathLong(t)), 120 + (Math.Sin(54 * Math.PI / 180) * MathLong(t)));
            Point rp5 = new Point(120 + (Math.Cos(18 * Math.PI / 180) * MathLong(r)), 120 - (Math.Sin(18 * Math.PI / 180) * MathLong(r)));
            PathFigure rpf = new PathFigure();
            rpf.StartPoint = rp1;
            rpf.IsClosed = true;
            LineSegment rl1 = new LineSegment(rp2, true);
            LineSegment rl2 = new LineSegment(rp3, true);
            LineSegment rl3 = new LineSegment(rp4, true);
            LineSegment rl4 = new LineSegment(rp5, true);
            rpf.Segments.Add(rl1);
            rpf.Segments.Add(rl2);
            rpf.Segments.Add(rl3);
            rpf.Segments.Add(rl4);
            PathGeometry rpg = new PathGeometry();
            rpg.Figures.Add(rpf);
            this.Radar.Data = rpg;
        }
        /// <summary>
        /// 计算雷达图中维度的长度
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private double MathLong(double num)
        {
            double one = 120.0 / 1000.0;
            double ret = one * num;
            if (ret > 120)
            {
                ret = 120.0f;
            }
            return ret;
        }
        /// <summary>
        /// 当训练被移除时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseTrain(object sender, RoutedEventArgs e)
        {
           OnStopAllTimerAndHD();
  
            
        }
        /// <summary>
        /// 停止所有timer和硬件设备
        /// </summary>
        private void OnStopAllTimerAndHD()
        {
            if (startTimer.IsEnabled == true)
            {
                startTimer.Stop();
                startTimer = new System.Windows.Threading.DispatcherTimer();
                this.playGrid.Children.Clear();
            }
            if (isStart)
            {
                isStart = false;
                isImage = false;
                hd.StopDriver();
                HRVReadTimer.Stop();
                HRVReadTimer = new System.Windows.Threading.DispatcherTimer();
                //EPData = new ArrayList();
            }
            if (timeTimer.IsEnabled == true)
            {
                timeTimer.Stop();
                //timeTimer = new System.Windows.Threading.DispatcherTimer();
            }

            if (isGameStart)
            {
                //结束游戏
        //        MessageBox.Show("结束游戏");
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
//                    HRVDataCalc["TimeType"] = Convert.ToInt32(tInfo["tid"]);//HRV检测时间类型
                    HRVDataCalc["TimeType"] = 20;//HRV检测时间类型
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
        }
        /// <summary>
        /// 重新开训练
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResetTrain(object sender, RoutedEventArgs e)
        {
            epOver500 = 0.0;
            playIndex = 0;//计数器归零
            this.epGrid.Opacity = 1;//显示ep条
            //this.timeGrid.Opacity = 1;//显示进度条
            this.playGrid.Opacity = 1;
            this.scoreGrid.Opacity = 0;
            OnAnimationForEP(0);//ep条归零
            OnStartHD();//开始测量HRV
            //this.numText.Text = (playIndex + 1) + "/" + playList.Count.ToString();
            this.RootGrid.Children.Remove(this.WraingImage);
            startTimer = new System.Windows.Threading.DispatcherTimer();
            startTimer.Interval = new TimeSpan(0, 0, 1);
            startTimer.Tick += OnShowRe;
            startTimer.Start();
 //           timeTimer.Start();

            isGameStart = true;
            HRVData = new ArrayList();//初始化HRV曲线数组
            EPData = new ArrayList();//初始化EP数组
            IBIData = new ArrayList();//初始化IBI数组
            PPGData = new ArrayList();//初始化PPG数组
            hrvMarkArr = new ArrayList();//初始化时间标记数组

     //       MessageBox.Show("开始游戏");

        }
    }
}
