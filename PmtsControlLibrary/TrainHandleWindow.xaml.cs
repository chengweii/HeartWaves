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
using System.Windows.Shapes;
using System.Xml;
using pmts_net.HMRead;
using System.Windows.Threading;
using System.Collections;

namespace PmtsControlLibrary
{
    /// <summary>
    /// TrainHandleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TrainHandleWindow : Window
    {

        private Hashtable tInfo = new Hashtable();

        public TrainHandleWindow(Hashtable t)
        {
            InitializeComponent();

            tInfo = t;
            if (Convert.ToInt32(tInfo["tid"]) == 30)
            {
                //神笔马良
            }
            else if (Convert.ToInt32(tInfo["tid"]) == 31)
            {
                //冒险岛
            }
            else if (Convert.ToInt32(tInfo["tid"]) == 32)
            {
                //射箭
            }



        }

    }

        
}
