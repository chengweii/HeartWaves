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
using pmts_net.Plugin;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using PmtsControlLibrary;
using System.Xml.Serialization;
using System.IO;
using pmts_net;
using System.Runtime.InteropServices;
using HeartWavesSDK.API;
using HeartWavesSDK.Model;

namespace pmts_net.AppWindow
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : System.Windows.Window
    {
        [DllImport("Kernel32.dll")]
        public static extern int WinExec(string strExe, int otherType);

        private bool isFinish = true;
        //private DB_Conn mDB = null;
        private bool isReceive = true;
        private DispatcherTimer timeOut = new DispatcherTimer();

        private TcpClient hrvClient = null;
        private Socket socketClient = null;
        private static byte[] protoMessage;
        //       private string protoMessage;

        private byte[] result = new byte[1024];
        //private p

        private NodeXml objNodeXml = new NodeXml();

        public override void BeginInit()
        {
            base.BeginInit();
        }

        public Login()
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


            int proNum = 0;
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in ps)
            {
                if (p.ProcessName == "PmtsNetClient")
                {
                    proNum += 1;
                }
            }
            if (proNum > 1)
            {
                PmtsMessageBox.CustomControl1.Show("已经启动一个程序！", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                this.Close();
            }
            else
            {

                //检查硬件
                bool isClose = true;
                HDCheck.HIDD_VIDPID[] HDList = HDCheck.AllVidPid;
                foreach (HDCheck.HIDD_VIDPID hid in HDList)
                {
                    if (hid.VendorID == 4292 && hid.ProductID == 1)
                    {
                        isClose = false;
                    }
                }

                //bool isClose = false;
                isClose = false;//lich
                if (isClose)
                {
                    PmtsMessageBox.CustomControl1.Show("请先确认设备已经连接！", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    this.Close();
                }
                //开始连接数据库
                // mDB = new DB_Conn();

                String xmlTest = "./Config/UserList.xml";
                try
                {
                    XmlSerializer objSerializer = new XmlSerializer(typeof(NodeXml));
                    Stream objStream = new FileStream(xmlTest, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    objNodeXml = (NodeXml)objSerializer.Deserialize(objStream);
                    objStream.Close();
                    if (objNodeXml.uList.Count > 0)
                    {
                        for (int i = 0; i < objNodeXml.uList.Count; i++)
                        {
                            this.userText.Items.Add(objNodeXml.uList[i].uName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write("初始化登录框时出错：" + ex.Message + "\n");
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

        //线程委托
        delegate void GetIpAddress(String ipStr);
        private Boolean ConnectServer(String ipStr, int portNumber)
        {
            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint ipEnd = new IPEndPoint(ip, portNumber);
            //定义套接字类型
            //            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            hrvClient = new TcpClient();
            try
            {
                //                socketClient.Connect(ipEnd);
                hrvClient.Connect(ipEnd);
                return true;
            }
            //异常处理
            catch (SocketException e)
            {
                Console.WriteLine("Fail to connect server");
                Console.WriteLine(e.ToString());
                return false;
            }
            /*            hrvClient = new TcpClient();
                        IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipStr), portNumber);
                        hrvClient.Connect(ip);

                        if (hrvClient.Connected)
                            return true;
                        else
                            return false;
             */

        }
        private void OnIpChanged(String ipStr)
        {

            System.Diagnostics.Debug.Write("局域网内Mysql服务器的IP地址为：" + ipStr + "\n");
            isFinish = false;
            ipStr = "127.0.0.1";
            DB_Login dbl = new DB_Login(ipStr);
            if (dbl.Select_CheckUser(this.userText.Text, this.pwdText.Password))

            //            if(true)
            {
                //Main mainWindow = new Main(ipStr, this.userText.Text);
                IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipa = ipe.AddressList[0];
                String ipLocal = ipa.ToString();

                if (dbl.IsUserLogin(this.userText.Text))
                //                if(true)
                {

                    dbl.OnUserLogin(this.userText.Text, ipLocal);

                    // dbl.OnUserLogin(this.userText.Text, ipStr);    revise
                    Main mainWindow = new Main(ipStr, this.userText.Text);
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    if (dbl.IsUserLoginWithIp(this.userText.Text, ipLocal))
                    {
                        PmtsMessageBox.CustomControl1.Show("该用户已经登录。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                    }
                    else
                    {
                        Main mainWindow = new Main(ipStr, this.userText.Text);
                        mainWindow.Show();
                        this.Close();
                    }
                }
            }
            else
            {
                PmtsMessageBox.CustomControl1.Show("用户名和密码不正确!", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                isFinish = true;
            }
        }
        private void OnCloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 拖拽事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMoveDrag(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Image img = new Image();
            BitmapImage bitmapImg = new BitmapImage();
            bitmapImg.BeginInit();
            bitmapImg.UriSource = new System.Uri("/PmtsNetClient;component/Image/login2.png", UriKind.RelativeOrAbsolute);
            bitmapImg.EndInit();
            img.Source = bitmapImg;
            loginButton.Content = img;

            OnLogin();
        }
        /// <summary>
        /// 登录动作
        /// </summary>
        private void OnLogin()
        {
            if (String.IsNullOrEmpty(this.userText.Text) || String.IsNullOrEmpty(this.pwdText.Password))
            {
                PmtsMessageBox.CustomControl1.Show("请输入用户ID或密码", PmtsMessageBox.ServerMessageBoxButtonType.OK);
                return;
            }

            bool isAdd = true;
            if (objNodeXml.uList.Count > 0)
            {
                for (int i = 0; i < objNodeXml.uList.Count; i++)
                {
                    String nameStr = objNodeXml.uList[i].uName;
                    if (nameStr == this.userText.Text)
                    {
                        isAdd = false;
                        break;
                    }
                }
            }
            else
            {
                isAdd = true;
            }
            if (isAdd)
            {
                try
                {
                    LogUserList addUser = new LogUserList();
                    addUser.uName = this.userText.Text;
                    objNodeXml.uList.Add(addUser);
                    String xmlTest = "./Config/UserList.xml";
                    XmlSerializer objSerializer = new XmlSerializer(typeof(NodeXml));
                    StreamWriter objStream = new StreamWriter(xmlTest);
                    objSerializer.Serialize(objStream, objNodeXml);
                    objStream.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write("生成XML时出错：" + ex.Message + "\n");
                }
            }

            try
            {
                var resp = APIClient._Login(new LoginRequest()
                {
                    username = this.userText.Text,
                    password = this.pwdText.Password,
                });

                if (null == resp || null == resp.data)
                {
                    MessageBox.Show("网络异常，请稍后重试");
                }
                else if (resp.data.success != "1")
                {
                    MessageBox.Show(resp.data.message);
                }
                else
                {
                    Main main = new Main("127.0.0.1", this.userText.Text);

                    UserInfoStatic.UserInfo = resp.data.userInfo;
                    UserInfoStatic.UserInfo.password = this.pwdText.Password;

                    main.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 接受来自UPD的广播包
        /// </summary>
        private void OnUdpReceive()
        {
            UdpClient receivingUdpClient = new UdpClient(5901);

            isReceive = true;
            //这里指定远端机器的ip和端口，如果只是接收不发送的话，可以随便指定
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (isReceive)
                {
                    //接收数据，并返回远程机器的ep
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    //PmtsMessageBox.CustomControl1.Show(returnData);
                    if (Regex.IsMatch(returnData, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$"))
                    {
                        isReceive = false;
                        if (timeOut.IsEnabled == true)
                        {
                            timeOut.Stop();
                        }
                        ///与主线程建立委托
                        GetIpAddress methodFroIp = OnIpChanged;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, methodFroIp, returnData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            finally
            {
                receivingUdpClient.Close();
                receivingUdpClient = null;
            }
        }
        /// <summary>
        /// UDP超时处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUDPTimeOut(object sender, EventArgs e)
        {
            DispatcherTimer tmp = (DispatcherTimer)sender;
            tmp.Stop();
            PmtsMessageBox.CustomControl1.Show("查询服务器超时，请确认服务器已经开启。", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            this.Close();
        }
        /// <summary>
        /// 用户ID输入监听回车键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUserID(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.pwdText.Focus();
            }
        }
        /// <summary>
        /// 密码输入监听回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyPwd(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnLogin();
            }
        }
        private void OnClearUserID(object sender, EventArgs e)
        {
            if (objNodeXml.uList.Count > 0)
            {
                objNodeXml.uList.Clear();
                String xmlTest = "./Config/UserList.xml";
                XmlSerializer objSerializer = new XmlSerializer(typeof(NodeXml));
                StreamWriter objStream = new StreamWriter(xmlTest);
                objSerializer.Serialize(objStream, objNodeXml);
                objStream.Close();
                this.userText.Items.Clear();
            }
        }
        private void guestButton_Click(object sender, RoutedEventArgs e)
        {
            //PmtsMessageBox.CustomControl1.Show("你是游客！！！", PmtsMessageBox.ServerMessageBoxButtonType.OK);
            /*            this.Close();
                        Directory.SetCurrentDirectory(@"Guest");//还原当前路径
                        WinExec(@"Guest\HD-service.exe", 1);
             */
            //lich
            String ipStr = null;
            Main mainWindow = new Main(ipStr, this.userText.Text);
            mainWindow.Show();
            this.Close();
        }
    }
    /*--------------------------------------序列化xml-----------------------------------------------------*/
    [Serializable]
    [XmlRootAttribute("LoginUserList")]
    public class NodeXml
    {


        private List<LogUserList> _uList = new List<LogUserList>();

        [XmlElementAttribute(ElementName = "User", Type = typeof(LogUserList))]
        public List<LogUserList> uList
        {
            set
            {
                _uList = value;
            }
            get
            {
                return _uList;
            }
        }
    }
    [Serializable]
    public class LogUserList
    {
        [XmlAttribute(AttributeName = "name")]
        public String uName { get; set; }
    }
    /*--------------------------------------序列化xml-----------------------------------------------------*/
}
