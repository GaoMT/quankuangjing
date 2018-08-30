using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace InternetDataMine.Models.Graphics
{
    public class Station
    {
        private Image _StationImage;

        public Image StationImage
        {
            get { return _StationImage; }
            set { _StationImage = value; }
        }

        private string _StationState;

        public string StationState
        {
            get { return _StationState; }
            set { _StationState = value; }
        }


        private PointF _StationPoint;

        public PointF StationPoint
        {
            get { return _StationPoint; }
            set { _StationPoint = value; }
        }

        private float _StationPointX;

        public float StationPointX
        {
            get { return _StationPointX; }
            set { _StationPointX = value; }
        }

        private float _StationPointY;

        public float StationPointY
        {
            get { return _StationPointY; }
            set { _StationPointY = value; }
        }


        private PointF _StationNowPoint;

        public PointF StationNowPoint
        {
            get { return _StationNowPoint; }
            set { _StationNowPoint = value; }
        }

        private string _StationName;

        public string StationName
        {
            get { return _StationName; }
            set { _StationName = value; }
        }

        private string _StationID;

        public string StationID
        {
            get { return _StationID; }
            set { _StationID = value; }
        }

        private string _StationOther = "共0人";

        public string StationOther
        {
            get { return _StationOther; }
            set { _StationOther = value; }
        }

        private string _DivName;

        public string DivName
        {
            get { return _DivName; }
            set { _DivName = value; }
        }

        private bool _HasShowed = true;

        public bool HasShowed
        {
            get { return _HasShowed; }
            set { _HasShowed = value; }
        }

        public Station(float pointx, float pointy, string name, string id, string state, Image img)
        {
            this.StationPoint = new PointF(pointx, pointy);
            this.StationNowPoint = StationPoint;
            this.StationName = name;
            this.StationID = id;
            this.StationState = state;
            this.StationImage = img;
        }
        public Station(float pointx, float pointy, string name, string id, string state, string divname)
        {
            this.StationPointX = pointx;
            this.StationPointY = pointy;
            this.StationName = name;
            this.StationID = id;
            this.StationState = state;
            this.DivName = divname;
        }
        public Station(float pointx, float pointy, string name, string id, Image img)
        {
            this.StationPoint = new PointF(pointx, pointy);
            this.StationNowPoint = StationPoint;
            this.StationName = name;
            this.StationID = id;
            this.StationImage = img;
        }
        public Station(float pointx, float pointy, string name, string id, string state, string empnum, Image img)
        {
            this.StationPoint = new PointF(pointx, pointy);
            this.StationNowPoint = StationPoint;
            this.StationName = name;
            this.StationID = id;
            this.StationState = state;
            this.StationOther = "共" + empnum + "人";
            this.StationImage = img;
        }
        public Station(float pointx, float pointy, string name, string id, string state, string empnum, string divname, Image img)
        {
            this.StationPoint = new PointF(pointx, pointy);
            this.StationNowPoint = StationPoint;
            this.StationName = name;
            this.StationID = id;
            this.StationState = state;
            this.StationOther = "共" + empnum + "人";
            this.DivName = divname;
            this.StationImage = img;
        }
    }
}