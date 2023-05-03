//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataLayer
//{
//    public class DataMapper_test
//    {
//        public static void Run(SqlConnector SqlConn)
//        {
//            Console.WriteLine("\nDATA MAPPER TEST RUNNING //////////");

//            var mapper = new DataMapper<IPerson>(SqlConn);
//            //mapper.Fetch();

//            foreach (var p in mapper.DomainObject)
//            {
//                Console.WriteLine(p.ToString());
//            }

//            RunDeleteTest(SqlConn, mapper);
//        }

//        public static void RunDeleteTest(SqlConnector SqlConn, DataMapper<IPerson> mapper)
//        {
//            Console.WriteLine("RUN DELETE TEST ///");

//            mapper.Insert(new IPerson("Hovnivál4", 31));
//            mapper.Insert(new IPerson("Hovnivál5", 31));

//            mapper.Delete(x => x.Id > 10);
//            mapper.Save();

//            mapper.Insert(new IPerson("Hovnivál6", 31));
//            mapper.Insert(new IPerson("Hovnivál7", 31));

//            foreach (var p in mapper) { Console.WriteLine(p.ToString()); }

//            mapper.Save();

//            Console.WriteLine("\nSECOND MAPPER //");

//            var mapper2 = new DataMapper<IPerson>(SqlConn);
//            foreach (var p in mapper2) {  Console.WriteLine(p.ToString()); }

//            mapper2.Insert(new IPerson("Hovnivál8", 31));
//            mapper2.Insert(new IPerson("Hovnivál9", 31));

//            //mapper.Fetch();
//            foreach (var p in mapper2)
//            {
//                Console.WriteLine(p.ToString());
//            }
//            mapper2.Save();
//        }
//    }
//}

