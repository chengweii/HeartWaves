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
using System.ComponentModel;


namespace PmtsControlLibrary
{
    /// <summary>
    /// Breathing.xaml 的交互逻辑
    /// </summary>
    public partial class Breathing : UserControl
    {
        private int isStatus = 1;//小球的状态，1为下降，0为上升
        private TimeSpan animeTime = new TimeSpan(0, 0, 5);
        private Grid MainWindow = new Grid();
        private Button StartBut = new Button();


        public Breathing()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Main"></param>
        /// <param name="BreathBut"></param>
        public Breathing(Grid Main,Button BreathBut)
        {
            InitializeComponent();
            this.MainWindow = Main;
            this.StartBut = BreathBut;  
        }

        private void OnStoryCompleted(object sender, EventArgs e)
        {
            this.myStory.Stop();
            this.myDoubleAnime.Duration = new Duration(animeTime);
            System.Diagnostics.Debug.Write("动画停止,下一次动画时间："+this.myDoubleAnime.Duration+"\n");
            if (isStatus == 1)
            {
                isStatus = 0;
                this.myDoubleAnime.To = 170;
            }
            else
            {
                isStatus = 1;
                this.myDoubleAnime.To = 24;
            }
            this.myStory.Begin();
        }
       
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            int second = Convert.ToInt32(Math.Floor(e.NewValue));
            int mSecond = Convert.ToInt32(Math.Floor((e.NewValue % 1) * 100));
            animeTime = new TimeSpan(0, 0, 0, second, mSecond);
            //System.Diagnostics.Debug.Write("动画时间：" + animeTime + "\n");
        }

        private void Anime_Loaded(object sender, RoutedEventArgs e)
        {
            this.myDoubleAnime.Duration = new Duration(animeTime);
            this.myDoubleAnime.To = 24;
            this.myStory.Completed += OnStoryCompleted;
            this.myStory.Begin();
        }

        private void breathingButton_Click(object sender, RoutedEventArgs e)
        {
            this.MainWindow.Children.Remove(this);
            System.Diagnostics.Debug.Write("呼吸助手结束时分配内存：" + GC.GetTotalMemory(true) + "\n");
            this.StartBut.IsEnabled = true;
        }
        
    }
}
