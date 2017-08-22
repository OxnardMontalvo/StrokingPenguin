using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebAPI_Pure.Misc {
	public class EnforceHttpsHandler : DelegatingHandler {
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			// if request is local, just serve it without https
			object httpContextBaseObject;
			if ( request.Properties.TryGetValue("MS_HttpContext", out httpContextBaseObject) ) {
				var httpContextBase = httpContextBaseObject as HttpContextBase;

				if ( httpContextBase != null && httpContextBase.Request.IsLocal ) {
					return base.SendAsync(request, cancellationToken);
				}
			}

			// if request is remote, enforce https
			if ( request.RequestUri.Scheme != Uri.UriSchemeHttps ) {
				return Task<HttpResponseMessage>.Factory.StartNew(
					() => {
						var response = new HttpResponseMessage(HttpStatusCode.Forbidden) {
							Content = new StringContent("HTTPS Required")
						};

						return response;
					});
			}

			return base.SendAsync(request, cancellationToken);
		}
	}

	public class RequreSecureConnectionFilter : RequireHttpsAttribute {
		public override void OnAuthorization(AuthorizationContext filterContext) {
			if ( filterContext == null ) {
				throw new ArgumentNullException("filterContext");
			}

			if ( filterContext.HttpContext.Request.IsLocal ) {
				// when connection to the application is local, don't do any HTTPS stuff
				return;
			}

			base.OnAuthorization(filterContext);
		}
	}
}