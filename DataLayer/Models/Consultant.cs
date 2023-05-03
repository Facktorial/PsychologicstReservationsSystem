using DataLayer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Consultant : IPerson
    {
        public Consultant()
        {
            Id = -1;
            Name = String.Empty;
            Email = String.Empty;
            PhoneNumber = String.Empty;
            Erudicity = Specialization.General;
        }

        public Consultant(int id, string name, string email, string phoneNumber, Specialization spec)
        {
            Id = id;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Erudicity = spec;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        //public List<Specialization> Erudicity { get; set; }
        public Specialization Erudicity { get; set; }

        public override string ToString()
        {
            //return $"Name: {Name}, Phone: {PhoneNumber}, Email: {Email}, Specialization: {string.Join(", ", Erudicity)}";
            return $"Name: {Name}, Phone: {PhoneNumber}, Email: {Email}, Specialization: {Erudicity}";
        }
    }
}
