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
using System.Xml;
using System.IO;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.Office.Interop.Excel;
using pmts_net.HMRead;


namespace PmtsControlLibrary
{
    /// <summary>
    /// RecordPlayerView.xaml 的交互逻辑
    /// </summary>
    public partial class RecordPlayerView : UserControl
    {
        private double[,] EPRange = { { 0, 0.6, 1 }, { 0.6, 1.5, 2 }, { 1.5, 4, 3 }, { 4, 7, 5 }, { 7, 15, 8 }, { 15, 25, 10 }, { 25, 45, 12 }, { 45, 70, 15 }, { 70, 100, 20 }, { 100, 150, 25 }, { 150, 230, 35 }, { 230, 350, 50 }, { 350, 450, 70 }, { 450, 600, 80 }, { 600, 65530, 100 } };//ep档位范围
        private Grid mainWindow = new Grid();
        private System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        private AxShockwaveFlashObjects.AxShockwaveFlash shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
        private DBPlugin.CourseDB cdb = new DBPlugin.CourseDB();
        private bool isPlaying = false;
        private ClassTopView ctv = null;
        private RecordMusicView rmv = null;//自选音乐 
        private TreeView courseTreeView = null;
        private System.Windows.Shapes.Rectangle mark = new System.Windows.Shapes.Rectangle();//屏蔽层
        private MediaPlayer MusicPlayer = null;//音乐播放
        private bool isMusicing = false;//是否有mp3音乐 

        private DateTime startTime;//开始时间

        private bool isStart = false;
        private DispatcherTimer HRVReadTimer;//读取HRV时的Timer
        private HDRead hd = new HDRead();
        private bool isGameStart = false;
        private ArrayList hrvMarkArr = new ArrayList();//事件标记记录数组（结构：{HashMap,HashMap}）
        private ArrayList HRVData = new ArrayList();//HRV曲线数组
        private ArrayList IBIData = new ArrayList();//IBI柱状图数组
        private ArrayList PPGData = new ArrayList();//PPG折线图
        private ArrayList EPData = new ArrayList();//EP值数组
        private double EPScore = 0;//EP得分
        
        private HRVControlWEB hrvdb = null;
        
        public RecordPlayerView()
        {
            InitializeComponent();

        }
        public RecordPlayerView(Grid Main)
        {
            InitializeComponent();
            mainWindow = Main;
            cdb = new DBPlugin.CourseDB();
            double left = this.Margin.Left - 80;
            this.Margin = new Thickness(left, 20, 0, 0);
            this.CourseListGrid.Visibility = System.Windows.Visibility.Visible;
            tmrProgress = new DispatcherTimer();
            //设置计时器的时间间隔为1秒
            tmrProgress.Interval = new TimeSpan(0, 0, 1);
            //计时器触发事件处理
            tmrProgress.Tick += SetDisplayMessage;
            SetImageForMediaElement();

            ctv = new ClassTopView();
            ctv.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            ctv.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            ctv.Margin = new Thickness(24, 5, 0, 0);

            ctv.clasTopImage.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/relaxTopImage.png", UriKind.Relative));
            ctv.RelaxNameText.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText3.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText4.Visibility = System.Windows.Visibility.Hidden;
            if (!this.mainWindow.Children.Contains(ctv))
            {
                this.mainWindow.Children.Add(ctv);
            }
            courseTreeView = new TreeView();
            MusicPlayer = new MediaPlayer();
            //button声音
            Grid uiButton = this.Content as Grid;
            UIElementCollection Childrens = uiButton.Children;
            foreach (UIElement ui in Childrens)
            {
                //ui转成控件
                if (ui is System.Windows.Controls.Button)
                {
                    ui.MouseEnter += new MouseEventHandler(ui_MouseEnter);
                }
            }
            initFullScreenVideo();
        }

        private MediaElement myPlayer = null;
        void initFullScreenVideo()
        {
            myPlayer = new MediaElement();
            Grid parent = (Grid)mainWindow.Parent;
            Grid root = (Grid)parent.Parent;

            myPlayer.Margin = new Thickness(0, 0, 0, 0);
            myPlayer.Width = root.ActualWidth;
            myPlayer.Height = root.ActualHeight;
            myPlayer.Stretch = Stretch.Fill;

            myPlayer.LoadedBehavior = MediaState.Manual;
            myPlayer.MouseDown += videoScreenMediaElement_MouseDown2;

            root.Children.Add(myPlayer);
        }

        //鼠标在button上
        void ui_MouseEnter(object sender, MouseEventArgs e)
        {
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
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
                            OnAnimationForEP((Single)tmpEPArr[ep]);
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

            if (isGameStart)
            {
                //结束游戏
                //              MessageBox.Show("结束游戏");
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
                    HRVDataCalc["TimeType"] = 100;//HRV检测时间类型
                    //                HRVDataCalc["Mood"] = this.systemMeg["Mood"];//测量时心情状态
                    HRVDataCalc["Mood"] = 101;
                    HRVDataCalc["HRVMark"] = hrvMarkArr;//事件标记

                    //开始数据库操作
                    //lich
                    if (UserInfoStatic.ipAdd != null)
                    	hrvdb.OnInsertHRVDataAndEpData(HRVData, EPData, hrvMarkArr, HRVDataCalc, PPGData,"3");
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
            //if (timeTimer.IsEnabled == true)
            //{
            //    timeTimer.Stop();
            //    //timeTimer = new System.Windows.Threading.DispatcherTimer();
            //}
        }

        private void viewGrid_Loaded(object sender, RoutedEventArgs e)
        {

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
                    double epOffice = EPValue * ((698 / 15) / (EPRange[i, 1] - EPRange[i, 0]));
                    epMaskValue = (i * (698 / 15)) + epOffice;
                    EPScore += EPRange[i, 2];
                    string ScoreStr = Convert.ToString(Math.Floor(EPScore)).PadLeft(6, '0');

                }
            }
            epDoubleAnime.To = epMaskValue / 698;
            epStory.Begin();

        }

        private void FromFlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            //   System.Diagnostics.Debug.Write("解析nodeXml："+NodeXml( e.request.ToString())+"\n");
            //MessageBox.Show(e.request.ToString());
        }
        private void changeVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Diagnostics.Debug.Write("Slider值：" + e.NewValue + "\n");
            shockwave.CallFunction("<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>" + e.NewValue + "</number></arguments></invoke>");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Children.Remove(this);
        }



        /// <summary>
        /// 载入课程列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool firstLoad = true;
        private void OnLoadListView(object sender, RoutedEventArgs e)
        {
            if (firstLoad)
            {
                firstLoad = false;
                courseTreeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(courseTreeView_SelectedItemChanged);
                //courseTreeView.FontFamily = new System.Windows.Media.FontFamily("Microsoft YaHei");
                courseTreeView.FontSize = 14;
                courseTreeView.Height = 584;

                courseTreeView.Background = null;
                courseTreeView.Margin = new Thickness(0);

                DirectoryInfo folder = new DirectoryInfo(@"Resources\放松中心");
                DirectoryInfo[] chldFolders = folder.GetDirectories();
                foreach (DirectoryInfo chldFolder in chldFolders)
                {
                    if (UserInfoStatic.hasAuth(chldFolder.Name))
                    {
                        TreeViewItem chldNode = new TreeViewItem();
                        chldNode.Header = chldFolder.Name;
                        chldNode.Foreground = new SolidColorBrush(Colors.White);  //用固态画刷填充前景色
                        chldNode.IsExpanded = true;
                        chldNode.Tag = chldFolder.FullName;
                        GetFiles(chldFolder.FullName, chldNode);
                        courseTreeView.Items.Add(chldNode);
                    }
                }
                if (!this.ClassListTreeView.Children.Contains(courseTreeView))
                {
                    this.ClassListTreeView.Children.Add(courseTreeView);
                }
            }
        }

        //
        void courseTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ctv.RelaxNameText.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText3.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText4.Visibility = System.Windows.Visibility.Hidden;
            MusicPlayer.Stop();
            var selected = courseTreeView.SelectedItem as TreeViewItem;
            DirectoryInfo folder = new DirectoryInfo(selected.Tag.ToString());
            Uri selecteUri = new Uri(selected.Tag.ToString());

            var name = folder.Name;
            List<string> list = new List<string>();

            FolderNames(selected.Tag.ToString(), list);

            ctv.RelaxNameText.Visibility = System.Windows.Visibility.Visible;


            ctv.RelaxNameText.Text = list[0];
            if (list.Count > 2)
            {

                ctv.RelaxNameText.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText.Text = list[2];

                ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText2.Text = list[1];

                ctv.RelaxNameText3.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText4.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText4.Text = list[0];
            }
            else if (list.Count > 1)
            {
                ctv.RelaxNameText.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText.Text = list[1];

                ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Visible;

                ctv.RelaxNameText2.Text = list[0];
            }
            else
            {
                ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Hidden;
                ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Hidden;
            }

            string extension; //
            extension = System.IO.Path.GetExtension(selected.Tag.ToString());

            if (extension.ToLower() == ".mp3")
            {
                //videoScreenMediaElement.Source = uri;

                FileInfo[] chldFiles = folder.Parent.GetFiles("*.*");
                foreach (FileInfo chlFile in chldFiles)
                {
                    var rs1 = chlFile.Name.ToLower().Replace(".mp4", "");
                    var rs2 = chlFile.Name.Split('.')[0];
                    if (rs2.Equals("1"))
                    {
                        Uri uri = new Uri(chlFile.FullName);
                        videoScreenMediaElement.Source = uri;
                    }

                }


                MusicPlayer.Open(selecteUri);
                MusicPlayer.Play();
                videoScreenMediaElement.IsMuted = true;
                MusicPlayer.IsMuted = false;
                isPlaying = false;
                isMusicing = true;
                myplay();
                mute.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/mute.png", UriKind.Relative));



            }
            if (extension.ToLower() == ".mp4")
            {
                videoScreenMediaElement.Source = selecteUri;
                isPlaying = false;
                isMusicing = false;
                MusicPlayer.IsMuted = true;
                videoScreenMediaElement.IsMuted = false;
                myplay();
            }
        }

        private void FolderNames(string file, List<string> list)
        {
            DirectoryInfo folder = new DirectoryInfo(file);
            var rs1 = folder.Name.ToString().ToLower().Replace(".mp3", "");
            rs1 = rs1.ToLower().Replace(".mp4", "");
            list.Add(rs1);
            if (!folder.Parent.ToString().Equals("放松中心"))
            {
                var rs2 = folder.FullName.ToString().ToLower().Remove(folder.FullName.ToString().Length - folder.Name.ToString().Length - 1);
                FolderNames(rs2, list);

            }
        }

        private void GetFiles(string filePath, TreeViewItem node)
        {
            DirectoryInfo folder = new DirectoryInfo(filePath);
            //node.Header = folder.Name;
            //node.Tag = folder.FullName;
            DirectoryInfo[] chldFolders = folder.GetDirectories();
            foreach (DirectoryInfo chldFolder in chldFolders)
            {
                if (UserInfoStatic.hasAuth(chldFolder.Name))
                {
                    TreeViewItem chldNode = new TreeViewItem();
                    chldNode.Header = chldFolder.Name;
                    chldNode.Foreground = new SolidColorBrush(Colors.White);  //用固态画刷填充前景色
                    chldNode.IsExpanded = true;
                    chldNode.Tag = chldFolder.FullName;
                    GetFiles(chldFolder.FullName, chldNode);
                    node.Items.Add(chldNode);
                }
            }
            FileInfo[] chldFiles = folder.GetFiles("*.*");
            foreach (FileInfo chlFile in chldFiles)
            {
                var rs2 = chlFile.Name.Split('.')[0];
                if (UserInfoStatic.hasAuth(rs2))
                {
                    var rs1 = chlFile.Name.ToLower().Replace(".mp4", "");
                    if (!rs2.Equals("1"))
                    {
                        TreeViewItem chldNode = new TreeViewItem();
                        chldNode.Header = rs2;
                        chldNode.Tag = chlFile.FullName;
                        chldNode.Foreground = new SolidColorBrush(Colors.White);  //用固态画刷填充前景色
                        chldNode.IsExpanded = true;
                        node.Items.Add(chldNode);
                    }
                }
            }
        }

        /// <summary>
        /// 开始播放课程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayCourse(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            Hashtable tmp = (Hashtable)tvi.Tag;
            host.Dispose();
            shockwave.Dispose();
            host = new System.Windows.Forms.Integration.WindowsFormsHost();
            shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
            host.Child = shockwave;
            //this.playerPanel.Children.Add(host);

            // 设置 .swf 文件相对路径
            string swfPath = System.Environment.CurrentDirectory;
            swfPath += "\\Course\\" + tmp["mid"] + "-" + tmp["cid"] + ".swf";
            shockwave.Movie = swfPath;
            shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(FromFlashCall);
            shockwave.CallFunction("<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>50</number></arguments></invoke>");
            // this.changeVolume.Value = 50;
            shockwave.Loop = false;
            //this.changeVolume.ValueChanged += changeVolume_ValueChanged;
            cdb.OnUpCourseLook(tmp);

            //           MessageBox.Show("开始。。。");
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            host.Dispose();
            videoScreenMediaElement.Stop();
            MusicPlayer.Stop();
            if (isStart)
                OnStopAllTimerAndHD();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;

        }

        private void PlayPause(object sender, ExecutedRoutedEventArgs e)
        {
            myplay();
        }



        private void play_Click(object sender, RoutedEventArgs e)
        {

            myplay();

        }

        void myplay()
        {


            //调整ep槽

            //启动计时器
            if (!tmrProgress.IsEnabled)
            {
                tmrProgress.Start();
            }
            if (isPlaying)
            {
                tmrProgress.Start();
                videoScreenMediaElement.Pause();
                isPlaying = false;
                play.IsChecked = false;
                MusicPlayer.Pause();
            }
            else
            {
                classImage.Visibility = System.Windows.Visibility.Hidden;
                videoScreenMediaElement.Play();
                isPlaying = true;
                play.IsChecked = true;
                MusicPlayer.Play();
                if (!isGameStart)
                {
                    //                 MessageBox.Show("开始。。。");
                    if (enabledDevice())
                    {
                        isGameStart = true;
                        HRVData = new ArrayList();//初始化HRV曲线数组
                        EPData = new ArrayList();//初始化EP数组
                        IBIData = new ArrayList();//初始化IBI数组
                        PPGData = new ArrayList();//初始化PPG数组
                        hrvMarkArr = new ArrayList();//初始化时间标记数组
                        OnAnimationForEP(0);//协调状态条
                        EPScore = 0;
                    }
                }
            }
        }

        private void Stop(object sender, ExecutedRoutedEventArgs e)
        {

            videoScreenMediaElement.Stop();
            play.IsChecked = false;
            isPlaying = false;


        }



        //将录像的第一帧作为播放前MediaElement显示的录像截图
        public void SetImageForMediaElement()
        {
            videoScreenMediaElement.Play();
            videoScreenMediaElement.Pause();
            videoScreenMediaElement.Position = TimeSpan.Zero;
        }

        //计时器，定时更新进度条和播放时间
        private DispatcherTimer tmrProgress = new DispatcherTimer();

        //计时器触发事件处理
        private void SetDisplayMessage(Object sender, System.EventArgs e)
        {
            //if (videoScreenMediaElement.NaturalDuration.HasTimeSpan)根据视频时间

            if (isMusicing == true)
            {
                //根据音乐时间倒计时
                if (MusicPlayer.NaturalDuration.HasTimeSpan)
                {

                    //TimeSpan currentPositionTimeSpan = videoScreenMediaElement.Position;根据视频算
                    //根据音乐算
                    TimeSpan currentPositionTimeSpan = MusicPlayer.Position;
                    string currentPosition = string.Format("{0:00}:{1:00}:{2:00}", (int)currentPositionTimeSpan.TotalHours, currentPositionTimeSpan.Minutes, currentPositionTimeSpan.Seconds);

                    //TimeSpan totaotp = videoScreenMediaElement.NaturalDuration.TimeSpan;
                    TimeSpan totaotp = MusicPlayer.NaturalDuration.TimeSpan;
                    videoAllTime.Text = "/" + string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);
                    string totalPostion = string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);

                    currentPositionTime.Text = currentPosition;


                    //注
                    //playProgressSlider.Value = videoScreenMediaElement.Position.TotalSeconds;
                    if (MusicPlayer.Position == MusicPlayer.NaturalDuration)
                    {
                        //MusicPlayer.Position = TimeSpan.Zero;
                        MusicPlayer.Stop();
                        videoScreenMediaElement.Stop();
                        play.IsChecked = false;
                        isPlaying = false;
                        classImage.Visibility = System.Windows.Visibility.Visible;
                        if (isStart)
                            OnStopAllTimerAndHD();
                    }


                }
            }
            else
            {

                if (videoScreenMediaElement.NaturalDuration.HasTimeSpan)
                {
                    //根据视频算
                    TimeSpan currentPositionTimeSpan = videoScreenMediaElement.Position;

                    string currentPosition = string.Format("{0:00}:{1:00}:{2:00}", (int)currentPositionTimeSpan.TotalHours, currentPositionTimeSpan.Minutes, currentPositionTimeSpan.Seconds);

                    TimeSpan totaotp = videoScreenMediaElement.NaturalDuration.TimeSpan;
                    string totalPostion = string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);

                    currentPositionTime.Text = currentPosition;
                    //注
                    //playProgressSlider.Value = videoScreenMediaElement.Position.TotalSeconds;
                    if (videoScreenMediaElement.Position == videoScreenMediaElement.NaturalDuration)
                    {
                        //MusicPlayer.Position = TimeSpan.Zero;
                        MusicPlayer.Stop();
                        videoScreenMediaElement.Stop();
                        play.IsChecked = false;
                        isPlaying = false;
                        classImage.Visibility = System.Windows.Visibility.Visible;
                        if (isStart)
                            OnStopAllTimerAndHD();
                    }
                }

            }


        }



        //当完成媒体加载时发生
        private void videoScreenMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            //注
            //playProgressSlider.Minimum = 0;
            //playProgressSlider.Maximum = videoScreenMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            if (isMusicing == false)
            {
                TimeSpan totaotp = videoScreenMediaElement.NaturalDuration.TimeSpan;

                videoAllTime.Text = "/" + string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);
                currentPositionTime.Text = "00:00:00";
            }



        }


        //在鼠标拖动Thumb的过程中记录其值。
        private TimeSpan ts = new TimeSpan();
        private void playProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts = TimeSpan.FromSeconds(e.NewValue);
            string currentPosition = string.Format("{0:00}:{1:00}:{2:00}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);

            currentPositionTime.Text = currentPosition;

        }

        //当拖动Thumb的鼠标放开时，从指定时间开始播放
        private void playProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            videoScreenMediaElement.Position = ts;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (tmrProgress.IsEnabled)
            {
                tmrProgress.Stop();
            }

        }
        //视频结束
        private void videoScreenMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            //videoScreenMediaElement.Position = TimeSpan.Zero;
            //videoScreenMediaElement.Stop();
            //MusicPlayer.Stop();
            //play.IsChecked = false;
            //classImage.Visibility = System.Windows.Visibility.Visible;
            if (isMusicing == false)
            {
                videoScreenMediaElement.Position = TimeSpan.Zero;
                videoScreenMediaElement.Stop();
                MusicPlayer.Stop();
                play.IsChecked = false;
                classImage.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                //视频结束音乐时长还有，循环播放视频 音乐无视频无都结束
                if (videoScreenMediaElement.Position == videoScreenMediaElement.NaturalDuration && MusicPlayer.Position != MusicPlayer.NaturalDuration)
                {
                    videoScreenMediaElement.Position = TimeSpan.Zero;
                    videoScreenMediaElement.Play();
                }
                if (videoScreenMediaElement.Position == videoScreenMediaElement.NaturalDuration && MusicPlayer.Position == MusicPlayer.NaturalDuration)
                {
                    videoScreenMediaElement.Position = TimeSpan.Zero;
                    MusicPlayer.Position = TimeSpan.Zero;
                    MusicPlayer.Stop();
                    videoScreenMediaElement.Stop();
                    play.IsChecked = false;
                    isPlaying = false;
                    classImage.Visibility = System.Windows.Visibility.Visible;
                    if (isStart)
                        OnStopAllTimerAndHD();
                }
            }

            //          MessageBox.Show("结束。。。");

        }

        private void pause_Click(object sender, RoutedEventArgs e)
        {
            videoScreenMediaElement.Pause();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            videoScreenMediaElement.Stop();
        }

        private void playImage_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Image image = sender as Image;
            Uri uri = new Uri(@"Images/pause.png");
            image.Source = new BitmapImage(uri);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 23;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 20;
        }

        private void playImage_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 28;
        }

        private void playImage_MouseLeave(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Height = 25;

        }

        //自选音乐
        private void musicImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            videoScreenMediaElement.Stop();
            MusicPlayer.Stop();
            play.IsChecked = false;
            isPlaying = false;
            rmv = new RecordMusicView();
            rmv.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            rmv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            rmv.Margin = new Thickness(160, 200, 240, 160);
            mark.Fill = Brushes.Transparent;
            mark.Margin = new Thickness(0, 60, 0, 0);
            mainWindow.Children.Add(mark);//添加屏蔽层
            rmv.CloseButton.MouseLeftButtonUp += OnCloseConstant;
            if (!this.mainWindow.Children.Contains(rmv))
            {
                this.mainWindow.Children.Add(rmv);
            }
        }
        /// <summary>
        /// 关闭自选音乐窗口 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseConstant(object sender, RoutedEventArgs e)
        {

            this.Visibility = System.Windows.Visibility.Visible;
            rmv.Visibility = System.Windows.Visibility.Visible;
            this.mainWindow.Children.Remove(rmv);
            this.mainWindow.Children.Remove(mark);
        }

        //自选音乐
        private void musicImage_MouseMove(object sender, MouseEventArgs e)
        {

            musicImage.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/music2.png", UriKind.Relative));
        }

        private void musicImage_MouseLeave(object sender, MouseEventArgs e)
        {
            musicImage.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/music.png", UriKind.Relative));
        }

        //public class BgMusic
        //{
        //    public string BgMusicPath { get; set; }
        //}
        //private void playSound()
        //{
        //    BgMusic bm = new BgMusic();
        //    bm.BgMusicPath = AppDomain.CurrentDomain.BaseDirectory + "背景音乐//bgMusic.mp3";
        //    videoScreenMediaElement.DataContext = bm;
        //}

        private void mute_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isMusicing == false)
            {
                if (videoScreenMediaElement.IsMuted == true)
                {
                    mute.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/mute.png", UriKind.Relative)); ;
                    videoScreenMediaElement.IsMuted = false;
                }
                else
                {
                    mute.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/mute2.png", UriKind.Relative));
                    videoScreenMediaElement.IsMuted = true;
                }
            }
            else
            {
                if (MusicPlayer.IsMuted == true)
                {
                    mute.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/mute.png", UriKind.Relative)); ;
                    videoScreenMediaElement.IsMuted = true;
                    MusicPlayer.IsMuted = false;
                }
                else
                {
                    mute.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/mute2.png", UriKind.Relative));
                    videoScreenMediaElement.IsMuted = true;
                    MusicPlayer.IsMuted = true;
                }
            }
        }

        private void videoScreenMediaElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            myPlayer.Source = videoScreenMediaElement.Source;
            videoScreenMediaElement.Visibility = Visibility.Hidden;
            myPlayer.Position = videoScreenMediaElement.Position;
            videoScreenMediaElement.Pause();
            myPlayer.Play();
            myPlayer.Visibility = Visibility.Visible;
        }

        private void videoScreenMediaElement_MouseDown2(object sender, MouseButtonEventArgs e)
        {
            videoScreenMediaElement.Position = myPlayer.Position;
            myPlayer.Pause();
            videoScreenMediaElement.Play();
            myPlayer.Visibility = Visibility.Hidden;
            videoScreenMediaElement.Visibility = Visibility.Visible;
        }

    }
}

