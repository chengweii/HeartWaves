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
using System.Windows.Shapes;

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainHistory.xaml 的交互逻辑
    /// </summary>
    public partial class TrainHistory : Window
    {
        ArrayList historyList = new ArrayList();
        private PmtsControlLibrary.DBPlugin.TrainDB tdb = null;
        private int thTID = 0;

        public TrainHistory()
        {
            InitializeComponent();
        }
        public TrainHistory(int tid)
        {
            InitializeComponent();
            thTID = tid;
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
        /// 载入历史记录列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadHistoryList(object sender, RoutedEventArgs e)
        {
            this.OnRefreshList();
        }

        private void OnRefreshList()
        {
            tdb = new DBPlugin.TrainDB();
            historyList = tdb.GetHistoryByTID(thTID);
            if (historyList.Count > 0)
            {
                this.HistoryListGrid.RowDefinitions.Clear();
                this.HistoryListGrid.Children.Clear();
                /*画垂直的2条线*/
                Rectangle rVerticalOne = new Rectangle();
                rVerticalOne.Width = 1;
                Grid.SetColumn(rVerticalOne, 1);
                Grid.SetRowSpan(rVerticalOne, (historyList.Count * 2) - 1);
                this.HistoryListGrid.Children.Add(rVerticalOne);

                Rectangle rVerticalTwo = new Rectangle();
                rVerticalTwo.Width = 1;
                Grid.SetColumn(rVerticalTwo, 3);
                Grid.SetRowSpan(rVerticalTwo, (historyList.Count * 2) - 1);
                this.HistoryListGrid.Children.Add(rVerticalTwo);

                for (int i = 0; i < historyList.Count; i++)
                {
                    Hashtable info = (Hashtable)historyList[i];
                    RowDefinition rd = new RowDefinition();
                    rd.Height = new GridLength(30);
                    this.HistoryListGrid.RowDefinitions.Add(rd);

                    ///添加日期
                    TextBlock tbTime = new TextBlock();
                    tbTime.Text = info["time"].ToString();
                    Grid.SetColumn(tbTime, 0);
                    Grid.SetRow(tbTime, i * 2);
                    this.HistoryListGrid.Children.Add(tbTime);
                    ///添加查看详细信息按钮
                    Button detailBut = new Button();
                    detailBut.Content = "查看详细";
                    detailBut.Tag = info;
                    detailBut.Click += OnShowDetailPage;
                    Grid.SetRow(detailBut, i * 2);
                    Grid.SetColumn(detailBut, 2);
                    this.HistoryListGrid.Children.Add(detailBut);
                    ///添加删除按钮
                    Button delBut = new Button();
                    delBut.Content = "删除";
                    delBut.Tag = info;
                    delBut.Click += OnDelOne;
                    Grid.SetRow(delBut, i * 2);
                    Grid.SetColumn(delBut, 4);
                    this.HistoryListGrid.Children.Add(delBut);

                    if (i < historyList.Count - 1)
                    {
                        RowDefinition rdLine = new RowDefinition();
                        rdLine.Height = new GridLength(1);
                        this.HistoryListGrid.RowDefinitions.Add(rdLine);
                        Rectangle rRow = new Rectangle();
                        rRow.Height = 1;
                        Grid.SetRow(rRow, (i * 2) + 1);
                        Grid.SetColumnSpan(rRow, 5);
                        this.HistoryListGrid.Children.Add(rRow);
                    }
                }
            }
            else
            {
                this.Close();
            }
        }
        /// <summary>
        /// 详情按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowDetailPage(object sender, RoutedEventArgs e)
        {
            Button tmp = (Button)sender;
            Hashtable info = (Hashtable)tmp.Tag;
            this.ListScrollView.Visibility = System.Windows.Visibility.Hidden;
            this.HistoryDetailGrid.Visibility = System.Windows.Visibility.Visible;
            this.userID.Text = UserInfoStatic.UserID;
            this.userName.Text = UserInfoStatic.UserName;
            this.nameText.Text = info["tname"].ToString();
            this.timeText.Text = info["time"].ToString();
            this.scoreText.Text = info["s"].ToString();
            this.nowGateText.Text = info["nowGate"].ToString();
            this.totalGateText.Text = info["totalGate"].ToString();
            this.diffText.Text = info["diff"].ToString();
            //开始计算柱形图增长
            ArrayList ts = (ArrayList)info["totals"];
            double big = Convert.ToDouble(ts[0]);
            for (int i = 1; i < ts.Count; i++)
            {
                if (Convert.ToDouble(ts[i]) > big)
                {
                    big = Convert.ToDouble(ts[i]);
                }
            }
            
            double oneHeigh = 150 / big;
            if (big <= 0)
            {
                oneHeigh = 0;
            }
            //柱形变化
            this.oberveTo.To = Convert.ToDouble(info["to"])*oneHeigh;
            this.thinkTo.To = Convert.ToDouble(info["tt"]) * oneHeigh; ;
            this.emotionsTo.To = Convert.ToDouble(info["te"]) * oneHeigh; ;
            this.willpowerTo.To = Convert.ToDouble(info["tw"]) * oneHeigh; ;
            this.remberTo.To = Convert.ToDouble(info["tr"]) * oneHeigh; ;
            //文字位置变化
            this.oberverTextTo.To = new Thickness(40, 0, 0, 38 + Convert.ToDouble(info["to"])*oneHeigh);
            this.thinkTextTo.To = new Thickness(275, 0, 0, 38 + Convert.ToDouble(info["tt"])*oneHeigh);
            this.emotionsTextTo.To = new Thickness(212, 0, 0, 38 + Convert.ToDouble(info["te"])*oneHeigh);
            this.willpowerTextTo.To = new Thickness(152, 0, 0, 38 + Convert.ToDouble(info["tw"])*oneHeigh);
            this.remberTextTo.To = new Thickness(92, 0, 0, 38 + Convert.ToDouble(info["tr"])*oneHeigh);
            //文字变化
            this.oberveText.Text = "+"+Math.Ceiling(Convert.ToDouble(info["o"])).ToString();
            this.thinkText.Text = "+" + Math.Ceiling(Convert.ToDouble(info["t"])).ToString();
            this.emotionsText.Text = "+" + Math.Ceiling(Convert.ToDouble(info["e"])).ToString();
            this.willpowerText.Text = "+" + Math.Ceiling(Convert.ToDouble(info["w"])).ToString();
            this.remberText.Text = "+" + Math.Ceiling(Convert.ToDouble(info["r"])).ToString();
            //开始动画
            this.oberverTextStory.Begin();
            this.thinkTextStory.Begin();
            this.emotionsTextStory.Begin();
            this.willpowerTextStory.Begin();
            this.remberTextStory.Begin();
            this.observeStory.Begin();
            this.thinkStory.Begin();
            this.emotionsStory.Begin();
            this.willpowerStory.Begin();
            this.remberStory.Begin();
        }
        /// <summary>
        /// 删除按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDelOne(object sender, RoutedEventArgs e)
        {

            Button tmp = (Button)sender;
            Hashtable info = (Hashtable)tmp.Tag;
            tdb.OnDeleteRecordOne(Convert.ToInt32(info["tid"]), Convert.ToInt32(info["trid"]));
            OnRefreshList();
        }
        /// <summary>
        /// 关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 返回历史记录列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.ListScrollView.Visibility = System.Windows.Visibility.Visible;
            this.HistoryDetailGrid.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// 载入柱形图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImagesLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
