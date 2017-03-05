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
using PmtsControlLibrary.DBPlugin;

namespace PmtsControlLibrary
{
    /// <summary>
    /// HRVConstant.xaml(HRV常量列表) 的交互逻辑
    /// </summary>
    public partial class HRVConstant : UserControl
    {
        private Grid mainWindow = new Grid();
        private int timeType = 0;
        private int mood = 0;
        private ArrayList constArr = new ArrayList();//查询返回的数组
        private HRVControlDB hrvdb = null;//控制DB类
        private Hashtable dataMathed = new Hashtable();//计算后的数据

        public HRVConstant(Hashtable SystemMeg)
        {
            InitializeComponent();
            this.MoodSelect.SelectedIndex = 0;
            this.TimeSelect.SelectedIndex = 0;
            this.contSelect.SelectedIndex = 0;
            constArr = new ArrayList();
            if (hrvdb == null)
            {
                hrvdb = new HRVControlDB(SystemMeg);
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
        /// 初始化界面
        /// </summary>
        private void ControlInit()
        {
            this.mhrtText.Text = "";
            this.adjustText.Text = "";
            this.hrvsText.Text = "";
            this.stableText.Text = "";
            this.pressureText.Text = "";
            this.totalText.Text = "";
            this.contSelect.SelectedIndex = 0;
            this.ChartGrid.Children.Clear();
        }
        /// <summary>
        /// 更新按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetDataFromDB();
        }
        /// <summary>
        /// 从数据库取得数据，并计算,并且写入界面
        /// </summary>
        private void GetDataFromDB()
        {
            ControlInit();
            double mhrt = 0, hrvScore = 0, totalScore = 0, pressure = 0, stable = 0, adjust = 0;
            constArr = hrvdb.GetConstAndHistoryListData(timeType, mood);
            if (constArr.Count > 0)
            {
                double mhrtMax = 0, mhrtMin = Convert.ToSingle(((Hashtable)constArr[0])["mhrt"]);
                double hrvScoreMax = 0, hrvScoreMin = Convert.ToSingle(((Hashtable)constArr[0])["hrvScore"]);
                double totalScoreMax = 0, totalScoreMin = Convert.ToSingle(((Hashtable)constArr[0])["totalScore"]);
                double pressureMax = 0, pressureMin = Convert.ToSingle(((Hashtable)constArr[0])["pressure"]);
                double stableMax = 0, stableMin = Convert.ToSingle(((Hashtable)constArr[0])["stable"]);
                double adjustMax = 0, adjustMin = Convert.ToSingle(((Hashtable)constArr[0])["adjust"]);
                for (int i = 0; i < constArr.Count; i++)
                {
                    mhrt += Convert.ToSingle(((Hashtable)constArr[i])["mhrt"]);
                    mhrtMax = Math.Max(mhrtMax, Convert.ToSingle(((Hashtable)constArr[i])["mhrt"]));
                    mhrtMin = Math.Min(mhrtMin, Convert.ToSingle(((Hashtable)constArr[i])["mhrt"]));

                    hrvScore += Convert.ToSingle(((Hashtable)constArr[i])["hrvScore"]);
                    hrvScoreMax = Math.Max(hrvScoreMax, Convert.ToSingle(((Hashtable)constArr[i])["hrvScore"]));
                    hrvScoreMin = Math.Min(hrvScoreMin, Convert.ToSingle(((Hashtable)constArr[i])["hrvScore"]));

                    totalScore += Convert.ToSingle(((Hashtable)constArr[i])["totalScore"]);
                    totalScoreMax = Math.Max(totalScoreMax, Convert.ToSingle(((Hashtable)constArr[i])["totalScore"]));
                    totalScoreMin = Math.Min(totalScoreMin, Convert.ToSingle(((Hashtable)constArr[i])["totalScore"]));

                    pressure += Convert.ToSingle(((Hashtable)constArr[i])["pressure"]);
                    pressureMax = Math.Max(pressureMax, Convert.ToSingle(((Hashtable)constArr[i])["pressure"]));
                    pressureMin = Math.Min(pressureMin, Convert.ToSingle(((Hashtable)constArr[i])["pressure"]));

                    stable += Convert.ToSingle(((Hashtable)constArr[i])["stable"]);
                    stableMax = Math.Max(stableMax, Convert.ToSingle(((Hashtable)constArr[i])["stable"]));
                    stableMin = Math.Min(stableMin, Convert.ToSingle(((Hashtable)constArr[i])["stable"]));

                    adjust += Convert.ToSingle(((Hashtable)constArr[i])["adjust"]);
                    adjustMax = Math.Max(adjustMax, Convert.ToSingle(((Hashtable)constArr[i])["adjust"]));
                    adjustMin = Math.Min(adjustMin, Convert.ToSingle(((Hashtable)constArr[i])["adjust"]));
                }
                dataMathed["mhrtMax"] = mhrtMax;
                dataMathed["mhrtMin"] = mhrtMin;
                dataMathed["hrvScoreMax"] = hrvScoreMax;
                dataMathed["hrvScoreMin"] = hrvScoreMin;
                dataMathed["totalScoreMax"] = totalScoreMax;
                dataMathed["totalScoreMin"] = totalScoreMin;
                dataMathed["pressureMax"] = pressureMax;
                dataMathed["pressureMin"] = pressureMin;
                dataMathed["stableMax"] = stableMax;
                dataMathed["stableMin"] = stableMin;
                dataMathed["adjustMax"] = adjustMax;
                dataMathed["adjustMin"] = adjustMin;
                mhrt =Math.Floor( mhrt / constArr.Count);
                hrvScore = Math.Floor(hrvScore / constArr.Count);
                totalScore = Math.Floor(totalScore / constArr.Count);
                pressure = Math.Floor(pressure / constArr.Count);
                adjust = Math.Floor(adjust / constArr.Count);
                stable = Math.Floor(stable / constArr.Count);
                this.mhrtText.Text = mhrt.ToString();
                this.adjustText.Text = adjust.ToString();
                this.hrvsText.Text = hrvScore.ToString();
                this.stableText.Text = stable.ToString();
                this.pressureText.Text = pressure.ToString();
                this.totalText.Text = totalScore.ToString();
            }
        }
        
        /// <summary>
        /// 切换图标下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (constArr.Count > 1)
            {
                ComboBox tempComboBox = (ComboBox)sender;
                int index = tempComboBox.SelectedIndex;
                if (index > 0)
                {
                    System.Diagnostics.Debug.Write("开始生成曲线图\n");
                    if (this.ChartGrid.Children.Count > 0)
                    {
                        this.ChartGrid.Children.Clear();
                    }
                    //设置颜色
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                    double max = 0, min = 0;
                    String tableIndex = "";
                    switch (index)
                    {
                        case 1://绿色,平均心率
                            mySolidColorBrush.Color = Color.FromRgb(38, 213, 38);
                            max = Convert.ToSingle(dataMathed["mhrtMax"]);
                            min = Convert.ToSingle(dataMathed["mhrtMin"]);
                            tableIndex = "mhrt";
                            break;
                        case 2://蓝色，HRV得分
                            mySolidColorBrush.Color = Color.FromRgb(41, 162, 225);
                            max = Convert.ToSingle(dataMathed["hrvScoreMax"]);
                            min = Convert.ToSingle(dataMathed["hrvScoreMin"]);
                            tableIndex = "hrvScore";
                            break;
                        case 3://红色，压力指数
                            mySolidColorBrush = Brushes.Red;
                            max = Convert.ToSingle(dataMathed["pressureMax"]);
                            min = Convert.ToSingle(dataMathed["pressureMin"]);
                            tableIndex = "pressure";
                            break;
                        case 4://紫色,调节指数
                            mySolidColorBrush.Color = Color.FromRgb(168, 0, 255);
                            max = Convert.ToSingle(dataMathed["adjustMax"]);
                            min = Convert.ToSingle(dataMathed["adjustMin"]);
                            tableIndex = "adjust";
                            break;
                        case 5://黄色，稳定指数
                            mySolidColorBrush.Color = Color.FromRgb(233, 225, 9);
                            max = Convert.ToSingle(dataMathed["stableMax"]);
                            min = Convert.ToSingle(dataMathed["stableMin"]);
                            tableIndex = "stable";
                            break;
                        case 6://灰色，综合得分
                            mySolidColorBrush.Color = Color.FromRgb(173, 173, 173);
                            max = Convert.ToSingle(dataMathed["totalScoreMax"]);
                            min = Convert.ToSingle(dataMathed["totalScoreMin"]);
                            tableIndex = "totalScore";
                            break;
                        default://白色
                            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 255);
                            tableIndex = "mhrt";
                            break;
                    }
                    System.Diagnostics.Debug.Write("最小坐标:" + min + "\n");
                    System.Diagnostics.Debug.Write("最大坐标:" + max + "\n");
                    ArrayList coordinate = new ArrayList();
                    //开始绘图
                    for (int l = 0; l < constArr.Count; l++)
                    {
                        double XOffset = 10 + l * (934 / (constArr.Count - 1));
                        double YOffset = ((Convert.ToSingle(((Hashtable)constArr[l])[tableIndex]) - min) / (max - min)) * 200;
                        if (max == min)
                        {
                            YOffset = 0;
                        }
                        Point tmpPoint = new Point(XOffset, 200 - YOffset + 8);
                        /*添加竖线*/
                        Rectangle line = new Rectangle();
                        line.Opacity = 0.3;
                        line.Fill = Brushes.White;
                        line.Width = 1;
                        line.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        line.Margin = new Thickness(XOffset, 0, 0, 0);
                        this.ChartGrid.Children.Add(line);
                        /*添加圆点*/
                        Ellipse ellipse = new Ellipse();
                        ellipse.Width = 16;
                        ellipse.Height = 16;
                        ellipse.Fill = mySolidColorBrush;
                        ellipse.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        ellipse.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                        ellipse.Margin = new Thickness(XOffset - 8, 0, 0, YOffset);
                        this.ChartGrid.Children.Add(ellipse);
                        coordinate.Add(tmpPoint);
                    }
                   
                    PathFigure myPathFigure = new PathFigure();
                    myPathFigure.StartPoint = (Point)coordinate[0];
                    for (int i = 1; i < coordinate.Count; i++)
                    {
                        myPathFigure.Segments.Add(new LineSegment((Point)coordinate[i], true));
                    }
                    /// Create a PathGeometry to contain the figure.
                    PathGeometry myPathGeometry = new PathGeometry();
                    myPathGeometry.Figures.Add(myPathFigure);

                    Path chart = new Path();
                    chart.Stroke = mySolidColorBrush;
                    chart.StrokeThickness = 1;
                    chart.Data = myPathGeometry;
                    this.ChartGrid.Children.Add(chart);
                }
                else
                {
                    this.ChartGrid.Children.Clear();
                }
            }
        }

        private void MoodSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox tmp = (ComboBox)sender;
            mood = tmp.SelectedIndex;
        }

        private void TimeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox tmp = (ComboBox)sender;
            timeType = tmp.SelectedIndex;
        }
    }
}
