using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;

namespace DataLayer
{
    ///
    /// Change when changing database
    ///
    using SQLConnection = SqliteConnection;
    using SQLReader = SqliteDataReader;
    using SQLCommand = SqliteCommand;
    using SQLTransaction = SqliteTransaction;

    //public static Func<A, C> After<A, B, C>(this Func<B, C> f, Func<A, B> g) => value => f(g(value));
    //public static Func<A, C> Composition<A, B, C>(Func<B, C> f, Func<A, B> g) => value => f(g(value));
    //Func<string, int> parse = int.Parse; // string -> int
    //Func<int, int> abs = Math.Abs; // int -> int
    //Func<string, int> composition1 = abs.After(parse);
    //Func<string, int> composition2 = Composition(abs, parse);

    public record StateOfCollection(HashSet<int> Added, HashSet<int> Changed, HashSet<int> Removed);

    public class SQLTypes
    {
        public SQLTypes(SQLConnection connection, SQLReader reader, SQLCommand command)
        {
            Connection = connection;
            Reader = reader;
            Command = command;
        }

        public SQLConnection? Connection { get; set; } = null;
        public SQLReader? Reader { get; set; } = null;
        public SQLCommand? Command { get; set; } = null;
    }

    public class Method
    {
        public Method(string str) { Name = str; }
        public string Name { get; set; }
    }
    public class PatternPrototype<T> : IHaveIndexer where T : new()
    {
        public T DomainObject { get; set; }
        public SqlConnector SqlConnect { get; set; }

        public PatternPrototype()
        {
            DomainObject = new T();
            SqlConnect = new SqlConnector("new to be properly initialized");
        }

        public PatternPrototype(T domainObject, int id, SqlConnector sql)
        {
            DomainObject = domainObject;
            SqlConnect = sql;
        }

        protected PatternPrototype(SqlConnector sql, params object[] attributeValues)
        {
            SqlConnect = sql;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            if (properties.Length != attributeValues.Length)
            {
                throw new ArgumentException("Number of attribute values does not match number of properties in domain object.");
            }

            DomainObject = (T)Activator.CreateInstance(type);
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i].SetValue(DomainObject, attributeValues[i]);
            }
        }

        public static List<PropertyInfo> ListProperties()
        {
            return typeof(T).GetProperties().ToList();
        }

        public static List<MethodInfo> ListMethods()
        {
            return typeof(T).GetMethods().ToList();
        }

        public object this[string propertyName]
        {
            get
            {
                PropertyInfo property = DomainObject.GetType().GetProperty(propertyName);
                return property.GetValue(DomainObject);
            }
            set
            {
                PropertyInfo property = DomainObject.GetType().GetProperty(propertyName);
                property.SetValue(DomainObject, value);
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

        protected object InvokeMethod(MethodInfo method, params object[] args)
        {
            return method.Invoke(DomainObject, args);
        }

        public int GetMaxId()
        {
            using (SQLConnection connection = new SQLConnection(SqlConnect.ConnectionString))
            {
                connection.Open();

                return GetMaxId(connection);
            }
        }

        public int GetMaxId<InnerTYpe>()
        {
            using (SQLConnection connection = new SQLConnection(SqlConnect.ConnectionString))
            {
                connection.Open();

                return GetMaxId<InnerTYpe>(connection);
            }
        }

        public static int GetMaxId(SQLConnection connect)
        {
            string tableName = typeof(T).Name;
            string sql = $"SELECT MAX(Id) FROM {tableName}";
            SqliteCommand command = new SqliteCommand(sql, connect);
            object maxID = command.ExecuteScalar();
            maxID = maxID == "" ? "0" : maxID;

            return (maxID == DBNull.Value) ? 1 : Convert.ToInt32(maxID) + 1;
        }
        public static int GetMaxId<InnerType>(SQLConnection connect)
        {
            string tableName = typeof(InnerType).Name;
            string sql = $"SELECT MAX(Id) FROM {tableName}";
            SqliteCommand command = new SqliteCommand(sql, connect);
            object maxID = command.ExecuteScalar();
            maxID = maxID == "" ? "0" : maxID;

            return (maxID == DBNull.Value) ? 1 : Convert.ToInt32(maxID) + 1;
        }

        public string ToString(string tp)
        {
            return $"{tp} of {typeof(T).Name}: {DomainObject?.ToString()}";
        }

        public string ToString(string tp, string lstr)
        {
            return $"{tp} of {typeof(T).Name}: {lstr}";
        }

        public static Pattern FindInner<Pattern>(int id, SqlConnector conn, Func<SQLTypes, Pattern> fn)
        {
            using (SQLConnection connection = new SQLConnection(conn.ConnectionString))
            {
                connection.Open();

                string tableName = typeof(T).Name;
                string sql = $"SELECT * FROM {tableName} WHERE Id = @Id";

                SQLCommand command = new SQLCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using (SQLReader reader = command.ExecuteReader())
                {
                    // FIXME return null
                    if (fn == null) { return default(Pattern); }
                    return fn(new(connection, reader, null));
                }
            }
        }

        public static Pattern FindInner<Pattern>(
            SqlConnector conn, Func<SQLTypes, Pattern> fn
        )
        {
            return FindInner<Pattern>(conn, null, null, fn);
        }

        public static Pattern FindInner<Pattern>(
            SqlConnector conn, PropertyInfo prop, object value, Func<SQLTypes, Pattern> fn
        )
        {
            using (SQLConnection connection = new SQLConnection(conn.ConnectionString))
            {
                connection.Open();

                string tableName = typeof(T).Name;

                string sql;
                if (prop != null)
                {
                    sql = $"SELECT * FROM {tableName} WHERE {prop.Name} = @{prop.Name}";
                }
                else
                {
                    sql = $"SELECT * FROM {tableName}";
                }
                Console.WriteLine("FindInner: " + sql);

                SQLCommand command = new SQLCommand(sql, connection);
                command.Parameters.AddWithValue("@" + prop.Name, value);

                using (SQLReader reader = command.ExecuteReader())
                {
                    // FIXME return null
                    if (fn == null) { return default(Pattern); }
                    return fn(new(connection, reader, null));
                }
            }
        }

        protected void InnerFetch<DomainType>(Action<T, DomainType> Do_fetch)
            where DomainType : IEntity, new()
        {
            using (SQLConnection connection = new SQLConnection(SqlConnect.ConnectionString))
            {
                connection.Open();

                string tableName = typeof(DomainType).Name;

                string sql = $"SELECT * FROM {tableName}";

                using (SQLCommand command = new SQLCommand(sql, connection))
                {
                    using (SQLReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obj = new DomainType();

                            obj.Id = Convert.ToInt32(reader["Id"]);
                            SQLHelper<DomainType>.MapProperties(reader, obj, -1, SqlConnect);

                            Do_fetch(DomainObject, obj);
                        }
                    }
                }
            }
        }
    }

    public class PatternObjPrototype<T> : PatternPrototype<T>, IHaveIndexer where T : IEntity, new()
    {
        public PatternObjPrototype() : base() { }
        public PatternObjPrototype(SqlConnector sql)
        {
            DomainObject = new T();
            SqlConnect = sql;
        }

        public PatternObjPrototype(T domainObject, int id, SqlConnector sql)
            : base(domainObject, id, sql) { }

        protected PatternObjPrototype(SqlConnector sql, params object[] attributeValues)
            : base(sql, attributeValues) { }

        public void SQLAction(string sql, Func<SQLConnection, int>? getID = null)
        {
            SQLAction(sql, DomainObject, getID);
        }

        public void SQLAction(string sql, T obj, Func<SQLConnection, int>? getID = null)
        {
            using (SQLConnection connection = new SQLConnection(SqlConnect.ConnectionString))
            {
                connection.Open();

                int index = -1;
                if (getID != null)
                {
                    index = getID(connection);
                    // PropertyInfo idProperty = DomainObject.GetType().GetProperty("Id");
                }

                SQLCommand command = new SQLCommand(sql, connection);
                SQLHelper<T>.MapParameters(command, obj, index);
                Console.WriteLine("command: " + command.CommandText);
                command.ExecuteNonQuery();
            }
        }
    }
}
