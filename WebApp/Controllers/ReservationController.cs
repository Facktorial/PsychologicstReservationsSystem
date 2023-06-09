﻿using DataLayer.Models;
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

		private readonly IHttpContextAccessor _httpContextAccessor;
		private ISession Session => _httpContextAccessor.HttpContext.Session;

		Dictionary<string, string?> DnyCeskyDict;
		Dictionary<string, string?> SchuzeCeskyDict;

		public ReservationController(
			ReservationService shi,
			ConsultantService chi,
			IHttpContextAccessor httpContextAccessor,
			ResourceManager resourceManager
		)
		{
			this.reservationService = shi;
			//this.patientService = phi;
			this.consultantService = chi;

			this.resourceManager = resourceManager;

			_httpContextAccessor = httpContextAccessor;

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

		public IActionResult Error()
		{
			return View("Error"); // Specify the full path to the shared error view
		}

		public IActionResult Index()
		{
			if (Session.GetString("Patient") == null)
			{
				return RedirectToAction("Error");
			}

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

		public T? GetEnumDict<T>(Dictionary<string, string> dict, string suspect) where T : struct, Enum
        {
			foreach ((var key, var value) in dict)
			{
				if (suspect == value)
				{
					T enumValue;
					if (Enum.TryParse<T>(key, out enumValue))
					{
						return enumValue;
					}
				}
			}
			return default(T?);
		}

		public T? GetEnum<T>(IEnumerable<string> source, string suspect) where T : struct, Enum
		{
			foreach (var value in source)
			{
				if (suspect == value)
				{
					T enumValue;
					if (Enum.TryParse<T>(value, out enumValue))
					{
						return enumValue;
					}
				}
			}
			return default(T?);
		}

		[HttpPost]
		public IActionResult Index(ReservationForm form, int hour, DateTime day)
		{
			ViewBag.Consultants = consultantService.Consultants();
			ViewBag.Times = GetTimes();
			ViewBag.DnyCeskyDict = DnyCeskyDict;
			ViewBag.Opening = (InitHours, StopHours);
			ViewBag.EventType = Enum.GetNames(typeof(EventType)).Select(e => SchuzeCeskyDict[e]);

			if (ModelState.IsValid)
			{
				EventType? event_ack = GetEnumDict<EventType>(SchuzeCeskyDict, form.SelectedEventTypeOption);
				
				if (event_ack is null) { return View("ErrorNoTranslation"); }

				Reservation newReservation = new Reservation
				{
					Subject = form.Subject ?? string.Empty,
					DateTime = new DateTime(day.Year, day.Month, day.Day, hour, 0, 0, 0),
					Patient = Patient.Deserialize(Session.GetString("Patient")),
					Consultant = consultantService.Consultants().FirstOrDefault(form.Consultant),
					Type = (EventType) event_ack,
                };

                reservationService.Mapper.Insert(newReservation);
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
			Session.Clear();
			//Session.Remove("Patient");
			return View();
		}

		//public override void OnActionExecuted(ActionExecutedContext context)
		//{
		//	if (!reservationService.IsFetched)
		//	{
		//		reservationService.Fetch();
		//	}
		//	ViewBag.ReservationCount = reservationService.Reservations().Count;
		//}

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
