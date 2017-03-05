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

namespace PmtsControlLibrary
{
    /// <summary>
    /// RecordMusicView.xaml 的交互逻辑
    /// </summary>
    public partial class RecordMusicView : UserControl
    {
        private MediaPlayer player = null;
        private Path _closeButton = new Path();
        public RecordMusicView()
        {
            InitializeComponent();
            _closeButton = this.closeButton;
            player = new MediaPlayer();
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
        /// 实现关闭按钮
        /// </summary>
        public Path CloseButton
        {
            set { _closeButton = value; }
            get { return _closeButton; }
        }
        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush myBrush = new SolidColorBrush(Colors.White);
            this.closeButton.Fill = myBrush;
        }

        private void Path_MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(126, 255, 255, 255));
            this.closeButton.Fill = myBrush;
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(255, 35, 50, 74));
            this.closeButton.Fill = myBrush;
            
            player.Pause();
            
            
        }
        private void UploadmusicButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            //"jpg(*.jpg)|bmp(*.bmp)|gif(*.gif)  " ;  txt files (*.txt)|*.txt|All files (*.*)|*.*
            openFileDialog.Filter = "Media   Files(*.mp3;*.WMA)|*.mp3;*.WMA";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //    videoScreenMediaElement.Source = new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute);
                //    videoScreenMediaElement.Play();
                musictextBlock.Text = openFileDialog.FileName;
           
                player.Open(new Uri(openFileDialog.FileName, UriKind.Relative));
                player.Play();

            }
        }

        private void musicPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (musictextBlock.Text == " ")
            {
                return;
            }
            player.Play();
        }

        private void musicPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (musictextBlock.Text == " ")
            {
                return;
            }
            player.Pause();
        }
    }
}
