//// See https://aka.ms/new-console-template for more information
//using DesignPatterns;
//using System.Collections.Generic;
////using Microsoft.Data.Sqlite;
//using System.Data.SQLite;
//using System.Reflection;
//using System.Security.Cryptography.X509Certificates;

//namespace DataLayer
//{
//    public class Nullable<T>
//    {
//        public T? Value { get; set;}
//    }

//    internal class Program
//    {
//        public class FOrganizer : ForeignKeyMapping<Organizer>
//        {
//            public string OrganizationName { get; set; }
//            public FOrganizer(Organizer _domainObj, int id, string orgName, List<(Type, object, int)> objects)
//                : base(_domainObj, id, objects) { OrganizationName = orgName;  }

//            public override string ToString()
//            {
//                var props = typeof(FOrganizer).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
//                var propStrings = props.Select(p => p.Name + ": " + p.GetValue(this, null));

//                return "---\n" + typeof(FOrganizer).Name +  "\n" + base.ToString() + string.Join(", ", propStrings);
//            }
//        }

//        public class Enrollment : AssociativeTableMapping<IPerson, Reservation>
//        {
//            [NullableColumn(false)]
//            public DateTime EnrollmentDate { get; set; }
//        }
//        private static void Main(string[] args)
//        {
//            IPerson[] persons = {
//            new IPerson("Jakub", 27),
//            new IPerson("Michal", 24),
//            new IPerson("Karel", 26),
//            new IPerson("Karel2", 26),
//            new IPerson("Karel3", 26),
//            new IPerson("Karel4", 26),
//            new IPerson("Karel5", 26),
//            new IPerson("Chranibor", 26),
//            new IPerson("Chranislav", 47),
//            new IPerson("Chrudoš", 99),
//            new IPerson("Hovnivál", 31),
//            new IPerson("Hovnivál2", 31),
//            new IPerson("Hovnivál3", 31),
//        };

//            SqlConnector SqlConn = new SqlConnector("Data Source=my_new123457.db");
//            //SqlConnector.TestDB(SqlConn.ConnectionString);

//            using (var conn = new SQLiteConnection(SqlConn.ConnectionString))
//            {
//                SqlConn.CreateTable<IPerson>(true);
//                SqlConn.CreateTable<Enrollment>(true);
//            }

//            ActiveRecord_test.Run(persons, SqlConn);

//            DataMapper_test.Run(SqlConn);

//            Lazy_test.Run(SqlConn);
            
//            ///
//            /// Associative Table Mapping
//            ///
//            var properties = typeof(Enrollment).GetProperties();
//            foreach (var prop in properties)
//            {
//                Console.WriteLine(prop.Name);
//            }

//            ///
//            /// Foreign key Mapping
//            ///
//            int index = 2;
//            var p = persons[index];
//            var e = new Reservation();
//            var ls = new List<(Type, object, int)> { (typeof(IPerson), p, index), (typeof(Reservation), null, -1) };
//            var organizer = new FOrganizer(new Organizer(), index, "Test organization", ls);

//            Console.WriteLine(organizer.ToString());

//            Console.WriteLine();

//            Console.WriteLine(organizer.FindObject(typeof(IPerson)));

//            //var i = new IdentityMap<IdWrapper<Person>>();
//        }
//    }
//}