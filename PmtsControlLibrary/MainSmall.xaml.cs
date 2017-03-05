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
    /// MainSmall.xaml 的交互逻辑
    /// </summary>
    public partial class MainSmall : System.Windows.Controls.UserControl
    {
        public MainSmall()
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
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MouseNotOver.Visibility = Visibility.Hidden;
            this.MouseClick.Visibility = Visibility.Hidden;
            this.MouseOnOver.Visibility = Visibility.Visible;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.MouseOnOver.Visibility = Visibility.Hidden;
            this.MouseClick.Visibility = Visibility.Hidden;
            this.MouseNotOver.Visibility = Visibility.Visible;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseOnOver.Visibility = Visibility.Hidden;
            this.MouseClick.Visibility = Visibility.Visible;
            this.MouseNotOver.Visibility = Visibility.Hidden;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MouseOnOver.Visibility = Visibility.Visible;
            this.MouseClick.Visibility = Visibility.Hidden;
            this.MouseNotOver.Visibility = Visibility.Hidden;
        }
    }
}
