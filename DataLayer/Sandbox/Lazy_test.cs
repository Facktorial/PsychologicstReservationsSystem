//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataLayer
//{
//    public class Lazy_test
//    {
//        public static void Run(SqlConnector SqlConn)
//        {
//            Console.WriteLine("\nLAZY LOAD TEST RUNNING //////////");

//            int id = 2;
//            ////var fn = () => ActiveRecord<Person>.Find(id, SqlConn);
//            var lazyMe = new Lazyload<ActiveRecord<IPerson>>(id, SqlConn);
//            Console.WriteLine(lazyMe["Name"]);
//            lazyMe["Id"] = 1;
//            Console.WriteLine(lazyMe.ToString());
           
//            lazyMe.Save();
            
//            lazyMe["Id"] = 2;
//            Console.WriteLine(lazyMe.ToString());
            
//            lazyMe.Fetch();
//            Console.WriteLine(lazyMe.ToString());
//            Console.WriteLine(lazyMe[new Method("Calculate")](new object[] { 2, 3 }));

//            ////var mp_fn = () => {
//            ////    var tmp = new DataMapper<Person>();
//            ////    tmp.Fetch();
//            ////    return tmp;
//            ////};
            
//            // FIXME
//            var lazyYou = new Lazyload<DataMapper<IPerson>>(SqlConn);
//            Console.WriteLine(lazyYou["Count"]);
//            DataMapper<IPerson> lazyMapper = lazyYou.GetObject();
//            Console.WriteLine(lazyMapper["Count"]);
//        }
//    }
//}
