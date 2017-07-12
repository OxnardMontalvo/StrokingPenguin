using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Web.Mvc;

namespace WebAPI_Pure.Controllers {
	public class HomeController : Controller {
		public ActionResult Index() {
			return View();
		}
	}
}
