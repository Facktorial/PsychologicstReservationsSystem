using DataLayer.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Patient : IPerson
    {
        public Patient(int id, string name, string email, string phoneNumber)
        {
            Id = id;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }
        public Patient()
        {
            Id = -1;
            Name = String.Empty;
            Email = String.Empty;
            PhoneNumber = String.Empty;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, Phone: {PhoneNumber}, Email: {Email}";
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Patient Deserialize(string str)
        {
            return JsonConvert.DeserializeObject<Patient>(str);
        }
    }
}