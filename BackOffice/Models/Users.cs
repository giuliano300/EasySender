using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static BackOffice.Models.EnumClass;
using static BackOffice.Models.Users;

namespace BackOffice.Models
{
    public enum UserPriority : int
    {
        [Description("1 (Più Veloce)")]
        veloce = 1,
        [Description("2 (Intermedio)")]
        intermedio = 2,
        [Description("3 (Più Lento)")]
        lento = 3
    }
    public enum Property : int
    {
        [Description("Easywaytechnology")]
        EWT = 1,
        [Description("Noviservice")]
        NOVISERVICE = 2,
        [Description("H2H Consulting")]
        H2H = 3,
        [Description("RED LOGIC")]
        REDLOGIC = 4,
        [Description("INFO TECH")]
        INFOTECH = 5,
        [Description("INDI")]
        INDI = 6,
        [Description("Tekmerion")]
        Tekmerion = 7
    }

    public class Users
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string usernamePoste { get; set; }
        [Required]
        public string pwdPoste { get; set; }

        [Required]
        public Property porpertyId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        public int userType { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public Nullable<System.Guid> guidUser { get; set; }
        public string businessName { get; set; }
        [Required]
        public string baseUrl { get; set; }
        [Required]
        public string pwd { get; set; }

        public string address { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string mobile { get; set; }
        public bool demoUser { get; set; }
        public int parentId { get; set; }
        public bool sms { get; set; }
        
        public string emailPec { get; set; }
        public string piva { get; set; }
        public string fiscalCode { get; set; }
        public string sendersId { get; set; }
        public string usernamePosteAreaTest { get; set; }
        public string pwdPosteAreaTest { get; set; }
        public bool areaTestUser { get; set; }
        public bool mol { get; set; }
        public string CodiceContrattoMOL { get; set; }
        public string CodiceContrattoCOL { get; set; }
        public Nullable<bool> changePwd { get; set; }
        public UserPriority userPriority { get; set; }
        public bool col { get; set; }
        public bool GED { get; set; }
        public bool abilitato { get; set; }
        public bool hidePrice { get; set; }

        public bool downloadFile { get; set; }
        public string usernameGED { get; set; }
        public string passwordGED { get; set; }

        public bool master { get; set; }
        public string collegatoMasterId { get; set; }
        public bool? conciliazioneBollettini { get; set; }
        public bool? rendicontazioneFatture { get; set; }
        public bool Pacchi { get; set; }
        public bool sso { get; set; }
        public string usernamePacchi { get; set; }
        public string passwordPacchi { get; set; }
        public string centerCostPacchi { get; set; }
        public SenderPgkId? senderPkgId { get; set; }

        public System.DateTime? expireDate { get; set; }
        public System.DateTime? insertDate { get; set; }

        public string CodiceContrattoAGOLM { get; set; }
        public string CodiceContrattoAGOLB { get; set; }

        public Users()
        {
            id = 0;
            usernamePoste = null;
            pwdPoste = null;
            porpertyId = (Property)1;
            name = "";
            lastName = "";
            userType = 1;
            email = "";
            guidUser = null;
            businessName = "";
            baseUrl = "/public/Ewt/";
            pwd = "";
            address = "";
            cap = "";
            city = "";
            province = "";
            mobile = "";
            demoUser = false;
            parentId = 0;
            sendersId = null;
            usernamePosteAreaTest = "";
            pwdPosteAreaTest = "";
            areaTestUser = false;
            mol = false;
            CodiceContrattoMOL = "";
            col = false;
            CodiceContrattoCOL = "";
            changePwd = false;
            userPriority = (UserPriority)3;
            downloadFile = false;
            usernameGED = "";
            passwordGED = "";
            GED = false;
            master = false;
            collegatoMasterId = null;
            conciliazioneBollettini = false;
            rendicontazioneFatture = false;
            abilitato = true;
            sso = false;

            Pacchi = false;
            usernamePacchi = "";
            passwordPacchi = "";
            centerCostPacchi = "";
            senderPkgId = null;

            expireDate = DateTime.Now;
            insertDate = DateTime.Now;
        }
    }
}