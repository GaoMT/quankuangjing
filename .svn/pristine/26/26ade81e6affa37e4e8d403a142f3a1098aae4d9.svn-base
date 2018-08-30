using OPCAutomation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;

namespace SHH.TF.Core
{
    /// <summary>
    /// OPC服务器
    /// </summary>
    public class SHHOPCServer : INotifyPropertyChanged
    {
        public SHHOPCServer(Guid id, IPAddress ip, string opcServerName, string name)
        {
            if (id == null || ip == null || string.IsNullOrEmpty(opcServerName) || string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("IP地址或者OPC服务名不能为空");
            }

            Name = name;
            ID = id;
            _IPAddress = ip;
            _OPCServerName = opcServerName;
            _GroupList = new ObservableCollection<SHHOPCGroup>();
            _OPCServer = new OPCServerClass();

            _OPCServer.ServerShutDown += OPCServer_ServerShutDown;

            //初始化组默认属性
            OPCGroups groups = _OPCServer.OPCGroups;
            groups.DefaultGroupIsActive = true;
            groups.DefaultGroupUpdateRate = 1000;
            groups.DefaultGroupDeadband = 0;
            groups.DefaultGroupTimeBias = 0;
            groups.DefaultGroupLocaleID = 1033;

        }

        public Guid ID { private set; get; }

        //节点名称
        public string Name { set { _name = value; } get { return _name; } }

        //IP地址
        public IPAddress IP
        {
            set
            {
                _IPAddress = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IP"));
                }
            }
            get
            {
                return _IPAddress;
            }
        }

        //OPC服务名
        public string OPCServerName
        {
            set
            {
                _OPCServerName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OPCServerName"));
                }
            }
            get
            {
                return _OPCServerName;
            }
        }

        //OPC服务对象
        internal OPCServer OPCServer { get { return _OPCServer; } }

        public ObservableCollection<SHHOPCGroup> Groups { get { return _GroupList; } }



        //链接OPC服务器
        public void Connnect()
        {
            try
            {
                OPCServerState state = (OPCServerState)(_OPCServer.ServerState);

                if (state != OPCServerState.OPCRunning)
                {
                    _OPCServer.Connect(_OPCServerName, _IPAddress.ToString());

                    for (int i = 0; i < Groups.Count; i++)
                    {
                        Groups[i].DetachFromServer(this);//重新注册一下各个组
                        Groups[i].AttachToServer(this);
                    }
                }
            }
            catch
            { }
        }

        //断开OPC链接
        public void Disconnect()
        {
            try
            {
                OPCServerState state = (OPCServerState)(_OPCServer.ServerState);
                if (state != OPCServerState.OPCDisconnected)
                {
                    for (int i = 0; i < Groups.Count; i++)
                    {
                        Groups[i].DetachFromServer(this);//解除各个组的注册
                    }

                    _OPCServer.ServerShutDown -= OPCServer_ServerShutDown;

                    OPCServer_ServerShutDown(string.Empty);//关闭链接
                }
            }
            catch
            { }
        }

        //添加组到服务器
        public void AddGroup(SHHOPCGroup group)
        {
            if (_GroupList.Contains(group) == false)
            {
                _GroupList.Add(group);
                group.AttachToServer(this);
            }
        }

        //从服务器中移除组
        public void RemoveGroup(SHHOPCGroup group)
        {
            if (_GroupList.Contains(group) == true)
            {
                group.DetachFromServer(this);
                _GroupList.Remove(group);
            }
        }

        //关闭子项
        private void OPCServer_ServerShutDown(string Reason)
        {
            try
            {
                this._OPCServer.Disconnect();
            }
            catch (Exception ee)
            { }
        }


        private IPAddress _IPAddress;//服务器IP地址
        private string _OPCServerName;//服务器上的OPC服务名
        private ObservableCollection<SHHOPCGroup> _GroupList;//服务器上的组

        private OPCServer _OPCServer;//OPC自动化，OPC服务端实例

        private String _name;//节点名称

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
