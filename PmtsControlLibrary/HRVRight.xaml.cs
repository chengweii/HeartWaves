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
    /// HRVLeft.xaml 的交互逻辑
    /// </summary>
    public partial class HRVRight : UserControl
    {
        private TextBlock _hrText;//及时心率
        private AxShockwaveFlashObjects.AxShockwaveFlash shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
        private System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        /// <summary>
        /// 构造函数
        /// </summary>
        public HRVRight()
        {
            InitializeComponent();
            _hrText = this.HeartRateText;
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
        /// 即使心率
        /// </summary>
        public TextBlock HRText
        {
            set { _hrText = value; }
            get { return _hrText; }
            
        }
        public Button BreathingButton
        {
            get { return this.breathingButton; }
        }
        private void OnLoadedStatus(object sender, RoutedEventArgs e)
        {
            //开始播放Flash
 /*           this.host = new System.Windows.Forms.Integration.WindowsFormsHost();
            shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
            host.Child = shockwave;
            this.BallGrid.Children.Add(host);

            string swfPath = System.Environment.CurrentDirectory;
            swfPath += "\\S\\Ball.swf";//556x128
            // 设置 .swf 文件相对路径
            shockwave.Movie = swfPath;
            String cmd = "<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>1</number></arguments></invoke>";
            shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(FromFlashData);
            shockwave.CallFunction(cmd);
            shockwave.Play();
            //调整ep槽
            OnAnimationForEP(0);
*/
            this.host = new System.Windows.Forms.Integration.WindowsFormsHost();
            shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
            host.Child = shockwave;
            this.StatusGrid.Children.Add(host);

            shockwave.BGColor = "28A9ED";
            if (shockwave.IsPlaying())
            {
                shockwave.Stop();
                shockwave.Refresh();
            }
        /*    string swfPath = System.Environment.CurrentDirectory;
            swfPath += "\\S\\s3.swf";//556x128
            // 设置 .swf 文件相对路径
            shockwave.Movie = swfPath;
            shockwave.BGColor = "1E78E7";
            shockwave.Play();
         */ 
        }

        public void StopAnime()
        {
            if (shockwave.IsPlaying())
            {
                shockwave.Stop();
                shockwave.Refresh();
            }
            OnAnime(0);
        }
        /// <summary>
        /// 神经活跃性动画效果。
        /// </summary>
        /// <param name="nb"></param>
        public void OnAnime(double nb)
        {
            double to = 0;
            string swfPath = System.Environment.CurrentDirectory;
            if (nb > 0 && nb < 1.15)
            {
                if (nb > 0 && nb <= 0.4)
                {
                    to = (90 - (nb * (45 / 0.4))) * -1;
                    swfPath += "\\S\\s4.swf";//556x128
                }
                else if (nb > 0.4 && nb <= 0.8)
                {
                    to = (90 - ((nb - 0.4) * (27 / 0.4) + 45)) * -1;
                    swfPath += "\\S\\s5.swf";//556x128
                }
                else if (nb > 0.8 && nb < 1.15)
                {
                    to = (90 - (((nb - 0.8) * (18 / 0.35)) + 72)) * -1;
                    swfPath += "\\S\\s3.swf";//556x128
                }
            }
            else
            {
                if (nb >= 1.15 && nb <= 1.5)
                {
                    to = ((nb - 1.15) * (18 / 0.35));
                    swfPath += "\\S\\s3.swf";//556x128
                }
                else if (nb > 1.5 && nb <= 5)
                {
                    to = 27 + ((nb - 1.5) * (27 / 3.5));
                    swfPath += "\\S\\s2.swf";//556x128
                }
                else if (nb > 5)
                {
                    to = 45 + ((nb - 5) * (45 / 10));
                    
                    if (to >= 90)
                    {
                        to = 90;
                    }
                    swfPath += "\\S\\s1.swf";//556x128
                }
            }
            myDoubleAnime.To = to;
            myStory.Begin();
            shockwave.Movie = swfPath;
            shockwave.Play();
        }

       

        private void breathingbutton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
