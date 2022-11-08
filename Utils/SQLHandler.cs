using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Utils
{
    public class SQLHandler
    {
        private static string defaultDatabaseHost;
        private static string defaultDatabaseName;
        private static string path;

        private static void GetDBPath()
        {
            //Library library = new Library();
            path = Library.GetUtilityFile($"{CommonTestSettings.AppName}Database");
        }
        public static string GetDefaultDBHost() { return defaultDatabaseHost; }
        public static string GetDefaultDBName() { return defaultDatabaseName; }

        public static void SetDatabaseDefaults(string environment)
        {
            //Library library = new Library();
            //string path = library.GetUtilityFile($"{Constants.AppName}Database");
            Library.LoadVariables(path, "VirginiaRMS" + environment);

            defaultDatabaseHost = Library.dict["DBHost"];
            defaultDatabaseName = Library.dict["DBName"];
            Library.ClearVariables();
        }

        public static string GetDBHost(string excelKey, string environment)
        {
            //Library library = new Library();
            //string path = library.GetPath($"{Constants.AppName}Database");

            Library.LoadVariables(path, excelKey + environment);
            string databaseHost = Library.dict["DBHost"];
            Library.ClearVariables();

            return databaseHost;
        }

        public static string GetDBName(string excelKey, string environment)
        {
            //Library library = new Library();
            //string path = library.GetPath("MCTDatabase");

            Library.LoadVariables(path, excelKey + environment);
            string databaseName = Library.dict["DBName"];
            Library.ClearVariables();

            return databaseName;
        }

        /// <summary>
        /// Returns the value found in the specified column for a single record in the database
        /// </summary>
        public static string GetDatabaseValue(string sqlStatement, string columnName, string dbHost, string dbName)
        {
            if (sqlStatement.ToUpper().Contains("UPDATE")) { throw new Exception("Detected update statement in SQL statement meant for reading values"); }

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            //using (SqlConnection connection = new SqlConnection(@"Data Source = " + dbHost + "; Initial Catalog = " + dbName + "; Integrated Security = True"))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                //connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader[columnName].ToString();
                    }
                }

                throw new Exception("Could not find data with column: " + columnName);
            }

        }
        public static int GetDatabaseInt(string sqlStatement,string columnName,string dbHost,string dbName)
        {
            return int.Parse(GetDatabaseValue(sqlStatement, columnName, dbHost, dbName));
        }
        /// <summary>
        /// Returns a list of values found in each specified column for a single record in the database
        /// </summary>
        public static List<string> GetDatabaseValues(string sqlStatement, List<string> columnNames, string dbHost, string dbName)
        {
            if (sqlStatement.ToUpper().Contains("UPDATE")) { throw new Exception("Detected update statement in SQL statement meant for reading values"); }

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<string> values = new List<string>();
                    if (reader.Read())
                    {
                        foreach (string columnName in columnNames)
                        {
                            values.Add(reader[columnName].ToString().Trim());
                        }
                    }
                    return values;
                }
            }
        }

        public static List<string> GetStringResults(string sqlStatement, string dbHost, string dbName)
        {
            List<string> values = new List<string>();
            //if (sqlStatement.ToUpper().Contains("UPDATE")) { throw new Exception("Detected update statement in SQL statement meant for reading values"); }

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            values.Add(reader.GetString(0));
                        }
                        reader.NextResult();
                    }
                }
            }
            return values;
        }
        public static List<int> GetIntResults(string sqlStatement, string columnName, string dbHost, string dbName)
        {
            List<int> values = new List<int>();

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            values.Add(reader.GetInt32(0));
                        }
                        reader.NextResult();
                    }
                }
            }
            return values;
        }

        public static List<int> GetSmallIntResults(string sqlStatement, string columnName, string dbHost, string dbName)
        {
            List<int> values = new List<int>();
            if (sqlStatement.ToUpper().Contains("UPDATE")) { throw new Exception("Detected update statement in SQL statement meant for reading values"); }

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            values.Add(reader.GetInt16(0));
                        }
                        reader.NextResult();
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Executes the update SQL statement, returns the number of rows affected
        /// </summary>
        public static int UpdateDatabaseValue(string sqlStatement, string dbHost, string dbName)
        {
            if (!sqlStatement.ToUpper().Contains("UPDATE")) { throw new Exception("Expected an update SQL statement, but instead received: " + sqlStatement); }

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                command.CommandText = sqlStatement;

                int numRowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return numRowsAffected;
            }
        }

        /// <summary>
        /// Executes the update SQL statement, returns the number of rows affected
        /// </summary>
        public static int DeleteDatabaseValue(string sqlStatement, string dbHost, string dbName)
        {
            if (!sqlStatement.ToUpper().Contains("DELETE")) { throw new Exception("Expected a delete SQL statement, but instead received: " + sqlStatement); }

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                command.CommandText = sqlStatement;

                int numRowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return numRowsAffected;
            }
        }

        /// <summary>
        /// Executes the update SQL statement, returns the number of rows affected
        /// </summary>
        public static int InsertDatabaseValue(string sqlStatement, string dbHost, string dbName)
        {
            if (!sqlStatement.ToUpper().Contains("INSERT")) { throw new Exception("Expected an insert SQL statement, but instead received: " + sqlStatement); }

            using (SqlConnection connection = Library.ConnectToDatabase(dbHost, dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                command.CommandText = sqlStatement;

                int numRowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return numRowsAffected;
            }
        }

        public static string GetSQLCountQuery(string tableName, string columnName, string IDKey)
        {
            return string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = '{2}'", tableName, columnName, IDKey);
        }
        public static bool VerifyRowCount(string sqlStatement, int numExpectedRows, string dbHost, string dbName)
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source = " + dbHost + "; Initial Catalog = " + dbName + "; Integrated Security = True"))
            using (SqlCommand command = new SqlCommand(sqlStatement, connection))
            {
                connection.Open();

                int numFoundRows = (int?)command.ExecuteScalar() ?? 0;
                return (numFoundRows == numExpectedRows);
            }
        }
    }
}
