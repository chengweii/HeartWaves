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
using System.Windows.Threading;

namespace PmtsControlLibrary
{
    /// <summary>
    /// MainRight.xaml 的交互逻辑
    /// </summary>
    public partial class MainRight : UserControl
    {
        private Hashtable uInfo = new Hashtable();
        private Hashtable medal = null;

        private DispatcherTimer NoticeTimer;//读取HRV时的Timer
        private String strNotice = "";

        public MainRight()
        {
            InitializeComponent();
        }
        public MainRight(Hashtable sysMeg)
        {
            InitializeComponent();
            GetUserInfo udb = new GetUserInfo(sysMeg);
            uInfo = udb.GetUserInfoByUID();
            medal = udb.GetUserMedal();
            UserInfoStatic.UserName = uInfo["name"].ToString();
            if (Convert.ToInt32(uInfo["sex"]) != 1)
            {
                UserInfoStatic.UserSex = "女";
            }
            UserInfoStatic.UserAge = uInfo["age"].ToString();
            UserInfoStatic.UserWorkYear = Convert.ToInt32(uInfo["wYear"]);
            UserInfoStatic.UserWorkType = uInfo["pType"].ToString();
            UserInfoStatic.UserWorkArea = uInfo["wArea"].ToString();
            UserInfoStatic.UserMR = uInfo["mr"].ToString();
  
        }
        public void OnRefreshInfo()
        {
            GetUserInfo udb = new GetUserInfo();
            uInfo = udb.GetUserInfoByUID();
            medal = udb.GetUserMedal();
            if (uInfo != null)
            {
                this.UserNameText.Text = uInfo["name"].ToString();
                this.PoliceTypeText.Text = uInfo["pType"].ToString();
                if (!String.IsNullOrEmpty(uInfo["age"].ToString()))
                {
                    TimeSpan nowTick = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan birTick = new TimeSpan(Convert.ToDateTime(uInfo["age"].ToString()).Ticks);
                    TimeSpan diffTick = nowTick.Subtract(birTick).Duration();
                    this.AgeText.Text = Math.Floor((diffTick.TotalDays / 365)).ToString();
                }
                if (uInfo["sex"].ToString() == "1")
                {
                    this.SexText.Text = "男";
                }
                else
                {
                    this.SexText.Text = "女";
                }
                this.AreaText.Text = uInfo["area"].ToString();
                UserInfoStatic.O = Convert.ToDouble(uInfo["O"]);
                UserInfoStatic.R = Convert.ToDouble(uInfo["R"]);
                UserInfoStatic.T = Convert.ToDouble(uInfo["T"]);
                UserInfoStatic.E = Convert.ToDouble(uInfo["E"]);
                UserInfoStatic.W = Convert.ToDouble(uInfo["W"]);
                UserInfoStatic.HRVS = Convert.ToDouble(uInfo["HRVS"]);
            }
            if (medal != null)
            {
                if (Convert.ToInt32(medal["ALLC"]) == 1)
                {
                    this.CourseAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.CourseAllShow.Visibility = System.Windows.Visibility.Visible;
                }
                if (Convert.ToInt32(medal["ALLT"]) == 1)
                {
                    this.TrainAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.TrainAllShow.Visibility = System.Windows.Visibility.Visible;
                }
                if (Convert.ToInt32(medal["ALLE"]) == 1)
                {
                    this.EPAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.EPAllShow.Visibility = System.Windows.Visibility.Visible;
                }
            }
            this.SystemNoticeText.Text = udb.GetNotice();
        }
        public void OnRefreshMedal()
        {
            GetUserInfo udb = new GetUserInfo();
            medal = udb.GetUserMedal();
            if (medal != null)
            {
                if (Convert.ToInt32(medal["ALLC"]) == 1)
                {
                    this.CourseAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.CourseAllShow.Visibility = System.Windows.Visibility.Visible;
                }
                if (Convert.ToInt32(medal["ALLT"]) == 1)
                {
                    this.TrainAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.TrainAllShow.Visibility = System.Windows.Visibility.Visible;
                }
                if (Convert.ToInt32(medal["ALLE"]) == 1)
                {
                    this.EPAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.EPAllShow.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public Button InfoButton
        {
            get { return this.UserInfoButton; }
        }
        public Button MoodButton
        {
            get { return this.WorkMoodButton; }
        }
        /// <summary>
        /// 空间被载入后发生事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (uInfo != null)
            {
                this.UserNameText.Text = uInfo["name"].ToString();
                this.PoliceTypeText.Text = uInfo["pType"].ToString();
                if (!String.IsNullOrEmpty(uInfo["age"].ToString()))
                {
                    TimeSpan nowTick = new TimeSpan(DateTime.Now.Ticks);
                    TimeSpan birTick = new TimeSpan(Convert.ToDateTime(uInfo["age"].ToString()).Ticks);
                    TimeSpan diffTick = nowTick.Subtract(birTick).Duration();
                    this.AgeText.Text = Math.Floor((diffTick.TotalDays / 365)).ToString();
                }
                if (uInfo["sex"].ToString() == "1")
                {
                    this.SexText.Text = "男";
                }
                else
                {
                    this.SexText.Text = "女";
                }
                this.AreaText.Text = uInfo["area"].ToString();
                UserInfoStatic.O = Convert.ToDouble(uInfo["O"]);
                UserInfoStatic.R = Convert.ToDouble(uInfo["R"]);
                UserInfoStatic.T = Convert.ToDouble(uInfo["T"]);
                UserInfoStatic.E = Convert.ToDouble(uInfo["E"]);
                UserInfoStatic.W = Convert.ToDouble(uInfo["W"]);
                UserInfoStatic.HRVS = Convert.ToDouble(uInfo["HRVS"]);
            }
            if (medal != null)
            {
                if (Convert.ToInt32(medal["ALLC"]) == 1)
                {
                    this.CourseAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.CourseAllShow.Visibility = System.Windows.Visibility.Visible;
                }
                if (Convert.ToInt32(medal["ALLT"]) == 1)
                {
                    this.TrainAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.TrainAllShow.Visibility = System.Windows.Visibility.Visible;
                }
                if (Convert.ToInt32(medal["ALLE"]) == 1)
                {
                    this.EPAllMark.Visibility = System.Windows.Visibility.Hidden;
                    this.EPAllShow.Visibility = System.Windows.Visibility.Visible;
                }
            }
            GetUserInfo udb = new GetUserInfo();
            this.SystemNoticeText.Text = udb.GetNotice();
            strNotice = this.SystemNoticeText.Text;

            NoticeTimer = new DispatcherTimer();
            NoticeTimer.Tick += new EventHandler(OnTimerNotice);
            NoticeTimer.Interval = new TimeSpan(0, 0, 0, 10, 0);
            NoticeTimer.Start();

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (NoticeTimer.IsEnabled == true)
            {
                NoticeTimer.Stop();
            }
        }

        private void OnTimerNotice(object source, EventArgs e)
        {
            GetUserInfo udb = new GetUserInfo();
            this.SystemNoticeText.Text = udb.GetNotice();
            if (strNotice != this.SystemNoticeText.Text)
            {
                strNotice = this.SystemNoticeText.Text;
                PmtsMessageBox.CustomControl1.Show("有新的系统公告，请在右侧面板查看!", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }
        }
    }
}
