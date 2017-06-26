using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAPI_Pure.Models;

namespace WebAPI_Pure.Controllers {
	public class HomeController : Controller {
		private AppDB _db;
		private AppUserManager _userManager;

		public AppDB DB {
			get { return _db ?? HttpContext.GetOwinContext().Get<AppDB>(); }
			set { _db = value; }
		}

		public AppUserManager UserManager {
			get {
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
			}
			private set {
				_userManager = value;
			}
		}

		public ActionResult Index() {
			var pass = GeneratePassword();
			pass = GeneratePassword();
			pass = GeneratePassword();
			pass = GeneratePassword();
			pass = GeneratePassword();
			pass = GeneratePassword();
			GetUsers();
			return View();
		}

		public ActionResult About() {
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact() {
			ViewBag.Message = "Your contact page.";

			return View();
		}


		[HttpPost]
		public JsonResult GetUsers() {
			var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).ToList();
			return Json(users);
		}

		[HttpPost]
		public JsonResult GetUser(string id) {
			var user = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == id);
			return Json(user);
		}

		[HttpPost]
		public JsonResult GetFlyersFromUser(string id) {
			var flyers = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == id).Flyers;
			return Json(flyers);
		}

		[HttpPost]
		public JsonResult GetAllFlyers() {
			var flyers = DB.Flyers.ToList();
			return Json(flyers);
		}

		[HttpPost]
		public JsonResult GetFlyersFromCat(int id) {
			var flyers = DB.Categories.Include(x => x.Flyers).FirstOrDefault(x => x.ID == id);
			return Json(flyers);
		}

		[HttpPost]
		public JsonResult GetAllCats() {
			var cats = DB.Categories.ToList();
			return Json(cats);
		}

		[HttpPost]
		public JsonResult GetAllCatsWithFlyers() {
			var cats = DB.Categories.Include(x => x.Flyers).ToList();
			return Json(cats);
		}

		[HttpPost]
		public async Task<JsonResult> AddUser(AddUserViewModel vm) {
			if ( ModelState.IsValid ) {
				var flyer = DB.Flyers.FirstOrDefault();
				if ( flyer != null ) {

					var user = new AppUser {
						Name = vm.Name,
						Address = vm.Address,
						UserName = vm.Email,
						Email = vm.Email,
						PostalCode = vm.PostalCode,
						County = vm.County
					};
					user.Flyers.Add(flyer);
					var result = await UserManager.CreateAsync(user, GeneratePassword());
					if ( result.Succeeded ) {
						UserManager.AddToRole(user.Id, "User");
						await DB.SaveChangesAsync();
						return Json("Adding...");
					}
				} else {
					return Json("Error: C07321AE");
				}
			}
			return Json(false);
		}

		public JsonResult DeleteUser(string id) {
			var user = DB.Users.FirstOrDefault(x => x.Id == id);
			if ( user != null ) {
				DB.Users.Remove(user);
				if ( DB.SaveChanges() > 0 ) {
					return Json(true);
				} else {
					return Json("Error: 0F0F0F0F");
				}
			}
			return Json(false);

		}

		public JsonResult DeactivateUser(string id) {
			var user = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == id);
			if ( user != null ) {
				user.Flyers.Clear(); // 0 flyers = deactivated
				if ( DB.SaveChanges() > 0 ) {
					return Json(true);
				} else {
					return Json("Error: 0F0F0F0F");
				}
			}
			return Json(false);
		}

		// Helper functions
		public string GeneratePassword() {
			string password = "";
			string passwordChars = "abcdefghijklmnopqrstuvwxyzåäöæøå0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖÆØÅ_*$?&=!%{}()/";
			Random r = new Random();
			int length = r.Next(20, 32);
			for ( int i = 0; i <= length; i++ )
				password += passwordChars.Substring(r.Next(0, passwordChars.Length - 1), 1);
			return password;
		}
	}
}
