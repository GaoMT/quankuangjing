using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHH.TF.Core
{
    /// <summary>
    /// 设备特性对象
    /// </summary>
    public class SHHEquipment
    {
        //特性编号
        private SHHEquipmentID _id = SHHEquipmentID.Unknown;
        //设备类型
        private SHHEquipmentType _equipType = SHHEquipmentType.Analog;
        //设备名称
        private String _equipName = "";
        //单位
        private String _unit = "";
        //开关量0态显示
        private String _kaiGuan_0_Show = "";
        //开关量1态显示
        private String _kaiGuan_1_Show = "";
        //开关量2态显示(一般表示故障)
        private String _kaiGuan_2_Show = "";


        public SHHEquipment(SHHEquipmentID id, SHHEquipmentType equipType, String equipName, String unit)
        {
            _id = id;
            _equipType = equipType;
            _equipName = equipName;
            _unit = unit;
        }

        public SHHEquipmentID Id { get { return _id; } set { _id = value; } }
        public SHHEquipmentType EquipType { get { return _equipType; } set { _equipType = value; } }
        public string EquipName { get { return _equipName; } set { _equipName = value; } }
        public string Unit { get { return _unit; } set { _unit = value; } }
        public string KaiGuan_0_Show { get { return _kaiGuan_0_Show; } set { _kaiGuan_0_Show = value; } }
        public string KaiGuan_1_Show { get { return _kaiGuan_1_Show; } set { _kaiGuan_1_Show = value; } }
        public string KaiGuan_2_Show { get { return _kaiGuan_2_Show; } set { _kaiGuan_2_Show = value; } }




    }

    /// <summary>
    /// 设备类型
    /// </summary>
    public enum SHHEquipmentType
    {
        Analog = 0,//模拟量
        Switch = 1,//开关量
        Cumulant = 2//累积量
    }

    /// <summary>
    /// 特性编号
    /// </summary>
    public enum SHHEquipmentID
    {

        Unknown = 0,//未知
        CO = 1,//一氧化碳
        WindSpeed = 2,//风速
        Temperature = 3,//温度
        Gas = 4,//瓦斯
        WindPressure = 5,//风压
        NegativePressure = 6,//负压
        WaterLevel = 7,//水位
        Speed = 8,//速度
        CoalLevel = 9,//霉位
        Flow = 10,//流量
        HighLowGas = 11,//高低浓瓦斯
        ActivePower = 12,//有功功率
        ReactivePower = 13,//无功功率
        ElectricCurrent = 14,//电流
        PowerFactor = 15,//功率因数
        Frequency = 16,//频率
        Voltage = 17,//电压
        ActiveElectricity = 18,//有功电量
        ReactiveElectricity = 19,//无功电量
        HooksNum = 20,//钩数
        Yield = 21,//产量
        StartStop = 22,//开停
        Switch = 23,//开关
        CoalBunker = 24,//煤仓空满
        AirDoor = 25,//风门
        AirFan = 26,//风机
        AirDuct = 27,//风筒
        State = 28,//状态
        KnifeGate = 29,//刀闸
        OverWind = 30,//过卷
        Smoke = 31//烟雾
    }
}
