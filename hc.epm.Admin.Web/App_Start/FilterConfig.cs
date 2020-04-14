using hc.epm.UI.Common;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthCheckAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
