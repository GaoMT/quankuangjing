using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetDataMine.Models
{
    public class EmpMoverModel
    {
        public List<string> Route { get; set; }
        public List<string> Time { get; set; }
        public List<string> Place { get; set; }
        public List<string> Station { get; set; }
        public string Emp { get; set; }
    }
}