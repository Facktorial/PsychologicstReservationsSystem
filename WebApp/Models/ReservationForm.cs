using DataLayer.Models;
using System.ComponentModel.DataAnnotations;


namespace WebApp.Models
{
    public class ReservationForm
    {
        [Display(Name = "Jak se máte?")]
        public string? Subject { get; set; }

        [Display(Name = "Date and Time")]
        public DateTime DateTime { get; set; }

        [Display(Name = "Patient")]
        public int Patient { get; set; }

        [Display(Name = "Terapeut")]
        public int? Consultant { get; set; }

        [Display(Name = "Typ schůze")]
        [Required(ErrorMessage = "Please select the event type.")]
        public EventType Type { get; set; }

        [Display(Name = "Selected EventType Option")]
        [Required(ErrorMessage = "Prosím, vyberte typ sezení.")]
        public string SelectedEventTypeOption { get; set; } = null;

        [Display(Name = "Selected Consultant Option")]
        public int? SelectedConsultantOption { get; set; } = null;
    }
}
