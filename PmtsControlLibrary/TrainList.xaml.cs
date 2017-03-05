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

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainList.xaml 的交互逻辑
    /// </summary>
    public partial class TrainList : UserControl
    {
        private Hashtable tInfo = new Hashtable();
        private UserControl PUI = new UserControl();
        private Grid mainWindow = new Grid();//主窗体中放置控件的层
        public TrainList()
        {
            InitializeComponent();

        }
        public TrainList(Grid Main, Hashtable trainInfo, UserControl parentUI)
        {
            InitializeComponent();
            tInfo = trainInfo;
            PUI = parentUI;
            mainWindow = Main;
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            if (Convert.ToInt32(tInfo["open"]) == 0)
            {
                this.TrainButton.Tag = "../Image/Train/Lock.png";
                this.TrainButton.IsEnabled = false;
            }
            else
            {
                this.TrainButton.Tag = "../Image/Train/" + tInfo["tid"].ToString() + ".png";
                this.TrainButton.Click += OnOpenTrain;

            }
            //this.TrainName.Text = tInfo["tname"].ToString();
            if (Convert.ToInt32(tInfo["historyNum"]) > 0)
            {
                //his.HistoryButton.Tag = tInfo["tid"];
            }
            else
            {
                //his.HistoryButton.Tag = tInfo["tid"];
                //his.HistoryButton.IsEnabled = false;
            }
            if (Convert.ToDouble(tInfo["gateNum"].ToString()) == 0f)
            {
                //his.GetNumShow.ScaleX = 1;
            }
            else
            {
                double gatePer = Convert.ToDouble(tInfo["gateOpen"].ToString()) / Convert.ToDouble(tInfo["gateNum"].ToString());
                //is.GetNumShow.ScaleX = gatePer;
            }

        }

        /// <summary>
        /// 训练按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenTrain(object sender, RoutedEventArgs e)
        {
            PUI.Visibility = System.Windows.Visibility.Hidden;
            Grid mainGrid = (Grid)PUI.Parent;
            TrainPlayerView tpv = new TrainPlayerView(mainWindow, tInfo, PUI);
            mainGrid.Children.Add(tpv);
            mainGrid.Margin = new Thickness(0, 89, 0, 20);
            mainGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            mainGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }
        /// <summary>
        /// 打开记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenHistory(object sender, RoutedEventArgs e)
        {
            TrainHistory th = new TrainHistory(Convert.ToInt32(tInfo["tid"]));
            th.ShowDialog();

        }
    }
}
