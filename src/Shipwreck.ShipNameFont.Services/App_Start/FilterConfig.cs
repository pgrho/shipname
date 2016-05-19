using System.Web;
using System.Web.Mvc;

namespace Shipwreck.ShipNameFont.Services
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
