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
using System.Collections;
using pmts_net.AppWindow;
using PmtsControlLibrary.WEBPlugin;
using HeartWavesSDK.Model;

namespace PmtsControlLibrary
{
    /// <summary>
    /// WorkingMood.xaml 的交互逻辑
    /// </summary>
    public partial class WorkingMood : Window
    {
        private pmts_net.AppWindow.Main Main;
        private String user = "";
        private WorkMoodWEB MoodDB = null;
        private String[] HighText = new String[31];
        private Boolean isInput = false;
        private String hostIP = "";
        private Double moodValue = 0;

        public WorkingMood(pmts_net.AppWindow.Main MainWindow, String UserID, String HostIp)
        {
            InitializeComponent();
            Main = MainWindow;
            user = UserID;
            hostIP = HostIp;
            if (MoodDB == null)
            {
                MoodDB = new WorkMoodWEB(user, hostIP);
            }
            ArrayList dates = MoodDB.OnGetMoodDate(DateTime.Now);
            SetCalendarHighLight(DateTime.Now, dates);
        }
        /// <summary>
        /// 关闭窗口按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 清空输入框内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.MoodText.Clear();
        }
        /// <summary>
        /// 窗口关闭时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (isInput)
            {
                Main.MoodReturn(moodValue);
            }
        }
        /// <summary>
        /// 提交工作心理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (MoodDB.OnInsertMood(this.MoodValueSlider.Value, this.MoodText.Text))
            {
                UserInfoStatic.UserInfo.mood = new Mood { moodsocre = Convert.ToString(this.MoodValueSlider.Value), moodmark = this.MoodText.Text, moodtime = DateTime.Now.ToString() };
                moodValue = this.MoodValueSlider.Value;
                isInput = true;
                PmtsMessageBox.CustomControl1.Show("提交成功。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                this.MoodText.Clear();
                this.MoodValueSlider.Value = 50;
                Main.MoodReturn(moodValue);
            }
        }

        /// <summary>
        ///  载入日历
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fsCalendar1_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Write("进入日历面板\n");
            SetCalendarDispaly(DateTime.Now);

        }
        /// <summary>
        /// 设置日期高亮
        /// </summary>
        /// <param name="newDate"></param>
        /// <param name="dates"></param>
        private void SetCalendarHighLight(DateTime newDate, ArrayList dates)
        {
            HighText = new String[31];
            int lastDay = DateTime.DaysInMonth(newDate.Year, newDate.Month);
            if (dates.Count > 0)
            {

                int index = 0;
                for (int i = 0; i < 31; i++)
                {
                    HighText[i] = null;
                    if (i > lastDay) continue;
                    if (index < dates.Count)
                    {
                        if (i == ((DateTime)dates[index]).Day - 1)
                        {
                            HighText[i] = dates[index].ToString();
                            if (index < dates.Count - 1)
                            {
                                index += 1;
                            }
                        }
                    }
                }
            }
            this.fsCalendar1.HighlightedDateText = HighText;
            this.fsCalendar1.ShowDateHighlighting = true;
            this.fsCalendar1.Refresh();
        }
        /// <summary>
        /// 下个月按钮重写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextMonthButtonClick(object sender, RoutedEventArgs e)
        {
            DateTime nowDate = (DateTime)this.fsCalendar1.DisplayDate;
            int newMonth = nowDate.Month + 1;
            int newYear = nowDate.Year;
            if (newMonth > 12)
            {
                newMonth = 1;
                newYear += 1;
            }
            DateTime newDate = new DateTime(newYear, newMonth, 1);

            ArrayList dates = MoodDB.OnGetMoodDate(newDate);
            SetCalendarHighLight(newDate, dates);
            SetCalendarDispaly(newDate);
            this.fsCalendar1.DisplayDate = newDate;
        }
        /// <summary>
        /// 上个月按钮重写
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPrevMonthButtonClick(object sender, RoutedEventArgs e)
        {
            DateTime nowDate = (DateTime)this.fsCalendar1.DisplayDate;
            int newMonth = nowDate.Month - 1;
            int newYear = nowDate.Year;
            if (newMonth < 1)
            {
                newMonth = 12;
                newYear -= 1;
            }
            DateTime newDate = new DateTime(newYear, newMonth, 1);

            ArrayList dates = MoodDB.OnGetMoodDate(newDate);
            SetCalendarHighLight(newDate, dates);
            SetCalendarDispaly(newDate);
            this.fsCalendar1.DisplayDate = newDate;
        }
        /// <summary>
        /// 设置日历显示范围，只允许显示当月的数据。
        /// </summary>
        /// <param name="nowDate"></param>
        private void SetCalendarDispaly(DateTime nowDate)
        {
            int lastDay = DateTime.DaysInMonth(nowDate.Year, nowDate.Month);
            this.fsCalendar1.DisplayDateStart = new DateTime(nowDate.Year, nowDate.Month, 1);
            this.fsCalendar1.DisplayDateEnd = new DateTime(nowDate.Year, nowDate.Month, lastDay);
        }
        /// <summary>
        /// 点击具体日期查看当天详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fsCalendar1_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.fsCalendar1.SelectedDate != null)
            {
                System.Diagnostics.Debug.Write("选择的日期" + this.fsCalendar1.SelectedDate + "\n");
                this.InputPage.Visibility = System.Windows.Visibility.Hidden;
                this.DetailePage.Visibility = System.Windows.Visibility.Visible;
                DateTime selectDate = (DateTime)this.fsCalendar1.SelectedDate;
                List<MoodDataList> moodList = MoodDB.OnGetMoodDetail(selectDate);

                this.ShowMoodDetaile.DataContext = moodList;
                this.ShowMoodDetaile.Items.Refresh();

            }
            this.fsCalendar1.SelectedDate = null;
        }
        /// <summary>
        /// 返回工作心理输入框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.InputPage.Visibility = System.Windows.Visibility.Visible;
            this.DetailePage.Visibility = System.Windows.Visibility.Hidden;
            
        }


        //--------------------------------------------------------------工作心情按钮----------------------------------------------------
        private void OnSetMoodTo0(Object sender, RoutedEventArgs e)
        {
            this.MoodValueSlider.Value = 5;
        }
        private void OnSetMoodTo20(Object sender, RoutedEventArgs e)
        {
            this.MoodValueSlider.Value = 15;
        }
        private void OnSetMoodTo40(Object sender, RoutedEventArgs e)
        {
            this.MoodValueSlider.Value = 32;
        }
        private void OnSetMoodTo50(Object sender, RoutedEventArgs e)
        {
            this.MoodValueSlider.Value = 50;
        }
        private void OnSetMoodTo60(Object sender, RoutedEventArgs e)
        {
            this.MoodValueSlider.Value = 67;
        }
        private void OnSetMoodTo80(Object sender, RoutedEventArgs e)
        {
            this.MoodValueSlider.Value = 85;
        }
        private void OnSetMoodTo100(Object sender, RoutedEventArgs e)
        {
            this.MoodValueSlider.Value = 95;
        }
    }
}
