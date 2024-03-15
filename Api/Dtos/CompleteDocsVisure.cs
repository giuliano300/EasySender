using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class CompleteDocsVisure
    {
        public int value { get; set; }
        public int documentType { get; set; }
        public string documentTypeName { get; set; }
        public string attribute { get; set; }
        public string description { get; set; }
        public string[] chiusure { get; set; }
        public string chiusuraDefault { get; set; }
    }
}