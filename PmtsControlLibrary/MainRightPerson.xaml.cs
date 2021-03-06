﻿using System;
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
using PmtsControlLibrary.WEBPlugin;
using PmtsControlLibrary.Common;


namespace PmtsControlLibrary
{
    /// <summary>
    /// MainRightPerson.xaml 的交互逻辑
    /// </summary>
    public partial class MainRightPerson : UserControl
    {
        private Hashtable uInfo = new Hashtable();
        private Hashtable medal = new Hashtable();

        private DispatcherTimer NoticeTimer;//读取HRV时的Timer
        private String strNotice = "";

        /////0904
        public static ArrayList TmpHrvRecord = new ArrayList();

        private GetUserInfoWEB udbWEB = new GetUserInfoWEB();

        public MainRightPerson()
        {
            InitializeComponent();
        }
        public MainRightPerson(Hashtable sysMeg)
        {
            //lich
            InitializeComponent();
            if (UserInfoStatic.ipAdd == null)
            {
                UserInfoStatic.UserSex = "－";
                UserInfoStatic.UserAge = "－";
                UserInfoStatic.UserWork = "未知";
            }
            //UserInfoStatic.UserWorkYear = Convert.ToInt32(uInfo["wYear"]);
            //UserInfoStatic.UserWorkType = uInfo["pType"].ToString();
            //UserInfoStatic.UserWorkArea = uInfo["wArea"].ToString();
            //UserInfoStatic.UserMR = uInfo["mr"].ToString();

        }

        public void SetEmojByMood()
        {
            string source;
            double mood = Convert.ToDouble(UserInfoStatic.UserInfo.mood.moodsocre);
            if (mood <= 5)
            {
                source = "../Image/Mood/8.png";
            }
            else if (mood <= 15)
            {
                source = "../Image/Mood/9.png";
            }
            else if (mood <= 32)
            {
                source = "../Image/Mood/10.png";
            }
            else if (mood <= 50)
            {
                source = "../Image/Mood/11.png";
            }
            else if (mood <= 67)
            {
                source = "../Image/Mood/12.png";
            }
            else if (mood <= 85)
            {
                source = "../Image/Mood/13.png";
            }
            else
            {
                source = "../Image/Mood/14.png";
            }

            usermood.Source = new BitmapImage(new Uri(source, UriKind.Relative));
        }

        private const string WELCOME_MSG = "您好，";

        public void OnRefreshInfo()
        {
            medal = udbWEB.GetUserMedal();
            updateUserInfo();

            /*
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
            }**/

            this.SystemNoticeText.Text = udbWEB.GetNotice();
        }


        public void OnRefreshMedal()
        {
            medal = udbWEB.GetUserMedal();
            /*
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
            }**/
        }
        public Button InfoButton
        {
            get { return this.UserInfoButton; }
        }
        public Button MoodButton
        {
            get { return this.WorkMoodButton; }
        }

        public void updateUserInfo()
        {
            uInfo = udbWEB.GetUserInfoByUID();
            this.UserNameText.Text = WELCOME_MSG + uInfo["name"].ToString();
            this.SexText.Text = uInfo["sex"].ToString();
            this.AgeText.Text = uInfo["age"].ToString();
            UserInfoStatic.O = Convert.ToDouble(uInfo["O"]);
            UserInfoStatic.R = Convert.ToDouble(uInfo["R"]);
            UserInfoStatic.T = Convert.ToDouble(uInfo["T"]);
            UserInfoStatic.E = Convert.ToDouble(uInfo["E"]);
            UserInfoStatic.W = Convert.ToDouble(uInfo["W"]);
            UserInfoStatic.HRVS = Convert.ToDouble(uInfo["HRVS"]);
        }
        /// <summary>
        /// 空间被载入后发生事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            updateUserInfo();
            /*
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
            }*/
            //lich
            if (UserInfoStatic.ipAdd != null) //非游客
            {
                this.SystemNoticeText.Text = udbWEB.GetNotice();
                strNotice = this.SystemNoticeText.Text;

                NoticeTimer = new DispatcherTimer();
                NoticeTimer.Tick += new EventHandler(OnTimerNotice);
                NoticeTimer.Interval = new TimeSpan(0, 0, 0, 10, 0);
                NoticeTimer.Start();
            }
            else
            {
                this.SystemNoticeText.Text = "欢迎使用党校心理调适训练系统，您正在以游客身份登录！";
                strNotice = this.SystemNoticeText.Text;
            }
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
            this.SystemNoticeText.Text = udbWEB.GetNotice();
            if (strNotice != this.SystemNoticeText.Text)
            {
                strNotice = this.SystemNoticeText.Text;
                PmtsMessageBox.CustomControl1.Show("有新的系统公告，请在右侧面板查看!", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }
        }

        private void WorkMoodButton_MouseMove(object sender, MouseEventArgs e)
        {

            WorkMoodButton.Background = new SolidColorBrush(Color.FromArgb(255, 24, 128, 239));

        }

        private void WorkMoodButton_MouseEnter(object sender, MouseEventArgs e)
        {
            WorkMoodButton.Background = new SolidColorBrush(Color.FromArgb(255, 24, 128, 239));
        }

        private void WorkMoodButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WorkMoodButton.Background = new SolidColorBrush(Color.FromArgb(255, 24, 128, 239));
        }

        private void WorkMoodButton_MouseLeave(object sender, MouseEventArgs e)
        {
            WorkMoodButton.Background = new SolidColorBrush(Color.FromArgb(255, 24, 128, 239));
        }


        private void button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
        }
    }
}
