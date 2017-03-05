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
    /// HRVMark.xaml 的交互逻辑
    /// </summary>
    public partial class HRVMark : UserControl
    {
        private Path _closeButton = new Path();
        private Button _saveButton = new Button();
        private double _saveTime = 0;
        private DateTime _markDateTime = new DateTime();


        public HRVMark()
        {
            InitializeComponent();
            _closeButton = this.closeButton;
            _saveButton = this.saveButton;
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
        /// <summary>
        /// 实现保存按钮
        /// </summary>
        public Button SaveButton
        {
            set { _saveButton = value; }
            get { return _saveButton; }
        }
        /// <summary>
        /// 事件标记的时间
        /// </summary>
        public double SaveTime
        {
            set { _saveTime = value; }
            get { return _saveTime; }
        }
        /// <summary>
        /// 
        /// 时间戳
        /// </summary>
        public DateTime MarkDateTime
        {
            set;
            get;
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
        }

    }
}
