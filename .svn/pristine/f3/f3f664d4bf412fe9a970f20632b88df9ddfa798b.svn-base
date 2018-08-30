using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace InternetDataMine.Models
{
    
    //public class Mover
    //{

    //    #region
    //    //移动个人信息
    //    public String LabHead { get; set; }
    //    //移动其实时间、目的地等信息
    //    public string LabFoot { get; set; }
    //    //移动着图片
    //    public Image MoverImage { get; set; }
    //    public List<string> StationsRecord { get; set; }
    //    public string Route { get; set; }
    //    public string[] Routes { get; set; }
    //    //当前X
    //    public float X { get;set;}
    //    //当前Y
    //    public float Y { get; set; }
    //    public float Oldleft { get; set; }
    //    public float Oldtop { get; set; }
    //    internal StepXY step { get; set; }
    //    public string ZFilePath { get; set; }

    //    public string FFilePath { get; set; }
    //    public string DateAndTime { get; set; }
    //    public string[] DataAndTimes { get; set; }
    //    public string NextStation { get; set; }
    //    public string[] NextStations { get; set; }
    //    public string NextStationPoint { get; set; }
    //    public string[] NextStationPoints { get; set; }
    //    public string NowFilePath { get; set; }

    //    #endregion

    //    public void MoverCreate(string route, string datatime, string stations, string stationpoints, string name, string Zfilepath, string Ffilepath, List<string> stationrecord)
    //    {
    //        this.StationsRecord = stationrecord;
    //        this.Route = route;
    //        this.Routes = route.Split('|');
    //        string[] XY=this.Routes[0].Split(',');
    //        this.X = float.Parse(XY[0]);
    //        this.Y = float.Parse(XY[1]);
    //        this.X = float.Parse(XY[0]);
    //        this.Y = float.Parse(XY[1]);
    //        this.Oldleft = float.Parse(XY[0]);
    //        this.Oldtop = float.Parse(XY[1]);
    //        XY = this.Routes[1].Split(',');
    //        this.step = GetSetp(this.X, this.Y, float.Parse(XY[0]), float.Parse(XY[1]));
    //        this.ZFilePath = Zfilepath;
    //        this.FFilePath = Ffilepath;
    //        if (datatime != null && datatime != "")
    //        {
    //            this.DateAndTime = datatime;
    //            this.DataAndTimes = DateAndTime.Split('|');
    //        }
    //        if (stations != null && stations != "")
    //        {
    //            this.NextStation = stations;
    //            NextStations = NextStation.Split('|');
    //        }
    //        if (stationpoints != null && stationpoints != "")
    //        {
    //            this.NextStationPoint = stationpoints;
    //            NextStationPoints = NextStationPoint.Split('|');
    //        }
    //        //PicBox = new PictureBox();
    //        //PicBox.SizeMode = PictureBoxSizeMode.StretchImage;
    //        //PicBox.BackColor = System.Drawing.Color.Transparent;
    //        //PicBox.Size = new System.Drawing.Size(50, 50);
    //        LabHead = name;
    //        LabFoot = "";
    //        if (DataAndTimes.Length > 0)
    //        {
    //            LabFoot = LabFoot + DataAndTimes[0];
    //            //NowDatetime++;
    //        }
    //        if (NextStations.Length > 0)
    //        {
    //            LabFoot = LabFoot + "\r\n目的地:" + NextStations[0];
    //            //NowNextStation++;
    //        }
    //        if (step.IsRtoL)
    //        {
    //            this.MoverImage = new System.Drawing.Bitmap(ZFilePath);
    //            this.NowFilePath = ZFilePath;
    //        }
    //        else
    //        {
    //            this.MoverImage = new System.Drawing.Bitmap(FFilePath);
    //            this.NowFilePath = FFilePath;
    //        }

    //    }


    //    private StepXY GetSetp(float fromx, float fromy, float tox, float toy)
    //    {
    //        float lenghtX = tox - fromx;
    //        float lengthY = toy - fromy;
    //        StepXY stepxy = new StepXY();
    //        if (lenghtX > 0)
    //        {
    //            stepxy.IsRtoL = true;
    //        }
    //        else
    //        {
    //            stepxy.IsRtoL = false;
    //        }
    //        if (lengthY > 0)
    //        {
    //            stepxy.IsTtoB = true;
    //        }
    //        else
    //        {
    //            stepxy.IsTtoB = false;
    //        }
    //        if (Math.Abs(lenghtX) > Math.Abs(lengthY))
    //        {
    //            stepxy.X = lenghtX / Math.Abs(lenghtX);
    //            stepxy.Y = lengthY / Math.Abs(lenghtX);
    //        }
    //        else
    //        {
    //            stepxy.X = lenghtX / Math.Abs(lengthY);
    //            stepxy.Y = lengthY / Math.Abs(lengthY);
    //        }
    //        return stepxy;
    //    }


    //    class StepXY
    //    {
    //        private float x;
    //        /// <summary>
    //        /// X的步差值
    //        /// </summary>
    //        public float X
    //        {
    //            get { return x; }
    //            set { x = value; }
    //        }
    //        private float y;
    //        /// <summary>
    //        /// Y的步差值
    //        /// </summary>
    //        public float Y
    //        {
    //            get { return y; }
    //            set { y = value; }
    //        }

    //        private bool isRtoL;
    //        /// <summary>
    //        /// X轴是否正向
    //        /// </summary>
    //        public bool IsRtoL
    //        {
    //            get { return isRtoL; }
    //            set { isRtoL = value; }
    //        }

    //        private bool isTtoB;
    //        /// <summary>
    //        /// Y轴是否正向
    //        /// </summary>
    //        public bool IsTtoB
    //        {
    //            get { return isTtoB; }
    //            set { isTtoB = value; }
    //        }

    //        public StepXY()
    //        {

    //        }

    //        public StepXY(float x, float y, bool isrtol)
    //        {
    //            this.X = x;
    //            this.Y = y;
    //            this.isRtoL = isrtol;
    //        }
    //    }

    //}
}