using SHH.TF.Core;
using SHH.TF.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;

namespace SHH.TF.BLL
{
    /// <summary>
    /// OPC服务管理类
    /// </summary>
    public class OPCManager
    {
        public static ObservableCollection<SHHOPCServer> ServerList { get { return _serverList; } }

        //遍历所有测点
        public static ObservableCollection<SHHOPCItem> ItemList
        {
            get
            {
                foreach (SHHOPCServer server in ServerList)
                {
                    foreach (SHHOPCGroup group in server.Groups)
                    {
                        foreach (SHHOPCItem item in group.ItemList)
                        {
                            _itemList.Add(item);
                        }
                    }
                }
                return _itemList;
            }
        }

        //加载参数
        public static void Load()
        {
            SqlDataReader opcserver = TB_TF_OPCServer.GetServer();

            while (opcserver.Read())
            {
                SHHOPCServer server = new SHHOPCServer(new Guid(opcserver["OPCServerID"].ToString()), IPAddress.Parse(opcserver["MachineIP"].ToString()), opcserver["OPCServerName"].ToString(), opcserver["Name"].ToString());
                SqlDataReader opcgroup = TB_TF_OPCGroups.GetGroups();
                while (opcgroup.Read())
                {
                    if (opcserver["OPCServerID"].ToString() == opcgroup["OPCServerID"].ToString())
                    {
                        SHHOPCGroup group = new SHHOPCGroup(new Guid(opcgroup["OPCGroupID"].ToString()), opcgroup["Name"].ToString(), Int32.Parse(opcgroup["UpdateRate"].ToString()), float.Parse(opcgroup["DeadBend"].ToString()), Int32.Parse(opcgroup["TimeBias"].ToString()), (bool)opcgroup["IsActive"], (bool)opcgroup["IsSubscribed"]);
                        SqlDataReader opcpoint = TB_TF_Points.GetPoints();
                        while (opcpoint.Read())
                        {
                            if (opcgroup["OPCGroupID"].ToString() == opcpoint["OPCGroupID"].ToString())
                            {
                                SHHOPCItem item = new SHHOPCItem(new Guid(opcpoint["PointID"].ToString()), new SHHEquipment((SHHEquipmentID)Int32.Parse(opcpoint["EquipID"].ToString()), SHHEquipmentType.Analog, opcpoint["PointName"].ToString(), "MPa"), opcpoint["PointName"].ToString(), opcpoint["EquipPlace"].ToString(), opcpoint["PointAddress"].ToString());


                                group.AddItem(item);

                                /**********删除测试数据*******/
                                //RemovePoint(group, item);
                                /***************************/
                            }
                        }

                        //**********添加测试数据 * *********/
                        //for (int i = 0; i < 5; ++i)
                        //{
                        //    string s = group.Name.Substring(group.Name.Length - 1, 1);
                        //    SHHOPCItem testPoint = new SHHOPCItem(Guid.NewGuid(), new SHHEquipment((SHHEquipmentID)i, SHHEquipmentType.Analog, "测点" + i, "kpa"), "测点" + i, "地点" + i, "Channel_" + s + ".Device_" + 0 + ".Tag_" + i);
                        //    AddPoint(group, testPoint);
                        //}
                        ///******************************

                        server.AddGroup(group);
                    }
                }
                ServerList.Add(server);
            }
        }

        public static void StartAll()
        {
            for (int i = 0; i < ServerList.Count; i++)
            {
                ServerList[i].Connnect();


            }
        }

        public static void CloseAll()
        {
            for (int i = 0; i < ServerList.Count; i++)
            {
                ServerList[i].Disconnect();
            }

        }

        //添加服务端
        public static void AddServer(SHHOPCServer server)
        {
            _serverList.Add(server);

            TB_TF_OPCServer.AddOPCServer(server);
        }

        //修改服务
        public static void ModifyServer(SHHOPCServer server)
        {
            TB_TF_OPCServer.UpdateOPCServer(server);
        }

        //移除服务端及其子项
        public static void RemoveServer(SHHOPCServer server)
        {

            foreach (SHHOPCGroup group in server.Groups)
            {
                foreach (SHHOPCItem item in group.ItemList)
                {
                    RemovePoint(group, item);
                }
                RemoveGroup(server, group);
            }



            TB_TF_OPCServer.DeleteOPCServer(server.ID);

            ServerList.Remove(server);
        }

        //添加服务端组
        public static void AddGroup(SHHOPCServer server, SHHOPCGroup group)
        {
            server.AddGroup(group);

            TB_TF_OPCGroups.AddOPCGroups(group);
        }

        //移除服务端组
        public static void RemoveGroup(SHHOPCServer server, SHHOPCGroup group)
        {
            foreach (SHHOPCItem item in group.ItemList)
            {
                RemovePoint(group, item);
            }

            server.RemoveGroup(group);
            TB_TF_OPCGroups.DeleteOPCGroup(group.ID);
        }

        //修改组
        public static void ModifyGroup(SHHOPCGroup group)
        {
            TB_TF_OPCGroups.UpdateOPCGroups(group);
        }

        //添加测点
        public static void AddPoint(SHHOPCGroup group, SHHOPCItem point)
        {
            group.AddItem(point);
            TB_TF_Points.AddPoints(point);
        }

        //删除测点
        public static void RemovePoint(SHHOPCGroup group, SHHOPCItem point)
        {
            group.RemoveItem(point);
            TB_TF_Points.DeletePoints(point.ID);
        }

        //修改测点
        public static void ModifyPoint(SHHOPCItem point)
        {
            TB_TF_Points.UpdatePoints(point);
        }

        private static ObservableCollection<SHHOPCServer> _serverList = new ObservableCollection<SHHOPCServer>();

        private static ObservableCollection<SHHOPCItem> _itemList = new ObservableCollection<SHHOPCItem>();
    }
}
