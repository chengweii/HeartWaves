using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

namespace HrvFlashWinForm
{
    public partial class MyShockwaveFlash : AxShockwaveFlashObjects.AxShockwaveFlash
    {
        //定义一个公共类FlashRightKey(类名自己定义)来继承
        // AxShockwaveFlashObjects.AxShockwaveFlash(在实例化Shockwave Flash Object控件后生成)类
        protected override void WndProc(ref Message m) //重载WndProc方法(此方法即消息处理机制)
        {
            if (m.Msg == 0X0204) //0×0204即鼠标右键的16进制编码
                return; //返回并不输出
            else
                base.WndProc(ref m); //如果不是右键的话则返回正常的信息
        }
        //public event MouseEventHandler MouseRightDown;
        //public delegate void MouseEventHandler(object sender, System.Windows.Forms.MouseEventArgs e);
        //private const int WM_LBUTTONDOWN = 0x0201;
        //private const int WM_RBUTTONDOWN = 0x0204;

        //protected override void WndProc(ref Message m)
        //{
        //    switch (m.Msg)
        //    {
        //        case WM_RBUTTONDOWN:
        //            Int16 x = (Int16)m.LParam;
        //            Int16 y = (Int16)((int)m.LParam >> 16);
        //            MouseRightDown(this, new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Right, 1, x, y, 0));
        //            break;
        //    }
        //    if (m.Msg == WM_RBUTTONDOWN)
        //    {
        //        return;
        //    }
        //    base.WndProc(ref m);
        //}
    }
}