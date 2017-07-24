using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using WebAPI_Pure.Models;
using System.Net.Mail;

namespace WebAPI_Pure {
	// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

	public class AppUserManager : UserManager<AppUser> {
		public AppUserManager(IUserStore<AppUser> store) : base(store) { }

		public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context) {
			var manager = new AppUserManager(new UserStore<AppUser>(context.Get<AppDB>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<AppUser>(manager) {
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};
			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator {
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};
			manager.EmailService = new EmailService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if ( dataProtectionProvider != null ) {
				manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}

	public class EmailService : IIdentityMessageService {
		public Task SendAsync(IdentityMessage message) {
			// TODO: Plug in your real email service here to send an email.
			try {
				var mailpath = @"c:\mail\";
				MailMessage o = new MailMessage("noreply@localhost.com", message.Destination, message.Subject, message.Body);
				o.IsBodyHtml = true;
				//SmtpClient client = new SmtpClient("aspmx.l.google.com", 25);
				SmtpClient client = new SmtpClient();
				client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
				client.PickupDirectoryLocation = mailpath;

				//Write out the link to a text file for testing
				System.IO.File.WriteAllText(mailpath + "LatestLink.txt", message.Body);

				return client.SendMailAsync(o);

			} catch ( System.Exception ex ) {
			}
			return null;
		}
	}
}
