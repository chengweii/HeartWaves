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

namespace PmtsControlLibrary
{
    /// <summary>
    /// HRVMarkList.xaml 的交互逻辑
    /// </summary>
    public partial class HRVMarkList : UserControl
    {
        private ArrayList marks = new ArrayList();
        private DateTime start = new DateTime();

        public HRVMarkList()
        {
            InitializeComponent();
        }
        public HRVMarkList(ArrayList markList,DateTime startTime)
        {
            InitializeComponent();
            marks = markList;
            start = startTime;
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
        /// 初始化列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadedViewGrid(object sender, RoutedEventArgs e)
        {
            if (marks.Count > 0)
            {
                for (int i = 0; i < marks.Count; i++)
                {
                    Hashtable markInfo = marks[i] as Hashtable;

                    Grid g = new Grid();
                    g.Height = 25;
                    g.Width = 430;
                    g.Margin = new Thickness(0);

                    RowDefinition rd1 = new RowDefinition();
                    rd1.Height = new GridLength(24);
                    g.RowDefinitions.Add(rd1);
                    RowDefinition rd2 = new RowDefinition();
                    rd2.Height = new GridLength(1);
                    g.RowDefinitions.Add(rd2);

                    ColumnDefinition cd1 = new ColumnDefinition();
                    cd1.Width = new GridLength(100);
                    g.ColumnDefinitions.Add(cd1);
                    TextBlock timeTB = new TextBlock();
                    //timeTB.Text = MathTime(Convert.ToInt32(Math.Floor(Convert.ToDouble(markInfo["Time"])/2)));
                    TimeSpan diffTick = Convert.ToDateTime(markInfo["DateTime"]).Subtract(start).Duration();
                    timeTB.Text = diffTick.Minutes + "分" + diffTick.Seconds.ToString().PadLeft(2, '0') + "秒";
                    Grid.SetColumn(timeTB, 0);
                    Grid.SetRow(timeTB,0);
                    g.Children.Add(timeTB);

                    ColumnDefinition cd2 = new ColumnDefinition();
                    cd2.Width = new GridLength(1);
                    g.ColumnDefinitions.Add(cd2);
                    Rectangle r = new Rectangle();
                    r.Fill = Brushes.Gray;
                    r.Width = 1;
                    Grid.SetColumn(r, 1);
                    Grid.SetRow(r,0);
                    g.Children.Add(r);

                    ColumnDefinition cd3 = new ColumnDefinition();
                    cd3.Width = new GridLength(329);
                    g.ColumnDefinitions.Add(cd3);
                    TextBlock tb = new TextBlock();
                    tb.Text = markInfo["Content"].ToString();
                    Grid.SetRow(tb, 0);
                    Grid.SetColumn(tb, 2);
                    g.Children.Add(tb);

                    Rectangle br = new Rectangle();
                    br.Fill = Brushes.Gray;
                    br.Height = 1;
                    Grid.SetColumnSpan(br, 3);
                    Grid.SetRow(br, 1);
                    g.Children.Add(br);


                    this.listView.Children.Add(g);
                }
            }
        }
        private String MathTime(int times)
        {
            int second = times % 60;//取得秒数
            int min = Convert.ToInt32(Math.Floor(times / 60.0f));//取得分钟
            return  min + " 分" + second + " 秒";
        }
    }
}
