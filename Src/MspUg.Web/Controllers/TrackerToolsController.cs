using System;
using System.Globalization;
using System.Net;
using System.Web.Mvc;
using LaunchSitecore.Models;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Mvc.Controllers;

namespace MspUg.Web.Controllers
{
	public class TrackerToolsController : SitecoreController
	{
		public ActionResult FlushSession()
		{
			if (Tracker.IsActive && (Tracker.Current != null))
			{
				Tracker.Current.CurrentPage.Cancel();
			}
			Session.Abandon();
			return new EmptyResult();
		}

		public ActionResult FlushSessionPage()
		{
			return View("Flush");
		}

		public ActionResult FlushSessionTool()
		{
			if (!Context.PageMode.IsNormal)
			{
				return ShowEditorAlert();
			}
			return View("FlushSessionTool", new VisitInformation());
		}

		private ActionResult ShowEditorAlert()
		{
			if (!Context.PageMode.IsExperienceEditorEditing)
			{
				return null;
			}
			return View("ShowPageEditorAlert", new PageEditorAlert(ID.Parse("{639B00A4-5650-4078-9C87-F9CA4C0BD0DA}")));
		}

		[HttpPost]
		public ActionResult SimulateVisitDate(string date)
		{
			DateTime parsedDate;
			if (!Tracker.IsActive || (Tracker.Current == null))
			{
				return new EmptyResult();
			}
			Tracker.Current.CurrentPage.Cancel();
			if (!DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out parsedDate))
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			DateTime originalStartDateTime = Tracker.Current.Interaction.StartDateTime;
			DateTime overridingDate = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, originalStartDateTime.Hour, originalStartDateTime.Minute, originalStartDateTime.Second, DateTimeKind.Utc);
			Tracker.Current.Interaction.StartDateTime = overridingDate;
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}

		public ActionResult VisitDateOverride()
		{
			if (!Context.PageMode.IsNormal)
			{
				return ShowEditorAlert();
			}
			return View("VisitDateOverride", new VisitInformation());
		}
	}

}