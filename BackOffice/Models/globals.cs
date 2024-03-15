using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace BackOffice.Models
{

    public static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
    }

    public class Globals
    {
        public static string staticUrl = "https://backoffice.easysender.it/";
        //public static string staticUrl = "http://localhost:49248//";
        public static string apiUri = "https://apinewtest.easysender.it/";
        //public static string apiUri = "http://localhost:5010/";
        public static string staticUsername = "admin";
        public static string staticPwd = "back2020!";
        //public static string downladFile = "https://ewt.group/private/";
        public static string downladFile = "https://private.easysender.it/private-files/";


        //MULTIGESTIONE
        public static string usernameMultigestione = "demoapi2@texsrv3";
        public static string pwdMultigestione = "da2.2020";
        public static string sourceMultigestione = "APP";
        public static string idDestinatarioMultigestione = "";
        public static int isPrivatoMultigestione = 1;
        public static string modalitaPagamentoMultigestione = "MP01";


        //EMAIL
        public static string mittente = "";
        public static string destinatarioAdmin = "";

        public static async Task<HttpResponseMessage> HttpClientSend(string type, string url, object o = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage r = new HttpResponseMessage();
            switch (type.ToUpper())
            {
                case "POST":
                    r = await client.PostAsJsonAsync(url, o);
                    break;
                case "PUT":
                    r = await client.PutAsJsonAsync(url, o);
                    break;
                case "GET":
                    r = await client.GetAsync(url);
                    break;
                default:
                    break;
            }

            return r;

        }

        public static string NoFoto()
        {
            return "/images/noFoto.jpg";
        }

        public static async Task<HttpResponseMessage> HttpClientGet(string url)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var get = await client.GetAsync(url);
            return get;
        }



        public static string rewriteUrl(string Stringa)
        {
            Stringa = Stringa.Replace("'", "-");
            Stringa = Stringa.Replace(".", "-");
            Stringa = Stringa.Replace(",", "-");
            Stringa = Stringa.Replace("è", "e");
            Stringa = Stringa.Replace("à", "a");
            Stringa = Stringa.Replace("ò", "o");
            Stringa = Stringa.Replace(" ", "-");
            Stringa = Stringa.Replace("/", "-");
            Stringa = Stringa.Replace("&", "-");
            Stringa = Stringa.Replace(":", "-");
            Stringa = Stringa.Replace(";", "-");
            return Stringa;
        }

        public static string leftString(string Stringa, int index)
        {
            return Stringa.Substring(0, Math.Min(index, Stringa.Length));
        }

    }
}