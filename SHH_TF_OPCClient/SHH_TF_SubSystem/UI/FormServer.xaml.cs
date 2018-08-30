using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SHH.TF.UI
{
    /// <summary>
    /// FormServer.xaml 的交互逻辑
    /// </summary>
    public partial class FormServer : Window
    {
        public FormServer()
        {
            InitializeComponent();
        }

       //节点名称
        public new string Name { get; protected set; }

        //OPC服务名称
        public string OPCServerName { get; protected set; }

        //机器IP地址
        public IPAddress ServerIP { get; protected set; }

        //确定按钮
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Name = name.Text;
            this.ServerIP = IPAddress.Parse(serverIP.Text);
            this.OPCServerName = serverName.Text;
            
            this.DialogResult = true;
        }

        //取消按钮
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
