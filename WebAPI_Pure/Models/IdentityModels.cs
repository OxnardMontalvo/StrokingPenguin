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
		public int PostalCode { get; set; }
		public string County { get; set; }
		public int? DistrictNumber { get; set; }
		public string DeliveryOrderNumber { get; set; }
		public HashSet<Flyer> Flyers { get; set; }
	}

	public class AppDB : IdentityDbContext<AppUser> {
		public AppDB() : base("DefaultConnection") { }

		public static AppDB Create() {
			return new AppDB();
		}

		public DbSet<Flyer> Flyers { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Message> Messages { get; set; }
	}

	public class Flyer {
		public int ID { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
		public Range Range { get; set; } = new Range();
		public Category Category { get; set; }
		public HashSet<AppUser> Users { get; set; }
	}

	public class Category {
		public int ID { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
		public HashSet<Flyer> Flyers { get; set; }
	}

	public class Message {
		public int ID { get; set; }
		public string Bulletin { get; set; }
	}

	// Helpers

	public class Range {
		public int Min { get; set; } = int.MinValue;
		public int Max { get; set; } = int.MaxValue;
	}

	// VMs
	public class CategoryViewModel {
		[Required]
		public string Name { get; set; }
		[Required]
		public bool Active { get; set; }
	}

	public class FlyerViewModel {
		[Required]
		public string Name { get; set; }
		[Required]
		public bool Active { get; set; }
		public string RangeMax { get; set; }
		public string RangeMin { get; set; }
		[Required]
		public int CategoryID { get; set; }
	}

	public class UserFlyersViewModel {
		[Required]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public bool Selected { get; set; }
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