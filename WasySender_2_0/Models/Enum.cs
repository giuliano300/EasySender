using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{

    public enum LogType
    {
        login = 1,
        logout,
        visRol,
        visLol,
        visTol,
        visVis,
        visCol,
        visMol,
        visPgk,
        sendRol,
        sendLol,
        sendTol,
        sendVis,
        sendCol,
        sendMol,
        sendPgk,
        requestReport,
        requestArchive,
        modPersonalData,
        modPwd,
        crudSender,
        crudUsers,
        visLoghi,
        addLogo,
        visErrors
    }

    public enum StatusSpecialOperations
    {
        ok = 1,
        ko = 2
    }

    public enum ProductType
    {
        semplice = 0,
        bollettini = 1,
        singolo = 2,
        bollettinoSingolo = 3
    }

    public enum operationType
    {
        ROL = 1,
        LOL = 2,
        TELEGRAMMA = 3,
        MOL = 4,
        COL = 5,
        VOL = 6
    }

    public enum TipoStampa
    {
        BiancoNero = 1,
        Colori = 0
    }

    public enum FronteRetro
    {
        Si = 0,
        No = 1
    }

    public enum RicevutaDiRitorno
    {
        No = 0,
        Si = 1
    }

    public enum MittenteDaContratto
    {
        No = 0,
        Si = 1
    }

    public enum PecTye
    {
        [Display(Name = "Aruba")]
        Aruba = 2,
        [Display(Name = "Register")]
        Register = 3
    }

    public enum userType
    {
        [Display(Name = "Amministratore di sistema")]
        Master = 1,
        [Display(Name = "Agente inseritore")]
        Inseritore = 2,
        [Display(Name = "Agente visualizzatore")]
        Visualizzatore = 3
    }

    public enum ErrorMessageUploadCsv
    {
        [Display(Name = "")]
        NessunErrore = 0,
        [Display(Name = "Errore generico")]
        ErroreGenerico = 1,
        [Display(Name = "Errore nella creazione della lista")]
        ErroreCreazioneLista = 2,
        [Display(Name = "Il Csv non contiene tutti i campi obbligatori")]
        ErroreLunghezzaCsv = 3
    }

    public enum CurrentState
    {
        [Display(Name = "In attesa")]
        InAttesa = 0,
        [Display(Name = "Accettato")]
        PresoInCarico = 1,
        [Display(Name = "In lavorazione")]
        ErroreCreazioneLista = 2,
        [Display(Name = "Spedito")]
        Spedito = 3,
        [Display(Name = "Annullato")]
        Annullato = 4,
        [Display(Name = "Rifiutato")]
        ErroreSubmit = 5,
        [Display(Name = "Rifiutato")]
        ErroreValorizza = 6,
        [Display(Name = "Rifiutato")]
        ErroreConfirm = 7,
        [Display(Name = "Accettato On Line")]
        AccettatoOnLine = 9
    }
}