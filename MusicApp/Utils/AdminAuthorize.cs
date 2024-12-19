using System.Web;
using System.Web.Mvc;

namespace MusicApp.Utils
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var userRole = httpContext.Session["Role"] as string;
            return userRole == "Admin"; // Allow access only if the role is "Admin"
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirect unauthorized users to a custom access denied page
            filterContext.Result = new RedirectResult("~/Home/Index");
        }
    }
}