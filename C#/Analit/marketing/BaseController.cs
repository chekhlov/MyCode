using System;
using System.Linq;
using System.Web.Mvc;
using Marketing.Models;
using NHibernate;
using NHibernate.Linq;
using System.ComponentModel;

namespace Marketing.Controllers
{
	public class BasePromoterController : BaseController
	{
		public Promoter CurrentPromoter => HttpContext.Items[typeof (Promoter)] as Promoter;
		public RegionalAdmin CurrentAdmin => HttpContext.Items[typeof (RegionalAdmin)] as RegionalAdmin;

		private Association CurrentAssociationBoundWithSession { get; set; }

		public Association CurrentAssociation
		{
			get
			{
				if (CurrentAssociationBoundWithSession != null)
					return CurrentAssociationBoundWithSession;
				CurrentAssociationBoundWithSession = HttpContext.Items[typeof (Association)] as Association;
				return CurrentAssociationBoundWithSession;
			}
			set { CurrentAssociationBoundWithSession = value; }
		}

		public MarketingEvent CurrentMarketingEvent
		{
			get { return DbSession.Load<MarketingEvent>((uint) System.Web.HttpContext.Current.Session["MarketingEvent"] ); }
			set { System.Web.HttpContext.Current.Session["MarketingEvent"] = value.Id; }
		}
	}

	public class BaseAdminController : BaseController
	{
		public RegionalAdmin CurrentAdmin => HttpContext.Items[typeof (RegionalAdmin)] as RegionalAdmin;
	}

	public class BaseController : Controller
	{
		protected ISession DbSession => HttpContext.Items[typeof (ISession)] as ISession;

		public IImpersonatedUser ImpersonatedUser
			=> (IImpersonatedUser) (HttpContext.Items[typeof (RegionalAdmin)] ?? HttpContext.Items[typeof (Promoter)]);


		public void SuccessMessage(string message)
		{
			TempData["SuccessMessage"] = message;
		}

		public void ErrorMessage(string message)
		{
			TempData["ErrorMessage"] = message;
		}

		public RedirectToRouteResult BaseRedirectToAction(string action, string controller)
		{
			return RedirectToAction(action, controller);
		}

		[HttpPost]
		public virtual ActionResult UpdateProperty(string entity, uint id, string property, string value)
		{
			var entityClass = Type.GetType("Marketing.Models." + entity, true);
			var item = Activator.CreateInstance(entityClass);
			DbSession.Load(item, id);
			var pi = entityClass.GetProperty(property);
			var converter = TypeDescriptor.GetConverter(pi.PropertyType);
			pi.SetValue(item, converter.ConvertFrom(value));
			DbSession.Flush();
			return Json(new { result = "success" });
		}

		public string RenderPartialView(string viewName, object model)
		{
			ViewData.Model = model;
			using (var sw = new System.IO.StringWriter()) {
				var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
				var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
				viewResult.View.Render(viewContext, sw);
				viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
				return sw.GetStringBuilder().ToString();
			}
		}
	}
}