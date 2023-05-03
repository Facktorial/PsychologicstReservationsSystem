using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace DataLayer
{
    public class ActiveRecord<T>
        : PatternObjPrototype<T>, ICRUD<T>, IFetchable<ActiveRecord<T>>, IHaveIndexer
        where T : IEntity, new()
    {
        public ActiveRecord() : base() { }

        public ActiveRecord(T obj, SqlConnector sql) : base(obj, -1, sql) { }

        public ActiveRecord(SqlConnector sql, params object[] attributeValues) : base(sql, attributeValues) { }

        public void Save()
        {
            string sql = $"INSERT INTO {SQLHelper<T>.TableName()} ({SQLHelper<T>.GetColumnNames()}) VALUES ({SQLHelper<T>.GetParameterNames()});";
            SQLAction(sql, GetMaxId);
        }

        public void Update()
        {
            string sql = $"UPDATE {SQLHelper<T>.TableName()} SET {SQLHelper<T>.GetSetExpressions()} WHERE Id = @Id;";
            SQLAction(sql);
        }

        public void Delete()
        {
            string sql = $"DELETE FROM {SQLHelper<T>.TableName()} WHERE Id = @Id;";
            SQLAction(sql);
        }

        public static ActiveRecord<T>? Find(int id, SqlConnector conn)
        {
            var fn = GetActiveRecord(conn, id);
            return FindInner(id, conn, fn);
        }

        private static Func<SQLTypes, ActiveRecord<T>?> GetActiveRecord(SqlConnector conn, int id)
        {
            return (SQLTypes sql) => {
                if (!sql.Reader.Read()) { return null; }

                T activeRecord = new T();
                activeRecord.Id = id;
                ActiveRecord<T> tmp = new ActiveRecord<T>(activeRecord, conn);
                SQLHelper<T>.MapProperties(sql.Reader, tmp.DomainObject, id, conn);
                return tmp;
            };
        }

        public static ActiveRecord<T>? Fetch(int id, SqlConnector sql)
        {
            return Find(id, sql);
        }

        public ActiveRecord<T>? Fetch()
        {
            Console.WriteLine("FIXME");
            //var tmp = Find(DomainObject["Id"], SqlConnect);
            //if (tmp == null) { DomainObject["Id"] = -1; return this; }
            //DomainObject = tmp.DomainObject;
            //Id = tmp.Id;

            return this;
        }

        public override string ToString()
        {
            return ToString(this.GetType().Name.Split('`')[0]) + $", Id: {DomainObject.Id}";
        }

        public new object this[string propertyName]
        {
            get
            {
                // if (propertyName == "Id") { return (int) Id; }
                return base[propertyName];
            }
            set
            {
                // if (propertyName == "Id") { Id = (int) value; return; }
                base[propertyName] = value;
            }
        }

        public Func<object[], object> this[Method methodName]
        {
            get
            {
                var method = typeof(T).GetMethod(methodName.Name);
                return (object[] args) => InvokeMethod(method, args);
            }
        }
    }
}
