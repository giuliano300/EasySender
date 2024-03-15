//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Api.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Names
    {
        public int id { get; set; }
        public string businessName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string complementNames { get; set; }
        public string complementAddress { get; set; }
        public string dug { get; set; }
        public string address { get; set; }
        public string houseNumber { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string state { get; set; }
        public System.DateTime insertDate { get; set; }
        public int currentState { get; set; }
        public bool valid { get; set; }
        public int operationId { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string codice { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> vatPrice { get; set; }
        public Nullable<decimal> totalPrice { get; set; }
        public byte[] attachedFile { get; set; }
        public string fileName { get; set; }
        public Nullable<System.DateTime> presaInCaricoDate { get; set; }
        public Nullable<System.DateTime> consegnatoDate { get; set; }
        public string stato { get; set; }
        public string guidUser { get; set; }
        public Nullable<bool> tipoStampa { get; set; }
        public Nullable<bool> fronteRetro { get; set; }
        public Nullable<bool> ricevutaRitorno { get; set; }
        public bool locked { get; set; }
        public Nullable<System.Guid> reSendGuid { get; set; }
        public string fiscalCode { get; set; }
        public string mobile { get; set; }
        public int namePriority { get; set; }
        public string tipoLettera { get; set; }
        public Nullable<int> IndiceNelLotto { get; set; }
        public string idLotto { get; set; }
        public string pathRecoveryFile { get; set; }
        public Nullable<bool> finalState { get; set; }
        public Nullable<int> numberOfPages { get; set; }
        public Nullable<int> NREA { get; set; }
        public string codiceChiusura { get; set; }
        public Nullable<int> codiceDocumento { get; set; }
        public Nullable<int> tipoDocumento { get; set; }
        public string pathGEDUrl { get; set; }
        public Nullable<int> numberOfErrors { get; set; }
        public Nullable<bool> notificato { get; set; }
        public Nullable<bool> notificaLetta { get; set; }
        public string product { get; set; }
        public Nullable<System.DateTime> shipmentDate { get; set; }
        public Nullable<int> weight { get; set; }
        public Nullable<int> height { get; set; }
        public Nullable<int> length { get; set; }
        public Nullable<int> width { get; set; }
        public string contentText { get; set; }
        public string pathUrl { get; set; }
        public string additionalServices { get; set; }
        public Nullable<bool> senderFromContract { get; set; }
        public string contrassegno { get; set; }
        public string ritornoAlMittente { get; set; }
        public string assicurazione { get; set; }
        public Nullable<bool> sms { get; set; }
        public string testoSms { get; set; }
        public string logo { get; set; }
        public Nullable<bool> fileScaricato { get; set; }
        public string dataDownload { get; set; }
        public Nullable<bool> sendFromNewApi { get; set; }
        public Nullable<System.DateTime> ultimoControllo { get; set; }
        public Nullable<System.DateTime> dataScansione { get; set; }
        public Nullable<int> operationType { get; set; }
        public string cron { get; set; }
        public Nullable<bool> AvvisoRicevimentoDigitale { get; set; }
        public string pec { get; set; }
    
        public virtual Operations Operations { get; set; }
    }
}
