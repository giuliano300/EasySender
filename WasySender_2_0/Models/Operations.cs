using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class Operations
    {
        public int id { get; set; }
        public string name { get; set; }
        public int userId { get; set; }
        public int? userParentId { get; set; }
        public System.DateTime date { get; set; }
        public int operationType { get; set; }
        public bool demoOperation { get; set; }
        public bool complete { get; set; }
        public bool areaTestOperation { get; set; }
        public bool lotto { get; set; }
        public bool lottoCompleto { get; set; }
        public int operationPriority { get; set; }
        public bool formatoSpeciale { get; set; }
        public bool formatoSpecialeInviato { get; set; }
        public bool error { get; set; }
        public string errorMessage { get; set; }
        public string csvFileName { get; set; }
        public string logo { get; set; }
    }
}