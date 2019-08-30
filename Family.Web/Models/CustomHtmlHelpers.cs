using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Family.Web.Services;

namespace Family.Web.Models
{
    /// <summary>
    /// Custom Html Helpers
    /// </summary>
    public static class CustomHtmlHelpers
    {
        /// <summary>
        /// Returns "Yes" or "No" for a boolean value (for display)
        /// </summary>
        /// <param name="htmlHelper">The Html Helper object</param>
        /// <param name="yesNo">The boolean value for yes or no</param>
        /// <returns>The "yes" or "no" string</returns>
        public static MvcHtmlString YesNo(this HtmlHelper htmlHelper, bool yesNo)
        {
            var text = yesNo ? "Yes" : "No";
            return new MvcHtmlString(text);
        }

        /// <summary>
        /// Returns the name of the currently logged in user for display
        /// </summary>
        /// <param name="htmlHelper">The Html helper object</param>
        /// <returns>The name of the currently logged in user</returns>
        public static MvcHtmlString GetName(this HtmlHelper htmlHelper)
        {
            UserServices service = new UserServices();
            return new MvcHtmlString(service.GetUser().Name);
        }
    }
}