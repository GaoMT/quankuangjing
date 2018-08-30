using SHH.TF.BLL;
using SHH.TF.Core;
using SHH.TF.UI;
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

namespace SHH_TF_SubSystem
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitTreeViewContextMenu();//初始化一下树菜单
        }

        /// <summary>
        /// 右键菜单配置
        /// </summary>
        void InitTreeViewContextMenu()
        {
            serverMenu = new ContextMenu();
            groupMenu = new ContextMenu();
            nodeMenu = new ContextMenu();
            createMenu = new ContextMenu();

            MenuItem item = new MenuItem(); item.Header = "新增服务"; item.Click += AddServer;
            createMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "连接"; item.Click += ConnectServer;
            serverMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "断开"; item.Click += DisConnectServer;
            serverMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "新增服务"; item.Click += AddServer;
            serverMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "修改服务"; item.Click += ModifyServer;
            serverMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "删除服务"; item.Click += DeleteServer;
            serverMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "添加组信息"; item.Click += AddGroup;
            serverMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "修改组信息"; item.Click += ModifyGroup;
            groupMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "删除组信息"; item.Click += DeleteGroup;
            groupMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "添加测点"; item.Click += AddPoint;
            groupMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "修改测点"; item.Click += ModifyPoint;
            nodeMenu.Items.Add(item);

            item = new MenuItem(); item.Header = "删除测点"; item.Click += DeletePoint;
            nodeMenu.Items.Add(item);

        }

        //新增服务
        private void AddServer(object sender, RoutedEventArgs e)
        {
            FormServer form = new FormServer();

            form.Owner = this;
            if ((bool)form.ShowDialog())
            {
                SHHOPCServer server = new SHHOPCServer(Guid.NewGuid(), form.ServerIP, form.OPCServerName, form.Name);
                //确定修改
                OPCManager.AddServer(server);
            }
        }



        //连接服务器
        private void ConnectServer(object sender, RoutedEventArgs e)
        {
            SHHOPCServer node = tree.SelectedItem as SHHOPCServer;
            if (node != null)
            {
                node.Connnect();
            }
        }

        //断开连接
        private void DisConnectServer(object sender, RoutedEventArgs e)
        {
            SHHOPCServer node = tree.SelectedItem as SHHOPCServer;
            if (node != null)
            {
                node.Disconnect();
            }
        }

        //修改服务器
        private void ModifyServer(object sender, RoutedEventArgs e)
        {
            SHHOPCServer node = tree.SelectedItem as SHHOPCServer;
            FormServer form = new FormServer();

            //载入参数
            form.name.Text = node.Name;
            form.serverIP.Text = node.IP.ToString();
            form.serverName.Text = node.OPCServerName;
            form.Owner = this;

            if ((bool)form.ShowDialog())
            {
                node.Name = form.Name;
                node.IP = form.ServerIP;
                node.OPCServerName = form.OPCServerName;

                //确定修改
                OPCManager.ModifyServer(node);
            }
        }

        //删除服务
        private void DeleteServer(object sender, RoutedEventArgs e)
        {
            SHHOPCServer node = tree.SelectedItem as SHHOPCServer;
            if (node != null)
            {
                //确定删除
                OPCManager.RemoveServer(node);
            }
        }

        //删除测点
        private void DeletePoint(object sender, RoutedEventArgs e)
        {
            SHHOPCItem node = tree.SelectedItem as SHHOPCItem;
            if (node != null)
            {
                OPCManager.RemovePoint(node.Group, node);
            }
        }

        //修改测点
        private void ModifyPoint(object sender, RoutedEventArgs e)
        {
            SHHOPCItem node = tree.SelectedItem as SHHOPCItem;
            FormPoint form = new FormPoint();


            //载入参数
            form.cbx_EquipID.SelectedIndex =(int)node.Equipment.Id;
            form.tbx_Name.Text = node.PointName;
            form.tbx_Place.Text = node.PointPlace;
            form.tbx_Id.Text = node.OPCItemID;
            form.Owner = this;

            if ((bool)form.ShowDialog())
            {
                node.PointName = form.PointName;
                node.PointPlace = form.Place;
                node.Equipment.Id = (SHHEquipmentID)form.EquipID;


                //确定修改
                OPCManager.ModifyPoint(node);
            }
        }

        //新增测点
        private void AddPoint(object sender, RoutedEventArgs e)
        {
            FormPoint form = new FormPoint();
            form.Owner = this;
            if ((bool)form.ShowDialog())
            {
                //确定新增
                OPCManager.AddPoint((SHHOPCGroup)tree.SelectedItem, new SHHOPCItem(Guid.NewGuid(), new SHHEquipment((SHHEquipmentID)form.EquipID, SHHEquipmentType.Analog, form.PointName, "kpa"), form.PointName, form.Place, form.Id));
            }
        }

        //删除组
        private void DeleteGroup(object sender, RoutedEventArgs e)
        {
            SHHOPCGroup node = tree.SelectedItem as SHHOPCGroup;
            if (node != null)
            {
                //确定删除
                OPCManager.RemoveGroup(node.Server, node);
            }
        }

        //添加组
        private void AddGroup(object sender, RoutedEventArgs e)
        {
            FormGroup form = new FormGroup();
            form.Owner = this;
            if ((bool)form.ShowDialog())
            {
                OPCManager.AddGroup((SHHOPCServer)tree.SelectedItem, new SHHOPCGroup(Guid.NewGuid(), form.Name, form.UpdateRate, form.DeadBend, form.TimeBias, form.IsActive, form.IsSubscribed));
            }

        }

        //修改组
        private void ModifyGroup(object sender, RoutedEventArgs e)
        {
            SHHOPCGroup node = tree.SelectedItem as SHHOPCGroup;
            FormGroup form = new FormGroup();

            //载入参数
            form.tbx_Name.Text = node.Name;
            form.tbx_UpdateRate.Text = node.UpdateRate.ToString();
            form.tbx_DeadBend.Text = node.DeadBend.ToString();
            form.tbx_TimeBias.Text = node.TimeBias.ToString();
            form.cbx_IsActive.IsChecked = node.IsActive;
            form.cbx_IsSubscribed.IsChecked = node.IsSubscribed;

            form.Owner = this;
            if ((bool)form.ShowDialog())
            {
                //确定修改
                node.Name = form.Name;
                node.UpdateRate = form.UpdateRate;
                node.DeadBend = form.DeadBend;
                node.TimeBias = form.TimeBias;
                node.IsActive = form.IsActive;
                node.IsSubscribed = form.IsSubscribed;


                OPCManager.ModifyGroup(node);
            }
        }




        private void tree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeView tr = sender as TreeView;

            if (tr.SelectedItem != null)
            {
                if (tr.SelectedItem.GetType() == typeof(SHHOPCServer))
                {
                    tr.ContextMenu = serverMenu;
                    return;
                }
                else if (tr.SelectedItem.GetType() == typeof(SHHOPCGroup))
                {
                    tr.ContextMenu = groupMenu;
                    return;
                }
                else if (tr.SelectedItem.GetType() == typeof(SHHOPCItem))
                {
                    tr.ContextMenu = nodeMenu;
                    return;
                }
            }
            else
            {
                tr.ContextMenu = createMenu;
            }
            //tr.ContextMenu = null;

        }


        private ContextMenu serverMenu;
        private ContextMenu groupMenu;
        private ContextMenu nodeMenu;
        private ContextMenu createMenu;


    }
}
