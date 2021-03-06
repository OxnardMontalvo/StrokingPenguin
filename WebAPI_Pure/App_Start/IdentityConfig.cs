﻿using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Net.Mail;
using WebAPI_Pure.Models;
using WebAPI_Pure.Controllers;

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
				manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(dataProtectionProvider.Create("Happiness so hard to find. We'll never win this game. And tomorrow will be the same"));
			}
			return manager;
		}
	}

	public class EmailService : IIdentityMessageService {
		public Task SendAsync(IdentityMessage message) {
			// TODO: Plug in your real email service here to send an email.
			try {
				MailMessage mMessage = new MailMessage(SecretsManager.SendMail, message.Destination, message.Subject, message.Body);
				mMessage.IsBodyHtml = true;
				SmtpClient client = new SmtpClient();

				#region REMOVE
				var mailpath = @"c:\mail\";
				client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
				client.PickupDirectoryLocation = mailpath;

				//Write out the link to a text file for testing
				System.IO.File.WriteAllText(mailpath + "LatestLink.txt", message.Body);
				#endregion


				/*
					client.Host = SecretsManager.SMTPServer;
					client.EnableSsl = true;
					client.Port = 25;

					client.UseDefaultCredentials = false;
					client.Credentials = new System.Net.NetworkCredential(SecretsManager.SendUserName, SecretsManager.SendPassword);
				*/

				return client.SendMailAsync(mMessage);
			} catch {
			}
			return null;
		}
	}
}
