using System;
using System.Collections;
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
using System.Xml;


using System.Windows.Threading;
using System.IO;

namespace PmtsControlLibrary
{
    /// <summary>
    /// PlayerView.xaml 的交互逻辑
    /// </summary>
    public partial class ClassPlayerView : UserControl
    {

        private Grid mainWindow = new Grid();
        private System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        private AxShockwaveFlashObjects.AxShockwaveFlash shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
        private DBPlugin.CourseDB cdb = new DBPlugin.CourseDB();
        private bool isPlaying = false;
        private ClassTopView ctv = null;
        private TreeView courseTreeView = null;
        
        public ClassPlayerView()
        {
            InitializeComponent();

        }
        public ClassPlayerView(Grid Main)
        {
            InitializeComponent();
            mainWindow = Main;
            cdb = new DBPlugin.CourseDB();
            double left = this.Margin.Left - 80;
            this.Margin = new Thickness(left, 20, 0, 0);
            this.CourseListGrid.Visibility = System.Windows.Visibility.Visible;

            tmrProgress = new DispatcherTimer();
            //设置计时器的时间间隔为1秒
            tmrProgress.Interval = new TimeSpan(0, 0, 1);
            //计时器触发事件处理
            tmrProgress.Tick += SetDisplayMessage;
            SetImageForMediaElement();

            ctv = new ClassTopView();
            ctv.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            ctv.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            ctv.Margin = new Thickness(24, 5, 0, 0);
            ctv.clasTopImage.Source = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/classTopImage.png", UriKind.Relative));
            ctv.RelaxNameText.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText3.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText4.Visibility = System.Windows.Visibility.Hidden;
            if (!this.mainWindow.Children.Contains(ctv))
            {
                this.mainWindow.Children.Add(ctv);
            }
            courseTreeView = new TreeView();
        }

        private void viewGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void FromFlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            //   System.Diagnostics.Debug.Write("解析nodeXml："+NodeXml( e.request.ToString())+"\n");
        }
        private void changeVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            System.Diagnostics.Debug.Write("Slider值：" + e.NewValue + "\n");
            shockwave.CallFunction("<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>" + e.NewValue + "</number></arguments></invoke>");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Children.Remove(this);
        }


        /// <summary>
        /// 载入课程列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool firstLoad = true;
        private void OnLoadListView(object sender, RoutedEventArgs e)
        {
            if (firstLoad)
            {
                firstLoad = false;
                courseTreeView.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(courseTreeView_SelectedItemChanged);
                //courseTreeView.FontFamily = new System.Windows.Media.FontFamily("Microsoft YaHei");
                courseTreeView.FontSize = 14;
                courseTreeView.Height = 584;
                courseTreeView.Background = null;
                courseTreeView.Foreground = new SolidColorBrush(Colors.White);
                courseTreeView.Margin = new Thickness(0);

                DirectoryInfo folder = new DirectoryInfo(@"Resources\课程学习");
                DirectoryInfo[] chldFolders = folder.GetDirectories();
                foreach (DirectoryInfo chldFolder in chldFolders)
                {
                    TreeViewItem chldNode = new TreeViewItem();
                    chldNode.Header = chldFolder.Name;
                    chldNode.Tag = chldFolder.FullName;
                    chldNode.Foreground = new SolidColorBrush(Colors.White);  //用固态画刷填充前景色
                    chldNode.IsExpanded = true;
                    GetFiles(chldFolder.FullName, chldNode);
                    courseTreeView.Items.Add(chldNode);
                }
                if (!this.ClassListTreeView.Children.Contains(courseTreeView))
                {
                    this.ClassListTreeView.Children.Add(courseTreeView);
                }
            }
        }


        //
        void courseTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ctv.RelaxNameText.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText3.Visibility = System.Windows.Visibility.Hidden;
            ctv.RelaxNameText4.Visibility = System.Windows.Visibility.Hidden;

            var selected = courseTreeView.SelectedItem as TreeViewItem;
            Uri uri = new Uri(selected.Tag.ToString());

            DirectoryInfo folder = new DirectoryInfo(selected.Tag.ToString());

            var name = folder.Name;

            List<string> list = new List<string>();

            FolderNames(selected.Tag.ToString(), list);

            ctv.RelaxNameText.Visibility = System.Windows.Visibility.Visible;


            ctv.RelaxNameText.Text = list[0];
            if (list.Count > 1)
            {
                ctv.RelaxNameText.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText.Text = list[1];
                ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Visible;
                ctv.RelaxNameText2.Text = list[0];
            }
            else
            {
                ctv.RelaxNameText1.Visibility = System.Windows.Visibility.Hidden;
                ctv.RelaxNameText2.Visibility = System.Windows.Visibility.Hidden;
            }



            string extension; //
            extension = System.IO.Path.GetExtension(selected.Tag.ToString());

            if (extension.ToLower() == ".mp4")
            {
                videoScreenMediaElement.Source = uri;
                isPlaying = false;
                myplay();

            }
        }

        private void FolderNames(string file, List<string> list)
        {
            DirectoryInfo folder = new DirectoryInfo(file);
            var rs1 = folder.Name.ToString().ToLower().Replace(".mp4", "");
            list.Add(rs1);
            if (!folder.Parent.ToString().Equals("课程学习"))
            {
                var rs2 = folder.Parent.ToString().ToLower().Replace(".mp4", "");
                list.Add(rs2);

            }
        }



        private void GetFiles(string filePath, TreeViewItem node)
        {
            DirectoryInfo folder = new DirectoryInfo(filePath);
            //node.Header = folder.Name;
            //node.Tag = folder.FullName;

            FileInfo[] chldFiles = folder.GetFiles("*.*");
            foreach (FileInfo chlFile in chldFiles)
            {
                TreeViewItem chldNode = new TreeViewItem();
                var rs1 = chlFile.Name.ToLower().Replace(".mp4", "");
                var rs2 = chlFile.Name.Split('.')[0];

                chldNode.Header = rs2;
                chldNode.Tag = chlFile.FullName;
                chldNode.Foreground = new SolidColorBrush(Colors.White);  //用固态画刷填充前景色
                node.Items.Add(chldNode);
            }



        }

        private void GetFiles(string filePath, System.Windows.Forms.TreeNode node)
        {
            DirectoryInfo folder = new DirectoryInfo(filePath);
            node.Text = folder.Name;
            node.Tag = folder.FullName;

            FileInfo[] chldFiles = folder.GetFiles("*.*");
            foreach (FileInfo chlFile in chldFiles)
            {
                System.Windows.Forms.TreeNode chldNode = new System.Windows.Forms.TreeNode();
                chldNode.Text = chlFile.Name;
                chldNode.Tag = chlFile.FullName;
                node.Nodes.Add(chldNode);
            }

            DirectoryInfo[] chldFolders = folder.GetDirectories();
            foreach (DirectoryInfo chldFolder in chldFolders)
            {
                System.Windows.Forms.TreeNode chldNode = new System.Windows.Forms.TreeNode();
                chldNode.Text = folder.Name;
                chldNode.Tag = folder.FullName;
                node.Nodes.Add(chldNode);
                GetFiles(chldFolder.FullName, chldNode);
            }

        }

        /// <summary>
        /// 开始播放课程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayCourse(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            Hashtable tmp = (Hashtable)tvi.Tag;
            host.Dispose();
            shockwave.Dispose();
            host = new System.Windows.Forms.Integration.WindowsFormsHost();
            shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
            host.Child = shockwave;
            //this.playerPanel.Children.Add(host);

            // 设置 .swf 文件相对路径
            string swfPath = System.Environment.CurrentDirectory;
            swfPath += "\\Course\\" + tmp["mid"] + "-" + tmp["cid"] + ".swf";
            shockwave.Movie = swfPath;
            shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(FromFlashCall);
            shockwave.CallFunction("<invoke name=\"c2flash\" returntype=\"xml\"><arguments><number>50</number></arguments></invoke>");
            // this.changeVolume.Value = 50;
            shockwave.Loop = false;
            //this.changeVolume.ValueChanged += changeVolume_ValueChanged;
            cdb.OnUpCourseLook(tmp);

        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            host.Dispose();
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;

        }


        private void PlayPause(object sender, ExecutedRoutedEventArgs e)
        {
            myplay();

        }



        private void play_Click(object sender, RoutedEventArgs e)
        {

            myplay();
        }

        void myplay()
        {
            classImage.Visibility = System.Windows.Visibility.Hidden;
            //启动计时器
            if (!tmrProgress.IsEnabled)
            {
                tmrProgress.Start();
            }
            if (isPlaying)
            {
                tmrProgress.Start();
                videoScreenMediaElement.Pause();
                isPlaying = false;
                play.IsChecked = false;

            }
            else
            {
                videoScreenMediaElement.Play();
                isPlaying = true;

                play.IsChecked = true;
            }
        }



        private void Stop(object sender, ExecutedRoutedEventArgs e)
        {
            videoScreenMediaElement.Stop();
            play.IsChecked = false;
            isPlaying = false;
        }



        //将录像的第一帧作为播放前MediaElement显示的录像截图
        public void SetImageForMediaElement()
        {
            videoScreenMediaElement.Play();
            videoScreenMediaElement.Pause();
            videoScreenMediaElement.Position = TimeSpan.Zero;
        }

        //计时器，定时更新进度条和播放时间
        private DispatcherTimer tmrProgress = new DispatcherTimer();

        //计时器触发事件处理
        private void SetDisplayMessage(Object sender, System.EventArgs e)
        {
            if (videoScreenMediaElement.NaturalDuration.HasTimeSpan)
            {

                TimeSpan currentPositionTimeSpan = videoScreenMediaElement.Position;

                string currentPosition = string.Format("{0:00}:{1:00}:{2:00}", (int)currentPositionTimeSpan.TotalHours, currentPositionTimeSpan.Minutes, currentPositionTimeSpan.Seconds);

                TimeSpan totaotp = videoScreenMediaElement.NaturalDuration.TimeSpan;
                string totalPostion = string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);

                currentPositionTime.Text = currentPosition;
                playProgressSlider.Value = videoScreenMediaElement.Position.TotalSeconds;

            }
        }



        //当完成媒体加载时发生
        private void videoScreenMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            playProgressSlider.Minimum = 0;
            playProgressSlider.Maximum = videoScreenMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            TimeSpan totaotp = videoScreenMediaElement.NaturalDuration.TimeSpan;
            videoAllTime.Text = "/" + string.Format("{0:00}:{1:00}:{2:00}", (int)totaotp.TotalHours, totaotp.Minutes, totaotp.Seconds);
            currentPositionTime.Text = "00:00:00";


        }




        //在鼠标拖动Thumb的过程中记录其值。
        private TimeSpan ts = new TimeSpan();
        private void playProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ts = TimeSpan.FromSeconds(e.NewValue);
            string currentPosition = string.Format("{0:00}:{1:00}:{2:00}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
            currentPositionTime.Text = currentPosition;

        }

        //当拖动Thumb的鼠标放开时，从指定时间开始播放
        private void playProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            videoScreenMediaElement.Position = ts;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (tmrProgress.IsEnabled)
            {
                tmrProgress.Stop();
            }

        }

        private void videoScreenMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoScreenMediaElement.Position = TimeSpan.Zero;
            videoScreenMediaElement.Stop();
            play.IsChecked = false;
            classImage.Visibility = System.Windows.Visibility.Visible;
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }



        /*
        //获取视频目录  
        private void InitPath()
        {
            ApplicationPath ap = new ApplicationPath();
            root = ap.GetRtfPath("root");
            pathMedia = root + ap.GetRtfPath("pathMedia");
        }

        //将视频目录下的视频名称添加到ListView中  
        private void AddItemToListView()
        {
            string[] files = Directory.GetFiles(pathMedia);
            foreach (string file in files)
            {
                this.listView1.Items.Add(file.Substring(file.LastIndexOf('\\') + 1));
            }
        }

        //窗体加载时调用视频，进行播放  
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MediaElementControl();
        }

        //存储播放列表中视频的名称  
        List<string> fileNames = new List<string>();
        private void MediaElementControl()
        {
            this.mediaElement1.LoadedBehavior = MediaState.Manual;

            string[] files = Directory.GetFiles(pathMedia);

            foreach (string file in files)
            {
                fileNames.Add(file.Substring(file.LastIndexOf('\\') + 1));
            }

            this.mediaElement1.Source = new Uri(files[0]);

            this.mediaElement1.Play();
        }

        //视频播放结束事件  
        private void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            //获取当前播放视频的名称（格式为：xxx.wmv）  
            string path = this.mediaElement1.Source.LocalPath;
            string currentfileName = path.Substring(path.LastIndexOf('\\') + 1);

            //对比名称列表，如果相同，则播放下一个，如果播放的是最后一个，则从第一个重新开始播放  
            for (int i = 0; i < fileNames.Count; i++)
            {
                if (currentfileName == fileNames[i])
                {
                    if (i == fileNames.Count - 1)
                    {
                        this.mediaElement1.Source = new Uri(pathMedia + "//" + fileNames[0]);
                        this.mediaElement1.Play();
                    }
                    else
                    {
                        this.mediaElement1.Source = new Uri(pathMedia + "//" + fileNames[i + 1]);
                        this.mediaElement1.Play();
                    }
                    break;
                }
            }
        }

        //播放列表选择时播放对应视频  
        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fileName = this.listView1.SelectedValue.ToString();
            this.mediaElement1.Source = new Uri(pathMedia + "//" + fileName);
            this.mediaElement1.Play();
        }

       
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.mediaElement1.Position = this.mediaElement1.Position + TimeSpan.FromSeconds(20);
        }

       

        

       
        */


    }
}
