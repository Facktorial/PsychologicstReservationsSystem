using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;
using Microsoft.AspNetCore.Http;


namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PatientService patientService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        public HomeController(ILogger<HomeController> logger, PatientService patientService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            this.patientService = patientService;

            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public IActionResult Index(PatientForm form)
        {
            if (!patientService.IsFetched)
            {
                patientService.Fetch();
            }

            ViewBag.Patients = patientService.Patients();

            if (ModelState.IsValid)
            {
                Patient newPatient = new Patient
                {
                    Name = form.Name,
                    Email = form.Email,
                    PhoneNumber = form.PhoneNumber
                };

                if (!patientService.Patients()
                    .Any(person => person.Name == newPatient.Name && person.Email == newPatient.Email && person.PhoneNumber == newPatient.PhoneNumber)
                )
                {
                    patientService.Mapper.Insert(newPatient);
                    patientService.Mapper.Save();
                }

                Session.SetString("Patient", newPatient.Serialize());

                return RedirectToAction("Done", "Patient");
            }

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}