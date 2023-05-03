using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TableDataGateway<T> : PatternObjPrototype<T>, ICRUDObj<T>
        where T : IEntity, new()
    {
        public new T DomainObject
        {
            get { throw new Exception("Access to this field is disabled"); }
        }

        public TableDataGateway(SqlConnector sql) : base(sql) { }

        public void Save(T obj)
        {
            string sql = $"INSERT INTO {SQLHelper<T>.TableName()} ({SQLHelper<T>.GetColumnNames()}) VALUES ({SQLHelper<T>.GetParameterNames()});";
            var fn = (object _) => obj.Id;
            SQLAction(sql, obj, fn);
        }
        public void Update(T obj)
        {
            string sql = $"UPDATE {SQLHelper<T>.TableName()} SET {SQLHelper<T>.GetSetExpressions()} WHERE Id = @Id;";
            var fn = (object _) => obj.Id;
            SQLAction(sql, obj, fn);
        }

        public void Delete(T obj)
        {
            string sql = $"DELETE FROM {SQLHelper<T>.TableName()} WHERE Id = @Id;";
            var fn = (object _) => obj.Id;
            SQLAction(sql, obj, fn);
        }

        public T? Find(int id)
        {
            PropertyInfo prop = typeof(T).GetProperty("Id");

            var fn = GetObject(SqlConnect, id);
            return FindInner(id, SqlConnect, fn);
        }

        public List<T> FindByProperty(string propertyName, object value)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            var fn = GetList(SqlConnect, prop, value);
            return FindInner<List<T>>(SqlConnect, prop, value, fn);
        }

        private static Func<SQLTypes, T> GetObject(SqlConnector conn, int value)
        {
            return (SQLTypes sql) =>
            {
                if (!sql.Reader.Read()) { return (new T()); }

                T record = new T();
                SQLHelper<T>.MapProperties(sql.Reader, record, value, conn);
                return record;
            };
        }
        private static Func<SQLTypes, List<T>> GetList(
            SqlConnector conn, PropertyInfo prop, object value
        )
        { 
            return (SQLTypes sql) => {
                var ls = new List<T>();

                while (sql.Reader.Read())
                {
                    T record = new T();
                    SQLHelper<T>.MapProperties(sql.Reader, record, conn);
                    prop.SetValue(record, value);

                    ls.Add(record);
                }
                
                return ls;
            };
        }

        public override string ToString()
        {
            return ToString(this.GetType().Name.Split('`')[0]);
        }
    }
}
