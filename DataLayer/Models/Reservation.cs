using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public enum EventType
    {
        NewAppoitment = 0,
        PeriodicAppoitment = 1,
        CriticalAppoitment = 2,
        Register = 3
    }
    public class Reservation : IEntity
    {
        public int Id { get; set; }
        public bool IsCanceled { get; set; }
        public string Subject { get; set; }
        public DateTime DateTime { get; set; }


        [NullableColumnAttribute(false)]
        [FKColumnAttribute(true)]
        public Patient Patient { get; set; }

        [NullableColumnAttribute(true)]
        [FKColumnAttribute(true)]
        public Consultant? Consultant { get; set; }

        public EventType Type { get; set; }
        public Reservation()
        {
            Id = -1;
            IsCanceled = false;
            Subject = string.Empty;
            DateTime = DateTime.Now;
            Type = EventType.Register;
            Patient = new Patient();
            Consultant = null;
        }

        public Reservation(int id, Patient pat, Consultant? cons)
        {
            Id = id;
            IsCanceled = false;
            Subject = string.Empty;
            DateTime = DateTime.Now;
            Type = EventType.Register;
            Patient = pat;
            Consultant = cons;
        }

        public Reservation(
            int id, bool isCancel, string subject, DateTime datetime, Patient pat, Consultant? cons, EventType e
        )
        {
            Id = id;
            IsCanceled = isCancel;
            Subject = subject;
            DateTime = datetime;
            Type = EventType.Register;
            Patient = pat;
            Consultant = cons;
        }

        public override string ToString()
        {
            return $"Subject: {Subject}, Date: {DateTime}, Type: {Type},\nPatient: {Patient.Name},\nConsultant: {Consultant?.Name}\n";
        }
    }
}
