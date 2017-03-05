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

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainBack.xaml 的交互逻辑
    /// </summary>
    public partial class TrainBack : UserControl
    {
        private Grid mainWindow = new Grid();
        private TrainDB tdb = null;
        private int _haveHistorySID = 0;

        public TrainBack()
        {
            InitializeComponent();
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
        public TrainBack(Grid Main)
        {
            InitializeComponent();
            mainWindow = Main;
            tdb = new TrainDB();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Children.Remove(this);
        }
        /// <summary>
        /// 载入训练图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WrapPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ArrayList tList = tdb.GetTrainIsOpen();
            ArrayList tNumList = tdb.GetTrainHistoryNum();
            if (tNumList.Count > 0)
            {
                int tNumIndex = 0;
                Hashtable tNumTmp = (Hashtable)tNumList[tNumIndex];
                for (int i = 0; i < tList.Count; i++)
                {
                    Hashtable tmp = (Hashtable)tList[i];
                    TrainList tl = new TrainList(mainWindow,tmp, this);
                    tl.Margin = new Thickness(10, 10, 0, 0);
                    if (tmp["tid"].ToString() == tNumTmp["tid"].ToString())
                    {
                        tNumIndex += 1;
                        tmp["historyNum"] = tNumTmp["num"];
                        if (tNumIndex < tNumList.Count)
                        {
                            tNumTmp = (Hashtable)tNumList[tNumIndex];
                        }
                    }
                    this.TrainButtonGrid.Children.Add(tl);
                }
            }
            else
            {
                for (int i = 0; i < tList.Count; i++)
                {
                    Hashtable tmp = (Hashtable)tList[i];
                    TrainList tl = new TrainList(mainWindow,tmp, this);
                    tl.Margin = new Thickness(10, 10, 0, 0);
                    this.TrainButtonGrid.Children.Add(tl);
                }
            }

        }
        /// <summary>
        /// 显示或是隐藏是发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainButtonGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (this.TrainButtonGrid.Children.Count != 0)
                {
                    for (int i = 0; i < this.TrainButtonGrid.Children.Count; i++)
                    {
                        TrainList tl = this.TrainButtonGrid.Children[i] as TrainList;
                        
                    }
                }
            }
        }


    }


}
