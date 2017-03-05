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
    /// ClassTopView.xaml 的交互逻辑
    /// </summary>
    public partial class ClassTopView : UserControl
    {
        private Image _clasTopImage = new Image();
        private TextBlock _relax = new TextBlock();
        private TextBlock _relax1 = new TextBlock();
        private TextBlock _relax2 = new TextBlock();
        private TextBlock _relax3 = new TextBlock();
        private TextBlock _relax4 = new TextBlock();
       
        public ClassTopView()
        {
            InitializeComponent();
            _clasTopImage = this.clasTopImage;
            _relax = this.RelaxNameText;
            _relax1 = this.RelaxNameText1;
            _relax2 = this.RelaxNameText2;
            _relax3 = this.RelaxNameText3;
            _relax4 = this.RelaxNameText4;
           
        }
    }
}
