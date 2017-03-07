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
using System.Windows.Markup;

namespace PmtsControlLibrary
{
    /// <summary>
    /// HRVDetaile.xaml 的交互逻辑
    /// </summary>
    public partial class HRVDetaile : UserControl
    {
        private Grid main = new Grid();
        private HRVPoint hrvp = null;
        private ArrayList markArr = new ArrayList();
        private Rectangle mask = null;
        private HRVMarkList ml = null;//事件标记弹出框
        private DateTime start = new DateTime();

        public HRVDetaile()
        {
            InitializeComponent();

        }

        public HRVDetaile(ArrayList HrvData, Hashtable HrvCalc,Grid MainGrid)
        {
            InitializeComponent();
            ChartDraw(HrvData);//绘制整体HRV曲线
            this.Pressure.Content = Math.Floor(Convert.ToDouble(HrvCalc["Pressure"]));//压力指数////
            this.ReportText.Text = HrvCalc["report"].ToString();//评价报告///
            this.Adjust.Content = Math.Floor(Convert.ToDouble(HrvCalc["adjust"]));//调节指数///
            this.Stable.Content = Math.Floor(Convert.ToDouble(HrvCalc["stable"]));//稳定指数///
            this.TotalScore.Content = Math.Floor(Convert.ToDouble(HrvCalc["score"]));//综合得分///
            this.HRVScore.Content = HrvCalc["HRVScore"];//HRV得分///
            this.HeartRate.Content = Math.Floor(Convert.ToDouble(HrvCalc["fMean"]));//平均心率///
            this.UseID.Content = UserInfoStatic.UserInfo.id;//用户ID
            this.UseName.Content = UserInfoStatic.UserInfo.username;//用户姓名
            MathAngle(Convert.ToDouble(HrvCalc["NB"]));//偏转神经兴奋性的指针///
            MathTime(Convert.ToInt32(Math.Floor(Convert.ToDouble(HrvCalc["Time"]))));//计算测量时间
            markArr = HrvCalc["HRVMark"] as ArrayList;//时间标记列表
            start = Convert.ToDateTime(HrvCalc["StartTime"]);
            main = MainGrid;
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
        private void ChartDraw(ArrayList data)
        {
            double oneWidth = this.HrvChart.Width / data.Count;//每个点的宽度
            double oneHeight = this.HrvChart.Height / 100;//每个点的高度
            PathGeometry pg = new PathGeometry();//path几何图形的集合
            PathFigure pf = new PathFigure();//具体图形
            double frist = 0;//计算第一个点Y轴的位置
            if (data.Count==0 || Convert.ToDouble(data[0]) <= 60)
            {
                frist = this.HrvChart.Height - oneHeight;
            }
            else if (Convert.ToDouble(data[0]) >= 160)
            {
                frist = oneHeight;
            }
            else
            {
                frist = (160 - Convert.ToDouble(data[0])) * oneHeight;
            }
            pf.StartPoint = new Point(0, frist);
            for (int i = 1; i < data.Count; i++)
            {
                LineSegment lg = new LineSegment();
                double tmpY = 0;//
                if (Convert.ToDouble(data[i]) <= 60)
                {
                    tmpY = this.HrvChart.Height - oneHeight;
                }
                else if (Convert.ToDouble(data[i]) >= 160)
                {
                    tmpY = oneHeight;
                }
                else
                {
                    tmpY = (160 - Convert.ToDouble(data[i])) * oneHeight;
                }

                lg.Point = new Point(i * oneWidth, tmpY);
                pf.Segments.Add(lg);
            }
            pg.Figures.Add(pf);
            this.HrvChart.Data = pg;
        }
        private void MathAngle(double nb)
        {
            double to = 0;
            if (nb > 0 && nb < 1.15)
            {
                if (nb > 0 && nb <= 0.4)
                {
                    to = (90 - (nb * (45 / 0.4))) * -1;
                }
                else if (nb > 0.4 && nb <= 0.8)
                {
                    to = (90 - ((nb - 0.4) * (27 / 0.4) + 45)) * -1;
                }
                else if (nb > 0.8 && nb < 1.15)
                {
                    to = (90 - (((nb - 0.8) * (18 / 0.35)) + 72)) * -1;
                }
            }
            else
            {
                if (nb >= 1.15 && nb <= 1.5)
                {
                    to = ((nb - 1.15) * (18 / 0.35));
                }
                else if (nb > 1.5 && nb <= 5)
                {
                    to = 27 + ((nb - 1.5) * (27 / 3.5));
                }
                else if (nb > 5)
                {
                    to = 45 + ((nb - 5) * (45 / 10));

                    if (to >= 90)
                    {
                        to = 90;
                    }
                }
            }
            this.myAngle.Angle = to;
        }
        private void MathTime(int times)
        {
            int second = times % 60;//取得秒数
            int min = Convert.ToInt32(Math.Floor(times / 60.0f));//取得分钟
            this.HRVTime.Content = min + " 分" + second + " 秒";
        }

        /// <summary>
        /// 响应打印按钮点击事件，手动创建Grid，并赋值给打印预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hrvp = new HRVPoint(XamlWriter.Save(PointGrid),main);
            //hrvp.Margin = new Thickness(300, 100, 0, 0);
            hrvp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            hrvp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            main.Children.Add(hrvp);
        }
        /// <summary>
        /// 查看事件标记按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowMark(object sender, RoutedEventArgs e)
        {
            ml = new HRVMarkList(markArr,start);
            ml.closeButton.Click += OnCloseMark;
            mask= new Rectangle();
            mask.Fill = Brushes.Transparent;
            mask.Margin = new Thickness(0, 60, 0, 0);
            main.Children.Add(mask);
            main.Children.Add(ml);
        }
        /// <summary>
        /// 关闭事件标记弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseMark(object sender, RoutedEventArgs e)
        {
            main.Children.Remove(ml);
            main.Children.Remove(mask);
        }
        ///// <summary>
        ///// 关闭窗口
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    main.Children.Remove(this);
        //}
    }
}
