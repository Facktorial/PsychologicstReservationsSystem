using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Models;
using System.Resources;

namespace WebApp.Controllers
{
	public class TimeRecord
    {
        public TimeRecord(DateTime date, DayOfWeek day, Dictionary<int, bool> hours)
        {
            Date = date;
            Day = day;
            Hours = hours;
        }

        public DateTime Date { get; set; }
		public DayOfWeek Day { get; set; }
		public Dictionary<int, bool> Hours { get; set; }
    }
    public class ReservationController : Controller
    {
		public const int InitHours = 8;
		public const int StopHours = 16;

		private ReservationService reservationService;
		//private PatientService patientService;
		private ConsultantService consultantService;

		private readonly ResourceManager resourceManager;

		Dictionary<string, string?> DnyCeskyDict;
		Dictionary<string, string?> SchuzeCeskyDict;

		public ReservationController(ReservationService shi, ConsultantService chi, ResourceManager resourceManager)
		{
			this.reservationService = shi;
			//this.patientService = phi;
			this.consultantService = chi;
			this.resourceManager = resourceManager;
			
			DnyCeskyDict = new Dictionary<string, string?>()
			{
				{ "Monday", resourceManager.GetString("Monday") },
				{ "Tuesday", resourceManager.GetString("Tuesday") },
				{ "Wednesday", resourceManager.GetString("Wednesday") },
				{ "Thursday", resourceManager.GetString("Thursday") },
				{ "Friday", resourceManager.GetString("Friday") },
				{ "Saturday", resourceManager.GetString("Saturday") },
				{ "Sunday", resourceManager.GetString("Sunday") },	
			};

			SchuzeCeskyDict = new Dictionary<string, string?>()
			{
				{ "NewAppoitment", resourceManager.GetString("NewAppoitment") },
				{ "PeriodicAppoitment", resourceManager.GetString("PeriodicAppoitment") },
				{ "CriticalAppoitment", resourceManager.GetString("CriticalAppoitment") },
				{ "Register", resourceManager.GetString("Register") },
			};
		}

		public IActionResult Index()
		{
			if (!reservationService.IsFetched)
			{
				reservationService.Fetch();
			}

			ViewBag.Reservations = reservationService.Reservations();
			ViewBag.Consultants = consultantService.Consultants();
			ViewBag.Times = GetTimes();
			ViewBag.DnyCeskyDict = DnyCeskyDict;
			ViewBag.Opening = (InitHours, StopHours);
			ViewBag.EventType = Enum.GetNames(typeof(EventType)).Select(e => SchuzeCeskyDict[e]);

			return View();
		}

		[HttpPost]
		public IActionResult Index(ReservationForm form, int hour, DateTime day)
		{
			ViewBag.Reservations = reservationService.Reservations();
			ViewBag.Consultants = consultantService.Consultants();
			ViewBag.Times = GetTimes();
			ViewBag.DnyCeskyDict = DnyCeskyDict;
			ViewBag.Opening = (InitHours, StopHours);
			ViewBag.EventType = Enum.GetNames(typeof(EventType)).Select(e => SchuzeCeskyDict[e]);

			if (ModelState.IsValid)
			{
				Console.WriteLine(form.DateTime);
				//Reservation newReservation = new Reservation
				//{
				//	Name = form.Name,
				//	Email = form.Email,
				//	PhoneNumber = form.PhoneNumber
				//};

				//reservationService.Mapper.Insert(newReservation);
				reservationService.Mapper.Save();
				return RedirectToAction("Done");
			}

			return View();
		}

		public static DateTime GetDate(TimeRecord record, int hour)
        {
			return new DateTime(record.Date.Year, record.Date.Month, record.Date.Day, hour, 0, 0, 0);
		}

		public IActionResult Done()
		{
			return View();
		}

		public override void OnActionExecuted(ActionExecutedContext context)
		{
			if (!reservationService.IsFetched)
			{
				reservationService.Fetch();
			}
			ViewBag.ReservationCount = reservationService.Reservations().Count;
		}

		//public List<(DateTime, DayOfWeek, List<int>)> GetTimes()
		public List<TimeRecord> GetTimes()
		{
			DateTime now = DateTime.Now;
			DateTime nextFullHour = now.AddHours(1).Date.AddHours(now.Hour + 1);
			var datesUntilNextFriday = GetDatesUntilNextFriday(nextFullHour);

			var result = new List<TimeRecord>();

			List<int> hours = Enumerable
				.Range(0, (StopHours - InitHours) / 2 + 1)
				.Select(n => n * 2 + InitHours)
				.ToList();

			foreach ((DateTime date, DayOfWeek day) in datesUntilNextFriday)
			{
				var possibles = DayPossibleVisits(date, hours);

				//Console.WriteLine(day);
				//foreach ((var k, var v) in possibles)
    //            {
				//	Console.WriteLine($"{k}: {v}");
    //            }

				result.Add(new TimeRecord(date, day, possibles));
			}

			return result;
		}

		public static List<(DateTime, DayOfWeek)> GetDatesUntilNextFriday(DateTime startDateTime)
		{
			var dates = new List<(DateTime, DayOfWeek)>();
			DateTime currentDate = startDateTime;

			int n_fridays = 0;
			while (n_fridays < 2)
			{
				if (currentDate.DayOfWeek == DayOfWeek.Friday) { ++n_fridays; }

				dates.Add((currentDate.Date, currentDate.DayOfWeek));
				currentDate = currentDate.AddDays(1);
			}

			return dates;
		}
		Dictionary<int, bool> DayPossibleVisits(DateTime day, List<int> times)
		{
			DateTime now = DateTime.Now;
			int nextFullHour = now.AddHours(1).Date.AddHours(now.Hour + 1).Hour;

			//var mock_day = new DateTime(day.Year, day.Month, day.Day, 10, 0, 0, 0);
			var reservedHours = reservationService.Reservations()
				.Where(reservation => reservation.DateTime.Date == day.Date)
				.Select(reservation => reservation.DateTime.Hour)
				.ToList();

			Dictionary<int, bool> result = times
				.ToDictionary(hour => hour, hour => (
					(
						(day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday) &&
						(day.Date == now.Date ? (hour >= nextFullHour) : true) &&
						!(reservedHours.Contains(hour))
					)
					? true : false
				));

			return result;
		}
	}
}
