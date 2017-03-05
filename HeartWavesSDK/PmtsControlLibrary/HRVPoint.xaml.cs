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
using System.Windows.Markup;
//using System.IO;
using System.Xml;

namespace PmtsControlLibrary
{
    /// <summary>
    /// HRVPoint.xaml 的交互逻辑
    /// </summary>
    public partial class HRVPoint : UserControl
    {
        private Grid main = new Grid();
        public HRVPoint(string girdStr, Grid MainGrid)
        {
            InitializeComponent();
            main = MainGrid;
            System.IO.StringReader strBuf = new System.IO.StringReader(girdStr);
            XmlReader reader = XmlReader.Create(strBuf);
            Grid tmp = XamlReader.Load(reader) as Grid;
            for (int i = 0; i < tmp.Children.Count; i++)
            {
                UIElement ui = tmp.Children[i];
                //System.Diagnostics.Debug.Write(ui.GetType() + "\n");
                if (typeof(Rectangle) == ui.GetType())
                {
                    Rectangle tmpRect = (Rectangle)ui;
                    if (tmpRect.Tag != null)
                    {
                        if ((String)tmpRect.Tag == "del")
                        {
                            
                            tmp.Children.Remove(ui);
                        }
                        if ((String)tmpRect.Tag == "ColorChange")
                        {
                            ((Rectangle)ui).Fill = Brushes.White;
                        }
                    }
                }
                if (typeof(Label) == ui.GetType())
                {
                    ((Label)ui).Foreground = Brushes.Black;
                }
                if (typeof(TextBlock) == ui.GetType())
                {
                    ((TextBlock)ui).Foreground = Brushes.Black;
                }
                if (typeof(Path) == ui.GetType())
                {
                    if (((Path)ui).Tag != null)
                    {
                        if (((Path)ui).Tag.ToString() == "ColorChange")
                        {
                            ((Path)ui).Fill = Brushes.Gray;
                        }
                    }
                }
                if (typeof(Grid) == ui.GetType())
                {
                    Grid tmpGrid = (Grid)ui;
                    for (int n = 0; n < tmpGrid.Children.Count; n++)
                    {
                        if (typeof(Path) == tmpGrid.Children[n].GetType())
                        {
                            Path tmpPath = (Path)tmpGrid.Children[n];
                            if (tmpPath.Tag != null)
                            {
                                if ((String)tmpPath.Tag == "HrvChart")
                                {
                                    ((Path)tmpGrid.Children[n]).Stroke = Brushes.Black;
                                }
                                if ((String)tmpPath.Tag == "HrvLine")
                                {
                                    ((Path)tmpGrid.Children[n]).Stroke = Brushes.Black;
                                }
                            }
                        }
                        if (typeof(Rectangle) == tmpGrid.Children[n].GetType())
                        {
                            if (((Rectangle)tmpGrid.Children[n]).Tag != null)
                            {
                                if (((Rectangle)tmpGrid.Children[n]).Tag.ToString() == "HrvBlack")
                                {
                                    ((Rectangle)tmpGrid.Children[n]).Fill = Brushes.White;
                                }
                            }
                        }
                    }
                }
            }
            tmp.Margin = new Thickness(0, 0, 0, 0);
            
            FixedDocument fd = new FixedDocument();//文档
            PageContent pc = new PageContent();//文档中页面的合集
            FixedPage fp = new FixedPage();//单独的页面
            tmp.Margin = new Thickness(100, 20, 0, 0);
            fp.Children.Add(tmp);
            pc.Child = fp;
            fd.Pages.Add(pc);
            this.preview.Document = fd;
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.Write("MainGrid中Children的数量" + main.Children.Count + "\n");
            main.Children.Remove(this);
            
            System.Diagnostics.Debug.Write("MainGrid中Children的数量" + main.Children.Count + "\n");
        }
    }
}
