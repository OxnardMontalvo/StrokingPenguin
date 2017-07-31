using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_Pure.Models {
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class AppUser : IdentityUser {
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager, string authenticationType) {
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
			// Add custom user claims here
			return userIdentity;
		}

		public string Name { get; set; }
		public string Address { get; set; }
		public string PostalCode { get; set; }
		public string County { get; set; }
		public int? DistrictNumber { get; set; }
		public string DeliveryOrderNumber { get; set; }
		public ICollection<Flyer> Flyers { get; set; }
	}

	public class AppDB : IdentityDbContext<AppUser> {
		public AppDB() : base("StrokingPenguin", throwIfV1Schema: false) { }

		public static AppDB Create() {
			return new AppDB();
		}

		public DbSet<Flyer> Flyers { get; set; }
		public DbSet<Category> Categories { get; set; }
	}

	public class Flyer {
		public int ID { get; set; }
		public string Name { get; set; }
		public Category Category { get; set; }
		public ICollection<AppUser> Users { get; set; }
	}

	public class Category {
		public int ID { get; set; }
		public string Name { get; set; }
		public ICollection<Flyer> Flyers { get; set; }
	}

	public class UserViewModel {
		public string Id { get; set; }
		[Required]
		[StringLength(128, MinimumLength = 3)]
		public string Name { get; set; }
		[Required]
		[StringLength(128)]
		public string Address { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		[StringLength(6, MinimumLength = 5)]
		public string PostalCode { get; set; }
		[Required]
		[StringLength(128)]
		public string County { get; set; }
		public int? DistrictNumber { get; set; }
		public string DeliveryOrderNumber { get; set; }
	}

	public class ChangePasswordBindingModel {
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(512, MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class ForgotPasswordViewModel {
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class ResetPasswordViewModel {
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(512, MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
		public string ID { get; set; }
	}
}