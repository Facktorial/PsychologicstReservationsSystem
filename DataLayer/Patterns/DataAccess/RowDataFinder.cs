using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class RowDataFinder<T> where T : IEntity, new()
    {
        public static RowDataGateway<T>? Find(int id, SqlConnector conn)
        {
            var fn = FindingFunction(conn);
            return PatternPrototype<T>.FindInner(id, conn, fn);
        }

        public static List<RowDataGateway<T>> FindByProperty(string propertyName, object value, SqlConnector conn)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            var fn = FindingListFunction(conn);
            return PatternPrototype<T>.FindInner(conn, prop, value, fn);
        }

        public static List<RowDataGateway<T>> FindAll(SqlConnector conn)
        {
            var fn = FindingListFunction(conn);
            return PatternPrototype<T>.FindInner(conn, fn);
        }

        private static Func<SQLTypes, RowDataGateway<T>?> FindingFunction(SqlConnector conn)
        {
            return (SQLTypes sql) => {
                if (!sql.Reader.Read()) { return null; }// return new RowDataGateway<T>(new T(), conn); }

                T rowData = new T();
                RowDataGateway<T> tmp = new RowDataGateway<T>(rowData, conn);
                SQLHelper<T>.MapProperties(sql.Reader, tmp.DomainObject, PatternPrototype<T>.GetMaxId(sql.Connection), conn);
                return tmp;
            };
        }

        private static Func<SQLTypes, List<RowDataGateway<T>>>
        FindingListFunction(SqlConnector conn)
        {
            return (SQLTypes sql) => {
                var ls = new List<RowDataGateway<T>>();

                while (sql.Reader.Read())
                {
                    T rowData = new T();
                    RowDataGateway<T> tmp = new RowDataGateway<T>(rowData, conn);
                    SQLHelper<T>.MapProperties(sql.Reader, tmp.DomainObject, PatternPrototype<T>.GetMaxId(sql.Connection), conn);
                    ls.Add(tmp);
                }

                return ls;
            };
        }
    }
}
