using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using WebLibs;

namespace OrderAppAPITest.Models
{
    public class ConnectionDB
    {
        public static string connString = "Server=.;Database=OrderFoodApp;User ID=sa;Password=123456;";
        public static bool SqlInsert(string table, IDictionary<string, Object> parameterMap)
        {
            var connectionString = connString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {

                    connection.Open();

                    //

                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = CreateInsertSql(table, parameterMap);

                        foreach (var pair in parameterMap)
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        Debug.WriteLine(command.CommandText.ToString());
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                return false;
            }
        }
        public static string CreateInsertSql(string table,
                                      IDictionary<string, Object> parameterMap)
        {
            var keys = parameterMap.Keys.ToList();
            // ToList() LINQ extension method used because order is NOT
            // guaranteed with every implementation of IDictionary<TKey, TValue>

            var sql = new StringBuilder("INSERT INTO ").Append(table).Append("(");

            for (var i = 0; i < keys.Count; i++)
            {
                sql.Append(keys[i]);
                if (i < keys.Count - 1)
                    sql.Append(",");
            }

            sql.Append(") VALUES(");

            for (var i = 0; i < keys.Count; i++)
            {
                sql.Append('@' + keys[i]);
                //.Append(keys[i]);
                if (i < keys.Count - 1)
                    sql.Append(", ");
            }
            return sql.Append(")").ToString();
        }
        private static string CreateUpdatetSql(string table,
                                IDictionary<string, Object> parameterMap, int id)
        {
            var keys = parameterMap.Keys.ToList();
            // ToList() LINQ extension method used because order is NOT
            // guaranteed with every implementation of IDictionary<TKey, TValue>

            var sql = new StringBuilder("UPDATE ").Append(table).Append(" SET ");

            for (var i = 0; i < keys.Count; i++)
            {
                sql.Append(keys[i]);
                sql.Append("=");
                sql.Append('@' + keys[i]);
                if (i < keys.Count - 1)
                    sql.Append(",");
            }
            sql.Append(" WHERE id=" + id);
            Debug.WriteLine("sql=" + sql.ToString());
            return sql.ToString();
        }
        public static bool SqlUpdate(string table, IDictionary<string, Object> parameterMap, int id)
        {
            Debug.WriteLine("current date=" + DateTime.Now.ToString());
            var connectionString = connString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {

                    connection.Open();

                    //

                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = CreateUpdatetSql(table, parameterMap, id);
                        command.CommandTimeout = 180;
                        foreach (var pair in parameterMap)
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        Debug.WriteLine(command.CommandText.ToString());
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                return false;
            }
        }
        public static List<Dictionary<string, object>> SqlSelect(string sqlQuery, IDictionary<string, Object> condition)
        {
            var ConnectionString = connString;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {

                    connection.Open();

                    //

                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = sqlQuery;

                        foreach (var pair in condition)
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        Debug.WriteLine("CommandText=" + command.CommandText.ToString());

                        // command.ExecuteNonQuery();

                        var reader = command.ExecuteReader();
                        DataTable schema = reader.GetSchemaTable();
                        //getDatatypeColumn(schema);
                        while (reader.Read())
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataRow dr in schema.Rows)
                            {
                                String name = dr[schema.Columns["ColumnName"]].ToString();
                                var value = reader[name];
                                //Debug.WriteLine(name+"=" + value);
                                row.Add(name, value);
                            }
                            rows.Add(row);
                        }
                    }
                }
                //Debug.WriteLine("Data Result=" + Newtonsoft.Json.JsonConvert.SerializeObject(rows));
                //return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                //return false;
            }

            return rows;
        }
        public static List<Dictionary<string, string>> SqlSelectString(string sqlQuery, IDictionary<string, string> condition)
        {
            var ConnectionString = connString;
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            Dictionary<string, string> row;
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {

                    connection.Open();

                    //

                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = sqlQuery;

                        foreach (var pair in condition)
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        Debug.WriteLine("CommandText=" + command.CommandText.ToString());

                        // command.ExecuteNonQuery();

                        var reader = command.ExecuteReader();
                        DataTable schema = reader.GetSchemaTable();
                        //getDatatypeColumn(schema);
                        while (reader.Read())
                        {
                            row = new Dictionary<string, string>();
                            foreach (DataRow dr in schema.Rows)
                            {
                                String name = dr[schema.Columns["ColumnName"]].ToString();
                                var value = reader[name].ToString();
                                //Debug.WriteLine(name+"=" + value);
                                row.Add(name, value);
                            }
                            rows.Add(row);
                        }
                    }
                }
                //Debug.WriteLine("Data Result=" + Newtonsoft.Json.JsonConvert.SerializeObject(rows));
                //return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                //return false;
            }
            return rows;
        }
        public static int SqlInsertGetID(string query)
        {
            int so = 0;
            var connectionString = connString;
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(connectionString))
                {

                    SqlCommand cmd = new SqlCommand(query, sqlConnection1);

                    cmd.Connection.Open();
                    so = int.Parse(cmd.ExecuteScalar().ToString());
                }
                return so;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                return 0;
            }
        }

        private static string CreateDeleteSql(string table,int id)
        {
            // ToList() LINQ extension method used because order is NOT
            // guaranteed with every implementation of IDictionary<TKey, TValue>

            var sql = new StringBuilder("DELETE ").Append(table);

            sql.Append(" WHERE id=" + id);
            return sql.ToString();
        }
        public static bool SqlDelete(string table, int id)
        {
            var connectionString = connString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //
                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = CreateDeleteSql(table, id);
                        command.CommandTimeout = 180;
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                return false;
            }
        }
        public static string CreateInsertSqlGetID(string table,
                                      IDictionary<string, Object> parameterMap)
        {
            var keys = parameterMap.Keys.ToList();
            // ToList() LINQ extension method used because order is NOT
            // guaranteed with every implementation of IDictionary<TKey, TValue>

            var sql = new StringBuilder("INSERT INTO ").Append(table).Append("(");

            for (var i = 0; i < keys.Count; i++)
            {
                sql.Append(keys[i]);
                if (i < keys.Count - 1)
                    sql.Append(",");
            }

            sql.Append(") VALUES(");

            for (var i = 0; i < keys.Count; i++)
            {
                sql.Append('@' + keys[i]);
                if (i < keys.Count - 1)
                    sql.Append(", ");
            }
            return sql.Append("); SELECT SCOPE_IDENTITY()").ToString();
        }
        public static int SqlInsertGetID(string table, IDictionary<string, Object> parameterMap)
        {
            int user_id = 0;
            var connectionString = connString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {

                    connection.Open();

                    //

                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = CreateInsertSqlGetID(table, parameterMap);

                        foreach (var pair in parameterMap)
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        user_id = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                return user_id;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                return 0;
            }
        }
        private static string CreateUpdateParamSql(string table,
                                IDictionary<string, Object> parameterMap, int id,string nameParam)
        {
            var keys = parameterMap.Keys.ToList();
            // ToList() LINQ extension method used because order is NOT
            // guaranteed with every implementation of IDictionary<TKey, TValue>

            var sql = new StringBuilder("UPDATE ").Append(table).Append(" SET ");

            for (var i = 0; i < keys.Count; i++)
            {
                sql.Append(keys[i]);
                sql.Append("=");
                sql.Append('@' + keys[i]);
                if (i < keys.Count - 1)
                    sql.Append(",");
            }
            sql.Append(" WHERE " + nameParam + "=" + id);
            return sql.ToString();
        }
        public static bool SqlUpdate(string table, IDictionary<string, Object> parameterMap, int id, string nameParam)
        {
            Debug.WriteLine("current date=" + DateTime.Now.ToString());
            var connectionString = connString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {

                    connection.Open();

                    //

                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = CreateUpdateParamSql(table, parameterMap, id, nameParam);
                        command.CommandTimeout = 180;
                        foreach (var pair in parameterMap)
                        {
                            command.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        Debug.WriteLine(command.CommandText.ToString());
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                return false;
            }
        }
        private static string CreateDeleteSql(string table, int id, string nameParam)
        {
            // ToList() LINQ extension method used because order is NOT
            // guaranteed with every implementation of IDictionary<TKey, TValue>

            var sql = new StringBuilder("DELETE ").Append(table);

            sql.Append(" WHERE " + nameParam + "=" + id);
            return sql.ToString();
        }
        public static bool SqlDelete(string table, int id, string nameParam)
        {
            var connectionString = connString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //
                    using (var command = connection.CreateCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = CreateDeleteSql(table, id, nameParam);
                        command.CommandTimeout = 180;
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception=" + e.ToString());
                return false;
            }
        }
    }
}