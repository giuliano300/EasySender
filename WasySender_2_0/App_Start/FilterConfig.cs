using System.Web;
using System.Web.Mvc;

namespace WasySender_2_0
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidateInputAttribute(false));
        }
    }
}
