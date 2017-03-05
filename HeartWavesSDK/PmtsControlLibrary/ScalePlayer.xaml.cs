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
using PmtsControlLibrary.DBPlugin;

namespace PmtsControlLibrary
{
    /// <summary>
    /// ScalePlayer.xaml 的交互逻辑
    /// </summary>
    public partial class ScalePlayer : UserControl
    {
        private ScaleTableDB stDB = null;
        private int isShowResult = 0;//是否显示结果
        private String playPath = "";//播放路径
        private int scaleTableID = 0;//量表ID
        private System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        private AxShockwaveFlashObjects.AxShockwaveFlash shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();

        public ScalePlayer()
        {
            InitializeComponent();
            stDB = new ScaleTableDB();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Grid main = (Grid)this.ScalePlayerView.Parent;
            main.Children.Clear();
            Grid mainParent = (Grid)main.Parent;
            for (int i = 0; i < mainParent.Children.Count; i++)
            {
                mainParent.Children[i].Visibility = System.Windows.Visibility.Visible;
            }
        }
        /// <summary>
        /// 显示隐藏量表列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.ScaleListGrid.Visibility == System.Windows.Visibility.Hidden)
            {
                double left = this.Margin.Left - 80;
                this.Margin = new Thickness(left, 0, 0, 0);
                this.ScaleListGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                double left = this.Margin.Left + 80;
                this.Margin = new Thickness(left, 0, 0, 0);
                this.ScaleListGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        /// <summary>
        /// 列表载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScaleListGrid_Loaded(object sender, RoutedEventArgs e)
        {
            ArrayList stList = stDB.GetScaleTableList();
            for (int i = 0; i < stList.Count; i++)
            {
                Hashtable stInfo = (Hashtable)stList[i];
                TreeViewItem tvi = new TreeViewItem();
                tvi.Width = stInfo["scaleName"].ToString().Length * 15;
                tvi.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tvi.Margin = new Thickness(5, 5, 0, 0);
                tvi.Template = (ControlTemplate)FindResource("ScaleName");
                tvi.Tag = stInfo;
                tvi.Header = stInfo["scaleName"].ToString();
                tvi.Selected += SelectSacleTable;
                this.ScaleListTreeView.Items.Add(tvi);
            }
        }
        /// <summary>
        /// 选中量表状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSacleTable(object sender, RoutedEventArgs e)
        {
            Hashtable info = (Hashtable)((TreeViewItem)sender).Tag;
            this.isShowResult = Convert.ToInt32(info["resultIsShow"]);
            this.scaleTableID = Convert.ToInt32(info["sacleID"]);
            this.playerPanel.Children.Clear();
            this.resultShow.Visibility = System.Windows.Visibility.Visible;
            this.resultListBorder.Visibility = System.Windows.Visibility.Visible;
            this.historyDetail.Visibility = System.Windows.Visibility.Hidden;
            this.ShowScaleTableName.Text = info["scaleName"].ToString();
            this.playPath = System.Environment.CurrentDirectory + "\\ScaleTable\\" + info["sacleID"] + "\\scale.swf";
            CreateReslutList(scaleTableID);
        }
        /// <summary>
        /// 创建结果列表
        /// </summary>
        private void CreateReslutList(int stID)
        {
            this.showResultList.Children.Clear();
            ArrayList rList = stDB.GetResultList(stID);
            int row = rList.Count;
            if (row < 13)
            {
                row = 13;
            }
            for (int i = 0; i < row; i++)
            {
                /*绘制表格样式*/
                Grid mainG = new Grid();
                mainG.Height = 25;
                mainG.Width = 370;
                if (i < row - 1)
                {
                    Rectangle rectBottom = new Rectangle();
                    rectBottom.Height = 1;
                    rectBottom.Width = 366;
                    rectBottom.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    rectBottom.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    rectBottom.Margin = new Thickness(2, 0, 2, 0);
                    mainG.Children.Add(rectBottom);
                }
                Rectangle rectLeft = new Rectangle();
                rectLeft.Width = 1;
                rectLeft.Height = 25;
                rectLeft.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                rectLeft.Margin = new Thickness(49.5, 0, 0, 0);
                mainG.Children.Add(rectLeft);
                Rectangle rectRight = new Rectangle();
                rectRight.Width = 1;
                rectRight.Height = 25;
                rectRight.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                rectRight.Margin = new Thickness(249.5, 0, 0, 0);
                mainG.Children.Add(rectRight);
                /*添加内容*/
                if (i < rList.Count)
                {
                    Hashtable info = (Hashtable)rList[i];
                    Grid g1 = new Grid();
                    g1.Width = 50;
                    g1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    g1.Margin = new Thickness(0, 0, 0, 0);
                    TextBlock t1 = new TextBlock();
                    t1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    t1.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    t1.Text = (i + 1).ToString();
                    g1.Children.Add(t1);

                    Grid g2 = new Grid();
                    g2.Width = 200;
                    g2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    g2.Margin = new Thickness(50, 0, 0, 0);
                    TextBlock t2 = new TextBlock();
                    t2.Text = info["time"].ToString();
                    t2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    t2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    g2.Children.Add(t2);

                    Grid g3 = new Grid();
                    g3.Width = 120;
                    g3.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    g3.Margin = new Thickness(0, 0, 0, 0);
                    Button bun = new Button();
                    bun.Content = "详情";
                    bun.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    bun.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    bun.Click += CreateHistory;
                    bun.Tag = info;
                    g3.Children.Add(bun);


                    mainG.Children.Add(g1);
                    mainG.Children.Add(g2);
                    mainG.Children.Add(g3);
                }
                this.showResultList.Children.Add(mainG);
            }

        }
        /// <summary>
        /// 创建历史详情页面
        /// </summary>
        private void CreateHistory(object sender, RoutedEventArgs e)
        {
            this.resultDetailGrid.Children.Clear();
            this.resultDetailGrid.RowDefinitions.Clear();
            this.resultExplanationGrid.Children.Clear();
            this.resultExplanationGrid.RowDefinitions.Clear();
            this.resultStandardGrid.Children.Clear();
            this.resultStandardGrid.RowDefinitions.Clear();

            Button tmp = (Button)sender;
            Hashtable scaleInfo = (Hashtable)tmp.Tag;
            String xmlStr = scaleInfo["xml"].ToString();
            xmlStr = ((xmlStr.Replace("&lt;", "<")).Replace("&gt;", ">")).Replace("&quot;", "\"");
            Hashtable node = this.NodeXml(xmlStr);
            this.TextScaleTime.Text = scaleInfo["time"].ToString();
            this.TextName.Text = UserInfoStatic.UserName;
            this.TextAge.Text = UserInfoStatic.UserAge.ToString();
            this.TextSex.Text = UserInfoStatic.UserSex;
            this.TextWorkYear.Text = UserInfoStatic.UserWorkYear.ToString();
            this.TextWorkType.Text = UserInfoStatic.UserWorkType;
            this.TextScaleName.Text = node["scaleName"].ToString();
            this.TextScaleOutline.Text = node["intro"].ToString();
            this.TextScaleNum.Text = node["scaleNum"].ToString() + "道题";
            ArrayList dimList = (ArrayList)node["dimList"];
            
            Rectangle r1 = new Rectangle();
            r1.Margin = new Thickness(0, 1, 0, 1);
            Grid.SetColumn(r1, 1);
            Grid.SetRowSpan(r1, (dimList.Count * 2) - 1);
            this.resultDetailGrid.Children.Add(r1);
            Rectangle r2 = new Rectangle();
            r2.Margin = new Thickness(0, 1, 0, 1);
            Grid.SetColumn(r2, 1);
            Grid.SetRowSpan(r2, (dimList.Count * 2) - 1);
            this.resultExplanationGrid.Children.Add(r2);
            for (int i = 0; i < dimList.Count; i++)
            {
                ///添加测试结果
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(30);
                this.resultDetailGrid.RowDefinitions.Add(rd);
                Hashtable info = (Hashtable)dimList[i];
                TextBlock tbName = new TextBlock();
                tbName.Text = info["resultName"].ToString();
                Grid.SetRow(tbName, i * 2);
                Grid.SetColumn(tbName, 0);
                this.resultDetailGrid.Children.Add(tbName);

                TextBlock tbResult = new TextBlock();
                tbResult.Text = "分数：" + info["resultNorm"].ToString();
                tbResult.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                Grid.SetRow(tbResult, i * 2);
                Grid.SetColumn(tbResult, 2);
                this.resultDetailGrid.Children.Add(tbResult);
                if (i < dimList.Count - 1)
                {
                    RowDefinition rectRD = new RowDefinition();
                    rectRD.Height = new GridLength(1);
                    this.resultDetailGrid.RowDefinitions.Add(rectRD);
                    Rectangle r = new Rectangle();
                    r.Height = 1;
                    Grid.SetColumnSpan(r, 3);
                    Grid.SetRow(r, (i * 2) + 1);
                    this.resultDetailGrid.Children.Add(r);
                }
                ///添加结果解释
                RowDefinition rd1 = new RowDefinition();
                rd1.Height = new GridLength(30);
                this.resultExplanationGrid.RowDefinitions.Add(rd1);
                
                TextBlock tbExpName = new TextBlock();
                tbExpName.Text = info["resultName"].ToString();
                Grid.SetRow(tbExpName, i * 2);
                Grid.SetColumn(tbExpName, 0);
                this.resultExplanationGrid.Children.Add(tbExpName);

                TextBlock tbExpContent = new TextBlock();
                tbExpContent.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                String resLevel = info["resultLevel"].ToString();
                String[] levelList = resLevel.Split(new char[] { ':' });
                tbExpContent.Text = "级别：" + levelList[0];
                if (levelList.Length > 1)
                {
                    tbExpContent.Text += "\n说明：" + levelList[1];
                }
                Grid.SetRow(tbExpContent, i * 2);
                Grid.SetColumn(tbExpContent, 2);
                this.resultExplanationGrid.Children.Add(tbExpContent);
                if (i < dimList.Count - 1)
                {
                    RowDefinition rectRD1 = new RowDefinition();
                    rectRD1.Height = new GridLength(1);
                    this.resultExplanationGrid.RowDefinitions.Add(rectRD1);
                    Rectangle rExp = new Rectangle();
                    rExp.Height = 1;
                    Grid.SetColumnSpan(rExp, 3);
                    Grid.SetRow(rExp, (i * 2) + 1);
                    this.resultExplanationGrid.Children.Add(rExp);
                }
            }
            /*添加规则解释*/
            ArrayList ruleList = (ArrayList)node["rule"];
            Rectangle r3 = new Rectangle();
            r3.Margin = new Thickness(0, 1, 0, 1);
            Grid.SetColumn(r3, 1);
            Grid.SetRowSpan(r3, (ruleList.Count * 2) - 1);
            this.resultStandardGrid.Children.Add(r3);
            for (int r = 0; r < ruleList.Count; r++)
            {
                Hashtable ruleInfo = (Hashtable)ruleList[r];
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(30, System.Windows.GridUnitType.Auto);
                this.resultStandardGrid.RowDefinitions.Add(rd);
                TextBlock tbName = new TextBlock();
                tbName.Text = ruleInfo["ruleName"].ToString();
                Grid.SetColumn(tbName, 0);
                Grid.SetRow(tbName, r * 2);
                this.resultStandardGrid.Children.Add(tbName);

                ArrayList ruleLevels = (ArrayList)ruleInfo["ruleLevels"];
                String ruleStr = "";
                ruleStr = "维度说明："+ruleInfo["ruleExp"] + "\n";
                for (int rl = 0; rl<ruleLevels.Count; rl++)
                {
                    Hashtable ruleTmp = (Hashtable)ruleLevels[rl];
                    ruleStr += "分数在" + Convert.ToDouble(ruleTmp["low"]).ToString();
                    ruleStr += "和" + Convert.ToDouble(ruleTmp["high"]).ToString();
                    ruleStr += "之间，" + ruleTmp["scoreLevel"];
                    if (rl != ruleLevels.Count-1)
                    {
                        ruleStr += "\n";
                    }
                }
                TextBlock tbRule = new TextBlock();
                tbRule.Text = ruleStr;
                tbRule.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tbRule.TextWrapping = TextWrapping.Wrap;
                Grid.SetRow(tbRule, r * 2);
                Grid.SetColumn(tbRule, 2);
                this.resultStandardGrid.Children.Add(tbRule);
                if (r < ruleList.Count - 1)
                { 
                    RowDefinition rectRD = new RowDefinition();
                    rectRD.Height = new GridLength(1);
                    this.resultStandardGrid.RowDefinitions.Add(rectRD);
                    Rectangle rect = new Rectangle();
                    rect.Height = 1;
                    Grid.SetColumnSpan(rect, 3);
                    Grid.SetRow(rect, (r * 2) + 1);
                    this.resultStandardGrid.Children.Add(rect);
                }
            }
            this.resultImage.Height = 200;
            
            this.resultListBorder.Visibility = System.Windows.Visibility.Hidden;
            this.historyDetail.Visibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// 返回历史列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReturnToList(object sender, RoutedEventArgs e)
        {
            this.historyDetail.Visibility = System.Windows.Visibility.Hidden;
            this.resultListBorder.Visibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartPlay_Click(object sender, RoutedEventArgs e)
        {
            if (playPath != "" || !String.IsNullOrEmpty(playPath))
            {
                host = new System.Windows.Forms.Integration.WindowsFormsHost();
                shockwave = new AxShockwaveFlashObjects.AxShockwaveFlash();
                host.Child = shockwave;
                this.playerPanel.Children.Add(host);

                // 设置 .swf 文件相对路径
                shockwave.Movie = playPath;
                shockwave.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(FromFlashCall);
                shockwave.Loop = false;
                this.ScaleListTreeView.IsEnabled = false;

                String cmd = "<invoke name=\"c2flash\" returntype=\"xml\"><arguments><string>";
                if (UserInfoStatic.UserSex == "男")
                {
                    cmd += "men";
                }
                else
                {
                    cmd += "women";
                }

                cmd += "</string></arguments></invoke>";

                shockwave.CallFunction(cmd);
                shockwave.Play();
                host.Unloaded += OnRemoveFlash;
                ///隐藏量表列表
                if (this.ScaleListGrid.Visibility == System.Windows.Visibility.Hidden)
                {
                    double left = this.Margin.Left - 80;
                    this.Margin = new Thickness(left, 0, 0, 0);
                    this.ScaleListGrid.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    double left = this.Margin.Left + 80;
                    this.Margin = new Thickness(left, 0, 0, 0);
                    this.ScaleListGrid.Visibility = System.Windows.Visibility.Hidden;
                }

            }
        }
        /// <summary>
        /// 移除Flash时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveFlash(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Write("移除flash\n");
            System.Windows.Forms.Integration.WindowsFormsHost tmp = (System.Windows.Forms.Integration.WindowsFormsHost)sender;
            tmp.Dispose();
        }
        /// <summary>
        /// 量表结束后返回结果的响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FromFlashCall(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            System.Diagnostics.Debug.Write("解析nodeXml：" + NodeXml(e.request.ToString()) + "\n");
            String xmlStr = e.request.ToString();
            xmlStr = ((xmlStr.Replace("&lt;", "<")).Replace("&gt;", ">")).Replace("&quot;", "\"");
            int resultID = stDB.GetReslutID();
            stDB.OnSaveSacleTableReslut(xmlStr, scaleTableID, isShowResult, resultID);
            this.ScaleListTreeView.IsEnabled = true;
            this.playerPanel.Children.Clear();
            CreateReslutList(scaleTableID);
        }
        /// <summary>
        /// 解析返回的XML
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Hashtable NodeXml(string s)
        {
            s = ((s.Replace("&lt;", "<")).Replace("&gt;", ">")).Replace("&quot;", "\"");
            Hashtable result = new Hashtable();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(s);
                /*解析题目原始信息*/
                XmlNode raw = (((((doc.SelectSingleNode("invoke"))).SelectSingleNode("arguments")).SelectSingleNode("string")).SelectSingleNode("result")).SelectSingleNode("raw");
                result["scaleNum"] = raw.ChildNodes.Count;

                /*解析结果解释和基本信息部分*/
                XmlNodeList rule = doc.GetElementsByTagName("base");//基本部分
                result["scaleName"] = ((XmlElement)rule[0]).GetElementsByTagName("names")[0].InnerText;//量表名称
                result["intro"] = ((XmlElement)rule[0]).GetElementsByTagName("intro")[0].InnerText;//量表概述
                XmlNodeList explain = ((XmlElement)rule[0]).GetElementsByTagName("column");
                ArrayList ruleList = new ArrayList();
                for (int n = 0; n < explain.Count; n++)
                {
                    Hashtable ruleTmp = new Hashtable();
                    String ruleKey = explain[n].Attributes.GetNamedItem("id").Value;
                    int ruleNum = explain[n].Attributes.Count;
                    if (ruleNum>1)
                    {
                        String ruleExp = explain[n].Attributes.GetNamedItem("Explain").Value;
                        ruleTmp["ruleExp"] = ruleExp;
                    }
                    else
                    {
                        ruleTmp["ruleExp"] = "";
                    }
                    
                    ruleTmp["ruleName"] = ruleKey;
                    String[] ruleContents = explain[n].InnerText.Split(new char[] { '|' });
                    if (ruleContents.Length >= 3)
                    {
                        ArrayList tmpList = new ArrayList();
                        for (int rc = 0; rc < ruleContents.Length; rc++)
                        {
                            Hashtable nums = new Hashtable();
                            String lowNum = (ruleContents[rc].Split(new char[] { ',' }))[0];
                            String highNum = (ruleContents[rc].Split(new char[] { ',' }))[1];
                            nums["low"] = lowNum;
                            nums["high"] = highNum;
                            nums["scoreLevel"] = (ruleContents[rc].Split(new char[] { ',' }))[2];
                            tmpList.Add(nums);
                        }
                        ruleTmp["ruleLevels"] = tmpList;
                    }
                    else
                    {
                        ruleTmp["ruleLevels"] = new ArrayList();
                    }
                    ruleList.Add(ruleTmp);
                }
                result["rule"] = ruleList;
                /*解析结果部分*/
                XmlNodeList results = doc.GetElementsByTagName("resultList");
                XmlNodeList resultList = ((XmlElement)results[0]).GetElementsByTagName("column");//取得结果的详细解释
                XmlNodeList resultTotal = ((XmlElement)results[0]).GetElementsByTagName("total");//取得总得分
                ArrayList Dimensions = new ArrayList();
                for (int i = 0; i < resultList.Count; i++)
                {
                    Hashtable tmp = new Hashtable();
                    System.Xml.XmlElement xe = (System.Xml.XmlElement)resultList[i];
                    tmp["resultName"] = xe.GetElementsByTagName("names")[0].InnerText;
                    tmp["resultRaw"] = xe.GetElementsByTagName("raw")[0].InnerText;
                    tmp["resultNorm"] = xe.GetElementsByTagName("norm")[0].InnerText;
                    if (xe.ChildNodes.Count > 3)
                    {
                        tmp["resultLevel"] = xe.GetElementsByTagName("level")[0].InnerText;
                    }
                    else
                    {
                        tmp["resultLevel"] = "-";
                    }
                    Dimensions.Add(tmp);
                }
                result["dimList"] = Dimensions;
                result["totalRaw"] = ((XmlElement)(resultTotal[0])).GetElementsByTagName("raw")[0].InnerText;
                result["totalNorm"] = ((XmlElement)(resultTotal[0])).GetElementsByTagName("norm")[0].InnerText;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("解析课程返回数据时出错：" + ex.Message + "\n");
            }
            return result;
        }

    }
}
