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
using System.Web.Http.OData;
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
			try {
				if ( !DB.Database.Exists() || await DB.Roles.CountAsync() == 0 ) {
					var adminRole = DB.Roles.Add(new IdentityRole { Name = "Admin" });
					DB.Roles.Add(new IdentityRole { Name = "User" });

					var adminEmail = SecretsManager.AdminEmail;
					var adminPass = GeneratePassword();

					var user = new AppUser { UserName = adminEmail, Email = adminEmail };
					var result = await UserManager.CreateAsync(user, adminPass);

					if ( result.Succeeded ) {
						await UserManager.AddToRoleAsync(user.Id, adminRole.Name);
						await DB.SaveChangesAsync();

						var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
						var callbackUrl = @"http://" + HttpContext.Current.Request.Url.Authority + $"/#!/ConfirmEmail/{user.Id}/{code.Replace('/', '_').Replace('+', '!')}";
						await UserManager.SendEmailAsync(user.Id, "Bekräfta er epost", "Var vänlig bekräfta att er epost är korrekt genom att klicka på länken: <a href=" + callbackUrl + ">länk</a>");

						return Ok(result);
					}
				}

				return Ok();

			} catch {
				return InternalServerError();
			}
		}

		// GET: api/Users/Query/Greta
		[HttpGet]
		[Route("api/Users/Query/{query?}")]
		public IHttpActionResult GetUsersByQuery(string query = "") {
			try {
				var SearchQuery = query.Trim().ToLower();
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => ( UserManager.IsInRole(x.Id, "User") &&
				( x.Name.ToLower().Contains(SearchQuery) || x.Address.ToLower().Contains(SearchQuery) ||
				x.PostalCode.ToString().Contains(SearchQuery) || x.County.ToLower().Contains(SearchQuery) ) )).Select(x => new {
					Id = x.Id,
					Name = x.Name,
					Address = x.Address,
					PostalCode = new string(x.PostalCode.ToString().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " "),
					County = x.County,
					Email = x.Email,
					DistrictNumber = x.DistrictNumber,
					DeliveryOrderNumber = x.DeliveryOrderNumber,
					Flyers = ( x.Flyers.Where(z => z.Range.Min <= x.PostalCode && z.Range.Max >= x.PostalCode && z.Active == true).Select(y => new {
						ID = y.ID,
						Name = y.Name
					}).OrderBy(y => y.Name) )
				}).OrderBy(u => u.DistrictNumber).ThenBy(u => u.DeliveryOrderNumber).ToList();
				return Ok(users);

			} catch {
				return InternalServerError();
			}
		}
		// GET: api/Users
		[Route("api/Users")]
		public IHttpActionResult Get() {
			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).Select(x => new {
					Id = x.Id,
					Name = x.Name,
					Address = x.Address,
					PostalCode = new string(x.PostalCode.ToString().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " "),
					County = x.County,
					Email = x.Email,
					DistrictNumber = x.DistrictNumber,
					DeliveryOrderNumber = x.DeliveryOrderNumber,
					Flyers = ( x.Flyers.Where(z => z.Range.Min <= x.PostalCode && z.Range.Max >= x.PostalCode && z.Active == true).Select(y => new {
						ID = y.ID,
						Name = y.Name
					}).OrderBy(y => y.Name) )
				}).OrderBy(u => u.DistrictNumber).ThenBy(u => u.DeliveryOrderNumber).ToList();

				return Ok(users);

			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		// GET: api/Users/5/2
		[Route("api/Users/{take}/{page}")]
		public IHttpActionResult Get(int take, int page = 0) {
			if ( take < 1 || page < 1 ) {
				return BadRequest("Invalid");
			}
			try {
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).Select(x => new {
					Id = x.Id,
					Name = x.Name,
					Address = x.Address,
					PostalCode = new string(x.PostalCode.ToString().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " "),
					County = x.County,
					Email = x.Email,
					DistrictNumber = x.DistrictNumber,
					DeliveryOrderNumber = x.DeliveryOrderNumber,
					Flyers = ( x.Flyers.Where(z => z.Range.Min <= x.PostalCode && z.Range.Max >= x.PostalCode && z.Active == true).Select(y => new {
						ID = y.ID,
						Name = y.Name
					}).OrderBy(y => y.Name) )
				}).OrderBy(u => u.DistrictNumber).ThenBy(u => u.DeliveryOrderNumber).Skip(take * ( page - 1 )).Take(take).ToList();
				return Ok(users);

			} catch {
				return InternalServerError();
			}
		}

		// GET: api/Users/5e19bf87-26e4-4f70-9206-ad209634fca0
		[Route("api/Users/{id}")]
		public IHttpActionResult Get(string id) {
			try {
				if ( string.IsNullOrWhiteSpace(id) ) {
					return BadRequest("ID not found");
				}

				var user = DB.Users.Include(x => x.Flyers).FirstOrDefault(x => x.Id == id);

				if ( !UserManager.IsInRole(user.Id, "User") ) {
					return BadRequest("User is not in user role.");
				}

				var o = new {
					Id = user.Id,
					Name = user.Name,
					Address = user.Address,
					PostalCode = new string(user.PostalCode.ToString().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " "),
					County = user.County,
					Email = user.Email,
					DistrictNumber = user.DistrictNumber,
					DeliveryOrderNumber = user.DeliveryOrderNumber,
					Flyers = ( user.Flyers.Where(z =>
						z.Range.Min <= user.PostalCode &&
						z.Range.Max >= user.PostalCode &&
						z.Active == true).Select(y => new {
							ID = y.ID,
							Name = y.Name
						}).OrderBy(y => y.Name) )
				};

				return Ok(o);
			} catch {
				return InternalServerError();
			}
		}

		// GET: api/Users/District/1001/2002
		[HttpGet]
		[Route("api/Users/District/{min?}/{max?}")]
		public IHttpActionResult GetRange(int min = int.MinValue, int max = int.MaxValue) {
			try {
				if ( min > max ) min = min ^ max ^ ( max = min );
				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).
					Where(x => x.DistrictNumber != null & x.DistrictNumber >= min && x.DistrictNumber <= max).Select(x => new {
						Id = x.Id,
						Name = x.Name,
						Address = x.Address,
						PostalCode = new string(x.PostalCode.ToString().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " "),
						County = x.County,
						Email = x.Email,
						DistrictNumber = x.DistrictNumber,
						DeliveryOrderNumber = x.DeliveryOrderNumber,
						Flyers = ( x.Flyers.Where(z => z.Range.Min <= x.PostalCode && z.Range.Max >= x.PostalCode && z.Active == true).Select(y => new {
							ID = y.ID,
							Name = y.Name
						}).OrderBy(y => y.Name) )
					}).OrderBy(u => u.DistrictNumber).ThenBy(u => u.DeliveryOrderNumber).ToList();
				return Ok(users);

			} catch {
				return InternalServerError();
			}
		}

		// GET: api/Users/Districts/8008-8019
		[HttpGet]
		[Route("api/Users/Districts/{query?}")]
		public IHttpActionResult GetRanges(string query = null) {
			try {
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

				var users = DB.Users.Include(x => x.Flyers).ToList().Where(x => UserManager.IsInRole(x.Id, "User")).
					Where(x => x.DistrictNumber != null & x.DistrictNumber >= min && x.DistrictNumber <= max).Select(x => new {
						Id = x.Id,
						Name = x.Name,
						Address = x.Address,
						PostalCode = new string(x.PostalCode.ToString().Where(c => char.IsDigit(c)).ToArray()).Insert(3, " "),
						County = x.County,
						Email = x.Email,
						DistrictNumber = x.DistrictNumber,
						DeliveryOrderNumber = x.DeliveryOrderNumber,
						Flyers = ( x.Flyers.Where(z => z.Range.Min <= x.PostalCode && z.Range.Max >= x.PostalCode && z.Active == true).Select(y => new {
							ID = y.ID,
							Name = y.Name
						}).OrderBy(y => y.Name) )
					}).OrderBy(u => u.DistrictNumber).ThenBy(u => u.DeliveryOrderNumber).ToList();
				return Ok(users);

			} catch {
				return InternalServerError();
			}
		}

		// POST: api/Users
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
				var cat = await DB.Categories.FirstOrDefaultAsync();

				if ( flyer == null ) {
					if ( cat == null )
						cat = DB.Categories.Add(new Category { Name = SecretsManager.DefaultCat, Active = true });
					flyer = DB.Flyers.Add(new Flyer { Name = SecretsManager.DefaultFlyer, Active = true, Category = cat });
				}

				int postalCode;
				if ( !int.TryParse(new string(vm.PostalCode.Where(Char.IsDigit).ToArray()), out postalCode) ) {
					return BadRequest();
				}

				var user = new AppUser {
					Name = vm.Name,
					Address = vm.Address,
					UserName = vm.Email,
					Email = vm.Email,
					PostalCode = postalCode,
					County = vm.County,
					Flyers = new HashSet<Flyer>() { flyer }
				};

				var result = await UserManager.CreateAsync(user, GeneratePassword());
				if ( result.Succeeded ) {
					await UserManager.AddToRoleAsync(user.Id, "User");

					var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = @"http://" + HttpContext.Current.Request.Url.Authority + $"/#!/ConfirmEmail/{user.Id}/{code.Replace('/', '_').Replace('+', '!')}";
					await UserManager.SendEmailAsync(user.Id, "Bekräfta er epost", "Var vänlig bekräfta att er epost är korrekt genom att klicka på länken: <a href=" + callbackUrl + ">länk</a>");

					await DB.SaveChangesAsync();
					return Ok(result);
				} else {
					return BadRequest();
				}
			} catch ( Exception ex ) {
				return InternalServerError(ex);
			}
		}

		[AllowAnonymous]
		[Route("ConfirmEmail", Name = "ConfirmEmail")]
		[HttpGet]
		public async Task<IHttpActionResult> ConfirmEmail(string userId, string code) {
			// Added to make sure the code is corrected again.
			if ( userId == null || code == null ) {
				return BadRequest("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code.Replace('_', '/').Replace('!', '+'));
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

				int postalCode;
				if ( !int.TryParse(new string(vm.PostalCode.Where(Char.IsDigit).ToArray()), out postalCode) ) {
					return BadRequest();
				}

				var user = DB.Users.FirstOrDefault(x => x.Id == id);
				user.Name = vm.Name;
				user.Address = vm.Address;
				user.UserName = vm.Email;
				user.Email = vm.Email;
				user.PostalCode = postalCode;
				user.County = vm.County;
				user.DistrictNumber = vm.DistrictNumber;
				user.DeliveryOrderNumber = vm.DeliveryOrderNumber;

				if ( await DB.SaveChangesAsync() == 0 ) {
					return Json(HttpStatusCode.NotFound);
				}
				return Ok();
			} catch {
				return InternalServerError();
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
			} catch {
				return InternalServerError();
			}
		}

		[AllowAnonymous]
		[Route("ForgotPassword")]
		[HttpGet]
		// Change from [FromBody] to [FromUri] to send info thru url.
		public async Task<IHttpActionResult> ForgotPassword([FromUri]ForgotPasswordViewModel vm) {
			if ( ModelState.IsValid ) {
				var user = await UserManager.FindByNameAsync(vm.Email);
				if ( user == null || !( await UserManager.IsEmailConfirmedAsync(user.Id) ) ) {
					// Don't reveal that the user does not exist or is not confirmed
					return Ok("ForgotPasswordConfirmation");
				}

				var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				string callbackUrl = @"http://" + HttpContext.Current.Request.Url.Authority + $"/#!/RecoverPassword/{user.Id}/{code.Replace('/', '_').Replace('+', '!')}";
				await UserManager.SendEmailAsync(user.Id, "Återställning av Lösenord", "Återställ ert lösenord genom att klicka på länken: <a href=\"" + callbackUrl + "\">länk</a>");
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
		public async Task<IHttpActionResult> ResetPassword([FromBody]ResetPasswordViewModel vm) {
			if ( !ModelState.IsValid ) {
				return BadRequest(ModelState);
			}

			var user = await UserManager.FindByNameAsync(vm.Email);
			if ( user == null || user.Id != vm.ID ) {
				// Don't reveal that the user does not exist
				return Ok();
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, vm.Code.Replace('_', '/').Replace('!', '+'), vm.Password);
			if ( result.Succeeded ) {
				return Ok();
			}
			return Ok();
		}

		#region Helpers
		public string GeneratePassword() {
			string password = "";
			string passwordChars = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃ";
			Random r = new Random();
			int length = r.Next(128, 256);
			for ( int i = 0; i <= length; i++ )
				password += passwordChars.Substring(r.Next(0, passwordChars.Length - 1), 1);
			return password;
		}
		#endregion
	}

	#region SecretsManager

	public static class SecretsManager {
		public static string AdminEmail {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["adminEmail"]; }
		}
		public static string DefaultFlyer {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["defaultFlyer"]; }
		}
		public static string DefaultCat {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["defaultCat"]; }
		}


		public static string SMTPServer {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["smtpServer"]; }
		}
		public static int SMTPServerPort {
			get {
				int port;
				int.TryParse(System.Web.Configuration.WebConfigurationManager.AppSettings["smtpServerPort"], out port);
				return port;
			}
		}
		public static string SendMail {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["sendMail"]; }
		}
		public static string SendPassword {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["endPassword"]; }
		}
		public static string SendToTest {
			get { return System.Web.Configuration.WebConfigurationManager.AppSettings["sendToTest"]; }
		}

	}
	#endregion


	//[Authorize] // For testing
	[Authorize(Roles = "User")]
	public class UserFlyersController : BaseApiController {
		[Route("api/UserFlyers")]
		public async Task<IHttpActionResult> Get() {
			try {
				var guid = User.Identity.GetUserId();
				if ( guid == null ) {
					return NotFound();
				}

				var user = await DB.Users.Include(x => x.Flyers).FirstOrDefaultAsync(x => x.Id == guid);
				var cats = DB.Categories.Include(x => x.Flyers).Where(x => x.Active == true && x.Flyers.Count > 0);

				if ( user == null || cats == null ) {
					return NotFound();
				}

				var result = cats.Select(c => new {
					Name = c.Name,
					Flyers = ( c.Flyers.Where(z => z.Range.Min <= user.PostalCode && z.Range.Max >= user.PostalCode && z.Active == true).Select(x => new {
						ID = x.ID,
						Name = x.Name,
						Selected = user.Flyers.Contains(x)
					}).OrderBy(y => y.Name) )
				});

				return Ok(result.Where(x => x.Flyers.Count() > 0));
			} catch {
				return InternalServerError();
			}
		}

		[Route("api/UserFlyers/{id}")]
		public async Task<IHttpActionResult> Get(int id) {
			try {
				var guid = User.Identity.GetUserId();
				if ( guid == null ) {
					return NotFound();
				}

				var user = await DB.Users.Include(x => x.Flyers).FirstOrDefaultAsync(x => x.Id == guid);
				var cat = await DB.Categories.Include(x => x.Flyers).FirstOrDefaultAsync(x => x.ID == id);

				if ( user == null || cat == null ) {
					return NotFound();
				}

				var result = cat.Flyers.Select(x => new { ID = x.ID, Name = x.Name, Selected = user.Flyers.Contains(x) });

				return Ok(result);
			} catch {
				return InternalServerError();
			}
		}

		[Route("api/UserFlyers")]
		public async Task<IHttpActionResult> Post([FromBody]HashSet<UserFlyersViewModel> vm) {
			try {
				var guid = User.Identity.GetUserId();
				if ( guid == null ) {
					return NotFound();
				}

				var user = await DB.Users.Include(x => x.Flyers).FirstOrDefaultAsync(x => x.Id == guid);
				var flyers = new HashSet<Flyer>(DB.Flyers.Where(x => x.Category.Active == true && x.Active == true && x.Category.Flyers.Count > 0));

				if ( user == null || flyers == null ) {
					return NotFound();
				}

				flyers.RemoveWhere(x => vm.FirstOrDefault(y => y.ID == x.ID).Selected != true);
				user.Flyers.RemoveWhere(x => vm.FirstOrDefault(y => y.ID == x.ID).Selected != true);

				foreach ( var f in flyers ) {
					if ( !user.Flyers.Contains(f) ) {
						user.Flyers.Add(f);
					}
				}

				var result = await DB.SaveChangesAsync();

				if ( result == 0 ) {
					return Conflict();
				}
				return Ok(user.Flyers);
			} catch {
				return InternalServerError();
			}
		}
	}


	[Authorize(Roles = "Admin")]
	public class FlyersController : BaseApiController {
		// GET: api/Flyers
		[EnableQuery()]
		[Route("api/Flyers")]
		public IHttpActionResult Get() {
			try {
				var flyers = DB.Flyers.Include(x => x.Category).Select(x => new {
					Name = x.Name,
					Active = x.Active,
					Category = new { ID = x.Category.ID, Name = x.Category.Name, Active = x.Category.Active },
					ID = x.ID,
					Range = new { Max = ( x.Range.Max == int.MaxValue ? "" : x.Range.Max.ToString() ), Min = ( x.Range.Min == int.MinValue ? "" : x.Range.Min.ToString() ) }
				});
				return Ok(flyers);

			} catch {
				return InternalServerError();
			}
		}

		// GET: api/Flyers/5
		[Route("api/Flyers/{id}")]
		public async Task<IHttpActionResult> Get(int id) {
			try {
				var flyer = await DB.Flyers.Include(x => x.Category).Select(x => new {
					Name = x.Name,
					Active = x.Active,
					Category = new { ID = x.Category.ID, Name = x.Category.Name, Active = x.Category.Active },
					ID = x.ID,
					Range = new { Max = ( x.Range.Max == int.MaxValue ? "" : x.Range.Max.ToString() ), Min = ( x.Range.Min == int.MinValue ? "" : x.Range.Min.ToString() ) }
				}).FirstOrDefaultAsync(x => x.ID == id);
				return Ok(flyer);
			} catch {
				return InternalServerError();
			}
		}

		// POST: api/Flyers
		[Route("api/Flyers")]
		public async Task<IHttpActionResult> Post([FromBody]FlyerViewModel vm) {
			try {
				if ( vm == null ) {
					return BadRequest("Flyer cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				int rangeMax = int.MaxValue, rangeMin = int.MinValue;
				if ( !string.IsNullOrWhiteSpace(vm.RangeMax) ) {
					int.TryParse(new string(vm.RangeMax.Trim().Where(c => Char.IsDigit(c)).ToArray()), out rangeMax);
					if ( rangeMax == 0 ) {
						rangeMax = int.MaxValue;
					}
				}
				if ( !string.IsNullOrWhiteSpace(vm.RangeMin) ) {
					int.TryParse(new string(vm.RangeMin.Trim().Where(c => Char.IsDigit(c)).ToArray()), out rangeMin);
					if ( rangeMin == 0 ) {
						rangeMin = int.MinValue;
					}
				}

				if ( rangeMin > rangeMax ) rangeMin = rangeMin ^ rangeMax ^ ( rangeMax = rangeMin );
				var range = new Range { Max = rangeMax, Min = rangeMin };

				var cat = await DB.Categories.FirstOrDefaultAsync(x => x.ID == vm.CategoryID);
				var flyer = new Flyer { Name = vm.Name, Active = vm.Active, Category = cat, Range = range };

				DB.Flyers.Add(flyer);

				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Conflict();
				} else {
					return Ok($"Flyer {flyer.Name} created.");
				}
			} catch {
				return InternalServerError();
			}
		}

		// PUT: api/Flyers/5
		[Route("api/Flyers/{id}")]
		public async Task<IHttpActionResult> Put(int id, [FromBody]FlyerViewModel vm) {
			try {
				if ( vm == null ) {
					return BadRequest("Flyer cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var flyer = await DB.Flyers.FirstOrDefaultAsync(x => x.ID == id);

				int rangeMax = flyer.Range.Max, rangeMin = flyer.Range.Min;
				if ( !string.IsNullOrWhiteSpace(vm.RangeMax) ) {
					int.TryParse(new string(vm.RangeMax.Trim().Where(c => Char.IsDigit(c)).ToArray()), out rangeMax);
					if ( rangeMax == 0 ) {
						rangeMax = flyer.Range.Max;
					}
				}
				if ( !string.IsNullOrWhiteSpace(vm.RangeMin) ) {
					int.TryParse(new string(vm.RangeMin.Trim().Where(c => Char.IsDigit(c)).ToArray()), out rangeMin);
					if ( rangeMin == 0 ) {
						rangeMin = flyer.Range.Min;
					}
				}

				if ( rangeMin > rangeMax ) rangeMin = rangeMin ^ rangeMax ^ ( rangeMax = rangeMin );
				var range = new Range { Max = rangeMax, Min = rangeMin };

				var cat = await DB.Categories.FirstOrDefaultAsync(x => x.ID == vm.CategoryID);
				flyer.Name = vm.Name;
				flyer.Active = vm.Active;
				flyer.Category = cat;
				flyer.Range = range;

				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Conflict();
				} else {
					return Ok($"Flyer {flyer.Name} updated.");
				}
			} catch {
				return InternalServerError();
			}
		}

		// DELETE: api/Flyers/5
		[Route("api/Flyers/{id}")]
		public async Task<IHttpActionResult> Delete(int id) {
			try {
				if ( id <= 0 ) {
					return BadRequest("ID must be valid");
				}

				var flyer = DB.Flyers.FirstOrDefault(x => x.ID == id);

				if ( flyer == null ) {
					return NotFound();
				}

				DB.Flyers.Remove(flyer);
				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Conflict();
				} else {
					return Ok($"Flyer {flyer.Name} deleted.");
				}
			} catch {
				return InternalServerError();
			}
		}
	}


	[Authorize(Roles = "Admin")]
	public class CatsController : BaseApiController {
		// GET: api/Cats
		[Route("api/Cats")]
		public IHttpActionResult Get() {
			try {
				var cats = DB.Categories.Include(x => x.Flyers).Select(x => new {
					Name = x.Name,
					Active = x.Active,
					ID = x.ID,
					Flyers = ( x.Flyers.Select(y => new {
						ID = y.ID,
						Name = y.Name,
						Active = y.Active,
						Range = new { Max = ( y.Range.Max == int.MaxValue ? "" : y.Range.Max.ToString() ), Min = ( y.Range.Min == int.MinValue ? "" : y.Range.Min.ToString() ) }
					}) )
				});
				return Ok(cats);
			} catch {
				return InternalServerError();
			}
		}

		// GET: api/Cats/5
		[Route("api/Cats/{id}")]
		public async Task<IHttpActionResult> Get(int id) {
			try {
				var cat = await DB.Categories.Include(x => x.Flyers).Select(x => new {
					Name = x.Name,
					Active = x.Active,
					ID = x.ID,
					Flyers = ( x.Flyers.Select(y => new {
						ID = y.ID,
						Name = y.Name,
						Active = y.Active,
						Range = new { Max = ( y.Range.Max == int.MaxValue ? "" : y.Range.Max.ToString() ), Min = ( y.Range.Min == int.MinValue ? "" : y.Range.Min.ToString() ) }
					}) )
				}).FirstOrDefaultAsync(x => x.ID == id);
				return Ok(cat);
			} catch {
				return InternalServerError();
			}
		}

		// POST: api/Cats
		[Route("api/Cats")]
		public async Task<IHttpActionResult> Post([FromBody]CategoryViewModel vm) {
			try {
				if ( vm == null ) {
					return BadRequest("Category cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var cat = DB.Categories.Add(new Category { Name = vm.Name, Active = vm.Active });

				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Conflict();
				} else {
					return Ok($"Category {cat.Name} created.");
				}
			} catch {
				return InternalServerError();
			}
		}

		// PUT: api/Cats/5
		[Route("api/Cats/{id}")]
		public async Task<IHttpActionResult> Put(int id, [FromBody]CategoryViewModel vm) {
			try {
				if ( vm == null ) {
					return BadRequest("Category cannot be null");
				}

				if ( !ModelState.IsValid ) {
					return BadRequest(ModelState);
				}

				var cat = await DB.Categories.FirstOrDefaultAsync(x => x.ID == id);
				cat.Name = vm.Name;
				cat.Active = vm.Active;

				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Ok("No changes made.");
				} else {
					return Ok($"Category {cat.Name} updated.");
				}
			} catch {
				return InternalServerError();
			}
		}

		// DELETE: api/Cats/5
		[Route("api/Cats/{id}")]
		public async Task<IHttpActionResult> Delete(int id) {
			try {
				if ( id <= 0 ) {
					return BadRequest("ID must be valid");
				}

				var cat = DB.Categories.FirstOrDefault(x => x.ID == id);

				if ( cat == null ) {
					return NotFound();
				}

				DB.Categories.Remove(cat);
				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Conflict();
				} else {
					return Ok($"Category {cat.Name} deleted.");
				}
			} catch {
				return InternalServerError();
			}
		}
	}

	[Authorize]
	public class MessagesController : BaseApiController {
		// GET: api/Messages
		[Route("api/Messages")]
		public IHttpActionResult Get() {
			try {
				var mess = DB.Messages.ToList();

				return Ok(mess);
			} catch {
				return InternalServerError();
			}
		}

		// GET: api/Messages/5
		[Route("api/Messages/{id}")]
		public async Task<IHttpActionResult> Get(int id) {
			try {
				var mess = await DB.Messages.FirstOrDefaultAsync(x => x.ID == id);

				return Ok(mess);
			} catch {
				return InternalServerError();
			}
		}


		// POST: api/Messages
		[Route("api/Messages")]
		public async Task<IHttpActionResult> Post([FromBody]string message) {
			try {
				if ( string.IsNullOrWhiteSpace(message) ) {
					return BadRequest("Message cannot be null");
				}

				var mess = DB.Messages.Add(new Message { Bulletin = message });

				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Conflict();
				} else {
					return Ok($"Message '{mess.Bulletin}' created.");
				}
			} catch {
				return InternalServerError();
			}
		}

		// PUT: api/Messages/5
		[Route("api/Messages/{id}")]
		public async Task<IHttpActionResult> Put(int id, [FromBody]string message) {
			try {
				if ( string.IsNullOrWhiteSpace(message) ) {
					return BadRequest("Message cannot be null");
				}

				var mess = await DB.Messages.FirstOrDefaultAsync(x => x.ID == id);
				mess.Bulletin = message;

				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Ok("No changes made.");
				} else {
					return Ok($"Message '{mess.Bulletin}' updated.");
				}
			} catch {
				return InternalServerError();
			}
		}

		// DELETE: api/Messages/5
		[Route("api/Messages/{id}")]
		public async Task<IHttpActionResult> Delete(int id) {
			try {
				var mess = await DB.Messages.FirstOrDefaultAsync(x => x.ID == id);

				if ( mess == null ) {
					return NotFound();
				}

				DB.Messages.Remove(mess);
				var result = await DB.SaveChangesAsync();
				if ( result == 0 ) {
					return Conflict();
				} else {
					return Ok($"Message '{mess.Bulletin}' deleted.");
				}
			} catch {
				return InternalServerError();
			}
		}
	}
}