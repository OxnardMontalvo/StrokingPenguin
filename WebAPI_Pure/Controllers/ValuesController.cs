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
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace WebAPI_Pure.Controllers {
	//[Authorize(Roles = "Admin")]
	public class UsersController : ApiController {
		AppDB _db;
		AppUserManager _userManager;

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

		[ResponseType(typeof(AddUserViewModel))]
		[Route("api/Users/ByDN")]
		[HttpGet]
		public IHttpActionResult GetByDN() {
			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).Select(x => new AddUserViewModel {
					Name = x.Name,
					Address = x.Address,
					PostalCode = x.PostalCode,
					County = x.County,
					Email = x.Email,
					DistrictNumber = x.DistrictNumber,
					DeliveryOrderNumber = x.DeliveryOrderNumber
				}).OrderBy(u => u.DistrictNumber).ThenBy(u => u.DeliveryOrderNumber).ToList();
				return Ok(users);

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Users/5e19bf87-26e4-4f70-9206-ad209634fca0
		[ResponseType(typeof(AddUserViewModel))]
		[Route("api/Users/{id}")]
		public IHttpActionResult Get(string id) {
			try {
				AddUserViewModel vm;
				if ( id.Length > 0 ) {
					vm = DB.Users.Include(x => x.Flyers).Where(x => x.Id == id).Select(x => new AddUserViewModel {
						Name = x.Name,
						Address = x.Address,
						PostalCode = x.PostalCode,
						County = x.County,
						Email = x.Email,
						DistrictNumber = x.DistrictNumber,
						DeliveryOrderNumber = x.DeliveryOrderNumber
					}).FirstOrDefault();
				} else {
					vm = new AddUserViewModel();
				}
				return Ok(vm);
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// POST: api/Users
		[ResponseType(typeof(AddUserViewModel))]
		[AllowAnonymous]
		[Route("api/Users")]
		public async Task<IHttpActionResult> Post([FromBody]AddUserViewModel vm) {
			try {
				if ( vm == null ) {
					return BadRequest("User cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var flyer = await DB.Flyers.FirstOrDefaultAsync();
				if ( flyer == null ) {
					return BadRequest("No flyers available");
				}

				var user = new AppUser {
					Name = vm.Name,
					Address = vm.Address,
					UserName = vm.Email,
					Email = vm.Email,
					PostalCode = vm.PostalCode,
					County = vm.County,
					Flyers = new Collection<Flyer>() { flyer }
				};
				var result = await UserManager.CreateAsync(user, GeneratePassword());
				if ( result.Succeeded ) {
					await UserManager.AddToRoleAsync(user.Id, "User");
					await DB.SaveChangesAsync();
					return Ok(result);
                }

				//return Created<AppUser>(Request.RequestUri + newUser.Id, newUser);
				return Created<AddUserViewModel>(Request.RequestUri + user.Id, vm);
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// PUT: api/Users/5e19bf87-26e4-4f70-9206-ad209634fca0
		[Route("api/Users/{id}")]
		public async Task<IHttpActionResult> Put(string id, [FromBody]AddUserViewModel vm) {
			try {
				if ( vm == null ) {
					return BadRequest("User cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var user = DB.Users.FirstOrDefault(x => x.Id == id);
				user = new AppUser {
					Id = user.Id,
					Name = vm.Name,
					Address = vm.Address,
					UserName = vm.Email,
					Email = vm.Email,
					PostalCode = vm.PostalCode,
					County = vm.County,
					DistrictNumber = vm.DistrictNumber,
					DeliveryOrderNumber = vm.DeliveryOrderNumber
				};
				if ( await DB.SaveChangesAsync() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// DELETE: api/Users/5e19bf87-26e4-4f70-9206-ad209634fca0
		[Route("api/Users/{id}")]
		public async Task<IHttpActionResult> Delete(string id) {
			try {
				if ( id == null ) {
					return BadRequest("User cannot be null");
				}

				var user = DB.Users.FirstOrDefault(x => x.Id == id);

				if ( user == null ) {
					return NotFound();
				}

				DB.Users.Remove(user);
				if ( await DB.SaveChangesAsync() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
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

	public class UserFlyersController : ApiController {
		AppDB _db;

		public AppDB DB {
			get { return _db ?? HttpContext.Current.GetOwinContext().Get<AppDB>(); }
			set { _db = value; }
		}

		// GET: api/UserFlyers/5e19bf87-26e4-4f70-9206-ad209634fca0
		[EnableQuery()]
		[ResponseType(typeof(Flyer))]
		[Route("api/UserFlyers/{guid}")]
		public IHttpActionResult Get(string guid) {
			try {
				return Ok(DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == guid).Flyers);

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Flyers/5e19bf87-26e4-4f70-9206-ad209634fca0/5
		[ResponseType(typeof(Flyer))]
		//[Authorize()]
		[Route("api/UserFlyers/{guid}/{id}")]
		public IHttpActionResult Get(string guid, int id) {
			try {
				var user = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == guid);
				return Ok(user.Flyers.FirstOrDefault(x => x.ID == id));
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// POST: api/Flyers/5e19bf87-26e4-4f70-9206-ad209634fca0
		[ResponseType(typeof(Flyer))]
		[Route("api/UserFlyers/{guid}/{id}")]
		public IHttpActionResult Post(string guid, int id) {
			try {
				if ( id <= 0 || string.IsNullOrEmpty(guid) ) {
					return BadRequest("Id's cannot be invalid");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}
				var user = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == guid);
				var flyer = DB.Flyers.FirstOrDefault(x => x.ID == id);

				if ( user == null || flyer == null ) {
					return NotFound();
				}

				if ( user.Flyers.Contains(flyer) ) {
					return BadRequest("Flyer exists in User");
				}

				user.Flyers.Add(flyer);
				if ( DB.SaveChanges() == 0 ) {
					return Conflict();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// DELETE: api/Flyers/5e19bf87-26e4-4f70-9206-ad209634fca0
		[Route("api/UserFlyers/{guid}")]
		public IHttpActionResult Delete(string guid) {
			try {
				if ( string.IsNullOrEmpty(guid) ) {
					return BadRequest("Id's cannot be invalid");
				}

				var user = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == guid);
				if ( user == null ) {
					return NotFound();
				}

				user.Flyers.Clear();
				if ( DB.SaveChanges() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// DELETE: api/Flyers/5e19bf87-26e4-4f70-9206-ad209634fca0/5
		[Route("api/UserFlyers/{guid}/{id}")]
		public IHttpActionResult Delete(string guid, int id) {
			try {
				if ( id <= 0 || string.IsNullOrEmpty(guid) ) {
					return BadRequest("Id's cannot be invalid");
				}

				var flyer = DB.Flyers.FirstOrDefault(x => x.ID == id);

				if ( flyer == null ) {
					return NotFound();
				}

				var user = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == guid);
				if ( user == null || flyer == null ) {
					return NotFound();
				}

				user.Flyers.Remove(flyer);

				if ( DB.SaveChanges() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}
	}

	public class FlyersController : ApiController {
		AppDB _db;

		public AppDB DB {
			get { return _db ?? HttpContext.Current.GetOwinContext().Get<AppDB>(); }
			set { _db = value; }
		}

		// GET: api/Flyers
		[EnableQuery()]
		[ResponseType(typeof(Flyer))]
		public IHttpActionResult Get() {
			try {
				return Ok(DB.Flyers.Include(x => x.Category).ToList());

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Flyers/5
		[ResponseType(typeof(Flyer))]
		//[Authorize()]
		public IHttpActionResult Get(int id) {
			try {
				return Ok(DB.Flyers.Include(x => x.Category).FirstOrDefault(x => x.ID == id));
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// POST: api/Flyers
		[ResponseType(typeof(Flyer))]
		public IHttpActionResult Post([FromBody]Flyer flyer) {
			try {
				if ( flyer == null ) {
					return BadRequest("Flyer cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var newFlyer = DB.Flyers.Add(flyer);
				if ( DB.SaveChanges() == 0 ) {
					return Conflict();
				}
				return Created<Flyer>(Request.RequestUri + newFlyer.ID.ToString(), newFlyer);
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// PUT: api/Flyers/5
		public IHttpActionResult Put(int id, [FromBody]Flyer flyer) {
			try {
				if ( flyer == null ) {
					return BadRequest("Flyer cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var getFlyer = DB.Flyers.FirstOrDefault(x => x.ID == id);
				getFlyer = flyer; // TODO Check if this works. Otherwise change it.
				if ( DB.SaveChanges() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// DELETE: api/Flyers/5
		public IHttpActionResult Delete(int id) {
			try {
				if ( id <= 0 ) {
					return BadRequest("ID must be valid");
				}

				var flyer = DB.Flyers.FirstOrDefault(x => x.ID == id);

				if ( flyer == null ) {
					return NotFound();
				}

				DB.Flyers.Remove(flyer);
				if ( DB.SaveChanges() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}
	}

	public class CatsController : ApiController {
		AppDB _db;

		public AppDB DB {
			get { return _db ?? HttpContext.Current.GetOwinContext().Get<AppDB>(); }
			set { _db = value; }
		}

		// GET: api/Cats
		[EnableQuery()]
		[ResponseType(typeof(Category))]
		public IHttpActionResult Get() {
			try {
				return Ok(DB.Categories.Include(x => x.Flyers).ToList());

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Cats/5
		[ResponseType(typeof(Category))]
		//[Authorize()]
		public IHttpActionResult Get(int id) {
			try {
				return Ok(DB.Categories.Include(x => x.Flyers).FirstOrDefault(x => x.ID == id));
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// POST: api/Cats
		[ResponseType(typeof(Category))]
		public IHttpActionResult Post([FromBody]Category category) {
			try {
				if ( category == null ) {
					return BadRequest("Category cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var newCat = DB.Categories.Add(category);
				if ( DB.SaveChanges() == 0 ) {
					return Conflict();
				}
				return Created<Category>(Request.RequestUri + newCat.ID.ToString(), newCat);
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// PUT: api/Cats/5
		public IHttpActionResult Put(int id, [FromBody]Category category) {
			try {
				if ( category == null ) {
					return BadRequest("Category cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var getCat = DB.Categories.FirstOrDefault(x => x.ID == id);
				getCat = category; // TODO Check if this works. Otherwise change it.
				if ( DB.SaveChanges() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// DELETE: api/Cats/5
		public IHttpActionResult Delete(int id) {
			try {
				if ( id <= 0 ) {
					return BadRequest("ID must be valid");
				}

				var cats = DB.Categories.FirstOrDefault(x => x.ID == id);

				if ( cats == null ) {
					return NotFound();
				}

				DB.Categories.Remove(cats);
				if ( DB.SaveChanges() == 0 ) {
					return NotFound();
				}
				return Ok();
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}
	}
}
