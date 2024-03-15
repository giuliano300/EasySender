using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class SpecialOperations
    {
        public int id { get; set; }
        public int userId { get; set; }
        public System.Guid guid { get; set; }
        public string name { get; set; }
        public int operationType { get; set; }
        public string operationText { get; set; }
        public System.DateTime insertDate { get; set; }
        public string csvName { get; set; }
        public string zipFileName { get; set; }
        public bool send { get; set; }
        public bool imported { get; set; }

        public int status { get; set; }

        public string errorMessage { get; set; }

        public SpecialOperations()
        {
            id = 0;
            userId = 0;
            guid = Guid.NewGuid();
            name = "";
            operationType = 0;
            operationText = "";
            insertDate = DateTime.Now;
            csvName = "";
            zipFileName = "";
            send = false;
            imported = false;
            status = (int)StatusSpecialOperations.ok;
            errorMessage = null;
        }

    }
}