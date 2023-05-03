//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataLayer
//{
//    public class ActiveRecord_test
//    {
//        public static void Run(IPerson[] persons, SqlConnector SqlConn)
//        {
//            Console.WriteLine("\nACTIVE RECORD TEST RUNNING //////////");

//            var ps = new List<ActiveRecord<IPerson>>();
//            for (int i = 0; i < persons.Length; i++)
//            {
//                ps.Add(new ActiveRecord<IPerson>(persons[i], SqlConn));
//            }

//            foreach (var _p in ps)
//            {
//                Console.WriteLine("FORLOOP: " + _p?.ToString() ?? "null");
//                _p.Save();
//                _p.Fetch();
//                Console.WriteLine("FORLOOP: " + _p?.ToString() ?? "null");
//                Console.WriteLine("");
//            }

//            //Console.WriteLine(ActiveRecord<Person>.Fetch(1, SqlConn).ToString());
//            //Console.WriteLine(ActiveRecord<Person>.Fetch(2, SqlConn).ToString());

//            for (int i = 0; i < 10; i++)
//            {
//                var p = ActiveRecord<IPerson>.Find(i, SqlConn);
//                Console.WriteLine($"{i}: " + p?.ToString() ?? "null");
//            }

//            var person = ActiveRecord<IPerson>.Find(3, SqlConn);
//            Console.WriteLine(person["Name"] + " " + person["Age"]);
//            var me = person[new Method("Calculate")];
//            Console.WriteLine(me(new object[] { 2, 3 }));

//            person["Age"] = 17;
//            person.Update();

//            person.Fetch();
//            Console.WriteLine(person[new Method("ToString")](null));
//            ////////////////////////////////////////////////////////////////////
            
//            RunDeleteTest(SqlConn);
//        }

//        public static void RunDeleteTest(SqlConnector SqlConn)
//        {
//            Console.WriteLine("RUN DELETE TEST ///");

//            var tmp0 = ActiveRecord<IPerson>.Find(1, SqlConn);
//            Console.WriteLine("tmp0: " + tmp0?.Id);
//            Console.WriteLine("tmp0: " + tmp0?.ToString());

//            var tmp = ActiveRecord<IPerson>.Find(1, SqlConn);
//            Console.WriteLine("tmp: " + tmp?.Id);
//            Console.WriteLine("tmp: " + tmp?.ToString());

//            tmp["Age"] = (int) tmp["Age"] + 4;
//            tmp.Update();

//            Console.WriteLine("tmp0: " + tmp0?.Id);
//            Console.WriteLine("tmp0: " + tmp0?.ToString());

//            tmp0.Fetch();
//            Console.WriteLine("tmp0: " + tmp0?.Id);
//            Console.WriteLine("tmp0: " + tmp0?.ToString());
//            Console.WriteLine("tmp: " + tmp?.Id);
//            Console.WriteLine("tmp: " + tmp?.ToString());

//            tmp.Delete();
            
//            var tmp2 = ActiveRecord<IPerson>.Find(1, SqlConn);
//            Console.WriteLine("tmp2: " + tmp2?.Id);
//            Console.WriteLine("tmp2: " + tmp2?.ToString());

//            tmp0.Fetch();
//            Console.WriteLine("tmp0: " + tmp0?.Id ?? "null");
//            Console.WriteLine("tmp0: " + tmp0?.ToString() ?? "null");
//        }
//    }
//}
