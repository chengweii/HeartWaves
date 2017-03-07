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
using PmtsControlLibrary.WEBPlugin;

namespace PmtsControlLibrary
{
    /// <summary>
    /// RecordCenter.xaml 的交互逻辑
    /// </summary>
    public partial class RecordCenter : UserControl
    {
        private HRVDetaile hd = null;//历史记录详情
        private Rectangle mark = null;//屏蔽层

        private Grid mainWindow = new Grid();//主窗体中放置控件的层
        private ArrayList HrvHistoryArr = new ArrayList();

        private ArrayList TrainHistoryArr = new ArrayList();
        private ArrayList RelaxHistoryArr = new ArrayList();

        private int _nowPage = 1;//当前页号
        private int _num = 15;//一页默认数据量
        private double _totalPage = 0;//总页数

        private int _showType = 1;//显示模式
        
        private static HRVControlWEB hrvdb = new HRVControlWEB();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Main"></param>
        public RecordCenter(Grid Main)
        {
            InitializeComponent();
            
            HrvHistoryArr = new ArrayList();
            GetHrvHistoryData();
           
            TrainHistoryArr = new ArrayList();
            GetTrainHistoryData();

            RelaxHistoryArr = new ArrayList();
            GetRelaxHistoryData();

            _showType = 1;
            OnDataOrPageChanged(1, _num);
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
        private void GetHrvHistoryData()
        {
 //           historyArr = hrvd.GetConstAndHistoryListData();
            if (UserInfoStatic.ipAdd == null)
            {
                if(MainRightPerson.TmpHrvRecord.Count > 0)
                {
                    UserHrvRecord[] tmpArr = new UserHrvRecord[MainRightPerson.TmpHrvRecord.Count];
                    MainRightPerson.TmpHrvRecord.CopyTo(tmpArr);
                    for (int i = 0; i < MainRightPerson.TmpHrvRecord.Count; i++)
                    {
                        UserHrvRecord tmp = tmpArr[i];
                        if(tmp.RecordType < 10)
                            HrvHistoryArr.Add(MainRightPerson.TmpHrvRecord[i]);
                    }
                }
            }else{
            	HrvHistoryArr=hrvdb.GetConstAndHistoryListData(1,1,"1","0");
            }
  /*          _totalPage = Math.Ceiling(Convert.ToDouble(HrvHistoryArr.Count / Convert.ToDouble(_num)));
            if (HrvHistoryArr.Count / Convert.ToDouble(_num) > 1)
            {
                // this.pageText.Content = "1 / " + _totalPage.ToString();
                this.PreviousButton.IsEnabled = false;
            }
            else
            {
                //this.pageText.Content = "1 / 1";
                this.PreviousButton.IsEnabled = false;
                this.NextButton.IsEnabled = false;
                this.GotoPage.IsEnabled = false;
                this.JumpButton.IsEnabled = false;
            }
   */ 
  //          OnDataOrPageChanged(1, _num);
        }

        private void GetTrainHistoryData()
        {
            //           historyArr = hrvd.GetConstAndHistoryListData();
            if (UserInfoStatic.ipAdd == null)
            {
                if (MainRightPerson.TmpHrvRecord.Count > 0)
                {
                    UserHrvRecord[] tmpArr = new UserHrvRecord[MainRightPerson.TmpHrvRecord.Count];
                    MainRightPerson.TmpHrvRecord.CopyTo(tmpArr);
                    for (int i = 0; i < MainRightPerson.TmpHrvRecord.Count; i++)
                    {
                        UserHrvRecord tmp = tmpArr[i];
                        if (tmp.RecordType >= 10 && tmp.RecordType < 100)
                            TrainHistoryArr.Add(MainRightPerson.TmpHrvRecord[i]);
                    }
                }
            }else{
            	TrainHistoryArr=hrvdb.GetConstAndHistoryListData(1,1,"2","0");
            }
        }

        private void GetRelaxHistoryData()
        {
            if (UserInfoStatic.ipAdd == null)
            {
                if (MainRightPerson.TmpHrvRecord.Count > 0)
                {
                    UserHrvRecord[] tmpArr = new UserHrvRecord[MainRightPerson.TmpHrvRecord.Count];
                    MainRightPerson.TmpHrvRecord.CopyTo(tmpArr);
                    for (int i = 0; i < MainRightPerson.TmpHrvRecord.Count; i++)
                    {
                        UserHrvRecord tmp = tmpArr[i];
                        if (tmp.RecordType >= 100)
                            RelaxHistoryArr.Add(MainRightPerson.TmpHrvRecord[i]);
                    }
                }
            }else{
            	RelaxHistoryArr=hrvdb.GetConstAndHistoryListData(1,1,"3","0");
            }
        }

        /// <summary>
        /// 分页处理
        /// </summary>
        /// <param name="page">当前的页号</param>
        /// <param name="num">每页的条数</param>
        private void OnDataOrPageChanged(int page, int num)
        {
//            Hashtable[] tmpArr = new Hashtable[num];
            if (_showType == 1)
            {
                this.HrvDataGrid.Visibility = Visibility.Visible;
                this.TrainDataGrid.Visibility = Visibility.Hidden;
                this.RelaxDataGrid.Visibility = Visibility.Hidden;

                _totalPage = Math.Ceiling(Convert.ToDouble(HrvHistoryArr.Count / Convert.ToDouble(_num)));

                this.PreviousButton.IsEnabled = true;
                this.NextButton.IsEnabled = true;
                if (_nowPage == 1)
                {
                    this.PreviousButton.IsEnabled = false;
                }
                if (_nowPage == _totalPage)
                {
                    this.NextButton.IsEnabled = false;
                }
                if (HrvHistoryArr.Count / Convert.ToDouble(_num) > 1)
                {
                    // this.pageText.Content = "1 / " + _totalPage.ToString();
                    this.GotoPage.IsEnabled = true;
                    this.JumpButton.IsEnabled = true;
                }
                else
                {
                    //this.pageText.Content = "1 / 1";
                    this.GotoPage.IsEnabled = false;
                    this.JumpButton.IsEnabled = false;
                }

                UserHrvRecord[] tmpArr = new UserHrvRecord[num];
                if (HrvHistoryArr.Count <= num)
                {
                    HrvHistoryArr.CopyTo(tmpArr);
                }
                else
                {
                    if (Math.Ceiling(Convert.ToDouble(HrvHistoryArr.Count / Convert.ToDouble(num))) < page)
                    {
                        page -= 1;
                        _totalPage -= 1;
                    }
                    if (page * num > HrvHistoryArr.Count && page == _totalPage)
                    {
                        int count = HrvHistoryArr.Count - ((page - 1) * num);
                        HrvHistoryArr.CopyTo((page - 1) * num, tmpArr, 0, count);
                    }
                    else
                    {
                        HrvHistoryArr.CopyTo((page - 1) * num, tmpArr, 0, num);
                    }
                }
                List<HRVRecordData> dataGridSource = new List<HRVRecordData>();

                //           for (int i = 0; i < tmpArr.Length; i++)
                for (int i = tmpArr.Length - 1; i >= 0; i--)
                {
                    //               Hashtable tmp = tmpArr[i];
                    UserHrvRecord tmp = tmpArr[i];
                    if (tmp != null)
                    {
                        string strType = "";
                        if (tmp.RecordType == 1)
                            strType = "基线测试";
                        else if (tmp.RecordType == 2)
                            strType = "5分钟测试";
                        else if (tmp.RecordType == 3)
                            strType = "10分钟测试";
                        dataGridSource.Add(new HRVRecordData()
                        {
                            Id = tmp.Id,
                            Checked = false,
                            Index = ((page - 1) * num + i + 1).ToString(),
                           // Type = tmp.RecordType.ToString(),
                            Type = strType,
                            StartTime = tmp.StartTime.ToString(),
                            TotalTime = MathTime((int)tmp.TimeLength),
                            HrvScore = Convert.ToInt32(tmp.Score).ToString(),
                            Adjust = Convert.ToInt32(tmp.Adjust).ToString(),
                            Stable = Convert.ToInt32(tmp.Stable).ToString(),
                            Pressure = Convert.ToInt32(tmp.Pressure).ToString(),
                            TotalScore = tmp.HrvScore.ToString()
                        });
                    }
                }

                this.pageText.Content = page.ToString() + " / " + _totalPage.ToString();
                this.HrvDataGrid.DataContext = dataGridSource;
            }
            else if (_showType == 2)
            {
                this.HrvDataGrid.Visibility = Visibility.Hidden;
                this.TrainDataGrid.Visibility = Visibility.Visible;
                this.RelaxDataGrid.Visibility = Visibility.Hidden;

                _totalPage = Math.Ceiling(Convert.ToDouble(TrainHistoryArr.Count / Convert.ToDouble(_num)));

                this.PreviousButton.IsEnabled = true;
                this.NextButton.IsEnabled = true;
                if (_nowPage == 1)
                {
                    this.PreviousButton.IsEnabled = false;
                }
                if (_nowPage == _totalPage)
                {
                    this.NextButton.IsEnabled = false;
                }
                if (TrainHistoryArr.Count / Convert.ToDouble(_num) > 1)
                {
                    // this.pageText.Content = "1 / " + _totalPage.ToString();
                    this.GotoPage.IsEnabled = true;
                    this.JumpButton.IsEnabled = true;
                }
                else
                {
                    //this.pageText.Content = "1 / 1";
                    this.GotoPage.IsEnabled = false;
                    this.JumpButton.IsEnabled = false;
                }

                UserHrvRecord[] tmpArr = new UserHrvRecord[num];
                if (TrainHistoryArr.Count <= num)
                {
                    TrainHistoryArr.CopyTo(tmpArr);
                }
                else
                {
                    if (Math.Ceiling(Convert.ToDouble(TrainHistoryArr.Count / Convert.ToDouble(num))) < page)
                    {
                        page -= 1;
                        _totalPage -= 1;
                    }
                    if (page * num > TrainHistoryArr.Count && page == _totalPage)
                    {
                        int count = TrainHistoryArr.Count - ((page - 1) * num);
                        TrainHistoryArr.CopyTo((page - 1) * num, tmpArr, 0, count);
                    }
                    else
                    {
                        TrainHistoryArr.CopyTo((page - 1) * num, tmpArr, 0, num);
                    }
                }
                List<HRVRecordData> dataGridSource = new List<HRVRecordData>();

                //           for (int i = 0; i < tmpArr.Length; i++)
                for (int i = tmpArr.Length - 1; i >= 0; i--)
                {
                    //               Hashtable tmp = tmpArr[i];
                    UserHrvRecord tmp = tmpArr[i];
                    if (tmp != null)
                    {
                        string strType = "";

                        if (tmp.RecordType == 61)
                            strType = "荷韵";
                        else if (tmp.RecordType == 62)
                            strType = "梅花";
                        else if (tmp.RecordType == 63)
                            strType = "丝绸之路";
                        else if (tmp.RecordType == 64)
                            strType = "菩提树";
                        else if (tmp.RecordType == 65)
                            strType = "生命之泉";
                        else if (tmp.RecordType == 66)
                            strType = "星空";
                        else if (tmp.RecordType == 41)
                            strType = "挪来移去";
                        else if (tmp.RecordType == 42)
                            strType = "看图绘画";
                        else if (tmp.RecordType == 43)
                            strType = "边缘视力";
                        else if (tmp.RecordType == 44)
                            strType = "多彩球";
                        else if (tmp.RecordType == 45)
                            strType = "方向瞬记";
                        else if (tmp.RecordType == 46)
                            strType = "以此类推";
                        else if (tmp.RecordType == 70)
                            strType = "神笔马良";
                        else if (tmp.RecordType == 71)
                            strType = "冒险岛";
                        else if (tmp.RecordType == 72)
                            strType = "射箭";
                        else if (tmp.RecordType == 20)
                            strType = "情境仿真";
                        else
                            strType = tmp.RecordType.ToString();
                        dataGridSource.Add(new HRVRecordData()
                        {
                            Id = tmp.Id,
                            Checked = false,
                            Index = ((page - 1) * num + i + 1).ToString(),
                            //Type = tmp.RecordType.ToString(),
                            Type = strType,
                            StartTime = tmp.StartTime.ToString(),
                            TotalTime = MathTime((int)tmp.TimeLength),
                            HrvScore = Convert.ToInt32(tmp.Score).ToString(),
                            Adjust = Convert.ToInt32(tmp.Adjust).ToString(),
                            Stable = Convert.ToInt32(tmp.Stable).ToString(),
                            Pressure = Convert.ToInt32(tmp.Pressure).ToString(),
                            TotalScore = tmp.HrvScore.ToString()
                        });
                    }
                }

                this.pageText.Content = page.ToString() + " / " + _totalPage.ToString();
                this.TrainDataGrid.DataContext = dataGridSource;
            }
            else
            {
                this.HrvDataGrid.Visibility = Visibility.Hidden;
                this.TrainDataGrid.Visibility = Visibility.Hidden;
                this.RelaxDataGrid.Visibility = Visibility.Visible;

                _totalPage = Math.Ceiling(Convert.ToDouble(RelaxHistoryArr.Count / Convert.ToDouble(_num)));

                this.PreviousButton.IsEnabled = true;
                this.NextButton.IsEnabled = true;
                if (_nowPage == 1)
                {
                    this.PreviousButton.IsEnabled = false;
                }
                if (_nowPage == _totalPage)
                {
                    this.NextButton.IsEnabled = false;
                }
                if (RelaxHistoryArr.Count / Convert.ToDouble(_num) > 1)
                {
                    // this.pageText.Content = "1 / " + _totalPage.ToString();
                    this.GotoPage.IsEnabled = true;
                    this.JumpButton.IsEnabled = true;
                }
                else
                {
                    //this.pageText.Content = "1 / 1";
                    this.GotoPage.IsEnabled = false;
                    this.JumpButton.IsEnabled = false;
                }

                UserHrvRecord[] tmpArr = new UserHrvRecord[num];
                if (RelaxHistoryArr.Count <= num)
                {
                    RelaxHistoryArr.CopyTo(tmpArr);
                }
                else
                {
                    if (Math.Ceiling(Convert.ToDouble(RelaxHistoryArr.Count / Convert.ToDouble(num))) < page)
                    {
                        page -= 1;
                        _totalPage -= 1;
                    }
                    if (page * num > RelaxHistoryArr.Count && page == _totalPage)
                    {
                        int count = RelaxHistoryArr.Count - ((page - 1) * num);
                        RelaxHistoryArr.CopyTo((page - 1) * num, tmpArr, 0, count);
                    }
                    else
                    {
                        RelaxHistoryArr.CopyTo((page - 1) * num, tmpArr, 0, num);
                    }
                }
                List<HRVRecordData> dataGridSource = new List<HRVRecordData>();

                //           for (int i = 0; i < tmpArr.Length; i++)
                for (int i = tmpArr.Length - 1; i >= 0; i--)
                {
                    //               Hashtable tmp = tmpArr[i];
                    UserHrvRecord tmp = tmpArr[i];
                    if (tmp != null)
                    {
                        dataGridSource.Add(new HRVRecordData()
                        {
                            Id = tmp.Id,
                            Checked = false,
                            Index = ((page - 1) * num + i + 1).ToString(),
                           // Type = tmp.RecordType.ToString(),
                            Type = "放松训练",
                            StartTime = tmp.StartTime.ToString(),
                            TotalTime = MathTime((int)tmp.TimeLength),
                            HrvScore = Convert.ToInt32(tmp.Score).ToString(),
                            Adjust = Convert.ToInt32(tmp.Adjust).ToString(),
                            Stable = Convert.ToInt32(tmp.Stable).ToString(),
                            Pressure = Convert.ToInt32(tmp.Pressure).ToString(),
                            TotalScore = tmp.HrvScore.ToString()
                        });
                    }
                }

                this.pageText.Content = page.ToString() + " / " + _totalPage.ToString();
                this.RelaxDataGrid.DataContext = dataGridSource;
            }
        }

        /// <summary>
        /// 下一页点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_nowPage == 1)
            {
                this.PreviousButton.IsEnabled = true;
            }
            _nowPage += 1;
            if (_totalPage == _nowPage)
            {
                this.NextButton.IsEnabled = false;
            }

            OnDataOrPageChanged(_nowPage, _num);
        }
        /// <summary>
        /// 上一页点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_totalPage == _nowPage)
            {
                this.NextButton.IsEnabled = true;
            }
            _nowPage -= 1;
            if (_nowPage == 1)
            {
                this.PreviousButton.IsEnabled = false;
            }
            //this.pageText.Content = _nowPage.ToString() + " / " + _totalPage.ToString();
            OnDataOrPageChanged(_nowPage, _num);
        }

        /// <summary>
        /// 把HashTable中的值转成数字。并向上取整
        /// </summary>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private String ConvertFromHashToString(Hashtable table, String key)
        {
            double tmp = 0;
            String ret = null;
            if (table[key] != DBNull.Value)
            {
                tmp = Convert.ToSingle(table[key]);
                ret = Math.Floor(tmp).ToString();
            }
            return ret;
        }

        private string MathTime(int times)
        {
            int second = times % 60;//取得秒数
            int min = Convert.ToInt32(Math.Floor(times / 60.0f));//取得分钟
            string Content = min + " 分" + second + " 秒";
            return Content;
        }

        /// <summary>
        /// 页面跳转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JumpButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.GotoPage.Text))
            {
                int newPage = Convert.ToInt32(this.GotoPage.Text);
                if (newPage <= _totalPage)
                {
                    this.OnDataOrPageChanged(newPage, _num);
                }
                else
                {
                    PmtsMessageBox.CustomControl1.Show("输入的页码超过最大值。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                }
            }
        }
        /// <summary>
        /// 输入框中只显示数字并且位数小于4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotoPage_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            //屏蔽非法按键
            if (((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || ((e.Key >= Key.D0 && e.Key <= Key.D9))) && txt.Text.Length < 4)
            {
                e.Handled = false;//执行，尚未执行的指令。
            }
            else
            {
                e.Handled = true;//不执行。表示已经执行完毕。
            }
        }

        private void recordbutton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/monitoringrecordsbg.png", UriKind.Relative));
            recordCenterImage.Source = ima;
            recordbutton.IsEnabled = false;
            trainbutton.IsEnabled = true;
            relaxbutton.IsEnabled = true;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            if (_showType != 1)
            {
                _showType = 1;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void trainbutton_Click(object sender, RoutedEventArgs e)
        {

            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/trainrecordbg.png", UriKind.Relative));
            recordCenterImage.Source = ima;
            recordbutton.IsEnabled = true;
            trainbutton.IsEnabled = false;
            relaxbutton.IsEnabled = true;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            if (_showType != 2)
            {
                _showType = 2;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void relaxbutton_Click(object sender, RoutedEventArgs e)
        {

            BitmapImage ima = new BitmapImage(new Uri("/PmtsControlLibrary;component/Image/relaxrecordbg.png", UriKind.Relative));
            recordCenterImage.Source = ima;
            recordbutton.IsEnabled = true;
            trainbutton.IsEnabled = true;
            relaxbutton.IsEnabled = false;
            MediaPlayer p = new MediaPlayer();
            p.Open(new Uri(@"Train/game/Button.mp3", UriKind.Relative));
            p.Play();
            if (_showType != 3)
            {
                _showType = 3;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void recordbutton_MouseMove(object sender, MouseEventArgs e)
        {
            Image img = new Image();  
            BitmapImage bitmapImg = new BitmapImage();  
            bitmapImg.BeginInit();  
            bitmapImg.UriSource = new System.Uri("/PmtsControlLibrary;component/Image/monitoringrecords2.png", UriKind.RelativeOrAbsolute);
            bitmapImg.EndInit();  
            img.Source = bitmapImg;
            recordbutton.Content = img;

            if (_showType != 1)
            {
                _showType = 1;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void recordbutton_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = new Image();
            BitmapImage bitmapImg = new BitmapImage();
            bitmapImg.BeginInit();
            bitmapImg.UriSource = new System.Uri("/PmtsControlLibrary;component/Image/monitoringrecords.png", UriKind.RelativeOrAbsolute);
            bitmapImg.EndInit();
            img.Source = bitmapImg;
            recordbutton.Content = img;

            if (_showType != 1)
            {
                _showType = 1;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void trainbutton_MouseMove(object sender, MouseEventArgs e)
        {
            Image img = new Image();
            BitmapImage bitmapImg = new BitmapImage();
            bitmapImg.BeginInit();
            bitmapImg.UriSource = new System.Uri("/PmtsControlLibrary;component/Image/trainrecord2.png", UriKind.RelativeOrAbsolute);
            bitmapImg.EndInit();
            img.Source = bitmapImg;
            trainbutton.Content = img;

            if (_showType != 2)
            {
                _showType = 2;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void trainbutton_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = new Image();
            BitmapImage bitmapImg = new BitmapImage();
            bitmapImg.BeginInit();
            bitmapImg.UriSource = new System.Uri("/PmtsControlLibrary;component/Image/trainrecord.png", UriKind.RelativeOrAbsolute);
            bitmapImg.EndInit();
            img.Source = bitmapImg;
            trainbutton.Content = img;

            if (_showType != 2)
            {
                _showType = 2;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void relaxbutton_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = new Image();
            BitmapImage bitmapImg = new BitmapImage();
            bitmapImg.BeginInit();
            bitmapImg.UriSource = new System.Uri("/PmtsControlLibrary;component/Image/relaxrecord.png", UriKind.RelativeOrAbsolute);
            bitmapImg.EndInit();
            img.Source = bitmapImg;
            relaxbutton.Content = img;

            if (_showType != 3)
            {
                _showType = 3;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void relaxbutton_MouseMove(object sender, MouseEventArgs e)
        {
            Image img = new Image();
            BitmapImage bitmapImg = new BitmapImage();
            bitmapImg.BeginInit();
            bitmapImg.UriSource = new System.Uri("/PmtsControlLibrary;component/Image/relaxrecord.png", UriKind.RelativeOrAbsolute);
            bitmapImg.EndInit();
            img.Source = bitmapImg;
            relaxbutton.Content = img;

            if (_showType != 3)
            {
                _showType = 3;
                OnDataOrPageChanged(1, _num);
            }
        }

        private void instructions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void export_Click(object sender, RoutedEventArgs e)
        {

        }

        private void screening_Click(object sender, RoutedEventArgs e)
        {

        }

        public Hashtable GetGuestHistoryByID(int SID)
        {
            Hashtable hInfo = new Hashtable();

            UserHrvRecord[] tmp = new UserHrvRecord[1];
            
            if (_showType == 1)
                HrvHistoryArr.CopyTo(SID - 1, tmp, 0, 1);
            else if (_showType == 2)
                TrainHistoryArr.CopyTo(SID - 1, tmp, 0, 1);
            else
                RelaxHistoryArr.CopyTo(SID - 1, tmp, 0, 1);

            UserHrvRecord tmpRecord = tmp[0];
            if (tmpRecord != null)
            {
                 hInfo["fMean"] = tmpRecord.TimeData[0];
                 hInfo["HRVScore"] = tmpRecord.HrvScore;
                 hInfo["score"] = tmpRecord.Score;
                 hInfo["Pressure"] = tmpRecord.Pressure;
                 hInfo["adjust"] = tmpRecord.Adjust;
                 hInfo["stable"] = tmpRecord.Stable;//稳定
                 hInfo["report"] = tmpRecord.Report;//评价报告
                 hInfo["NB"] = tmpRecord.AnsBalance;//神经兴奋性
                 hInfo["StartTime"] = tmpRecord.StartTime;//开始时间
                 hInfo["Time"] = tmpRecord.HRVData.Count / 2.0;
                 hInfo["hrvData"] = tmpRecord.HRVData;
                 hInfo["HRVMark"] = tmpRecord.MarkData;
            }
            return hInfo;
        }

        /// <summary>
        /// 点击记录详情按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpenHistoryDetail(object sender, RoutedEventArgs e)
        {
            Button tmp = (Button)sender;
            int sid = Convert.ToInt32(tmp.Tag);
 //           MessageBox.Show(sid.ToString());
            Hashtable hInfo = GetGuestHistoryByID(sid);

            if (hInfo.Count > 0)
            {
                hd = new HRVDetaile(hInfo["hrvData"] as ArrayList, hInfo, this.mainGrid);
                mark = new Rectangle();
                mark.Fill = Brushes.Transparent;
                mark.Margin = new Thickness();
                this.mainGrid.Children.Add(mark);
                this.mainGrid.Children.Add(hd);
                hd.closeButton.Click += OnCloseDetail;
            }


 /*           Hashtable hInfo = hrvd.GetHistoryByID(sid);
            ArrayList markList = hrvd.GetMarkByID(sid);
            if (hInfo.Count > 0)
            {
                hInfo["HRVMark"] = markList;
                hd = new HRVDetaile(hInfo["hrvData"] as ArrayList, hInfo, this.LayoutRoot);
                mark = new Rectangle();
                mark.Fill = Brushes.Transparent;
                mark.Margin = new Thickness();
                this.LayoutRoot.Children.Add(mark);
                this.LayoutRoot.Children.Add(hd);
                hd.closeButton.Click += OnCloseDetail;
            }
            else
            {

            }
  */ 
        }

        /// <summary>
        /// 关闭详细窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseDetail(object sender, RoutedEventArgs e)
        {
            this.mainGrid.Children.Remove(hd);
            this.mainGrid.Children.Remove(mark);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (PmtsMessageBox.CustomControl1.Show("确定要删除选择？") == PmtsMessageBox.ServerMessageBoxResult.OK)
            {
                Button tmp = (Button)sender;
                ArrayList para = new ArrayList();
                para.Add(tmp.Tag);
                hrvdb.DeleteHrvData(para);
                
                for (int i = 0; i < HrvHistoryArr.Count; i++)
                {
                    string tempstring = (i+1).ToString();
                    if (tempstring.Equals(tmp.Tag.ToString()))
                    {
                        HrvHistoryArr.RemoveAt(i);
                        break;
                    }
                }
                OnDataOrPageChanged(_nowPage, _num);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ArrayList dd = new ArrayList();
            for (int i = 0; i < this.HrvDataGrid.Items.Count; i++)
            {
                var cntr = HrvDataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                DataGridRow ObjROw = (DataGridRow)cntr;
                if (ObjROw != null)
                {
                    FrameworkElement objElement = HrvDataGrid.Columns[0].GetCellContent(ObjROw);
                    if (objElement != null)
                    {
                        //if (objElement.GetType().ToString().EndsWith("cRUID"))
                        //{
                        System.Windows.Controls.CheckBox objChk = (System.Windows.Controls.CheckBox)objElement;
                        if (objChk.IsChecked == true)
                        {
                            dd.Add(ObjROw.Item);
                        }
                        //}
                    }
                }
            }

            if (HrvDataGrid.SelectedItems.Count > 0)
            {
                int i = HrvDataGrid.SelectedIndex;

                foreach (var item in HrvDataGrid.SelectedItems)
                {
                    MessageBox.Show("1");
                }
            }
            else
            {
                PmtsMessageBox.CustomControl1.Show("请先选中一条数据!");
            }
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            CheckBox objChk = (CheckBox)sender;
            selectAll(HrvDataGrid, objChk.IsChecked);
        }

        private void selectAll(DataGrid grid,bool? isCheck) {
            for (int i = 0; i < grid.Items.Count; i++)
            {
                var cntr = grid.ItemContainerGenerator.ContainerFromIndex(i);
                DataGridRow ObjROw = (DataGridRow)cntr;
                if (ObjROw != null)
                {
                    FrameworkElement objElement = grid.Columns[0].GetCellContent(ObjROw);
                    if (objElement != null)
                    {
                        CheckBox objChk = (CheckBox)objElement;
                        objChk.IsChecked = isCheck;
                    }
                }
            }
        } 
    }

    /// <summary>
    /// DataGrid使用的数据封装
    /// </summary>
    public class HRVRecordData
    {
        public String Id { set; get; }
        public Boolean Checked { set; get; }
        public String Index { set; get; }
        public String Type { set; get; }
        public String StartTime { set; get; }
        public String TotalTime { set; get; }
        public String HrvScore { set; get; }
        public String Adjust { set; get; }
        public String Stable { set; get; }
        public String Pressure { set; get; }
        public String TotalScore { set; get; }
    }

}
