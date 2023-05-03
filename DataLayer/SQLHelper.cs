using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Data.Sqlite;

namespace DataLayer
{
    // FIXME
    using SQLCommand = SqliteCommand;
    using SQLReader = SqliteDataReader;

    public class SQLHelper<T> where T : IEntity, new ()
    {
        public static string GetColumnNames()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            string columnNames = string.Join(", ", Array.ConvertAll(
                properties, p => p.Name)
            );
            //string columnNames = "Id, " + string.Join(", ", Array.ConvertAll(
            //    properties, p => p.Name));

            return columnNames;
        }

        public static string GetParameterNames()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            string parameterNames = string.Join(", ", Array.ConvertAll(
                properties, p => "@" + p.Name)
            ); 
            //string parameterNames = "@Id, " + string.Join(", ", Array.ConvertAll(
                //properties, p => "@" + p.Name));

            return parameterNames;
        }

        public static string GetSetExpressions()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            string[] setExpressions = Array.ConvertAll(
                properties, p => p.Name + " = @" + p.Name);

            return string.Join(", ", setExpressions);
        }

        public static string TableName() => typeof(T).Name;


        public static void MapParameters(SQLCommand command, T? obj, int id)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            //Console.WriteLine("MapParameters: id: " + id);
            //command.Parameters.AddWithValue("@" + "Id", id);
            Console.WriteLine("MapParameters: cmd: " + command.CommandText);

            if (obj == null)
            {
                Console.WriteLine("MapParameters: cmd: " + command.CommandText);
                return;
            }

            foreach (PropertyInfo property in properties)
            {
                object value;
                FKColumnAttribute attribute = property.GetCustomAttribute<FKColumnAttribute>();
                if (attribute != null)
                {
                    value = ((IEntity)property.GetValue(obj))?.Id;
                }
                else
                {
                    value = property.GetValue(obj);
                }
                command.Parameters.AddWithValue("@" + property.Name, value);
            }
        }

        public static void MapParameters(SQLCommand command, int id)
        {
            Console.WriteLine("no implementaion");
            //Console.WriteLine("MapParameters: id: " + id);
            //command.Parameters.AddWithValue("@" + "Id", id);
            //Console.WriteLine("MapParameters: cmd: " + command.CommandText);
        }

        public static void MapProperties(SQLReader reader, T obj, SqlConnector sql)
        {
            // FIXME
            MapProperties(reader, obj, -1, sql);
        }

        public static void MapProperties(SQLReader reader, T obj, int id, SqlConnector sql)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                object value = reader[property.Name];

                switch (value)
                {
                    case int intValue:
                        if (property.PropertyType == typeof(bool))
                        {
                            property.SetValue(obj, intValue != 0 ? true : false);
                            break;
                        }
                        property.SetValue(obj, intValue);
                        break;
                    case long longValue:
                        if (property.PropertyType == typeof(bool))
                        {
                            property.SetValue(obj, longValue != 0 ? true : false);
                            break;
                        }

                        if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(obj, (int)longValue);
                        }
                        else
                        {
                            FKColumnAttribute attribute = property.GetCustomAttribute<FKColumnAttribute>();
                            if (attribute != null)
                            {
                                //var v_id = ((IEntity)property.GetValue(obj))?.Id;
                                //if (v_id == null)
                                //{
                                //    // FIXME
                                //    property.SetValue(obj, null);
                                //    break;
                                //}

                                // Construct a generic type using reflection
                                Type genericType = typeof(ActiveRecord<>).MakeGenericType(property.PropertyType);

                                // Create an instance of the constructed generic type
                                object genericInstance = Activator.CreateInstance(genericType);

                                // Invoke a generic method on the instance
                                MethodInfo findMethod = genericType.GetMethod(
                                    "Find", new[] { typeof(int), typeof(SqlConnector) }
                                );
                                object record = findMethod.Invoke(null, new object[] { (int)longValue, sql });

                                PropertyInfo domainObjectProperty = genericType.GetProperty("DomainObject");
                                object domainObject = domainObjectProperty.GetValue(record);

                                //var record = ActiveRecord<property.PropertyType>.Find((int)longValue, sql);
                                property.SetValue(obj, domainObject);
                                break;
                            }

                            property.SetValue(obj, longValue);
                        }
                        break;
                    case string stringValue:
                        if (property.PropertyType.IsEnum)
                        {
                            Type eNum = property.PropertyType;
                            property.SetValue(obj, Enum.Parse(eNum, stringValue));
                            break;
                        }
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(obj, DateTime.Parse(stringValue));
                            break;
                        }
                        property.SetValue(obj, stringValue);
                        break;
                        // Add cases for other data types as needed
                }
            }
        }
    }
}
