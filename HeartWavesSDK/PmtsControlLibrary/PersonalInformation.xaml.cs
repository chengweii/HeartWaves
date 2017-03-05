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
    /// PersonalInformation.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalInformation : UserControl
    {
        private Grid main = new Grid();
        private Hashtable radarData = new Hashtable();//五维度数据
        private GetUserInfo userDb = null;
        private List<DataGridCont> dataList = null;
        private String[,] DataGridContent = { { "挪来移去", "观察力 意志力" }, //1
                                            { "边缘视力", "观察力 意志力" }, //2
                                            { "明察秋毫", "观察力 意志力" }, //3
                                            { "方向瞬记", "记忆力 观察力 意志力" },//4
                                            { "看图绘画", "记忆力 观察力 意志力" },//5
                                            { "多彩球", "记忆力 观察力 意志力" },//6
                                            { "街景分析", "记忆力 观察力 思维方式 意志力" },//7
                                            { "通缉令", "记忆力 观察力 意志力" },//8
                                            { "红绿灯", "思维方式 意志力" },//9
                                            { "临危不惧", "情绪情感 意志力" },//10
                                            { "百步穿杨", "意志力 思维方式" },//11
                                            { "镇定自若", "志力 情绪情感" },//12
                                            { "过目不忘", "记忆力 情绪情感 观察力 思维方式 意志力" },//13
                                            { "以此类推", "思维方式 观察力 意志力" }};//14
        //职位选择
        public PersonalInformation(Grid MainWindowsGrid)
        {
            InitializeComponent();
            main = MainWindowsGrid;
            comboBox1.Items.Add("教员");
            comboBox1.Items.Add("职员");
            comboBox1.SelectedIndex = 0;
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
        /// 读取数据库并刷新数据
        /// </summary>
        public void UpDate(Hashtable Meg)
        {
            if (userDb == null)
            {
                userDb = new GetUserInfo(Meg);
            }
            radarData = userDb.GetUserRadarData();
            DrawRadar();
            this.TextName.Text = UserInfoStatic.UserName;
            this.TextUID.Text = UserInfoStatic.UserID;
            if (UserInfoStatic.UserSex == "男")
            {
                this.boy.IsChecked = true;
            }
            else
            {
                this.gril.IsChecked = true;
            }
            this.TextAge.Text = UserInfoStatic.UserAge.ToString();
            this.comboBox1.Text = "未知";
            this.TextWorkType.Text = UserInfoStatic.UserWorkType;
            this.TextWorkArea.Text = UserInfoStatic.UserWorkArea;
            this.TextMR.Text = UserInfoStatic.UserMR;
            this.TextO.Text = "观察力(" + radarData["o"] + ")";
            this.TextW.Text = "意志力(" + radarData["w"] + ")";
            this.TextE.Text = "情绪情感(" + radarData["e"] + ")";
            this.TextT.Text = "思维方式(" + radarData["t"] + ")";
            this.TextR.Text = "记忆力(" + radarData["r"] + ")";
            this.HRVScore.Text = "总HRV得分： " + radarData["hs"];
        }
        /// <summary>
        /// 绘制雷达图
        /// </summary>
        private void DrawRadar()
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
            Point rp1 = new Point(120, 120 - MathLong(Convert.ToDouble(radarData["o"])));
            Point rp2 = new Point(120 - (Math.Cos(18 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["w"]))), 120 - (Math.Sin(18 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["w"]))));
            Point rp3 = new Point(120 - (Math.Cos(54 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["e"]))), 120 + (Math.Sin(54 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["e"]))));
            Point rp4 = new Point(120 + (Math.Cos(54 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["t"]))), 120 + (Math.Sin(54 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["t"]))));
            Point rp5 = new Point(120 + (Math.Cos(18 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["r"]))), 120 - (Math.Sin(18 * Math.PI / 180) * MathLong(Convert.ToDouble(radarData["r"]))));
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
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.Children.Remove(this);
            for (int i = 0; i < main.Children.Count; i++)
            {
                UIElement ui = main.Children[i];
                if (typeof(Rectangle) == ui.GetType())
                {
                    if (((Rectangle)ui).Tag.ToString() == "mark")
                    {
                        main.Children.Remove(ui);
                    }
                }
            }
        }
        /// <summary>
        /// 点击训练文字,出现训练和维度的对应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            this.ItemGrid1.Visibility = System.Windows.Visibility.Hidden;
            this.ItemGrid2.Visibility = System.Windows.Visibility.Visible;
            if (dataList == null)
            {
                dataList = new List<DataGridCont>();
                for (int i = 0; i < DataGridContent.GetLength(0); i++)
                {
                    dataList.Add(new DataGridCont()
                    {
                        TrainName = DataGridContent[i, 0],
                        Ability = DataGridContent[i, 1]
                    });
                }
                this.showDataGrid.DataContext = dataList;
            }
        }
        /// <summary>
        /// 返回雷达图界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            this.ItemGrid1.Visibility = System.Windows.Visibility.Visible;
            this.ItemGrid2.Visibility = System.Windows.Visibility.Hidden;
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.pwd1.Password != this.pwd2.Password)
            {
                PmtsMessageBox.CustomControl1.Show("两次输入的密码不同", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }
            else if (String.IsNullOrEmpty(this.TextName.Text))
            {
                PmtsMessageBox.CustomControl1.Show("请输入姓名", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            }
            else
            {
                Hashtable userInfo = new Hashtable();
                userInfo["name"] = this.TextName.Text;
                if (String.IsNullOrEmpty(this.pwd1.Password) && String.IsNullOrEmpty(this.pwd2.Password))
                {
                    userInfo["pwd"] ="NoChange";
                }
                else
                {
                    userInfo["pwd"] = this.pwd1.Password;
                }
                if (this.boy.IsChecked == true)
                {
                    userInfo["sex"] = 1;
                }
                else
                {
                    userInfo["sex"] = 2;
                }
                userInfo["age"] = this.TextAge.Text;
                //userInfo["workyear"] = this.comboBox1.Text;
                userInfo["workarea"] = this.TextWorkArea.Text;
                userInfo["worktype"] = this.TextWorkType.Text;
                userInfo["mr"] = this.TextMR.Text;
                if (userDb.OnUpdateUserInfo(userInfo))
                {
                    UserInfoStatic.UserName = this.TextName.Text;
                    if (this.boy.IsChecked == true)
                    {
                        UserInfoStatic.UserSex = "男";
                    }
                    else
                    {
                        UserInfoStatic.UserSex = "女";
                    }
                    UserInfoStatic.UserAge = this.TextAge.Text;
                    UserInfoStatic.UserWorkType = this.TextWorkType.Text;
                    PmtsMessageBox.CustomControl1.Show("更改用户信息成功，点击确定关闭窗口并刷新主页面信息。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    main.Children.Remove(this);
                    for (int i = 0; i < main.Children.Count; i++)
                    {
                        UIElement ui = main.Children[i];
                        if (typeof(Rectangle) == ui.GetType())
                        {
                            if (((Rectangle)ui).Tag.ToString() == "mark")
                            {
                                main.Children.Remove(ui);
                            }
                        }
                    }

                }
            }
        }
        //职务选择
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (this.comboBox1.SelectedIndex == 0)
            {
                this.comboBox1.Text = "教员";
            }
            else if (this.comboBox1.SelectedIndex == 1)
            {
                this.comboBox1.Text = "职员";
            }
            else {
                this.comboBox1.Text = "未知";
            }
        }

        
        
        ///// <summary>
        ///// 检查输入的年龄
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void OnCheckAgeText(object sender, KeyEventArgs e)
        //{
        //    TextBox txt = sender as TextBox;

        //    //屏蔽非法按键
        //    if (((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || ((e.Key >= Key.D0 && e.Key <= Key.D9))) && txt.Text.Length < 2)
        //    {
        //        e.Handled = false;//执行，尚未执行的指令。
        //    }
        //    else
        //    {
        //        e.Handled = true;//不执行。表示已经执行完毕。
        //    }
        //}
    }
    public class DataGridCont
    {
        public String TrainName { set; get; }
        public String Ability { set; get; }
    }
}
