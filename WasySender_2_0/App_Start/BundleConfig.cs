using System.Web;
using System.Web.Optimization;

namespace WasySender_2_0
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryredir").Include(
                        "~/Scripts/jquery.redirect.js"));


            bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.unobtrusive.js"
                        ));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/scrollbar").Include(
                      "~/Scripts/jquery.nicescroll.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                        "~/Scripts/less.min.js",
                        "~/Scripts/Fullscreen.js",
                        "~/Scripts/wow.min.js",
                        "~/Scripts/jquery.divascookies-0.6.min.js",
                        "~/Scripts/jquery.modal.min.js",
                        "~/Scripts/Site.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Balloon.css",
                      "~/Content/dropzone.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/animate.css",
                      "~/Content/cookies.css",
                      "~/Content/site.css",
                      "~/Content/custom-1.0.css",
                      "~/Content/jquery.modal.min.css",
                      "~/Content/Media.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
                     "~/Scripts/jquery.unobtrusive-ajax.js"));

            // dropZone js
            bundles.Add(new ScriptBundle("~/bundles/dropzone").Include(
                        "~/Scripts/dropzone.js"));

            // char counter
            bundles.Add(new ScriptBundle("~/bundles/charsCount").Include(
                        "~/Scripts/charsCount.js"));

            // validateForm
            bundles.Add(new ScriptBundle("~/bundles/validateForm").Include(
                        "~/Scripts/validateForm.js"));

            // dropZone styles
            bundles.Add(new StyleBundle("~/Content/dropZoneStyles").Include(
                      "~/Content/basic.css",
                      "~/Content/dropzone.css"));

            // wa
            bundles.Add(new ScriptBundle("~/bundles/floating-wpp-css").Include(
                     "~/Content/floating-wpp.min.css"));

            bundles.Add(new StyleBundle("~/bundles/floating-wpp-js").Include(
                      "~/Scripts/floating-wpp.min.js"));


            //CUSTOM
            bundles.Add(new ScriptBundle("~/bundles/pacchi/invii").Include(
                     "~/Scripts/private/pacchi/invii.js"));

            bundles.Add(new ScriptBundle("~/bundles/raccomandata/DestinatariList").Include(
                     "~/Scripts/private/raccomandata/DestinatariList.js"));

            bundles.Add(new ScriptBundle("~/bundles/lettera/DestinatariList").Include(
                     "~/Scripts/private/lettera/DestinatariList.js"));

            bundles.Add(new ScriptBundle("~/bundles/raccomandata/DestinatariNew").Include(
                     "~/Scripts/private/raccomandata/DestinatariNew.js"));

            bundles.Add(new ScriptBundle("~/bundles/lettera/DestinatariNew").Include(
                     "~/Scripts/private/lettera/DestinatariNew.js"));

            bundles.Add(new ScriptBundle("~/bundles/raccomandata/Step3").Include(
                     "~/Scripts/private/raccomandata/Step3.js"));

            bundles.Add(new ScriptBundle("~/bundles/lettera/Step3").Include(
                     "~/Scripts/private/lettera/Step3.js"));

            bundles.Add(new ScriptBundle("~/bundles/user/AddDestinatari").Include(
                     "~/Scripts/private/user/AddDestinatari.js"));

            bundles.Add(new ScriptBundle("~/bundles/telegramma/NewMessage").Include(
                     "~/Scripts/private/telegramma/NewMessage.js"));

            bundles.Add(new ScriptBundle("~/bundles/corrispondenza/dettaglioLotto").Include(
                     "~/Scripts/private/corrispondenza/dettaglioLotto.js"));

            bundles.Add(new ScriptBundle("~/bundles/corrispondenza/report").Include(
                     "~/Scripts/private/corrispondenza/report.js"));

            bundles.Add(new ScriptBundle("~/bundles/corrispondenza/index").Include(
                     "~/Scripts/private/corrispondenza/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/raccomandata/index").Include(
                     "~/Scripts/private/raccomandata/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/visure/IntestatariNew").Include(
                   "~/Scripts/private/visure/IntestatariNew.js"));

            bundles.Add(new ScriptBundle("~/bundles/visure/dettaglioLotto").Include(
                 "~/Scripts/private/visure/dettaglioLotto.js"));

            bundles.Add(new ScriptBundle("~/bundles/visure/report").Include(
                    "~/Scripts/private/visure/report.js"));

            bundles.Add(new ScriptBundle("~/bundles/pacchi/DestinatariNew").Include(
                 "~/Scripts/private/pacchi/DestinatariNew.js"));

            bundles.Add(new ScriptBundle("~/bundles/corrispondenza/conciliazionePagamenti").Include(
                    "~/Scripts/private/corrispondenza/conciliazionePagamenti.js"));
        }
    }
}
