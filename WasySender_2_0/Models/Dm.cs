using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public enum products
    {
        CartoPlus10SC = 1,
        CartoPlusLGRidotto,
        CartoPlusLG,
        CartolinaA5,
        CartolinaA6
    }

    public enum recipientType
    {
        [Display(Name ="Nuova lista")]
        newList = 1,
        [Display(Name = "Lista Esistente")]
        existentList,
        [Display(Name = "Lista Richiesta")]
        requestList
    }

    public enum paymentMethod
    {
        carte = 1,
        bonifico,
        payPal
    }

    public class Dm
    {
        public int? id { get; set; }
        public int? userId { get; set; }
        public int? productId { get; set; }
        public bool haveCreativity { get; set; }
        public int? recipientsType { get; set; }
        public DateTime date { get; set; }
        public int numberOfNames { get; set; }
        public decimal netPrice { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }
        public string sessionUser { get; set; }
        public int? paymentMethod { get; set; }
        public bool complete { get; set; }
        public bool paid { get; set; }

        public Dm()
        {
            id = null;
            userId = null;
            haveCreativity = false;
            recipientsType = null;
            date = DateTime.Now;
            numberOfNames = 0;
            netPrice = 0;
            vatPrice = 0;
            totalPrice = 0;
            sessionUser = HttpContext.Current.Session.SessionID;
            paymentMethod = null;
            complete = false;
            paid = false;
        }
    }
}