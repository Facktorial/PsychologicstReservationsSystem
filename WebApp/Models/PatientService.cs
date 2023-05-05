using DataLayer;
using DataLayer.Models;
using System.Diagnostics;

namespace WebApp.Models
{
    public class PatientService
    {
        public bool IsFetched = false;
        private SqlConnector Sql;
        //private SqlConnector Sql = new SqlConnector(@"Data Source=..\..\..\..\..\Data\psycho.db");
        public DataMapper<Patient> Mapper;
        public List<Patient> Patients() { return Mapper.DomainObject; }

        public void Fetch()
        {
            Mapper = new DataMapper<Patient>(Sql);
            Mapper.Fetch();
            IsFetched = true;
        }

        public Patient? Get(int id)
        {
            return Patients().FirstOrDefault(x => x.Id == id);
        }

        public Patient? Get(Patient p)
        {
            return Patients().FirstOrDefault(x => x.Id == p.Id);
        }

        public void Upload(Patient p)
        {
            Mapper.Insert(p);
            Mapper.Save();
            Mapper.Fetch();
        }

        public PatientService(SqlConnector sql)
        {
            Sql = sql;
            Mapper = new DataMapper<Patient>(Sql);
        }
    }
}
