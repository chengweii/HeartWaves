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
using PmtsControlLibrary.DBPlugin;
using PmtsControlLibrary.WEBPlugin;

namespace PmtsControlLibrary
{
    /// <summary>
    /// HRVHistory.xaml 的交互逻辑
    /// </summary>
    public partial class HRVHistory : UserControl
    {
        private ArrayList historyArr = new ArrayList();
        private HRVControlWEB hrvd = null;
        private int _nowPage = 1;//当前页号
        private int _num = 15;//一页默认数据量
        private double _totalPage = 0;//总页数
        private HRVDetaile hd = null;//历史记录详情
        private Rectangle mark = null;//屏蔽层


        public HRVHistory(Hashtable SystemMeg)
        {
            InitializeComponent();
            historyArr   = new ArrayList();
            if (hrvd == null)
            {
                hrvd = new HRVControlWEB(SystemMeg);
            }
            GetHrvHistoryData();
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
        /// 从数据库中取得数据
        /// </summary>
        private void GetHrvHistoryData()
        {
            historyArr = hrvd.GetConstAndHistoryListData(0,0,"1","0");
            _totalPage = Math.Ceiling(Convert.ToDouble(historyArr.Count / Convert.ToDouble(_num)));
            if (historyArr.Count / Convert.ToDouble(_num) > 1)
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
            OnDataOrPageChanged(1,_num);
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
            Hashtable hInfo = hrvd.GetHistoryByID(sid);
            ArrayList markList = hrvd.GetMarkByID(sid);
            if (hInfo.Count > 0)
            {
                hInfo["HRVMark"] = markList;
                hd = new HRVDetaile(hInfo["hrvData"] as ArrayList,hInfo,this.LayoutRoot);
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
        }
        /// <summary>
        /// 关闭详细窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseDetail(object sender, RoutedEventArgs e)
        {
            this.LayoutRoot.Children.Remove(hd);
            this.LayoutRoot.Children.Remove(mark);
        }
        /// <summary>
        /// 点击删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (PmtsMessageBox.CustomControl1.Show("确定要删除选择？") == PmtsMessageBox.ServerMessageBoxResult.OK)
            {
                Button tmp = (Button)sender;
                ArrayList para = new ArrayList();
                para.Add(tmp.Tag);
                hrvd.DeleteHrvData(para);
                for (int i = 0; i < historyArr.Count; i++)
                {
                    String scaleIndex = ConvertFromHashToString((Hashtable)historyArr[i], "id");
                    if (scaleIndex.Equals(tmp.Tag.ToString()))
                    {
                        historyArr.RemoveAt(i);
                        break;
                    }
                }
                OnDataOrPageChanged(_nowPage, _num);
            }
        }
        /// <summary>
        /// 把HashTable中的值转成数字。并向上取整
        /// </summary>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private String ConvertFromHashToString(Hashtable table,String key)
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
        /// 分页处理
        /// </summary>
        /// <param name="page">当前的页号</param>
        /// <param name="num">每页的条数</param>
        private void OnDataOrPageChanged(int page,int num)
        {
            Hashtable[] tmpArr = new Hashtable[num];
            if (historyArr.Count <= num)
            {
                historyArr.CopyTo(tmpArr);
            }
            else
            {
                if (Math.Ceiling(Convert.ToDouble(historyArr.Count / Convert.ToDouble(num))) < page)
                {
                    page -= 1;
                    _totalPage -= 1;
                }
                if (page  * num > historyArr.Count && page==_totalPage)
                {
                    int count = historyArr.Count - ((page - 1) * num);
                    historyArr.CopyTo((page - 1) * num, tmpArr, 0, count);
                }
                else
                {
                    historyArr.CopyTo((page - 1) * num, tmpArr, 0, num);
                }
            }
            List<HRVHistoryData> dataGridSource = new List<HRVHistoryData>();
            for (int i = 0; i < tmpArr.Length; i++)
            {
                Hashtable tmp = tmpArr[i];
                if (tmp != null)
                {
                    dataGridSource.Add(new HRVHistoryData()
                    {
                        Checked = false,
                        Index = ConvertFromHashToString(tmp, "id"),
                        MHRT = ConvertFromHashToString(tmp, "mhrt"),
                        Adjust = ConvertFromHashToString(tmp, "adjust"),
                        Stable = ConvertFromHashToString(tmp, "stable"),
                        Pressure = ConvertFromHashToString(tmp, "pressure"),
                        HrvScore = ConvertFromHashToString(tmp, "hrvScore"),
                        TotalScore = ConvertFromHashToString(tmp, "totalScore"),
                        TotalTime = MathTime(Convert.ToInt32(ConvertFromHashToString(tmp, "totalTime"))),
                        StartTime = tmp["startTime"].ToString()
                    });
                }
            }
            this.pageText.Content = page.ToString() + " / " + _totalPage.ToString();
            this.showDataGrid.DataContext = dataGridSource;
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
        /// 全部选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            List<HRVHistoryData> dataGridSource = new List<HRVHistoryData>();
            dataGridSource = (List<HRVHistoryData>)this.showDataGrid.DataContext;
            for (int i = 0; i < dataGridSource.Count;i++ )
            {
                dataGridSource[i].Checked = true;
            }
            //this.showDataGrid.DataContext = null;
            this.showDataGrid.DataContext = dataGridSource;
            this.showDataGrid.Items.Refresh();
        }
        /// <summary>
        /// 全部取消选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<HRVHistoryData> dataGridSource = new List<HRVHistoryData>();
            dataGridSource = (List<HRVHistoryData>)this.showDataGrid.DataContext;
            for (int i = 0; i < dataGridSource.Count; i++)
            {
                dataGridSource[i].Checked = false;
            }
           // this.showDataGrid.DataContext = dataGridSource;
            this.showDataGrid.Items.Refresh();
        }
        /// <summary>
        /// 批量删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (PmtsMessageBox.CustomControl1.Show("确定要删除选择？") == PmtsMessageBox.ServerMessageBoxResult.OK)
            {
                List<HRVHistoryData> dataGridSource = new List<HRVHistoryData>();
                dataGridSource = (List<HRVHistoryData>)this.showDataGrid.DataContext;
                ArrayList delArr = new ArrayList();
                for (int i = 0; i < dataGridSource.Count; i++)
                {
                    if (dataGridSource[i].Checked)
                    {
                        delArr.Add(dataGridSource[i].Index);
                        for (int n = 0; n < historyArr.Count; n++)
                        {
                            String selectIndex = ConvertFromHashToString((Hashtable)historyArr[n], "id");
                            if (selectIndex.Equals(dataGridSource[i].Index))
                            {
                                historyArr.RemoveAt(n);
                                break;
                            }
                        }
                    }
                }
                OnDataOrPageChanged(_nowPage, _num);
                hrvd.DeleteHrvData(delArr);
            }
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
        /// <summary>
        /// 到处Excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HrvStartButton_Click(object sender, RoutedEventArgs e)
        {
             Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")+".xls"; // Default file name
            dlg.DefaultExt = ".xls"; // Default file extension
            dlg.Filter = "Excel文件 (.xls)|*.xls"; // Filter files by extension
            if (dlg.ShowDialog() == true)
            {
                String fileName = dlg.FileName;
/////////////-----------------------------------------------------开始写入Excel-------------------------------------------------------------------////////////////
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp == null)
                {
                    PmtsMessageBox.CustomControl1.Show("无法创建Excel对象，可能您的电脑中未安装Excel程序！", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    return;
                }
                //Workbook集合
                Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
                //Workbook
                Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                //WorkSheet
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得 sheet1
                worksheet.Name = "HRV测量历史记录";
                try
                {
                    //标题
                    for (int h = 1; h < this.showDataGrid.Columns.Count; h++)
                    {
                        worksheet.Cells[1, h] = this.showDataGrid.Columns[h].Header;
                        Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, h];
                        range.Font.Name = "宋体";//字体
                        range.Font.Size = 14;//字体大小
                        range.Font.Bold = true;//是否加粗
                    }
                    worksheet.Columns.EntireColumn.AutoFit();//列宽自适应。
                    //内容
                    for (int i = 0; i < this.historyArr.Count; i++)
                    {
                        Hashtable tmp = (Hashtable)this.historyArr[i];
                        for (int c = 0; c < this.showDataGrid.Columns.Count; c++)
                        {
                            switch (c)
                            {
                                case 0:
                                    worksheet.Cells[i + 2, c + 1] = tmp["id"];
                                    break;
                                case 1:
                                    worksheet.Cells[i + 2, c + 1] = tmp["startTime"];
                                    break;
                                case 2:
                                    worksheet.Cells[i + 2, c + 1] = tmp["totalTime"];
                                    break;
                                case 3:
                                    worksheet.Cells[i + 2, c + 1] = tmp["mhrt"];
                                    break;
                                case 4:
                                    worksheet.Cells[i + 2, c + 1] = tmp["adjust"];
                                    break;
                                case 5:
                                    worksheet.Cells[i + 2, c + 1] = tmp["stable"];
                                    break;
                                case 6:
                                    worksheet.Cells[i + 2, c + 1] = tmp["pressure"];
                                    break;
                                case 7:
                                    worksheet.Cells[i + 2, c + 1] = tmp["totalScore"];
                                    break;
                                case 8:
                                    worksheet.Cells[i + 2, c + 1] = tmp["hrvScore"];
                                    break;
                            }
                            Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[i+2, c+1];
                            range.Font.Name = "宋体";//字体
                            range.Font.Size = 12;//字体大小
                        }
                    }
                        
                    workbook.Saved = true;
                    workbook.SaveCopyAs(fileName);//保存复制到指定位置
                }
                catch (Exception ex)
                {
                    PmtsMessageBox.CustomControl1.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message, PmtsMessageBox.ServerMessageBoxButtonType.OK);
                }
                finally
                {
                    workbooks.Close();
                    xlApp.Quit();
                    GC.Collect();//强行销毁
                }
            }
        }
    }
    /// <summary>
    /// DataGrid使用的数据封装
    /// </summary>
    public class HRVHistoryData
    {
        public Boolean Checked { set; get; }
        public String Index { set; get; }
        public String StartTime { set; get; }
        public String TotalTime { set; get; }
        public String MHRT { set; get; }
        public String Adjust { set; get; }
        public String Stable { set; get; }
        public String Pressure { set; get; }
        public String HrvScore { set; get; }
        public String TotalScore { set; get; }
    }
}
