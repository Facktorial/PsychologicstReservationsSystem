using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models;
using System.Diagnostics;


namespace WebApp.Controllers
{
	public class ConsultantController : Controller
    {
		private ConsultantService consultantService ;

		public ConsultantController(ConsultantService shi)
		{
			this.consultantService = shi;
			consultantService.Fetch();
		}

		public IActionResult Index()
        {
			if (!consultantService.IsFetched)
			{
				consultantService.Fetch();
			}

			ViewBag.PreviousUrl = HttpContext.Request.Headers["Referer"].ToString();
			ViewBag.Consultants = consultantService.Consultants();

			return View();
		}

		public override void OnActionExecuted(ActionExecutedContext context)
		{
			if (!consultantService.IsFetched)
			{
				consultantService.Fetch();
			}
			ViewBag.PatientCount = consultantService.Consultants().Count;
		}
	}
}
