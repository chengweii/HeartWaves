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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Diagnostics;
using PmtsControlLibrary;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Interop;
using pmts_net;
using pmts_net.Plugin;
using PmtsHelp;

namespace pmts_net.AppWindow
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : System.Windows.Window
    {
        // List<int> i = new List<int>();
        //private TrainBack tb = null;//训练窗口
        private TrainCenter tc = null;//训练中心
        private ClassPlayerView cpv = null;//课程窗口
        private HRVView hv = null;//HRV曲线窗口
        private RecordPlayerView sp = null;//放松中心界面
        private RecordCenter record = null;//记录中心
        private Breathing breath = null;//呼吸助手
        private DispatcherTimer GCTimer = new DispatcherTimer();//垃圾回收Timer

        private PersonalInformation info = null;//个人信息框
        private Hashtable SystemMeg = new Hashtable();
        private String hostIP = "";//服务器端IP地址
        private NotifyIcon notifyIcon;//定义托盘图标
        private MainRightPerson mr = null;//主页面右侧信息栏
        private Rectangle mark = new Rectangle();//屏蔽层（截屏？）

        /*
         * usb操作代码
         */
        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;

        public Main(String HostIp, String userID)
        {
            InitializeComponent();
            Grid uiButton = this.Content as Grid;
            UIElementCollection Childrens = uiButton.Children;
            foreach (UIElement ui in Childrens)
            {
                //ui转成控件
                if (ui is System.Windows.Controls.Button)
                {

                    ui.MouseEnter += new System.Windows.Input.MouseEventHandler(ui_MouseEnter);
                }
            }
            hostIP = HostIp;
            this.LayoutRoot.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.LayoutRoot.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double panelTop = Math.Floor(System.Windows.SystemParameters.PrimaryScreenHeight / 2 - 100);
            this.mainLogos.Margin = new Thickness(100, panelTop, 0, 0);
            //this.MainWindow.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //this.MainWindow.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            //GCTimer = new DispatcherTimer();
            //GCTimer.Tick += OnDispose;
            //GCTimer.Interval = new TimeSpan(0, 10, 0);//间隔时间10秒
            //GCTimer.Start();
            UserInfoStatic.ipAdd = hostIP;
            UserInfoStatic.UserID = userID;
            UserInfoStatic.dbUser = Properties.Resources.DBuser;
            UserInfoStatic.dbPwd = Properties.Resources.DBpwd;
            SystemMeg["DBIP"] = HostIp;
            SystemMeg["DBName"] = Properties.Resources.DBnamge;
            SystemMeg["DBPwd"] = Properties.Resources.DBpwd;
            SystemMeg["DBUser"] = Properties.Resources.DBuser;
            SystemMeg["UserID"] = userID;
            SystemMeg["Mood"] = 101;
            OnIconShow();
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;
            if (width < 1280 || height < 768)
                System.Windows.MessageBox.Show("此软件运行时的屏幕分辨率最低为1280x768，当前分辨率不满足要求，可能会影响您的使用。请修改屏幕分辨率。");
            //PmtsNetServer mySocket = new PmtsNetServer(HostIp, 5900);
            //mySocket.AsyncOpen();
            //mySocket.AsyncSocketAcceptEvent += OnStartSocket;
            //mySocket.AsyncDataAcceptedEvent += OnReceEnd;
            //byte[] send = Encoding.ASCII.GetBytes("1," + UserInfoStatic.UserID);
            //mySocket.AsyncSend(send);

            mainButtonAuthCheck();
        }

        private void mainButtonAuthCheck()
        {
            if (!UserInfoStatic.hasAuth("教学资源"))
            {
                ClassBtn.Click -= MainLogoClass_MouseDown;
            }
            if (!UserInfoStatic.hasAuth("监测中心"))
            {
                HRVBtn.Click -= MainLogoHRV_MouseDown;
            }
            if (!UserInfoStatic.hasAuth("训练中心"))
            {
                TrainBtn.Click -= MainLogoTrain_MouseDown;
            }
            if (!UserInfoStatic.hasAuth("放松中心"))
            {
                ScaleBtn.Click -= MainLogoScale_MouseDown;
            }
            if (!UserInfoStatic.hasAuth("记录中心"))
            {
                RecordBtn.Click -= MainLogoRecord_MouseDown;
            }
        }

        //鼠标在button上
        void ui_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }
        /// <summary>
        /// 导入钩子
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(this.WndProc));
            }
        }
        private const int WM_USER = 0x0400;
        private const int WM_GAMEEXIT = WM_USER + 2000;
        private const int WM_GAMESTART = WM_USER + 2001;
        private const int WM_GAMESTOP = WM_USER + 2002;
        private const int WM_GAMEONLINE = WM_USER + 2003;
        private const int WM_GAMELEVEL = WM_USER + 2004;
        /// <summary>
        /// 检查传感器接入或是拔出
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_DEVICECHANGE:
                    Console.WriteLine("发现硬件更新");
                    switch (wParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            Console.WriteLine("1");
                            break;
                        case DBT_DEVICEARRIVAL:
                            Console.WriteLine("U盘插入");
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            Console.WriteLine("2");
                            break;
                        case DBT_CONFIGCHANGED:
                            Console.WriteLine("3");
                            break;
                        case DBT_CUSTOMEVENT:
                            Console.WriteLine("4");
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            Console.WriteLine("5");
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            Console.WriteLine("6");
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            Console.WriteLine("U盘拔出");
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            Console.WriteLine("7");
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            Console.WriteLine("8");
                            break;
                        case DBT_DEVNODES_CHANGED:
                            bool isClose = true;
                            HDCheck.HIDD_VIDPID[] HDList = HDCheck.AllVidPid;
                            foreach (HDCheck.HIDD_VIDPID hid in HDList)
                            {
                                if (hid.VendorID == 4292)
                                {
                                    isClose = false;
                                }
                                System.Diagnostics.Debug.Write("硬件id是：PID" + hid.ProductID + " | VID" + hid.VendorID + "\n");
                            }
                            if (isClose)
                            {
                                PmtsMessageBox.CustomControl1.Show("由于采集设备被意外拔除，软件即将退出。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                                this.Close();
                            }
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            Console.WriteLine("10");
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                    break;
                case WM_GAMESTART:
                    //                       PmtsMessageBox.CustomControl1.Show("收到开始信息。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    //                   tc.tl.startGame();
                    tc.startGame();
                    break;
                case WM_GAMESTOP:
                    //                  ControlTrain.OnGameStop(m.WParam, m.LParam);
                    //                                    PmtsMessageBox.CustomControl1.Show("收到停止信息。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    //                tc.tl.stopGame();
                    tc.stopGame();
                    break;
                case WM_GAMEONLINE:
                    //                   ControlTrain.OnGameOnline(m.WParam, m.LParam);
                    //                    PmtsMessageBox.CustomControl1.Show("收到开始信息。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    break;
                case WM_GAMELEVEL:
                    //                   ControlTrain.OnGameLevel(m.WParam, m.LParam);
                    //PmtsMessageBox.CustomControl1.Show("收到难度信息。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    break;
                case WM_GAMEEXIT:
                    //                   ControlTrain.OnGameExit(m.WParam, m.LParam);
                    //                   PmtsMessageBox.CustomControl1.Show("收到退出信息。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    //                   tc.tl.exitGame();
                    tc.exitGame();
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        #region Socket处理部分

        #endregion


        private void OnIconShow()
        {
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "党校心理能力训练系统网络版";
            this.notifyIcon.Text = "党校心理能力训练系统网络版";
            this.notifyIcon.Icon = new System.Drawing.Icon("MiniLogo.ico");
            this.notifyIcon.Visible = true;
            //notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            //lich
            //this.notifyIcon.ShowBalloonTip(500);

        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GCTimer.Stop();
            notifyIcon.Dispose();
            this.Close();
        }
        /// <summary>
        /// 主按钮载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainLogos_Loaded(object sender, RoutedEventArgs e)
        {
            this.MainHelpBut.Tag = 0;
            this.MainWindow.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.MainWindow.Height = Screen.PrimaryScreen.WorkingArea.Height - 70;
            mr = new MainRightPerson(SystemMeg);
            //lich
            //mr.Margin = new Thickness(0, 180, 10, 0);
            mr.Margin = new Thickness(0, 180, 24, 0);
            mr.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            mr.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            mr.Loaded += MainRight_Loaded;
            this.MainWindowParent.Children.Add(mr);
        }
        /// <summary>
        /// 点击学习课程按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainLogoClass_MouseDown(object sender, RoutedEventArgs e)
        {
            this.MainHelpBut.Tag = 1;
            cpv = new ClassPlayerView(this.MainWindow);
            cpv.Margin = new Thickness(80, 55, 80, 12);
            cpv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            cpv.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.MainWindow.Children.Add(cpv);
            this.mainLogos.Visibility = System.Windows.Visibility.Hidden;
            this.mr.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// 点击训练按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainLogoTrain_MouseDown(object sender, RoutedEventArgs e)
        {
            this.MainHelpBut.Tag = 2;
            tc = new TrainCenter(this.MainWindow);
            tc.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tc.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tc.Margin = new Thickness(80, 0, 80, 0);
            //tb.Unloaded += OnTrainUnLoaded;
            this.MainWindow.Children.Add(tc);
            //载入hrv右侧面板结束
            //隐藏4个主要按钮和右侧的信息栏
            this.mainLogos.Visibility = System.Windows.Visibility.Hidden;
            this.mr.Visibility = System.Windows.Visibility.Hidden;

        }
        /// <summary>
        /// 训练控件退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTrainUnLoaded(Object sender, RoutedEventArgs e)
        {
            this.MainHelpBut.Tag = 0;
            this.mainLogos.Visibility = System.Windows.Visibility.Visible;
            this.mr.Visibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// 进入HRV检测界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MainLogoHRV_MouseDown(object sender, RoutedEventArgs e)
        {
            this.MainHelpBut.Tag = 3;
            //载入曲线部分
            hv = new HRVView(this.MainWindow, SystemMeg, this.MainHelpBut);
            hv.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            hv.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //lich
            //            hv.Margin = new Thickness(80, 0, 0, 0);
            hv.Margin = new Thickness(50, 0, 0, 0);
            //hv.StartButton.Click += OnStartOrStopHrv;//暂时测试用按钮。
            this.MainWindow.Children.Add(hv);
            hv.HRVViewStart();
            //载入曲线部分结束

            //载入hrv右侧面板结束
            //隐藏4个主要按钮和右侧的信息栏
            //0904
            MainImage.ImageSource = new BitmapImage(new Uri("./UI/back-hrv.png", UriKind.Relative));
            this.mainLogos.Visibility = System.Windows.Visibility.Hidden;
            this.mr.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// 进入放松中心界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainLogoScale_MouseDown(object sender, RoutedEventArgs e)
        {

            this.MainHelpBut.Tag = 4;
            sp = new RecordPlayerView(this.MainWindow);
            sp.Margin = new Thickness(80, 55, 80, 12);
            sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.MainWindow.Children.Add(sp);
            this.mainLogos.Visibility = System.Windows.Visibility.Hidden;
            this.mr.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// 进入记录中心界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainLogoRecord_MouseDown(object sender, RoutedEventArgs e)
        {
            this.MainHelpBut.Tag = 7;
            record = new RecordCenter(this.MainWindow);
            record.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            record.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            record.Margin = new Thickness(80, 0, 0, 0);
            record.Tag = 0;
            this.MainWindow.Children.Add(record);
            this.mainLogos.Visibility = System.Windows.Visibility.Hidden;
            this.mr.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// 关闭HRV详情窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseHrvD(object sender, MouseButtonEventArgs e)
        {
            //this.MainWindow.Children.Remove(hrvd);
            //hrvd = null;
            //this.MainWindow.Children.Add(hv);
        }

        ///// <summary>
        ///// 呼吸助手的点击事件
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //    breath = new Breathing(this.LayoutRoot, this.BreathButton);
        //    breath.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
        //    breath.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
        //    breath.Margin = new Thickness(0, 0, 10, 10);
        //    this.LayoutRoot.Children.Add(breath);
        //    this.BreathButton.IsEnabled = false;
        //}

        /// <summary>
        /// 返回主界面，清空MainWindow中的控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetrunButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainHelpBut.Tag = 0;
            this.MainWindow.Children.Clear();
            //tc = null;//训练窗口
            //cpv = null;//课程窗口
            //hv = null;//HRV曲线窗口
            //0904
            MainImage.ImageSource = new BitmapImage(new Uri("./UI/back.jpg", UriKind.Relative));
            this.mainLogos.Visibility = System.Windows.Visibility.Visible;
            this.mr.Visibility = System.Windows.Visibility.Visible;
            mr.OnRefreshMedal();

            Grid main = (Grid)this.MainWindow.Parent;
            main.Margin = new Thickness(0, -20, 0, 0);
            main.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            main.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

        }

        private void OnDispose(object source, EventArgs e)
        {
            GC.Collect();
            GC.SuppressFinalize(this);
            GC.Collect();
        }
        /// <summary>
        /// 右侧面板载入时加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainRight_Loaded(object sender, RoutedEventArgs e)
        {
            MainRightPerson tmp = (MainRightPerson)sender;
            tmp.InfoButton.Click += OnOpenUserInfo;
            tmp.MoodButton.Click += OnMoodWindow;
        }
        /// <summary>
        /// 打开工作心理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoodWindow(object sender, RoutedEventArgs e)
        {
            if (hostIP == null)
            {
                hostIP = "127.0.0.1";
            }
            WorkingMood mood = new WorkingMood(this, SystemMeg["UserID"].ToString(), hostIP);
            mood.ShowDialog();
        }
        public void MoodReturn(double MoodValue)
        {
            SystemMeg["Mood"] = MoodValue;
            System.Diagnostics.Debug.Write("工作心理返回值：" + MoodValue + "\n");
        }
        /// <summary>
        /// 打开用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenUserInfo(object sender, RoutedEventArgs e)
        {
            if (!this.MainWindow.Children.Contains(info))
            {
                mark = new Rectangle();
                mark.Fill = Brushes.Transparent;
                mark.Margin = new Thickness(0, 60, 0, 0);
                mark.Tag = "mark";
                this.MainWindow.Children.Add(mark);
                info = new PersonalInformation(this.MainWindow);
                info.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                info.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                info.Margin = new Thickness(0, 200, 280, 0);
                info.UpDate();
                System.Windows.Controls.Panel.SetZIndex(info, 100);
                this.MainWindow.Children.Add(info);
                info.Unloaded += OnClosePersonalInfo;
            }
        }
        /// <summary>
        /// 关闭用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosePersonalInfo(Object sender, EventArgs e)
        {
            mr.OnRefreshInfo();
        }
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void OnSmallWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        /// <summary>
        /// 打开帮助文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenHelp(object sender, RoutedEventArgs e)
        {
            /*
             * 0：主页面
             * 1：课程界面
             * 2：训练界面
             * 3：HRV界面
             * 4：量表界面
             * 5：HRV记录列表界面
             * 6：HRV常量记录界面
             */
            System.Windows.Controls.Button tmp = sender as System.Windows.Controls.Button;
            Grid helpGrid = new Grid();
            helpGrid.Margin = new Thickness(0);
            Rectangle mark = new Rectangle();
            mark.Fill = Brushes.Transparent;
            mark.Margin = new Thickness(0);
            helpGrid.Children.Add(mark);

            if (tmp.Tag.ToString() == "0")
            {
                MainHelp mh = new MainHelp();
                mh.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                mh.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                helpGrid.Children.Add(mh);
            }
            else if (tmp.Tag.ToString() == "1")
            {
                ClassHelp ch = new ClassHelp();
                ch.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                ch.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                helpGrid.Children.Add(ch);
            }
            else if (tmp.Tag.ToString() == "2")
            {
                TrainHelp th = new TrainHelp();
                th.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                th.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                helpGrid.Children.Add(th);
            }
            else if (tmp.Tag.ToString() == "3")
            {
                HRVHelp hh = new HRVHelp();
                hh.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                hh.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                helpGrid.Children.Add(hh);
            }
            else if (tmp.Tag.ToString() == "4")
            {
                ScaleTableHelp sth = new ScaleTableHelp();
                sth.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                sth.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                helpGrid.Children.Add(sth);
            }
            else if (tmp.Tag.ToString() == "5")
            {
                HRVHistoryHelp hhh = new HRVHistoryHelp();
                hhh.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                hhh.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                helpGrid.Children.Add(hhh);
            }
            else if (tmp.Tag.ToString() == "6")
            {
                HRVConHelp hch = new HRVConHelp();
                hch.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                hch.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                helpGrid.Children.Add(hch);
            }

            this.MainWindow.Children.Add(helpGrid);
        }
        /// <summary>
        /// 退出软件时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExitWindows(object sender, EventArgs e)
        {
            DB_Login dl = new DB_Login(UserInfoStatic.ipAdd);
            dl.OnUserLogout(UserInfoStatic.UserInfo.id);
        }

        private void MainLogoClass_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }
    }
}
