using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Graph.Models
{
    public class StaffOrgChart
    {
        public StaffOrgChart()
        {
            tags = new List<string>();

        }
        public string id { get; set; }
        public string pid { get; set; }
        public List<string> tags { get; set; }

        public string stpid { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string visa { get; set; }
        public string img { get; set; }
        public string gender { get; set; }
        public string nationality { get; set; }
        public string ContractEndDate { get; set; }
        public string EmpContractType { get; set; }
        public string EmpReqType { get; set; }
    }
}