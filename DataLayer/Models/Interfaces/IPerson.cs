using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Interfaces
{
    public interface IPerson : IEntity
    {
        public string Name { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string ToString();
    }
}
