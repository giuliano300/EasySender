using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class structure
    {
        public struct newUsers
        {
            /// <summary>
            /// Username as Email address
            /// </summary>
            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Username { get; set; }
            [Required]
            public getAnagraficaUsers Anagrafica { get; set; }
        }

        public struct getUsers
        {
            public string Username { get; set; }
            public string TipoUtente { get; set; }
            public Nullable<bool> Attivo { get; set; }
            public getAnagraficaUsers Anagrafica { get; set; }
            public List<getChildUsers> child { get; set; }
        }

        public struct getChildUsers
        {
            /// <summary>
            /// Username as Email address
            /// </summary>
            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Username { get; set; }
            public string TipoUtente { get; set; }
            public Nullable<bool> Attivo { get; set; }
            public getAnagraficaUsers Anagrafica { get; set; }
        }

        public struct getAnagraficaUsers
        {
            /// <summary>
            /// Nome required if ragione sociale is empty
            /// </summary>
            public string Nome { get; set; }
            /// <summary>
            /// Cognome required if ragione sociale is empty
            /// </summary>
            public string Cognome { get; set; }
            public string Telefono { get; set; }
            /// <summary>
            /// Email like Username
            /// </summary>
            [Required]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string Email { get; set; }
            /// <summary>
            /// Complete address / es via toledo,40
            /// </summary>
            [Required]
            public string Indirizzo { get; set; }
            /// <summary>
            /// Cap only numbers
            /// </summary>
            [Required]
            [MaxLength(5)]
            [MinLength(5)]
            [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid Cap")]
            public string Cap { get; set; }
            /// <summary>
            /// Ragione Sociale not required
            /// </summary>
            public string RagioneSociale { get; set; }
            public string PartitaIva { get; set; }
            /// <summary>
            /// Stato / Italia
            /// </summary>
            [Required]
            public string Stato { get; set; }
            /// <summary>
            /// Province two (2) Letters
            /// </summary>
            [Required]
            [MaxLength(2)]
            [MinLength(2)]
            [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Invalid Province")]
            public string Provincia { get; set; }
            /// <summary>
            /// Città / Comune
            /// </summary>
            [Required]
            public string Citta { get; set; }
        }

        public struct GetCategorie
        {
            public string Categoria { get; set; }
            public Nullable<int>Posizione { get; set; }
            public Nullable<bool> Visibile { get; set; }
        }

        public struct GetFile
        {
            public string NomeFile { get; set; }
            public DateTime Datainserimento { get; set; }
            public Nullable<int> NumeroFileCreati { get; set; }
            public Nullable<int> NumeroFileCorretti { get; set; }
            public Nullable<int> NumeroFileErrati { get; set; }
        }

        public struct GetMittente
        {
            public string Nome { get; set; }
            public string Cognome { get; set; }
            public string Indirizzo { get; set; }
            public string Cap { get; set; }
            public string Citta { get; set; }
            public string Provincia { get; set; }
            public string Stato { get; set; }
            public string RagioneSociale { get; set; }
        }

        public struct GetOperations
        {
            public GetCategorie Category { get; set; }
            public GetFile File { get; set; }
            public Nullable <decimal> CostoSpedizione { get; set; }
            public Nullable<decimal> CostoStampa { get; set; }
            public Nullable<decimal> CostoIva { get; set; }
            public getChildUsers User { get; set; }
            public string GuidMessage { get; set; }
            public string IdOrdine { get; set; }
            public GetMittente Mittente { get; set; }
            public Nullable<DateTime> DataRichiesta { get; set; }
            public string StatOoperazione { get; set; }
        }

        public struct GetOperationsUser
        {
            public GetCategorie Category { get; set; }
            public GetFile File { get; set; }
            public Nullable<decimal> CostoSpedizione { get; set; }
            public Nullable<decimal> CostoStampa { get; set; }
            public Nullable<decimal> CostoIva { get; set; }
            public string GuidMessage { get; set; }
            public string IdOrdine { get; set; }
            public GetMittente Mittente { get; set; }
            public Nullable<DateTime> DataRichiesta { get; set; }
            public string StatOoperazione { get; set; }
        }

    }
}