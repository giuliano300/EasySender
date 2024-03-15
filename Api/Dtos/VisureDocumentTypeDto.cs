using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class VisureDocumentTypeDto
    {
        public int id { get; set; }
        public int documentType { get; set; }
        public int value { get; set; }
        public string attribute { get; set; }
        public string description { get; set; }
    }
}