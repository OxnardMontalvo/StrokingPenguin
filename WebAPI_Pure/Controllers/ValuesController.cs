using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI_Pure.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Http.Description;
using System.Web.Http.OData;

namespace WebAPI_Pure.Controllers {
	//[Authorize]
	public class ValuesController : ApiController {
		private AppDB _db;
		private AppUserManager _userManager;

		public AppDB DB {
			get { return _db ?? HttpContext.Current.GetOwinContext().Get<AppDB>(); }
			set { _db = value; }
		}

		public AppUserManager UserManager {
			get {
				return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
			}
			private set {
				_userManager = value;
			}
		}

		// GET: api/Products
		[EnableQuery()]
		[ResponseType(typeof(AddUserViewModel))]
		public IHttpActionResult Get() {
			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).Select(x => new AddUserViewModel {
					Name = x.Name,
					Address = x.Address,
					PostalCode = x.PostalCode,
					County = x.County,
					Email = x.Email
				}).ToList();
				return Ok(users);

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}
		// 5e19bf87-26e4-4f70-9206-ad209634fca0
		// GET: api/Products/5
		[ResponseType(typeof(AddUserViewModel))]
		//[Authorize()]
		public IHttpActionResult Get(string id) {
			try {
				AddUserViewModel vm;
				if ( id.Length > 0 ) {
					vm = DB.Users.Include(x => x.Flyers).Where(x => x.Id == id).Select(x => new AddUserViewModel {
						Name = x.Name,
						Address = x.Address,
						PostalCode = x.PostalCode,
						County = x.County,
						Email = x.Email
					}).FirstOrDefault();
				} else {
					vm = new AddUserViewModel();
				}
				return Ok(vm);
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// POST api/values
		public void Post([FromBody]string value) {
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value) {
		}

		// DELETE api/values/5
		public void Delete(int id) {
		}
	}
}
