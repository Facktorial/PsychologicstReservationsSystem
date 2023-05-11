using System.ComponentModel.DataAnnotations;


namespace WebApp.Models
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) { return false; }

            string phoneNumber = value.ToString();

            string pattern = @"^(\+\d{1,3})?\d{9}$";

            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, pattern);
        }
    }

    public class PatientForm
    {
        [Display(Name = "Jméno")]
        [Required(ErrorMessage = "Prosím, dopňte své jméno.")]
        public string Name { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Prosím, dopňte svůj email.")]
        [EmailAddress(ErrorMessage = "Vámi zadaný email je ve špatném formátu.")]
        public string Email { get; set; }
        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "Prosím, dopňte své telefonní číslo.")]
        [PhoneNumber(ErrorMessage = "Vámi zadané číslo je ve špatném formátu.")]
        public string PhoneNumber { get; set; }
    }
}
