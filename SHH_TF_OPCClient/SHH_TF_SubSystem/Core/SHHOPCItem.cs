using OPCAutomation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SHH.TF.Core
{
    /// <summary>
    /// OPC测点对象
    /// </summary>
    public class SHHOPCItem : INotifyPropertyChanged
    {
        public SHHOPCItem(Guid id, SHHEquipment equ, string name, string place, string itemID)
        {
            if (id == null || equ == null || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(place) || string.IsNullOrEmpty(itemID))
            {
                throw new ArgumentException("参数不能为空");
            }

            _id = id;
            _equipment = equ;
            _PointName = name;
            _PointPlace = place;
            _OPCItemID = itemID;
            _OPCItem = null;
        }

        public Guid ID { get { return _id; } }

        /// <summary>
        /// 设备类型属性
        /// </summary>
        public SHHEquipment Equipment { set { _equipment = value; } get { return _equipment; } }

        /// <summary>
        /// 测点名称
        /// </summary>
        public string PointName
        {
            set
            {
                _PointName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PointName"));
                }
            }
            get
            {
                return _PointName;
            }
        }

        /// <summary>
        /// 测点安装位置
        /// </summary>
        public string PointPlace
        {
            set
            {
                _PointPlace = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PointPlace"));
                }
            }
            get
            {
                return _PointPlace;
            }
        }

        /// <summary>
        /// 测点值
        /// </summary>
        public string PointValue
        {
            set
            {
                _pointValue = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PointValue"));
                }
            }
            get
            {
                return _pointValue;
            }
        }


        /// <summary>
        /// 对应OPC地址
        /// </summary>
        public string OPCItemID { get { return _OPCItemID; } }

        /// <summary>
        /// 对应OPC项
        /// </summary>
        public OPCItem OPCItem { get { return _OPCItem; } }

        /// <summary>
        /// 测点所属OPC组
        /// </summary>
        public SHHOPCGroup Group { get { return _group; } internal set { _group = value; } }

        //附加子项到组执行的操作
        internal void AttachToGroup(SHHOPCGroup group)
        {
            try
            {
                this.Group = group;
                if (group.OPCGroup != null)//OPC组已经连接上了
                {
                    _OPCItem = group.OPCGroup.OPCItems.AddItem(this.OPCItemID, this.GetHashCode());
                }
            }
            catch
            { }
        }

        internal void DetachFromGroup(SHHOPCGroup group)
        {
            try
            {
                
                if (_OPCItem != null)
                {
                    int[] arrayHandle = new int[1] { _OPCItem.ServerHandle };
                    Array arrayError;
                    group.OPCGroup.OPCItems.Remove(1, arrayHandle, out arrayError);
                }

                this.Group = null;

                _OPCItem = null;
            }
            catch
            { }
        }

        private Guid _id;//测点编号
        private SHHOPCGroup _group;//所属组
        private SHHEquipment _equipment;//对应设备类型
        private string _PointName;//测点名称
        private string _PointPlace;//测点安装位置
        private string _OPCItemID;//对应OPC项ID


        private OPCItem _OPCItem;
        private string _pointValue="0";

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
