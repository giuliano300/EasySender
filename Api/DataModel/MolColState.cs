using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class MolColState
    {
        public string identificativo { get; set; }
        public string descrizione { get; set; }
        public string tipologia { get; set; }
        public bool state { get; set; }
    }
}