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
using System.Windows.Media.Animation;
namespace PmtsControlLibrary
{
    /// <summary>
    /// HRVSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class HRVSettingView : UserControl
    {
        private Button _closeSettingButton = new Button();
        private Button _saveSettingButton = new Button();
        private CheckBox _onCheckBox = new CheckBox();//
        private CheckBox _offCheckBox = new CheckBox();//
        private CheckBox _basisCheckBox = new CheckBox();//
        private CheckBox _fiveminCheckBox = new CheckBox();//
        private CheckBox _tenminCheckBox = new CheckBox();// 
        private CheckBox _defaultCheckBox = new CheckBox(); //
        private CheckBox _defaultoffCheckBox = new CheckBox();//
        public string stringstring = null;
        public HRVSettingView()
        {
            InitializeComponent();
            _closeSettingButton = this.closeSettingButton;
            _saveSettingButton = this.saveSettingButton;
            _onCheckBox = this.onCheckBox;
            _offCheckBox = this.offCheckBox;
            _basisCheckBox = this.basisCheckBox;
            _fiveminCheckBox = this.fiveminCheckBox;
            _tenminCheckBox = this.tenminCheckBox;
            _defaultCheckBox = this.defaultCheckBox;
            _defaultoffCheckBox = this.defaultoffCheckBox;
            //stringstring = this.stringstring;

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
        public Button CloseSettingButton
        {
            set { _closeSettingButton = value; }
            get { return _closeSettingButton; }
        }
        /// <summary>
        /// 实现保存按钮
        /// </summary>
        public Button SaveSettingButton
        {
            set { _saveSettingButton = value; }
            get { return _saveSettingButton; }
        }

        private void onCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(onCheckBox.IsChecked ==false){
                onCheckBox.IsChecked = false;
                _offCheckBox.IsChecked = true;
            }else{
                onCheckBox.IsChecked = true;
                _offCheckBox.IsChecked = false;
            }
        }

        private void offCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (offCheckBox.IsChecked == false)
            {
                _onCheckBox.IsChecked = true;
                offCheckBox.IsChecked = false;
            }
            else
            {
                _onCheckBox.IsChecked = false;
                offCheckBox.IsChecked = true;
            }
        }

        private void basisCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (basisCheckBox.IsChecked == false)
            {
                basisCheckBox.IsChecked = false;
                _fiveminCheckBox.IsChecked = true;
                _tenminCheckBox.IsChecked = false;
            }
            else
            {
                basisCheckBox.IsChecked = true;
                _fiveminCheckBox.IsChecked = false;
                _tenminCheckBox.IsChecked = false;
            }
        }

        private void fiveminCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (fiveminCheckBox.IsChecked == false)
            {
                fiveminCheckBox.IsChecked = false;
                _basisCheckBox.IsChecked = false;
                _tenminCheckBox.IsChecked = true;
            }
            else
            {
                fiveminCheckBox.IsChecked = true;
                _basisCheckBox.IsChecked = false;
                _tenminCheckBox.IsChecked = false;
            }
        }

        private void tenminCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (tenminCheckBox.IsChecked == false)
            {
                tenminCheckBox.IsChecked = false;
                _fiveminCheckBox.IsChecked = false;
                _basisCheckBox.IsChecked = true;
            }
            else
            {
                tenminCheckBox.IsChecked = true;
                _fiveminCheckBox.IsChecked = false;
                _basisCheckBox.IsChecked = false;
            }
        }

        private void upMusicButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            //"jpg(*.jpg)|bmp(*.bmp)|gif(*.gif)  " ;  txt files (*.txt)|*.txt|All files (*.*)|*.*
            openFileDialog.Filter = "Media   Files(*.mp3;*.WMA)|*.mp3;*.WMA";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                //musictextBlock.Text = openFileDialog.FileName;
                stringstring = openFileDialog.FileName;
                //player.Open(new Uri(openFileDialog.FileName, UriKind.Relative));
                //player.Play();

            }
        }

        private void defaultoffCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (defaultoffCheckBox.IsChecked == false)
            {
                _defaultCheckBox.IsChecked = true;
                defaultoffCheckBox.IsChecked = false;
            }
            else
            {
                _defaultCheckBox.IsChecked = false;
                defaultoffCheckBox.IsChecked = true;
            }
        }

        private void defaultCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (defaultCheckBox.IsChecked == false)
            {
                defaultCheckBox.IsChecked = false;
                _defaultoffCheckBox.IsChecked = true;
            }
            else
            {
                defaultCheckBox.IsChecked = true;
                _defaultoffCheckBox.IsChecked = false;
            }
        }
    }
}
