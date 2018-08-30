using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OPCTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //数据项集合
        List<String> itemList = new List<string>();

        //OPC服务
        OPCServer opcServer;

        //OPC服务组
        OPCGroups opcGroups;

        //OPC服务
        OPCGroup opcGroup;

        //数据项
        OPCItems opcItems;


        List<OPCDataItem> selectItem;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 连接opc服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            //得到服务名列表
            List<String> serverList = ClientHelper.GetOPCServerName(tbx_IP.Text);

            foreach (String serverName in serverList)
            {
                //连接到服务
                OPCServer opcServertemp = ClientHelper.ConnectToServer(serverName, tbx_IP.Text);
                if (opcServertemp == null)
                {
                    continue;
                }
                this.opcServer = opcServertemp;

                //更新数据项
                UpdateItemList();
                //添加到listView_Server
                listView_Server.Items.Add(serverName);

            }

            CreateGroup();
        }

        /// <summary>
        /// 更新数据项
        /// </summary>
        /// <param name="opcServer"></param>
        private void UpdateItemList()
        {
            //清除listview
            listView_Server.Items.Clear();
            listView_Channel.Items.Clear();
            listView_Items.Items.Clear();
            listView_Item.Items.Clear();

            //获取数据项
            itemList = ClientHelper.GetItemList(opcServer);

            //CreateGroup();
        }

        /// <summary>
        /// 点击断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            opcServer.Disconnect();

            UpdateItemList();
        }

        /// <summary>
        /// 点击更新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemList();


        }

        /// <summary>
        /// 选择服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_Server_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //清除listview
            listView_Channel.Items.Clear();
            foreach (String item in itemList)
            {
                String[] s = item.Split('.');

                if (!listView_Channel.Items.Contains(s[0]))
                {
                    listView_Channel.Items.Add(s[0]);
                }
            }
        }

        /// <summary>
        /// 选择通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_Channel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView_Channel.SelectedItem == null)
                return;

            //清除listview
            listView_Items.Items.Clear();

            foreach (String item in itemList)
            {
                String[] s = item.Split('.');

                if (s.Contains(listView_Channel.SelectedItem.ToString()))
                {
                    if (!listView_Items.Items.Contains(s[1]))
                    {
                        listView_Items.Items.Add(s[1]);
                    }
                }
            }
        }

        /// <summary>
        /// 选择项组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView_Items.SelectedItem == null)
                return;

            //清除listview
            listView_Item.Items.Clear();

            foreach (String item in itemList)
            {
                String[] s = item.Split('.');

                if (s.Contains(listView_Items.SelectedItem.ToString()) &&
                    s.Contains(listView_Channel.SelectedItem.ToString()))
                {
                    if (!listView_Items.Items.Contains(s[2]))
                    {
                        listView_Item.Items.Add(s[2]);
                    }
                }
            }

        }

        /// <summary>
        /// 暂时放在一个组里
        /// </summary>
        private void CreateGroup()
        {
            try
            {
                opcGroups = opcServer.OPCGroups;
                opcGroup = opcGroups.Add("OPCGroup");
                SetGroupProperty();
                opcGroup.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(OpcGroup_DataChange);


                opcItems = opcGroup.OPCItems;
                //AddOpcItem();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddOpcItem(String s, int id)
        {
            opcItems.AddItem(s, id);
        }

        /// <summary>  
        /// 每当项数据有变化时执行的事件  
        /// </summary>  
        /// <param name="TransactionID">处理ID</param>  
        /// <param name="NumItems">项个数</param>  
        /// <param name="ClientHandles">项客户端句柄</param>  
        /// <param name="ItemValues">TAG值</param>  
        /// <param name="Qualities">品质</param>  
        /// <param name="TimeStamps">时间戳</param>  
        private void OpcGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {

            selectItem = new List<OPCDataItem>
            {
                new OPCDataItem()
            };

            try
            {
                for (int i = 0; i < NumItems; ++i)
                {
                    selectItem[i].ItemName = ClientHandles.GetValue(i + 1).ToString();
                    selectItem[i].ItemValue = ItemValues.GetValue(i + 1).ToString();
                    selectItem[i].Quality = Qualities.GetValue(i + 1).ToString();
                    selectItem[i].TimeStamp = TimeStamps.GetValue(i + 1).ToString();
                }


            }
            catch
            {

            }

            listView_Selected.ItemsSource = selectItem;

        }

        /// <summary>
        /// 设置组属性
        /// </summary>
        private void SetGroupProperty()
        {
            opcServer.OPCGroups.DefaultGroupIsActive = true;
            opcServer.OPCGroups.DefaultGroupDeadband = 0;
            opcGroup.UpdateRate = 3000;
            opcGroup.IsActive = true;
            opcGroup.IsSubscribed = true;
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            AddOpcItem(listView_Channel.SelectedItem.ToString()
                + "."
                + listView_Items.SelectedItem.ToString()
                + "."
                + listView_Item.SelectedItem.ToString(),
                listView_Selected.Items.Count + 1);
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

