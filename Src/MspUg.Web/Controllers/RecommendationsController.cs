using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LaunchSitecore.Configuration.SiteUI;
using LaunchSitecore.Models;
using MspUg.Web.Models;
using Sitecore.Data;

namespace MspUg.Web.Controllers
{
    public class RecommendationsController : LaunchSitecoreBaseController
    {
        // GET: Recommendations
        public ActionResult PathRecommendations()
        {
	        if (!Sitecore.Context.PageMode.IsNormal)
	        {
		        return ShowEditorAlert();
	        }
            return View("Recommendations", new Recommendations());
        }

		private ActionResult ShowEditorAlert()
		{
			if (!IsPageEditorEditing)
			{
				return null;
			}
			return base.View("ShowPageEditorAlert", new PageEditorAlert(ID.Parse("{8E8CE450-ED8F-4AC9-ACF6-5999457F4BE7}")));
		}

	}
}