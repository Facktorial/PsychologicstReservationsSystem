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
    public class RowDataGateway<T> : PatternObjPrototype<T>, ICRUD<T> where T : IEntity, new()
    {
        public RowDataGateway() : base() { }

        public RowDataGateway(T obj, SqlConnector sql) : base(obj, -1, sql) { }

        public RowDataGateway(SqlConnector sql, params object[] attributeValues) : base(sql, attributeValues) { }

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

        public override string ToString()
        {
            return ToString(this.GetType().Name.Split('`')[0]);
        }

        public T GetDomainObject() { return DomainObject; }
    }
}
