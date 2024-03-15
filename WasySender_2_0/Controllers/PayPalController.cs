using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using WasySender_2_0.Models;
using Newtonsoft.Json;
using PayPal.Api;
using System.Net.Http;

namespace WasySender_2_0.Controllers
{
    public class PayPalController : Controller
    {
        // GET: PayPal
        public ActionResult Index()
        {
            var IdOrder = Session["IdOrder"];
            var TotalToPay = Session["TotalToPay"];

            if (TotalToPay == null || IdOrder == null)
               return RedirectToAction("Error", "Dm");

            return View();
        }
        // GET: PayPal
        //public async Task<ActionResult> Notify()
        //{
        //    Users u = new Users();
        //    u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

        //    string strLive = "https://www.paypal.com/cgi-bin/webscr";
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strLive);
        //    req.Method = "POST";
        //    req.ContentType = "application/x-www-form-urlencoded";

        //    Byte[] Param = Request.BinaryRead(System.Web.HttpContext.Current.Request.ContentLength);
        //    String strRequest = Encoding.ASCII.GetString(Param);
        //    strRequest = strRequest + "&cmd=_notify-validate";
        //    req.ContentLength = strRequest.Length;

        //    StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
        //    streamOut.Write(strRequest);
        //    streamOut.Close();

        //    StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
        //    String strResponse = streamIn.ReadToEnd();
        //    streamIn.Close();

        //    if (strResponse == "VERIFIED")
        //    {
        //        int itemNo = 0;
        //        if (String.IsNullOrEmpty(Request["item_number"]) && Convert.ToInt32(Request["item_number"]) > 0)
        //        {
        //            itemNo = Convert.ToInt32(Request["item_number"]);
        //            if (String.IsNullOrEmpty(Request["payment_status"]) && Request["payment_status"].ToUpper() == "COMPLETED")
        //            {
        //                if ((string)Session["typeProduct"] == "Dm")
        //                {
        //                    await Globals.HttpClientSend("GET", "/api/Dm/SignPaid?id=" + Session["IdOrder"], u.areaTestUser);
        //                    return RedirectToAction("StepEnd", "Dm");
        //                }
        //                await Globals.HttpClientSend("GET", "/api/Sms/SignPaid?id=" + Session["IdOrder"] + "&planned=true", u.areaTestUser);
        //                return RedirectToAction("StepEnd", "Sms");
        //            };
        //        };
        //    };
        //    return RedirectToAction("Error", "Dm");
        //}

        public async Task<ActionResult> PaymentWithPaypal(string Cancel = null)
        {
            //ottenere l'apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                //Una risorsa che rappresenta un pagatore che finanzia un metodo di pagamento come paypal
                //L'ID pagatore verrà restituito quando il pagamento procede o fai clic per pagare
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //questa sezione verrà eseguita per prima perché PayerID non esiste
                    //viene restituito dalla chiamata della funzione di creazione della classe di pagamento

                    // Creare un pagamento
                    // baseURL è l'URL su cui paypal restituisce i dati.
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/PayPal/PaymentWithPayPal?";

                    //generzione guid per la memorizzazione dell'ID pagamento ricevuto in sessione
                    //che verrà utilizzato nell'esecuzione del pagamento

                    var order = Session["IdOrder"].ToString();

                    //La funzione CreatePayment ci fornisce l'URL di approvazione del pagamento
                    //su cui il pagatore viene reindirizzato per il pagamento dell'account paypal

                    var createdPayment = await this.CreatePayment(apiContext, baseURI + "orderId=" + order);

                    //ottenere link restituiti da paypal in risposta a Crea funzione chiamata

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //salva l'URL payapalredirect su cui l'utente verrà reindirizzato per il pagamento
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // salvare l'ID pagamento
                    Session.Add(order, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    Users u = new Users();
                    u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                    if ((string)Session["typeProduct"] == "Dm")
                    {
                        await Globals.HttpClientSend("GET", "/api/Dm/SignPaid?id=" + Session["IdOrder"], u.areaTestUser);
                        return RedirectToAction("StepEnd", "Dm");
                    }
                    await Globals.HttpClientSend("GET", "/api/Sms/SignPaid?id=" + Session["IdOrder"] + "&planned=true", u.areaTestUser);
                    return RedirectToAction("StepEnd", "Sms");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error500", "Home");
            }
        }

        private PayPal.Api.Payment payment;

        private async Task<Payment> CreatePayment(APIContext apiContext, string redirectUrl)
        {

            //creare un elenco di elementi e aggiungere oggetti oggetto
            var itemList = new ItemList() { items = new List<Item>() };

            var itemName = "SMS";
            if ((string)Session["typeProduct"] == "Dm")
                itemName = "DM";


            //Aggiunta di dettagli articolo come nome, valuta, prezzo ecc
            itemList.items.Add(new Item()
            {
                name = itemName,
                currency = "EUR",
                price = Session["TotalToPay"].ToString(),
                quantity = "1",
                sku = itemName + "-" + Session["IdOrder"]
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configura qui gli URL di reindirizzamento con l'oggetto RedirectUrls
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            // Aggiunta di dettagli fiscali, di spedizione e di totale parziale
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = Session["TotalToPay"].ToString()
            };

            //Importo finale con dettagli
            var amount = new Amount()
            {
                currency = "EUR",
                total = Session["TotalToPay"].ToString(),  // Il totale deve essere uguale alla somma delle tasse, spedizione e subtotale.
                details = details
            };

            var transactionList = new List<Transaction>();
            // Aggiunta di descrizione sulla transazione
            transactionList.Add(new Transaction()
            {
                description = "",
                invoice_number = "", // Genera un numero di fattura
                amount = amount,
                item_list = itemList
            });


            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Crea un pagamento utilizzando un APIContext
            return this.payment.Create(apiContext);
        }

    }
}