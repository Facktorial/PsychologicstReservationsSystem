using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class SqlConnector
    {
        public string ConnectionString { get; set; }
        private readonly string NULL = " NULL, ";

        public SqlConnector() { }
        public SqlConnector(string ConnectionStr) { ConnectionString = ConnectionStr; }

        public void CreateTable<T>(bool DoClearDB)
        {
            Type modelType = typeof(T);
            string tableName = modelType.Name;
            PropertyInfo[] properties = modelType.GetProperties();

            Console.WriteLine(tableName);

            string sql = "CREATE TABLE IF NOT EXISTS " + tableName + " (id INTEGER PRIMARY KEY,";
            foreach (var property in properties)
            {
                Console.WriteLine(property.Name);

                if (property.Name.Contains("Id")) { continue; }
                if (property.Name.Contains("IdEntity")) { continue; }

                string columnName = property.Name;
                Type columnType = property.PropertyType;

                if (property.Name.Contains("Entity"))
                {
                    columnName = property.PropertyType.Name;
                    columnType = typeof(int);
                }

                //if (columnType.Name.Contains("List"))
                //{
                //    columnType = typeof(string);
                //}

                FKColumnAttribute attribute = property.GetCustomAttribute<FKColumnAttribute>();
                if (attribute != null)
                {
                    columnType = typeof(int);
                }

                string sqlType = GetSqlType(columnType);

                string tmp = ", ";
                NullableColumnAttribute null_attribute = property.GetCustomAttribute<NullableColumnAttribute>();
                if (null_attribute != null)
                {
                    if (null_attribute.IsNullable)
                    {
                        tmp = NULL;
                    }
                }
                sql += columnName + " " + sqlType + tmp;
            }
            sql = sql.TrimEnd(',', ' ') + ");";

            using (var conn = new SQLiteConnection(ConnectionString))
            {
                using (var command = new SQLiteCommand(sql, conn))
                {
                    Console.WriteLine("Command: " + sql);
                    conn.Open();

                    int result = command.ExecuteNonQuery();
                    Console.WriteLine(result);
                }
            }

            TestCreating(typeof(T).Name, DoClearDB);
        }

        public void TestCreating(string tableName, bool DoClearDB)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
                using (var command = new SQLiteCommand(sql, conn))
                {
                    command.Parameters.AddWithValue("@tableName", tableName);
                    conn.Open();
                    var result = command.ExecuteScalar();
                    Console.WriteLine("result: " + result);
                    if (result != null)
                    {
                        Console.WriteLine("Table '" + tableName + "' found in database");
                        if (DoClearDB) { ClearTable(tableName); }
                    }
                    else
                    {
                        Console.WriteLine("Table '" + tableName + "' not found in database");
                    }
                }
            }
        }

        public void ClearTable(string tableName) { ClearTable(tableName, ConnectionString); }

        public static void ClearTable(string tableName, string ConnectionString)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                string clearSql = "DELETE FROM " + tableName;
                using (var clearCommand = new SQLiteCommand(clearSql, conn))
                {
                    clearCommand.ExecuteNonQuery();
                }
            }
        }

        public static void TestDB(string str)
        {
            using (var conn = new SQLiteConnection(str))
            {
                string sql = "SELECT name FROM sqlite_master WHERE type='table'";
                using (var command = new SQLiteCommand(sql, conn))
                {
                    conn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        Console.WriteLine("TESTDB: about to read");
                        while (reader.Read())
                        {
                            string tableName = reader.GetString(0);
                            Console.WriteLine("Table name: " + tableName);
                        }
                    }
                }
            }
        }

        private string GetSqlType(Type type)
        {
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(bool))
            {
                return "INTEGER";
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return "REAL";
            }
            else if (type == typeof(string) || type.IsEnum)
            {
                return "TEXT";
            }
            else if (type == typeof(DateTime))
            {
                return "DATETIME";
            }
            else if (type == typeof(byte[]))
            {
                return "BLOB";
            }
            else
            {
                Console.WriteLine("throw: " + type.FullName);
                throw new InvalidOperationException("Invalid property type");
            }
        }
    }
}