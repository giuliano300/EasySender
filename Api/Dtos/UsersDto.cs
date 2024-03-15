using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class UsersDto
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
        public System.Guid guidUser { get; set; }
        public string businessName { get; set; }
        public string baseUrl { get; set; }
        public string pwd { get; set; }
        public string address { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string mobile { get; set; }
        public int parentId { get; set; }
        public string sendersId { get; set; }
        public bool demoUser { get; set; }
        public bool areaTestUser { get; set; }
        public bool mol { get; set; }
        public bool col { get; set; }
        public bool hidePrice { get; set; }
        public bool abilitato { get; set; }
        public string CodiceContrattoMOL { get; set; }
        public string CodiceContrattoCOL { get; set; }
        public bool changePwd { get; set; }
        public bool sms { get; set; }

        public int userPriority { get; set; }

        public string logoPA { get; set; }
        public string denominazioneEntePA { get; set; }
        public string infoEntePA { get; set; }
        public string settoreEntePA { get; set; }
        public string codiceFiscaleEntePA { get; set; }
        public string piva { get; set; }
        public bool attivoPA { get; set; }
        public bool downloadFile { get; set; }
        public bool GED { get; set; }

        public string usernameGED { get; set; }
        public string passwordGED { get; set; }

        public bool Pacchi { get; set; }

        public string usernamePacchi { get; set; }
        public string passwordPacchi { get; set; }
        public string centerCostPacchi { get; set; }
        public string fiscalCode { get; set; }
        public string emailPec { get; set; }

        public bool master { get; set; }
        public int? collegatoMasterId { get; set; }
        public bool? conciliazioneBollettini { get; set; }
        public int? senderPkgId { get; set; }
        public DateTime? expireDate { get; set; }
        public DateTime? insertDate { get; set; }
        public bool? RendicontazioneFatture { get; set; }
        public bool? sso { get; set; }
        public string CodiceContrattoAGOLM { get; set; }
        public string CodiceContrattoAGOLB { get; set; }

        public UsersDto()
        {
            id = 0;
            hidePrice = true;
        }
    }
}