using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models;
using System.Diagnostics;

namespace WebApp.Controllers
{
    public class PatientController : Controller
    {
		private PatientService patientService;

		public PatientController(PatientService shi)
		{
			this.patientService = shi;
			patientService.Fetch();
		}

		public IActionResult Index()
		{
			if (!patientService.IsFetched)
			{
				patientService.Fetch();
			}

			ViewBag.Patients = patientService.Patients();

			return View();
		}

		public IActionResult Done()
		{
			return View();
		}

		public IActionResult GetPatients([FromServices] PatientService ps)
		{
			return new JsonResult(ps.Patients());
		}
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			if (!patientService.IsFetched)
            {
				patientService.Fetch();
            }
			ViewBag.PatientCount = patientService.Patients().Count;
		}
	}
}
