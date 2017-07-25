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
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebAPI_Pure.Controllers {
	public class BaseApiController : ApiController {
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
	}

	[Authorize(Roles = "Admin")]
	public class UsersController : BaseApiController {
		// GET: api/HoldMeBabyImAnAnimalManAndImFeelingSuchAnAnimalDesire
		[AllowAnonymous]
		[HttpGet]
		[Route("api/Users/HoldMeBabyImAnAnimalManAndImFeelingSuchAnAnimalDesire")]
		public async Task<IHttpActionResult> CheckAdminAndRoles() {
			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DB));

			if ( await DB.Roles.CountAsync() == 0 ) {
				var adminRole = DB.Roles.Add(new IdentityRole { Name = "Admin" });
				DB.Roles.Add(new IdentityRole { Name = "User" });

				var adminEmail = SecretsManager.AdminEmail;
				var adminPass = GeneratePassword();

				var user = new AppUser { UserName = adminEmail, Email = adminEmail };
				var result = await UserManager.CreateAsync(user, adminPass);

				if ( result.Succeeded ) {
					await UserManager.AddToRoleAsync(user.Id, adminRole.Name);
					await DB.SaveChangesAsync();
					return Ok(result);
				}
			}

			return Ok();
		}

		// GET: api/Users/Query/Greta
		[HttpGet]
		[ResponseType(typeof(UserViewModel))]
		[Route("api/Users/Query/{query}")]
		public IHttpActionResult GetUsersByQuery(string query = "") {
			var SearchQuery = query.Trim().ToLower();

			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => ( UserManager.IsInRole(x.Id, "User") &&
				( x.Name.ToLower().Contains(SearchQuery) || x.Address.ToLower().Contains(SearchQuery) ||
				x.PostalCode.ToLower().Contains(SearchQuery) || x.County.ToLower().Contains(SearchQuery) ) )).Select(x => new UserViewModel {
					Id = x.Id,
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
		// GET: api/Users
		[ResponseType(typeof(UserViewModel))]
		[Route("api/Users")]
		public IHttpActionResult Get() {
			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).Select(x => new UserViewModel {
					Id = x.Id,
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

		// GET: api/Users/5/2
		[ResponseType(typeof(UserViewModel))]
		[Route("api/Users/{take}/{page}")]
		public IHttpActionResult Get(int take, int page = 0) {
			if ( take < 1 || page < 1 ) {
				return BadRequest("Invalid");
			}
			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).Select(x => new UserViewModel {
					Id = x.Id,
					Name = x.Name,
					Address = x.Address,
					PostalCode = x.PostalCode,
					County = x.County,
					Email = x.Email,
					DistrictNumber = x.DistrictNumber,
					DeliveryOrderNumber = x.DeliveryOrderNumber
				}).OrderBy(u => u.DistrictNumber).ThenBy(u => u.DeliveryOrderNumber).Skip(take * ( page - 1 )).Take(take).ToList();
				return Ok(users);

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Users/5e19bf87-26e4-4f70-9206-ad209634fca0
		[ResponseType(typeof(UserViewModel))]
		[Route("api/Users/{id}")]
		public IHttpActionResult Get(string id) {
			try {
				UserViewModel vm;
				if ( id.Length > 0 ) {
					vm = DB.Users.Include(x => x.Flyers).Where(x => x.Id == id).Select(x => new UserViewModel {
						Id = x.Id,
						Name = x.Name,
						Address = x.Address,
						PostalCode = x.PostalCode,
						County = x.County,
						Email = x.Email,
						DistrictNumber = x.DistrictNumber,
						DeliveryOrderNumber = x.DeliveryOrderNumber
					}).FirstOrDefault();
				} else {
					vm = new UserViewModel();
				}
				return Ok(vm);
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Users/District/1001/2002
		[HttpGet]
		[ResponseType(typeof(UserViewModel))]
		[Route("api/Users/District/{min?}/{max?}")]
		public IHttpActionResult GetRange(int min = int.MinValue, int max = int.MaxValue) {
			if ( min > max ) min = min ^ max ^ ( max = min );

			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).
					Where(x => x.DistrictNumber != null & x.DistrictNumber >= min && x.DistrictNumber <= max).Select(x => new UserViewModel {
						Id = x.Id,
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

		// GET: api/Users/Districts/8008-8019
		[HttpGet]
		[ResponseType(typeof(UserViewModel))]
		[Route("api/Users/Districts/{query?}")]
		public IHttpActionResult GetRanges(string query = null) {
			int max = int.MaxValue, min = int.MinValue;
			if ( !string.IsNullOrWhiteSpace(query) ) {
				var trimSplit = new[] { ' ', '-', ':', '*', '+', '!', ',', '.' };
				var SearchQuery = new string(query.Trim(trimSplit).ToLower().Where(c => char.IsDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == ',' || c == '.').ToArray()).Split(trimSplit);
				if ( SearchQuery.Length == 1 ) {
					int.TryParse(SearchQuery[0], out max);
					min = max;
				} else if ( SearchQuery.Length > 1 ) {
					int.TryParse(SearchQuery[0], out min);
					int.TryParse(SearchQuery[SearchQuery.Length - 1], out max);
					if ( min > max ) min = min ^ max ^ ( max = min );
				}
			}

			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).
					Where(x => x.DistrictNumber != null & x.DistrictNumber >= min && x.DistrictNumber <= max).Select(x => new UserViewModel {
						Id = x.Id,
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

		// POST: api/Users
		[ResponseType(typeof(UserViewModel))]
		[AllowAnonymous]
		[Route("api/Users")]
		public async Task<IHttpActionResult> Post([FromBody]UserViewModel vm) {
			try {
				if ( vm == null ) {
					return BadRequest("User cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var flyer = await DB.Flyers.FirstOrDefaultAsync();
				if ( flyer == null ) {
					flyer = DB.Flyers.Add(new Flyer { Name = "DEFAULT" });
					//return BadRequest("No flyers available");
				}

				var user = new AppUser {
					Name = vm.Name,
					Address = vm.Address,
					UserName = vm.Email,
					Email = vm.Email,
					PostalCode = new string(vm.PostalCode.Trim().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " "),
					County = vm.County,
					Flyers = new Collection<Flyer>() { flyer }
				};
				var result = await UserManager.CreateAsync(user, GeneratePassword());
				if ( result.Succeeded ) {
					await UserManager.AddToRoleAsync(user.Id, "User");

					var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Link("ConfirmEmail", new { userId = user.Id, code = code });
					await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=" + callbackUrl + ">link</a>");

					await DB.SaveChangesAsync();
					return Ok(result);
				}

				return Created<UserViewModel>(Request.RequestUri + user.Id, vm);
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		[AllowAnonymous]
		[Route("ConfirmEmail", Name = "ConfirmEmail")]
		[HttpGet]
		public async Task<IHttpActionResult> ConfirmEmail(string userId, string code) {
			if ( userId == null || code == null ) {
				return BadRequest("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);
			return Ok(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		// PUT: api/Users/5e19bf87-26e4-4f70-9206-ad209634fca0
		[Route("api/Users/{id}")]
		public async Task<IHttpActionResult> Put(string id, [FromBody]UserViewModel vm) {
			try {
				if ( vm == null ) {
					return Json(( "User cannot be null" ));
				}

				if ( !ModelState.IsValid ) {
					return Json(HttpStatusCode.BadRequest);
				}

				var user = DB.Users.FirstOrDefault(x => x.Id == id);
				user.Name = vm.Name;
				user.Address = vm.Address;
				user.UserName = vm.Email;
				user.Email = vm.Email;
				user.PostalCode = new string(vm.PostalCode.Trim().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " ");
				user.County = vm.County;
				user.DistrictNumber = vm.DistrictNumber;
				user.DeliveryOrderNumber = vm.DeliveryOrderNumber;

				if ( await DB.SaveChangesAsync() == 0 ) {
					return Json(HttpStatusCode.NotFound);
				}
				return Ok();
			} catch ( Exception ex ) {
				return Json(ex);
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

		[AllowAnonymous]
		[Route("ForgotPassword")]
		[HttpGet]
		public async Task<IHttpActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel vm) {
			if ( vm != null && ModelState.IsValid ) {
				var user = await UserManager.FindByNameAsync(vm.Email);
				if ( user == null || !( await UserManager.IsEmailConfirmedAsync(user.Id) ) ) {
					// Don't reveal that the user does not exist or is not confirmed
					return Ok("ForgotPasswordConfirmation");
				}

				var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Link("RecoverPasswordResponse", new { userId = user.Id, code = code });
				await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
				return Ok("ForgotPasswordConfirmation");
			}
			// If we got this far, something failed, redisplay form
			return BadRequest();
		}

		[Authorize(Roles = "Admin, User")]
		[Route("api/Users/ChangePassword")]
		[HttpPost]
		public async Task<IHttpActionResult> ChangePassword([FromBody]ChangePasswordBindingModel bm) {
			var user = User.Identity.GetUserId();
			if ( !ModelState.IsValid || user == null ) {
				return BadRequest(ModelState);
			}
			var result = await UserManager.ChangePasswordAsync(user, bm.OldPassword, bm.NewPassword);
			return Ok(result);
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("ResetPassword", Name = "ResetPassword")]
		public async Task<IHttpActionResult> ResetPassword([FromBody]ResetPasswordViewModel model) {
			if ( !ModelState.IsValid ) {
				return BadRequest(ModelState);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if ( user == null || user.Id != model.ID) {
				// Don't reveal that the user does not exist
				return Ok();
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if ( result.Succeeded ) {
				return Ok();
			}
			return Ok();
		}

		#region Helpers
		public string GeneratePassword() {
			string password = "";
			string passwordChars = "abcdefghijklmnopqrstuvwxyzåäöæøå0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖÆØÅ_*$?&=!%{}()/";
			Random r = new Random();
			int length = r.Next(20, 32);
			for ( int i = 0; i <= length; i++ )
				password += passwordChars.Substring(r.Next(0, passwordChars.Length - 1), 1);
			return password;
		}
		#endregion
	}

	#region Helpers

	public static class SecretsManager {
		public static string AdminEmail {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["adminEmail"]; }
		}
	}
	#endregion

	[Authorize(Roles = "User")]
	public class UserFlyersController : BaseApiController {
		// GET: api/UserFlyers/5e19bf87-26e4-4f70-9206-ad209634fca0
		[EnableQuery()]
		[ResponseType(typeof(Flyer))]
		[Route("api/UserFlyers/{guid}")]
		public IHttpActionResult Get(string guid) {
			try {
				if ( User.Identity.GetUserId() != guid ) {
					return BadRequest("User do not match.");
				}
				return Ok(DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == guid).Flyers);

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Flyers/5e19bf87-26e4-4f70-9206-ad209634fca0/5
		[ResponseType(typeof(Flyer))]
		[Route("api/UserFlyers/{guid}/{id}")]
		public IHttpActionResult Get(string guid, int id) {
			try {
				if ( User.Identity.GetUserId() != guid ) {
					return BadRequest("User do not match.");
				}
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

				if ( User.Identity.GetUserId() != guid ) {
					return BadRequest("User do not match.");
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

				if ( User.Identity.GetUserId() != guid ) {
					return BadRequest("User do not match.");
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

				if ( User.Identity.GetUserId() != guid ) {
					return BadRequest("User do not match.");
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

	[Authorize(Roles = "Admin")]
	public class FlyersController : BaseApiController {
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

	[Authorize(Roles = "Admin")]
	public class CatsController : BaseApiController {
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