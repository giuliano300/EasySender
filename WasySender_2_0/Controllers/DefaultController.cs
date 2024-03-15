using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WasySender_2_0.DataModel;
using WasySender_2_0.Models;

namespace WasySender_2_0.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
        public async Task<int> NotificheNonLette()
        {
            int p = 0;
            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "Names/NotificheNonLette?userId=" + Request.QueryString["userId"], false);
            if (get.IsSuccessStatusCode)
                p = await get.Content.ReadAsAsync<int>();

            return p;
        }
        public async Task<string> GetOperations()
        {
            int p = 0;
            string usrname = "";
            if (Request.QueryString["usrname"] != null)
                usrname = "&username=" + Request.QueryString["usrname"];

            string code = "";
            if (Request.QueryString["code"] != null)
                code = "&code=" + Request.QueryString["code"];

            string esito = "";
            if (Request.QueryString["esito"] != null)
                esito = "&esito=" + Request.QueryString["esito"];

            string dataDa = "";
            if (Request.QueryString["dataDa"] != null)
                dataDa = "&dataDa=" + Request.QueryString["dataDa"];

            string dataA = "";
            if (Request.QueryString["dataA"] != null)
                dataA = "&dataA=" + Request.QueryString["dataA"];

            string type = "";
            if (Request.QueryString["type"] != null)
                type = "&type=" + Request.QueryString["type"];

            string pp = "";
            if (Request.QueryString["pp"] != null)
                type = "&pp=" + Request.QueryString["pp"];


            var l = new List<GetAllOperations>();

            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "Operations/GetAllOperations?guidUser=" + Request.QueryString["guiduser"] + "&prodotto=" + Request.QueryString["prodotto"] + "&userId=" + Request.QueryString["userId"] + usrname + code + esito + dataDa + dataA + type + pp, false);
            if (get.IsSuccessStatusCode)
                l = await get.Content.ReadAsAsync<List<GetAllOperations>>();

            return JsonConvert.SerializeObject(l);
        }
        public async Task<string> GetAllOperationsNoBulletins()
        {
            string usrname = "";
            if (Request.QueryString["username"] != null)
                usrname = "&username=" + Request.QueryString["username"];

            string code = "";
            if (Request.QueryString["code"] != null)
                code = "&code=" + Request.QueryString["code"];

            string ar = "";
            if (Request.QueryString["ricevutaRitorno"] != null)
                ar = "&ar=" + Request.QueryString["ricevutaRitorno"];

            string esito = "";
            if (Request.QueryString["esito"] != null)
                esito = "&esito=" + Request.QueryString["esito"];

            string dataDa = "";
            if (Request.QueryString["dataDa"] != null)
                dataDa = "&startDate=" + Request.QueryString["dataDa"];

            string dataA = "";
            if (Request.QueryString["dataA"] != null)
                dataA = "&endDate=" + Request.QueryString["dataA"];

            string type = "";
            if (Request.QueryString["type"] != null)
                type = "&type=" + Request.QueryString["type"];

            string mittente = "";
            if (Request.QueryString["mittente"] != null)
                mittente = "&mittente=" + Request.QueryString["mittente"];

            string prodotto = "";
            if (Request.QueryString["prodotto"] != null)
                prodotto = "&prodotto=" + Request.QueryString["prodotto"];


            var l = new List<GetOperationReport>();

            var url = "https://backend.easysender.it/api/ReportSpedizioni?guidUser=" + Request.QueryString["guiduser"] + "&userId=" + Request.QueryString["userId"] + usrname + code + esito + dataDa + dataA + type + mittente + prodotto + ar;

            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", url, false);
            if (get.IsSuccessStatusCode)            
                l = await get.Content.ReadAsAsync<List<GetOperationReport>>();

            return JsonConvert.SerializeObject(l);
        }

        public async Task<string> GetAllOperationsWithBulletins()
        {
            string usrname = "";
            if (Request.QueryString["usrname"] != null)
                usrname = "&username=" + Request.QueryString["usrname"];

            string code = "";
            if (Request.QueryString["code"] != null)
                code = "&code=" + Request.QueryString["code"];

            string ar = "";
            if (Request.QueryString["ricevutaRitorno"] != null)
                ar = "&ar=" + Request.QueryString["ricevutaRitorno"];

            string esito = "";
            if (Request.QueryString["esito"] != null)
                esito = "&esito=" + Request.QueryString["esito"];

            string dataDa = "";
            if (Request.QueryString["dataDa"] != null)
                dataDa = "&dataDa=" + Request.QueryString["dataDa"];

            string dataA = "";
            if (Request.QueryString["dataA"] != null)
                dataA = "&dataA=" + Request.QueryString["dataA"];

            string type = "";
            if (Request.QueryString["type"] != null)
                type = "&type=" + Request.QueryString["type"];

            string mittente = "";
            if (Request.QueryString["mittente"] != null)
                mittente = "&mittente=" + Request.QueryString["mittente"];

            string bollettini = "";
            if (Request.QueryString["bollettini"] != null)
                bollettini = "&bollettini=" + Request.QueryString["bollettini"];

            string pagato = "";
            if (Request.QueryString["pagato"] != null)
                pagato = "&pagato=" + Request.QueryString["pagato"];

            string dataDaPayments = "";
            if (Request.QueryString["dataDaPayments"] != null)
                dataDaPayments = "&dataDaPayments=" + Request.QueryString["dataDaPayments"];

            string dataAPayments = "";
            if (Request.QueryString["dataAPayments"] != null)
                bollettini = "&dataAPayments=" + Request.QueryString["dataAPayments"];


            var l = new List<GetOperationNames>();

            var url = "Operations/GetAllOperationsNewWithBulletins?guidUser=" + Request.QueryString["guiduser"] + "&userId=" + Request.QueryString["userId"] + usrname + code + esito + dataDa + dataA + type + bollettini + pagato + dataDaPayments + dataAPayments + ar;

            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", url, false);
            if (get.IsSuccessStatusCode)
                l = await get.Content.ReadAsAsync<List<GetOperationNames>>();

            return JsonConvert.SerializeObject(l);
        }
    }
}