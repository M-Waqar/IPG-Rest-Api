using System.Web;
using System.Web.Mvc;

namespace FD_Rest_Api_Implementation_New
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
