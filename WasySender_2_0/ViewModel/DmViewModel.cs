using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.DataModel;
using WasySender_2_0.Models;

namespace WasySender_2_0.ViewModel
{
    public class DmViewModel
    {
        public Preventivo Preventivo { get; set; }
        public GetDm Dm { get; set; }
        public DmProducts Product { get; set; }
        public DmConfiguration DmConfiguration { get; set; }
    }
}