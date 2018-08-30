using OPCAutomation;
using SHH.TF.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SHH.TF.Core
{
    /// <summary>
    /// OPC组
    /// </summary>
    public class SHHOPCGroup: INotifyPropertyChanged
    {
        public SHHOPCGroup(Guid id, string name, int updateRate, float deadBend, int timeBias, bool isActive, bool isSubscribed)
        {
            this.ID = id;
            this._Name = name;
            this._UpdateRate = updateRate;
            this._DeadBend = deadBend;
            this._TimeBias = timeBias;
            this._IsActive = isActive;
            this._IsSubscribed = isSubscribed;
            _opcGroup = null;
            _itemList = new ObservableCollection<SHHOPCItem>();

        }

        public Guid ID { private set; get; }

        //组名
        public string Name
        {
            set
            {
                _Name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _Name;
            }
        }

        //更新速率
        public int UpdateRate { set { _UpdateRate = value; } get { return _UpdateRate; } }

        //波动率
        public float DeadBend { set { _DeadBend = value; } get { return _DeadBend; } }

        //时差
        public int TimeBias { set { _TimeBias = value; } get { return _TimeBias; } }

        //是否激活
        public bool IsActive { set { _IsActive = value; } get { return _IsActive; } }

        //是否订阅
        public bool IsSubscribed { set { _IsSubscribed = value; } get { return _IsSubscribed; } }

        //OPC组
        internal OPCGroup OPCGroup { get { return _opcGroup; } }

        public ObservableCollection<SHHOPCItem> ItemList { get { return _itemList; } }

        public SHHOPCServer Server { get; internal set; }

        //添加子项
        public void AddItem(SHHOPCItem item)
        {
            if (_itemList.Contains(item) == false)
            {
                _itemList.Add(item);
                item.AttachToGroup(this);
            }
        }

        //移除子项
        public void RemoveItem(SHHOPCItem item)
        {
            if (_itemList.Contains(item))
            {
                item.DetachFromGroup(this);
                _itemList.Remove(item);
            }
        }

        //附加组到服务器执行的操作
        internal void AttachToServer(SHHOPCServer server)
        {
            try
            {
                this.Server = server;

                OPCServerState serverState = (OPCServerState)(server.OPCServer.ServerState);

                if (serverState != OPCServerState.OPCDisconnected)
                {
                    //OPC服务对象
                    _opcGroup = server.OPCServer.OPCGroups.Add(this.Name);
                    _opcGroup.TimeBias = TimeBias;
                    _opcGroup.UpdateRate = UpdateRate;
                    _opcGroup.IsActive = IsActive;
                    _opcGroup.IsSubscribed = IsSubscribed;
                    _opcGroup.DeadBand = DeadBend;
                    _opcGroup.DataChange += OPCGroup_DataChange;//测点值改变(一般是事件订阅触发)
                    _opcGroup.AsyncReadComplete += OPCGroup_AsyncReadComplete;//异步读成功
                    _opcGroup.AsyncWriteComplete += OPCGroup_AsyncWriteComplete;//异步写成功
                    _opcGroup.AsyncCancelComplete += OPCGroup_AsyncCancelComplete;//异步取消成功

                    //重新注册子项
                    for (int i = 0; i < ItemList.Count; i++)
                    {
                        ItemList[i].DetachFromGroup(this);
                        ItemList[i].AttachToGroup(this);
                    }


                }

                //Server = null;
            }
            catch
            { }
        }

        //从服务器分离执行的操作
        internal void DetachFromServer(SHHOPCServer server)
        {
            try
            {
                if (server.OPCServer.OPCGroups != null && _opcGroup != null)
                {
                    //取消事件
                    _opcGroup.DataChange -= OPCGroup_DataChange;//测点值改变(一般是事件订阅触发)
                    _opcGroup.AsyncReadComplete -= OPCGroup_AsyncReadComplete;//异步读成功
                    _opcGroup.AsyncWriteComplete -= OPCGroup_AsyncWriteComplete;//异步写成功
                    _opcGroup.AsyncCancelComplete -= OPCGroup_AsyncCancelComplete;//异步取消成功

                    server.OPCServer.OPCGroups.Remove(this.Name);//从OPC通讯中移除
                }
                _opcGroup = null;
            }
            catch
            { }
        }

        //测点值改变(一般是事件订阅触发)
        private void OPCGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            float[] values = ItemValues as float[];
            int[] qualities = Qualities as int[];
            DateTime[] times = TimeStamps as DateTime[];

            for (int i = 1; i <= NumItems; i++)
            {
                for (int j = 0; j < _itemList.Count; j++)
                {
                    SHHOPCItem item = _itemList[j];


                    if (Convert.ToInt32(ClientHandles.GetValue(i)) == item.GetHashCode())
                    {
                        object value = ItemValues.GetValue(i);
                        item.PointValue = value.ToString();

                        TB_TF_RealValues.UpdateValue(item.ID, value.ToString(), (DateTime)TimeStamps.GetValue(i), (int)SHHPointState.Normal);
                        break;
                    }
                }
            }
        }

        //异步读成功
        private void OPCGroup_AsyncReadComplete(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps, ref Array Errors)
        {

        }

        //异步写成功
        private void OPCGroup_AsyncWriteComplete(int TransactionID, int NumItems, ref Array ClientHandles, ref Array Errors)
        {

        }

        //异步取消成功
        private void OPCGroup_AsyncCancelComplete(int CancelID)
        {

        }


        private string _Name;//组名
        private int _UpdateRate;//组更新速率
        private float _DeadBend;//波动率
        private int _TimeBias;//时差
        private bool _IsActive;//是否激活
        private bool _IsSubscribed;//是否是订阅模式

        private OPCGroup _opcGroup;//OPC组
        private ObservableCollection<SHHOPCItem> _itemList;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
