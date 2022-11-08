using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Threading;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace Utils
{
    public class DatabaseReader
    {
        string reportAdd;
        string categoryAdd;
        bool status = true;

        //By adviseType = By.Id("AdvisoryType");
        //By statusType = By.Id("Status");
        //By callRef = By.Id("CallRefNo");
        //By streetName = By.Id("Street");
        //By crossRoads1 = By.Id("Crossroad1");
        //By crossRoads2 = By.Id("Crossroad2");
        //By agency = By.Id("Agency");
        //By nat = By.Id("Nature");
        //By pri = By.Id("Priority");
        //By eventId = By.Id("EventID");

        //Identifies if environment is Dev or Prod
        public string GetEnvironment(IWebDriver driver)
        {
            String url = driver.Url;
            String environment = "";

            //Prod
            if (url.Contains("spshprmsauto"))
            {
                environment = "Prod";
            }
            else if (url.Contains("spshpqasql2014"))
            {
                environment = "Dev";
            }
            //Assert.AreNotEqual(environment, "");
            if (environment != null)
            {
                return environment;
            }
            else
            {
                throw new ArgumentNullException("Environment cannot be null");
            }
            
        }
        
        /// <param name="values">List of tuples containing the category names for excel, database column names, and their expected values</param>
        /// <param name="table">Table that contains values to be checked</param>
        /// <param name="keyColumn">Known column with the key for the SQL string</param>
        /// <param name="key">Known value on the same row as values to be checked</param>
        public void DatabaseVerification(IWebDriver driver, List<Tuple<string, string, string>> values, string table, string keyColumn, string key, string database = "VirginiaRMS",
            [CallerFilePath] string source = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            // Tuple.Item1 = Excel category name
            // Tuple.Item2 = Database column name
            // Tuple.Item3 = Expected value

            string environment = GetEnvironment(driver);
            Library.ClearVariables();
            Library.LoadVariables(Library.GetDBFilePath(), database + environment);

            string sqlstr = "SELECT * from " + table + " where " + keyColumn + " = '" + key + "'";
            Library.ConnectDB(CommonTestSettings.dbHost, CommonTestSettings.dbName, sqlstr, out DataTable datab);

            if (datab.Rows.Count == 0)
            {
                reportAdd = "No rows found with query: " + sqlstr + "\nCould not verify: ";
                foreach (Tuple<string, string, string> value in values)
                {
                    reportAdd += "\"" + value.Item1 + "\", ";
                }
                ReportBuilder.ArrayBuilder(reportAdd.Substring(0,reportAdd.Length-2), false, "Database Check");
            }
            else
            {
                foreach (Tuple<string, string, string> value in values)
                {
                    if (Regex.IsMatch(value.Item1, "date", RegexOptions.IgnoreCase))
                    {
                        // Database stores date field values as DateTime, with default time set to 12am of that date
                        // Trim to match format of OSMCT form
                        string date = datab.Rows[0][value.Item2].ToString().Trim();
                        string formattedDate = DateTime.Parse(date).ToString("MM/dd/yyyy");
                        Library.LogResults(value.Item1, formattedDate, value.Item3, source, sourceLineNumber);
                    }
                    else
                    {
                        Library.LogResults(value.Item1, datab.Rows[0][value.Item2].ToString().Trim(), value.Item3, source, sourceLineNumber);
                    }
                }
            }
            Library.ClearVariables();
        }
        /// <param name="values">List of tuples containing the category names for excel, database column names, and their expected values</param>
        /// <param name="table">Table that contains values to be checked</param>
        public void DatabaseVerification(IWebDriver driver, List<Tuple<string, string, string>> values, string sqlSelectStatement,
            [CallerFilePath] string source = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            string path = Library.GetDBFilePath();
            string environment = GetEnvironment(driver);
            //TODO: Update when environments stable
            Library.LoadVariables(path, "VirginiaRMS" + environment);

            Library.ConnectDB(CommonTestSettings.dbHost, CommonTestSettings.dbName, sqlSelectStatement, out DataTable datab);

            if (datab.Rows.Count == 0)
            {
                reportAdd = "No rows found with query: " + sqlSelectStatement + "\nCould not verify: ";
                foreach (Tuple<string, string, string> value in values)
                {
                    reportAdd += "\"" + value.Item1 + "\", ";
                }
                ReportBuilder.ArrayBuilder(reportAdd.Substring(0, reportAdd.Length - 2), false, "Database Check");
            }
            else
            {
                foreach (Tuple<string, string, string> value in values)
                {
                    if (Regex.IsMatch(value.Item1, "date", RegexOptions.IgnoreCase))
                    {
                        string date = datab.Rows[0][value.Item2].ToString().Trim();
                        string formattedDate = FormatDate(date);
                        Library.LogResults(value.Item1, formattedDate, value.Item3, source, sourceLineNumber);
                    }
                    else if (Regex.IsMatch(value.Item1, "time", RegexOptions.IgnoreCase))
                    {
                        string time = datab.Rows[0][value.Item2].ToString().Trim();
                        string formattedTime = FormatTime(time);
                        Library.LogResults(value.Item1, formattedTime, value.Item3, source, sourceLineNumber);                       
                    }
                    else
                    {
                        Library.LogResults(value.Item1, datab.Rows[0][value.Item2].ToString().Trim(), value.Item3, source, sourceLineNumber);
                    }
                }
            }
            Library.ClearVariables();
        }
        /// <summary>
        /// Database stores date field values as DateTime with default time set to 12am of that date
        /// Return a trimmed version to match format of OSMCT form
        /// </summary>
        private string FormatDate(string date)
        {
            string dateFormat = "MM/dd/yyyy";
            try
            {
                return DateTime.Parse(date).ToString(dateFormat);
            }
            catch (FormatException e)
            {
                ReportBuilder.ArrayBuilder("Failed to convert " + date + " to " + dateFormat + " format", false, e.GetType().ToString());
                return date;
            }
        }
        /// <summary>
        /// Database stores time field values as DateTime, with default time set to 12am of that date
        /// Trim to match format of OSMCT form
        /// </summary>
        private string FormatTime(string time)
        {
            string timeFormat = "MM/dd/yyyy HH:mm";
            try
            {
                return DateTime.Parse(time).ToString(timeFormat);
            }
            catch (FormatException e)
            {
                ReportBuilder.ArrayBuilder("Failed to convert " + time + " to " + timeFormat + " format", false, e.GetType().ToString());
                return time;
            }
        }
        /// <param name="values">Tuple containing the category name for excel, database column name, and the expected value</param>
        /// <param name="table">Table that contains values to be checked</param>
        /// <param name="keyColumn">Known column with the key for the SQL string</param>
        /// <param name="key">Known value on the same row as values to be checked</param>
        public void DatabaseVerification(IWebDriver driver, Tuple<string, string, string> values, string sqlSelectStatement, string database)
        {
            
            string environment = GetEnvironment(driver);
            Library.ClearVariables();
            Library.LoadVariables(Library.GetDBFilePath(), database + environment);

            Library.ConnectDB(CommonTestSettings.dbHost, CommonTestSettings.dbName, sqlSelectStatement, out DataTable datab);

            if (datab.Rows.Count == 0)
            {
                reportAdd = "No rows found with query: " + sqlSelectStatement + "\nCould not verify: " + values.Item1;
                ReportBuilder.ArrayBuilder(reportAdd, false, "Database Check");
            }
            else
            {
                Library.LogResults(values.Item1, datab.Rows[0][values.Item2].ToString().Trim(), values.Item3);
            }
            Library.ClearVariables();
        }
        
        //Return information from target Excel document
        //public string GetScenarioDataFromExcel(string scenario, string spreadsheet, string field)
        //{
        //    String targetData;
            
        //    string ExcelFilePath = Directory.GetCurrentDirectory() + "Utility\\" + spreadsheet + ".xlsx";
            
        //    string path = "";

        //    if (System.Diagnostics.Debugger.IsAttached)
        //    {
        //        path = ExcelFilePath.Replace("bin\\Debug", "");
        //    }
        //    else
        //    {
        //        path = ExcelFilePath.Replace("bin\\Release", "");
        //    }

        //    Library.LoadVariables(path, scenario);
        //    targetData = dict[field];
        //    Library.ClearVariables();
        //    return (targetData);
        //}
        
        

        //Get server's address
        public String GetServerAddress(IWebDriver driver)
        {
            DataTable datab;
            String sqlstr, targetAddress;

            string dbFilePath = Library.GetDBFilePath();
            string path = "";

            if (System.Diagnostics.Debugger.IsAttached)
            {
                path = dbFilePath.Replace("bin\\Debug", "");
            }
            else
            {
                path = dbFilePath.Replace("bin\\Release", "");
            }
            //.LoadVariables(path, "MCT");
            string environment = GetEnvironment(driver);
            Library.LoadVariables(path, "VirginiaCAD" + environment);
            sqlstr = "SELECT CONNECTIONPROPERTY('client_net_address') AS client_net_address";
            Library.ConnectDB(CommonTestSettings.dbHost, CommonTestSettings.dbName, sqlstr, out datab);

            targetAddress = datab.Rows[0]["client_net_address"].ToString();
            Library.ClearVariables();

            return targetAddress;
        }

        

        /*Create a new record in test_log_parent table. Populate the start_time field with the current
        time. Return the prime_key assigned to this record.*/
        public static string CreateParentRecord()
        {
            DataTable datatable;
            String sqlstr, primeKey;
            String startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            //Load Dictionary
            //string dbFilePath = Library.GetDBFilePath();
            string path = "";

            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    path = dbFilePath.Replace("bin\\Debug", "");
            //}
            //else
            //{
            //    path = dbFilePath.Replace("bin\\Release", "");
            //}

            //Get Database Login Info
            //Library.LoadVariables(path, "Insert");

            sqlstr = $"INSERT INTO test_log_parent (start_time) VALUES ('{startTime}') " +
                $"SELECT prime_key FROM test_log_parent WHERE start_time = '{startTime}'";

            Library.ConnectDB(CommonTestSettings.ReportingDBHost, CommonTestSettings.ReportingDBName, sqlstr, out datatable,CommonTestSettings.ReportingDBUser,CommonTestSettings.ReportingDBP);

            primeKey = datatable.Rows[0]["Prime_Key"].ToString();

            //Library.ClearVariables();

            return (primeKey);
        }

        /*Add the current scenario to the scenario_list field, in the record with the correct 
         prime_key, in test_log_parent*/
        public void AddToScenarioList(String primeKey, String scenario)
        {
            DataTable datab;
            String sqlstr, scenarioNum, scenarioList, scenarioListUpdated;

            //Load Dictionary
            string dbFilePath = Library.GetDBFilePath();
            
            string path = "";
            if (System.Diagnostics.Debugger.IsAttached)
            {
                path = dbFilePath.Replace("bin\\Debug", "");
            }
            else
            {
                path = dbFilePath.Replace("bin\\Release", "");
            }
            //Get Database Login Info
            //LoadVariables(path, "Insert");
            sqlstr = "SELECT master_key FROM test_master WHERE master_desc = '" + scenario + "'";
            Library.ConnectDB(CommonTestSettings.ReportingDBHost, CommonTestSettings.ReportingDBName, sqlstr, out datab, CommonTestSettings.ReportingDBUser, CommonTestSettings.ReportingDBP);
            //Get the master_key for the current scenario from test_master.
            //If the current scenario is not in test_master, add an alert to the excel report,
            //and set scenarioNum to "Not in test_master."
            if (datab.Rows.Count > 0)
            {
                scenarioNum = datab.Rows[0]["master_Key"].ToString();
            }
            else
            {
                reportAdd = $"Scenario [{scenario}] was not found in test_master table.";
                categoryAdd = "ALERT";
                ReportBuilder.ArrayBuilder(reportAdd, false, categoryAdd);
                scenarioNum = "Not in test_master.";
            }
            
            //Get whatever is currently in scenario_list
            sqlstr = "SELECT scenario_list FROM test_log_parent WHERE prime_key = '" + primeKey + "'";
            Library.ConnectDB(CommonTestSettings.ReportingDBHost, CommonTestSettings.ReportingDBName, sqlstr, out datab, CommonTestSettings.ReportingDBUser, CommonTestSettings.ReportingDBP);

            //If scenario_list is empty, replace it with the current scenarioNum.
            //If scenario_list already has something in it, add the current scenarioNum to the end
            //of the current entry, separated by a comma and a space.
            if (datab.Rows[0]["scenario_list"].ToString() == "")
            {
                scenarioListUpdated = scenarioNum;
            }
            else
            {
                scenarioList = datab.Rows[0]["scenario_list"].ToString();
                scenarioListUpdated = scenarioList + ", " + scenarioNum;
            }
            sqlstr = "UPDATE test_log_parent SET scenario_list='" + scenarioListUpdated +
                    "' WHERE prime_key='" + primeKey + "' SELECT * FROM test_log_parent WHERE prime_key = '"
                    + primeKey + "'";
            Library.ConnectDB(CommonTestSettings.ReportingDBHost, CommonTestSettings.ReportingDBName, sqlstr, out datab, CommonTestSettings.ReportingDBUser, CommonTestSettings.ReportingDBP);

            Library.ClearVariables();
        }

        //Change the end_time field in test_log_parent to match the current time.
        public void UpdateEndTimeAndBuildVersion(String primeKey,string version)
        {
            DataTable datab;
            String sqlstr;
            DateTime timedate = DateTime.Now;
            String endTime = timedate.ToString("yyyy-MM-dd HH:mm:ss.fff");

            //Load Dictionary
            //string dbFilePath = GetDBFilePath();

            //string path = "";
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    path = dbFilePath.Replace("bin\\Debug", "");
            //}
            //else
            //{
            //    path = dbFilePath.Replace("bin\\Release", "");
            //}
            //Get Database Login Info
            //LoadVariables(path, "Insert");
            sqlstr = $"UPDATE test_log_parent SET end_time='{endTime}', build='{version}' " +
                $"WHERE prime_key='{primeKey}' " +
                $"SELECT * FROM test_log_parent WHERE prime_key = '{primeKey}'";
            Library.ConnectDB(CommonTestSettings.ReportingDBHost, CommonTestSettings.ReportingDBName, sqlstr, out datab, CommonTestSettings.ReportingDBUser, CommonTestSettings.ReportingDBP);

            //ClearVariables();
        }
        /// <summary>
        /// Takes an excel file name (without the file extension) and returns a string to its path in the Utility folder
        /// </summary>
        public string GetExcelFilePathInUtility(string fileName)
        {
            string ExcelFilePath = Path.Combine(Library.GetUtilityPath(), fileName + ".xlsx");

            if (System.Diagnostics.Debugger.IsAttached)
            {
                return ExcelFilePath.Replace("bin\\Debug", "");
            }
            else
            {
                return ExcelFilePath.Replace("bin\\Release", "");
            }
        }
        /// <summary>
        /// Returns path to file located in Utility folder
        /// </summary>
        public string GetFileInUtility(string fileName)
        {
            string filePath = Path.Combine(Library.GetUtilityPath(), fileName);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return filePath.Replace("bin\\Debug", "");
            }
            else
            {
                return filePath.Replace("bin\\Release", "");
            }
        }
        public string GetUntrimmedValueFromDatabase(IWebDriver driver, string columnName, string tableName, string whereColumn, string whereValue)
        {
            string dbFilePath = Library.GetDBFilePath();
            string environment = GetEnvironment(driver);
            //TODO: Change when env is stable
            Library.LoadVariables(dbFilePath, "VirginiaRMS" + environment);

            string sqlstr = "Select " + columnName + " FROM " + tableName + " WHERE " + whereColumn + " = '" + whereValue + "'";
            Library.ConnectDB(CommonTestSettings.dbHost, CommonTestSettings.dbName, sqlstr, out DataTable datab);

            string value = datab.Rows[0][columnName].ToString();
            Library.ClearVariables();

            return value;
        }
        /// <summary>
        /// Returns the value of the specified cell in the database with: 
        /// "SELECT columnName FROM tableName WHERE whereColumn  = 'whereValue'"
        /// </summary>
        /// <param name="columnName">column of desired cell</param>
        /// <param name="tableName">table desired cell is located in</param>
        /// <param name="whereColumn">column that holds cell with known value in same row as desired value</param>
        /// <param name="whereValue">known value in same row as desired value</param>
        /// <returns></returns>
        public string GetValueFromDatabase(IWebDriver driver, string columnName, string tableName, string whereColumn, string whereValue)
        {
            string dbFilePath = Library.GetDBFilePath();
            string environment = GetEnvironment(driver);
            Library.LoadVariables(dbFilePath, "VirginiaRMS" + environment);

            string sqlstr = "Select " + columnName + " FROM " + tableName + " WHERE " + whereColumn + " = '" + whereValue + "'";
            Library.ConnectDB(CommonTestSettings.dbHost, CommonTestSettings.dbName, sqlstr, out DataTable datab);

            string value = datab.Rows[0][columnName].ToString().Trim();
            Library.ClearVariables();

            return value;
        }
        /// <summary>
        /// Returns the value of the specified cell in the database with: 
        /// "SELECT columnName FROM tableName WHERE whereColumn  = 'whereValue'"
        /// </summary>
        /// <param name="database">name of the desired database</param>
        /// <param name="columnName">column of desired cell</param>
        /// <param name="tableName">table desired cell is located in</param>
        /// <param name="whereColumn">column that holds cell with known value in same row as desired value</param>
        /// <param name="whereValue">known value in same row as desired value</param>
        /// <returns></returns>
        public string GetValueFromDatabase(IWebDriver driver, string database, string column, string table, string keyColumn, string keyValue)
        {
            string dbFilePath = Library.GetDBFilePath();
            string environment = GetEnvironment(driver);
            Library.LoadVariables(dbFilePath, database + environment);

            string sqlstr = "SELECT " + column + " from " + table + " where " + keyColumn + " = '" + keyValue + "'";
            Library.ConnectDB(CommonTestSettings.dbHost, CommonTestSettings.dbName, sqlstr, out DataTable datab);

            if (datab.Rows.Count == 0)
            {
                reportAdd = "No rows found with sqlstr: " + sqlstr + "\nCould not get value in column: " + column;
                ReportBuilder.ArrayBuilder(reportAdd, false, "Database Check");
            }

            string value = datab.Rows[0][column].ToString().Trim();
            Library.ClearVariables();

            return value;
        }

    }
}
