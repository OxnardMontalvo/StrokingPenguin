using System.Web;
using System.Web.Mvc;
using WebAPI_Pure.Misc;

namespace WebAPI_Pure {
	public class FilterConfig {
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			//filters.Add(new RequreSecureConnectionFilter());
			filters.Add(new HandleErrorAttribute());
		}
	}
}
