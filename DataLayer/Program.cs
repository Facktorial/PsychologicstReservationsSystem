using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using DataLayer.Models;

namespace DataLayer
{
    public static class DataLayer
    {
        public static void CreateTables()
        {
            SqlConnector SqlConn = new SqlConnector("Data Source=psycho.db");
            //SqlConnector.TestDB(SqlConn.ConnectionString);

            using (var conn = new SQLiteConnection(SqlConn.ConnectionString))
            {
                SqlConn.CreateTable<Patient>(true);
                SqlConn.CreateTable<Consultant>(true);
                SqlConn.CreateTable<Reservation>(true);
            }
        }

        public static void Populate()
        {
            SqlConnector SqlConn = new SqlConnector("Data Source=psycho.db");

            var patients = new List<Patient>
            {
                new Patient(0, "Jakub Dvorak", "dvo0226@vsb.cz", "+420000000000"),
                new Patient(1, "Ladislav Dvorak", "dvo0226@vsb.cz", "+420000000000"),
                new Patient(2, "Karel Dvorak", "dvo0226@vsb.cz", "+420000000000"),
                new Patient(3, "Jan Dvorak", "dvo0226@vsb.cz", "+420000000000"),
            };

            var consultants = new List<Consultant>
            {
                new Consultant(0, "Jakub Vyr", "dvo0226@vsb.cz", "+420000000000", Specialization.MentallIssuesStudent),
                new Consultant(1, "Ladislav Vyr", "dvo0226@vsb.cz", "+420000000000", Specialization.General),
            };

            DateTime dateTime = new DateTime(2023, 5, 1, 14, 30, 0);
            DateTime currentDateTime = DateTime.Now;
            var reservations = new List<Reservation>
            {
                new Reservation(0, false, "Pozorovani", dateTime, patients[0], consultants[0], EventType.PeriodicAppoitment),
                new Reservation(1, false, "Pozorovani", currentDateTime, patients[1], consultants[0], EventType.PeriodicAppoitment),
                new Reservation(2, false, "Pozorovani", currentDateTime, patients[1], consultants[1], EventType.PeriodicAppoitment),
                new Reservation(3, false, "Pozorovani", dateTime, patients[0], consultants[1], EventType.PeriodicAppoitment),
            };

            var patientsDataMapper = new DataMapper<Patient>(patients, SqlConn);
            var consultantsDataMapper = new DataMapper<Consultant>(consultants, SqlConn);
            var reservationsDataMapper = new DataMapper<Reservation>(reservations, SqlConn);
            patientsDataMapper.Save();
            consultantsDataMapper.Save();
            reservationsDataMapper.Save();

            var patientsDataMapper2 = new DataMapper<Patient>(SqlConn);
            var consultantsDataMapper2 = new DataMapper<Consultant>(SqlConn);
            var reservationsDataMapper2 = new DataMapper<Reservation>(SqlConn);
            patientsDataMapper2.Fetch();
            consultantsDataMapper2.Fetch();
            reservationsDataMapper2.Fetch();

            Console.WriteLine(patientsDataMapper2.DomainObject.Count);
            Console.WriteLine(consultantsDataMapper2.DomainObject.Count);
            Console.WriteLine(reservationsDataMapper2.DomainObject.Count);

            foreach (var patient in patientsDataMapper2.DomainObject)
            {
                Console.WriteLine(patient);
            }
            foreach (var cons in consultantsDataMapper2.DomainObject)
            {
                Console.WriteLine(cons);
            }
            foreach (var reserv in reservationsDataMapper2.DomainObject)
            {
                Console.WriteLine(reserv);
            }
        }
    }
}
