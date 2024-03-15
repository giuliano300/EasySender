using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class Users
    {
        public int id { get; set; }
        public string usernamePoste { get; set; }
        public string pwdPoste { get; set; }
        public string usernamePosteAreaTest { get; set; }
        public string pwdPosteAreaTest { get; set; }
        public int porpertyId { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public int userType { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public System.Guid guidUser { get; set; }
        public string businessName { get; set; }
        public string baseUrl { get; set; }
        public string pwd { get; set; }
        public string address { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string mobile { get; set; }
        public string CodiceContrattoMOL { get; set; }
        public string CodiceContrattoCOL { get; set; }
        public int parentId { get; set; }
        public string sendersId { get; set; }
        public bool demoUser { get; set; }
        public bool areaTestUser { get; set; }
        public bool mol { get; set; }
        public bool col { get; set; }
        public bool changePwd { get; set; }
        public bool abilitato { get; set; }
        public bool sms { get; set; }
        public bool hidePrice { get; set; }

        public int userPriority { get; set; }
        public bool attivoPA { get; set; }
        public bool downloadFile { get; set; }
        public bool GED { get; set; }
        public bool Pacchi { get; set; }
        public bool? conciliazioneBollettini { get; set; }

        public Users()
        {
            hidePrice = true;
        }
    }
}