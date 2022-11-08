using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text.RegularExpressions;
using DataTable = System.Data.DataTable;

namespace Utils
{
    public class Library
    {
        public static Dictionary<string, string> dict = new Dictionary<string, string>();
        
        public static ArrayList myAL = new ArrayList();
        public SqlConnection con;      
        public SqlCommand cmd;
        public SqlDataAdapter da;
        public System.Data.DataTable dt;
        public static double checksCount = 0;
        public static double dataCount = 0;
        public static double counter = 0;
        public double endCount = 0;
        public double endChecks = 0;
        public double endFails;
        public static double mathCounter = 0;
        public static double passed;
        public static double endPassed = 0;
        public static double endingPercent;
        public static double fails = 0;
        public double failCounter = 0;
        public static List<String> scenariosTested = new List<String>();
        public static string AppName;

        public Library(string AppName)
        {
            Library.AppName = AppName;
        }

        /// <summary>
        /// Takes an excel file name (without the file extension) and returns a string to its path in the Utility folder
        /// </summary>
        public static string GetPath(string fileName)
        {
            string utilityFilePath = @"..\..\Utility\";
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), utilityFilePath);

            return string.Format("{0}{1}.xlsx", fullPath, fileName);
        }
                
        /// <summary>
        /// Returns the current directory + Utility\\
        /// </summary>
        /// <returns></returns>
        public static string GetUtilityDirectory()
        {
            string utilDir = Directory.GetCurrentDirectory() + "Utility\\";
            return utilDir.Contains("Debug") ? utilDir.Replace("bin\\Debug", "") : utilDir.Replace("bin\\Release", "");
        }


        #region Image Comparisons
        //Image Comparisons
        public static string GetImageFilePath(string scenario, string fileName)
        {
            //Library lib = new Library();
            string path = GetPath($"{AppName}Images");
            Library.LoadVariables(path, scenario);
            string filePath = Directory.GetCurrentDirectory() + "\\" + dict[fileName];
            ClearVariables();

            return filePath;
        }
        public static bool CompareBitmaps(Bitmap sourceImage, Bitmap targetImage)
        {
            if (targetImage.Size != sourceImage.Size) { return false; }

            for (int j = 0; j < targetImage.Height; j++)
            {
                for (int i = 0; i < targetImage.Width; i++)
                {
                    Color sourceColor = sourceImage.GetPixel(i, j);
                    Color targetColor = targetImage.GetPixel(i, j);
                    if (!sourceColor.Equals(targetColor)) { return false; }
                }
            }

            return true;
        }
        public static bool CompareImages(IWebDriver driver, string image1Name, string image2Name)
        {
            string image1Path = GetImageFilePath(image1Name, "Target Image");
            string image2Path = GetImageFilePath(image2Name, "Target Image");

            Bitmap image1 = new Bitmap(image1Path);
            Bitmap image2 = new Bitmap(image2Path);

            if (CompareBitmaps(image1, image2)) { return true; }
            else { return false; }
        }
        #endregion

        #region Random Character methods
        /// <summary>
        /// Returns a 32 character alphanumeric
        /// </summary>
        public static string GetRandomInput()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        public static string GetRandomAlphabetical()
        {
            return Regex.Replace(GetRandomInput(), @"[\d-]", string.Empty);
        }
        public static string GetRandomAlphabeticalUpper()
        {
            return GetRandomAlphabetical().ToUpper();
        }
        public static string GetRandomAlphabeticalLower()
        {
            return GetRandomAlphabetical().ToLower();
        }
        public static string GetRandomLetter()
        {
            return GetRandomAlphabetical().Substring(0, 1);
        }
        public static string GetRandomLetterUpper()
        {
            return GetRandomLetter().ToUpper();
        }
        public static string GetRandomLetterLower()
        {
            return GetRandomLetter().ToLower();
        }
        public static string GetRandomStringOfDigits(int length)
        {
            Random random = new Random();
            System.Text.StringBuilder digits = new System.Text.StringBuilder();
            for (int i = 0; i < length; i++)
            {
                digits.Append(random.Next(0, 9));
            }
            return digits.ToString();
        }
        #endregion

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
        public static string GetDBFilePath()
        {
            //TODO: Fix this to + getcurrent and utility
            return Path.Combine(GetUtilityDirectory(), AppName + "Database.xlsx");
        }
        #region Excel

        public string GetFromExcel(string scenario, string spreadsheet, string field)
        {
            String targetData;
            

            string dbFilePath = GetDBFilePath();

            string path = "";
            if (System.Diagnostics.Debugger.IsAttached)
            {
                path = dbFilePath.Replace("bin\\Debug", "");
            }
            else
            {
                path = dbFilePath.Replace("bin\\Release", "");
            }

            LoadVariables(path, scenario);
            targetData = dict[field];
            ClearVariables();
            return (targetData);
        }

        public static string GetValueFromExcel(string fileName, string scenario, string columnName)
        {
            
            ClearVariables();

            string path = GetPath(fileName);
            LoadVariables(path, scenario);

            string value = dict[columnName];
            ClearVariables();

            return value;
        }
        public static void LoadVariables(string filepath, string testcaseid)
        {
            // without excel - under construction
            #region
            //string ExPath = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0 Xml;HDR=Yes;\";";
            //string query = "Select * from [Sheet1$]";

            //Dictionary<string, string> output = new Dictionary<string, string>();
            //using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(ExPath))
            //using (System.Data.OleDb.OleDbCommand command = new System.Data.OleDb.OleDbCommand(query, conn))
            //{
            //    command.Connection.Open();
            //    System.Data.OleDb.OleDbDataReader reader = command.ExecuteReader();




            //    int row = 0;
            //    while (reader.Read())
            //    {
            //        if (reader.GetValue(0).ToString() == testcaseid)
            //        {
            //            break;
            //        }
            //    }

            //    for (int index = 1; index < reader.FieldCount; index++)
            //    {
            //        if (!string.IsNullOrWhiteSpace(reader.GetName(index)))
            //        {

            //            output.Add(reader.GetName(index), reader.GetValue(index) as dynamic);
            //        }
            //    }
            //    command.Connection.Close();
            //}

            //dict = output;
            #endregion

            // Loads Variables By Opening Excel and reading from it
            #region
            //dict.Add("DBHost", Globals.dbHost);

                //dict["DBHost"], dict["DBName"], dict["UserName"], dict["Password"]
            //Application xlApp;
            //Workbooks xlWorkBooks;
            //Workbook xlWorkBook;
            //Worksheet xlWorkSheet;
            //Sheets xlWorkSheets;
            //int RowCount, ColumnCount;
            //int RowIndex, ColumnIndex;

            //xlApp = new Application();
            //xlWorkBooks = xlApp.Workbooks;
            //xlWorkBook = xlWorkBooks.Open(filepath);
            //xlWorkSheets = xlWorkBook.Worksheets;
            //xlWorkSheet = (Worksheet)xlWorkSheets.get_Item(1);

            //RowCount = xlWorkSheet.UsedRange.Rows.Count;
            //ColumnCount = xlWorkSheet.UsedRange.Columns.Count;

            //try
            //{
            //    for (RowIndex = 2; RowIndex <= RowCount; RowIndex++)
            //    {

            //        if (xlWorkSheet.Cells[RowIndex, 1].Value.ToString() == testcaseid)
            //        {
            //            //dictionary entries
            //            for (ColumnIndex = 2; ColumnIndex <= ColumnCount; ColumnIndex++)
            //            {
            //                if (!(xlWorkSheet.Cells[1, ColumnIndex].Value is null))
            //                {

            //                    if (!(xlWorkSheet.Cells[RowIndex, ColumnIndex].Value is null))
            //                    {

            //                        dict.Add(xlWorkSheet.Cells[1, ColumnIndex].Value.ToString(), xlWorkSheet.Cells[RowIndex, ColumnIndex].Value.ToString());
            //                    }
            //                    else
            //                    {

            //                        dict.Add(xlWorkSheet.Cells[1, ColumnIndex].Value.ToString(), null);
            //                    }
            //                }
            //            }

            //            break;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //finally
            //{
            //    xlWorkBook.Close(false, Type.Missing, Type.Missing);
            //    xlWorkBooks.Close();
            //    xlApp.Quit();

            //    //-----Unmarshal COM objects to prevent memory leaks-------
            //    Marshal.FinalReleaseComObject(xlApp);
            //    Marshal.FinalReleaseComObject(xlWorkBook);
            //    Marshal.FinalReleaseComObject(xlWorkBooks);
            //    Marshal.FinalReleaseComObject(xlWorkSheet);
            //    Marshal.FinalReleaseComObject(xlWorkSheets);
            //}


            //xlWorkBook = null;
            //xlApp = null;
            #endregion
        }

        public static void ClearVariables()
        {
            dict.Clear();
        }

        [Obsolete]
        public void LoadVariablesMultiSheet(string filepath, string sheetName)
        {
            // Dictionary<string, string> dict = new Dictionary<string, string>();
            //Application xlApp;
            //Workbooks xlWorkBooks;
            //Workbook xlWorkBook;
            //Worksheet xlWorkSheet;
            //Sheets xlWorkSheets;
            //int RowCount, ColumnCount;
            //int RowIndex, ColumnIndex;

            //xlApp = new Application();
            //xlWorkBooks = xlApp.Workbooks;
            //xlWorkBook = xlWorkBooks.Open(filepath);
            //xlWorkSheets = xlWorkBook.Worksheets;
            //xlWorkSheet = (Worksheet)xlWorkSheets.get_Item(sheetName);

            //RowCount = xlWorkSheet.UsedRange.Rows.Count;
            //ColumnCount = xlWorkSheet.UsedRange.Columns.Count;
            //int dictRowCnt = 0;

            //for (RowIndex = 2; RowIndex <= RowCount; RowIndex++)
            //{
            //    for (ColumnIndex = 1; ColumnIndex <= ColumnCount; ColumnIndex++)
            //    {

            //        if (Convert.ToString(xlWorkSheet.Cells[RowIndex, ColumnIndex].Value) != null)
            //        {
            //            if(dictRowCnt == 0)
            //            {
            //                dict.Add(xlWorkSheet.Cells[1, ColumnIndex].Value.ToString(), xlWorkSheet.Cells[RowIndex, ColumnIndex].Value.ToString());
            //            }
            //            else
            //            {
            //                dict.Add(xlWorkSheet.Cells[1, ColumnIndex].Value.ToString() + dictRowCnt, xlWorkSheet.Cells[RowIndex, ColumnIndex].Value.ToString());
            //            }
            //        }
            //        else
            //        {
            //            if (dictRowCnt == 0)
            //            {
            //                dict.Add(xlWorkSheet.Cells[1, ColumnIndex].Value.ToString(), null);
            //            }
            //            else
            //            {
            //                dict.Add(xlWorkSheet.Cells[1, ColumnIndex].Value.ToString() + dictRowCnt, null);
            //            }    
            //        }
            //    }
            //    dictRowCnt++;

            //}

            //xlWorkBook.Close(false, Type.Missing, Type.Missing);
            //xlWorkBooks.Close();
            //xlApp.Quit();

            ////-----Unmarshal COM objects to prevent memory leaks-------
            //Marshal.FinalReleaseComObject(xlApp);
            //Marshal.FinalReleaseComObject(xlWorkBook);
            //Marshal.FinalReleaseComObject(xlWorkBooks);
            //Marshal.FinalReleaseComObject(xlWorkSheet);
            //Marshal.FinalReleaseComObject(xlWorkSheets);

            //xlWorkBook = null;
            //xlApp = null;
        }
        #endregion
        
        public static string ReportResults(string thescenario, string name, out string stat, string primeKey, List<String> allScenarios)
        {
            try
            {
                String timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                SqlConnection sqlConnection;
                ArrayList report = new ArrayList();
                ArrayList checkTitle = new ArrayList();
                ArrayList checkPointStatus = new ArrayList();
                ReportBuilder reportWriter = new ReportBuilder();
                string filepath;
                int dataindex = 8;
                int index = 8;
                fails = 0;
                string status;
                bool reportOutcome = true;
                string outcome;
                DateTime timedate = DateTime.Now;
                int chkPointType = 1;
                int parentKey = 0;
                try { parentKey = Convert.ToInt32(primeKey); } catch { }
                status = "PASS";
                
            
                filepath = GetReportPath() + thescenario + "_" + timedate.ToString("MMddyyHHmmss") + ".xlsx";

                //Get the content of Array List for current test
                report = ReportBuilder.reportChecks;
                checkPointStatus = ReportBuilder.passStatus;
                checkTitle = ReportBuilder.reportName;

                sqlConnection = ConnectToDatabase(CommonTestSettings.ReportingDBHost, CommonTestSettings.ReportingDBName, CommonTestSettings.ReportingDBUser, CommonTestSettings.ReportingDBP);
                //ClearVariables();

                for (int i = 0; i < report.Count; i++)
                {
                    try
                    {
                        scenariosTested.Add(name);
                        reportWriter.WriteResults(parentKey, name, chkPointType, checkTitle[i].ToString(), report[i].ToString(), checkPointStatus[i].ToString(), sqlConnection);
                    }
                    catch(Exception e)
                    {
                        using (System.IO.StreamWriter file =
                            new System.IO.StreamWriter(GetResultsFailureLogPath(), true))
                        {

                            file.WriteLine(timestamp + " : " + thescenario);
                            file.WriteLine(e.ToString());
                            file.WriteLine("parentKey = " + parentKey);
                            file.WriteLine("name = " + name);
                            file.WriteLine("chkPointType = " + chkPointType);
                            file.WriteLine("checkTitle[" + i + "].toString() = " + checkTitle[i].ToString());

                            try
                            {
                                    file.WriteLine("report[" + i + "].toString() = " + report[i].ToString());
                                }
                                catch
                                {
                                    if(report.Count == 0)
                                    {
                                        file.WriteLine("report[" + i + "].ToString doesn't work. Report count = 0");
                                    }
                                    if(report[i] == null)
                                    {
                                    file.WriteLine("report[" + i + "] is null");
                                }
                                
                            }

                            file.WriteLine("checkPointStatus[" + i + "].toString() = " + checkPointStatus[i].ToString());
                            file.WriteLine("sqlConnection = " + sqlConnection.ToString());
                        }
                    }
                    dataindex++;
                }

                if (checkPointStatus.Contains("Fail"))
                {
                    status = "FAIL";
                    fails += 1;
                }

                //Keeps track of total checkpoints passed
                checksCount = report.Count;

                dataCount = myAL.Count;

                foreach (string datstring in myAL)
                {
                    chkPointType = 2;
                    string[] arr = datstring.Split(',');

                    try { reportWriter.WriteResults(parentKey, name, chkPointType, arr[0].ToString(), arr[2].ToString(), arr[1].ToString(), sqlConnection); } catch { }
                    
                    if (arr[1] == "Fail")
                    {
                        //If theres a failure stores for math formula later
                        fails = fails + 1;
                        status = "FAIL";
                    }
                    else
                    {
                    }
                    index++;
                }
                
                //Gets Passed
                passed = dataCount - fails;
                passed += checksCount;

                //Gets total checks for tests
                mathCounter = dataCount + checksCount;
                double passCounter = (passed / mathCounter) * 100;
                double failCounter = (fails / mathCounter) * 100;

                //Gets the percent 
                endingPercent = (passed / mathCounter) * 100;
  
                //Keeps a total for multiple tests
                counter = mathCounter + counter;

                outcome = ReportBuilder.status(reportOutcome);
                if (status == "PASS" & outcome == "PASS")
                {
                    outcome = "Passed";
                }
                else
                {
                    outcome = "Failed";
                }

                endPassed = passed + endPassed;

                //Passes in the results to build a final report
                ReportBuilder.FinalReportResults(name, endingPercent, counter, outcome, endPassed);

                ReportBuilder.reportChecks.Clear();
                ReportBuilder.reportName.Clear();
                ReportBuilder.passStatus.Clear();
                
                myAL.Clear();

                stat = status + "," + filepath;

                return (stat);
            }
            catch(Exception e)
            {
                //If the method fails to write report to spreadsheet or database, will log this to text file
                WriteFailureLog(e.ToString());
                stat = "";
                return stat;
            } 
        }
        public static void LogResults(String testname, String actualValue, String expectedValue, [CallerFilePath] string source = "", [CallerLineNumber] int sourceLineNumber = 0)
        {          
            if (actualValue.Equals(expectedValue))
            {
                myAL.Add(string.Format("{0},Pass,Actual value \"{1}\" matches expected value", testname, actualValue));
            }
            else
            {
                string failureSource = " (" + Path.GetFileName(source) + " " + sourceLineNumber + ")";
                myAL.Add(string.Format("{0} {1},Fail,Actual value \"{2}\" does not match expected value \"{3}\"", testname, failureSource, actualValue, expectedValue));
            }
        }
        public void LogResults(String testname, String actualValue, String expectedValue, String methodCaller)
        {
            string r;

            if (actualValue.Equals(expectedValue))
            {
                string report = "Actual value \"" + actualValue + "\" matches expected value".Replace(",", "");
                r = testname + ",Pass,"+report;
            }
            else
            {
                r = testname + ",Fail,Actual value \"" + actualValue + "\" does not match expected value \"" + expectedValue + "\"" 
                    + " DatabaseReader method: " + methodCaller;
            }
            myAL.Add(r);
        }
        public void LogCompareResults(String testname, String actualValue, String expectedValue)
        {
            string r;

            if (actualValue.Contains(expectedValue))
            {
                r = testname + ",Pass,Actual value: " + actualValue + " contains expected value";
            }
            else
            {
                r = testname + ",Fail,Actual value: " + actualValue + " does not contain expected value " + expectedValue;
            }
            myAL.Add(r);
        }
        public void LogMapResults(String testname, String actualValue, String expectedValue)
        {
            string r;
            string normalizedExpectedValue = Regex.Replace(expectedValue, @"\s", "");
            string normalizedActualValue = Regex.Replace(actualValue, @"\s", "");

            if (normalizedExpectedValue.Contains(normalizedActualValue))
            {
                if (actualValue.Length > 25)
                {
                    r = testname + ",Pass,Actual value: \"" + normalizedActualValue.Substring(0, 15).Replace("\r","") + "...\"(trimmed)" + " matches expected value";
                }
                else { r = testname + ",Pass,Actual value: \"" + normalizedActualValue + "\" matches expected value"; }
            }
            else
            {
                r = testname + ",Fail,Actual value: \"" + actualValue + "\" does not match expected value: \"" + expectedValue + "\"";
            }
            myAL.Add(r);
        }

        public void VerifyXMLContainsExpectedText(string categoryName, string xmlData, string expectedValue)
        {
            if (xmlData.Contains(expectedValue))
            {
                myAL.Add(categoryName + ",Pass,XML data contains expected value: \"" + expectedValue + "\"");
            }
            else
            {
                myAL.Add(categoryName + ",Fail,XML data: \"" + xmlData + "\" does not contain value: \"" + expectedValue + "\"");
            }
        }
        public static void ConnectDB(string dbHost, string dbName, string sqlcommand, out System.Data.DataTable dt, string username = "", string password = "")
        {
            SqlConnection con;
            string conString = "Data Source = " + dbHost + "; Initial Catalog = " + dbName + ";";
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                conString += " Integrated Security = true;";
                con = new SqlConnection(conString);
            }
            else
            {
                SecureString p = new SecureString();
                foreach (char c in password)
                {
                    p.AppendChar(c);
                }
                p.MakeReadOnly();

                con = new SqlConnection(conString);
                con.Credential = new SqlCredential(username, p);
            }

            con.Open();
            SqlCommand cmd = new SqlCommand(sqlcommand);
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
        }

        public void InsertDB(string dbHost, string dbName, string sqlcommand)
        {
            con = new SqlConnection(@"Data Source = " + dbHost + "; Initial Catalog = " + dbName + "; Integrated Security = True");
            con.Open();
            cmd = new SqlCommand(sqlcommand);
            cmd.Connection = con;
            da = new SqlDataAdapter(cmd);
            
        }
        public static SqlConnection ConnectToDatabase(string dbHost, string dbName, string username = "", string password = "")
        {
            SqlConnection con;
            string conString = "Data Source=" + dbHost + ";Initial Catalog = " + dbName + ";";
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                conString += "Integrated Security = true;";
                con = new SqlConnection(conString);
            }
            else
            {
                SecureString p = new SecureString();
                foreach (char c in password)
                {
                    p.AppendChar(c);
                }
                p.MakeReadOnly();

                con = new SqlConnection(conString);
                con.Credential = new SqlCredential(username, p);
            }

            con.Open();
            return con;
        }
        public void insertCheckPointData(string sqlcommand, SqlConnection connection, out System.Data.DataTable dt)
        {
            cmd = new SqlCommand(sqlcommand);
            cmd.Connection = connection;
            da = new SqlDataAdapter(cmd);
            dt = new System.Data.DataTable();
            try
            {
                da.Fill(dt);
            }
            catch(Exception e1)
            {
                String exception = e1.ToString();
            }   
        }

        /*Generate a .png of the browser displayed by the indicated driver. The file will be named
         using the scenario name, and the current date and time*/
        public static void TakeScreenShot(IWebDriver driver, String scenario,out string filePath)
        {
            if (scenario.Contains("/"))
            {
                scenario.Replace("/", "-");
            }
            Directory.CreateDirectory(CommonTestSettings.Temp);
            string fileLocation = CommonTestSettings.Temp;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                fileLocation = fileLocation.Replace("bin\\Debug", "");
            }
            else
            {
                fileLocation = fileLocation.Replace("bin\\Release", "");
            }
            
            DateTime timedate = DateTime.Now;
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            string fileName = $"{scenario}_{timedate.ToString("MMddyyHHmmss")}.png";
            filePath = Path.Combine(fileLocation, fileName).ToString();
            screenshot.SaveAsFile(filePath.ToString(), ImageFormat.Png);
            TestContext.AddTestAttachment(filePath);
        }

        /*Generate a .png of the browser displayed by the indicated driver. The file will be named
         using the scenario name, and the current date and time*/
        public static void TakeScreenShot(IWebDriver driver, String scenario)
        {
            try
            {
                string fileLocation = Directory.GetCurrentDirectory();
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    fileLocation = fileLocation.Replace("bin\\Debug", "");
                }
                else
                {
                    fileLocation = fileLocation.Replace("bin\\Release", "");
                }
                string filePath;
                DateTime timedate = DateTime.Now;
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                fileLocation = fileLocation.Replace("bin\\Debug", "");
                filePath = fileLocation + "Reports\\" + scenario + "_" + timedate.ToString("MMddyyHHmmss") + ".png";
                screenshot.SaveAsFile(filePath, ImageFormat.Png);
                TestContext.AddTestAttachment(filePath);
                try
                {
                    filePath = $"O:\\Automation\\{AppName}\\{scenario}_{timedate.ToString("MMddyyHHmm")}.png";
                    screenshot.SaveAsFile(filePath, ImageFormat.Png);
                }
                catch { }
            }
            catch { }
        }
        /// <summary>
        /// Uses Windows System.Drawing.Graphics functionality to save a screenshot of the entire primary monitor, where the host app is running.
        /// Saves the screenshot to the local and the shared automation directories.
        /// </summary>
        //private void SaveScreenShotHostApp(IWebDriver driver, string localScreenShotFileName, string sharedScreenShotFileName)
        //{
        //    try
        //    {
        //        System.Drawing.Rectangle primaryScreenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        //        using (var bitmap = new System.Drawing.Bitmap(primaryScreenBounds.Width, primaryScreenBounds.Height))
        //        using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
        //        {
        //            graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(primaryScreenBounds.Width, primaryScreenBounds.Height));
        //            bitmap.Save(localScreenShotFileName, System.Drawing.Imaging.ImageFormat.Png);
        //            bitmap.Save(sharedScreenShotFileName, System.Drawing.Imaging.ImageFormat.Png);
        //        }
        //    }
        //    catch (Exception e1)
        //    {
        //        new ReportBuilder().arrayBuilder($"Received error when taking/saving {Globals.AppName} screenshot: {e1.ToString()}", false, "Save Screenshot");
        //    }
        //}
        /// <summary>
        /// Uses Selenium's native screencapture functionality to save a screenshot of the browser to the local and the shared automation directories
        /// </summary>
        private void SaveScreenShotBrowser(IWebDriver driver, string localScreenShotFileName, string sharedScreenShotFileName)
        {
            try
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(localScreenShotFileName, ImageFormat.Png);
                screenshot.SaveAsFile(sharedScreenShotFileName, ImageFormat.Png);
            }
            catch (Exception e1)
            {
                ReportBuilder.ArrayBuilder($"Received error when taking/saving Browser screenshot: {e1.ToString()}", false, "Save Screenshot");
            }                  
        }
        public void WriteSkippedScenariosToDB(String primeKey, String name)
        {
            ReportBuilder reportWriter = new ReportBuilder();
            int parentKey = 0;
            try { parentKey = Convert.ToInt32(primeKey); } catch { }

            //Start of creating connection here 
            string dbFilePath = GetDBFilePath();
            string path = Path.Combine(GetDirectory(), "Utility", AppName + "Database.xlsx");

            //Get Database Login Info
            LoadVariables(path, "Insert");

            SqlConnection sqlConnection = ConnectToDatabase(dict["DBHost"], dict["DBName"],dict["UserName"],dict["Password"]);
            ClearVariables();

            reportWriter.WriteResults(parentKey, name, 1, "Skipped", "Skipped", "Never Reached", sqlConnection);
        }

        public static void WriteFailureLog(string exception)
        {
            String timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            try
            {
                using (StreamWriter file =
                    new StreamWriter(GetResultsFailureLogPath(), true))
                {
                    file.WriteLine(timestamp + " : " + exception);
                }
            }
            catch
            {
                using (StreamWriter file =
                    new StreamWriter(GetWriteResultsFailureLogPath(), true))
                {
                    file.WriteLine(timestamp + " : " + exception);
                }
            }
        }

        public static string GetResultsFailureLogPath()
        {           
            return Path.Combine(GetResultsLogPath(), AppName + CommonTestSettings.FailureResultsFile);
        }

        public static string GetWriteResultsFailureLogPath()
        {
            return Path.Combine(GetResultsLogPath(),AppName + CommonTestSettings.FailureResultsFile);
        }

        public static string GetResultsLogPath()
        {
            return Path.Combine(CommonTestSettings.ReportDrive, "Automation", $"{AppName}_Automation");
        }
        

        public static string GetDirectory()
        {
            string currDir = Directory.GetCurrentDirectory();
            if (Debugger.IsAttached)
            {
                return currDir.Replace("bin\\Debug", "");
            }
            else
            {
                return currDir.Replace("bin\\Release", "");
            }
        }
        
        /// <summary>
        /// Gets the filePath to the ..\Utility\ folder
        /// </summary>
        /// <returns></returns>
        public static string GetUtilityPath()
        {
            string AutoUtilityPath = @"Utility\";
            return Path.Combine(Directory.GetCurrentDirectory(), AutoUtilityPath);
        }
        /// <summary>
        /// Takes an excel file name (without the file extension) and returns a string to its path in the ..\Utility folder
        /// </summary>
        public static string GetUtilityFile(string fileName)
        {
            return string.Format("{0}{1}.xlsx", GetUtilityPath(), fileName);
        }
        public static string GetReportPath()
        {
            string reportFolderPath = @"..\..\Reports\";
            return Path.Combine(Directory.GetCurrentDirectory(), reportFolderPath);
        }
        /// <summary>
        /// Gets how long to run the scenario from data in ..\<Tests>.xlsx spreadsheet
        /// </summary>
        [Obsolete]
        public int GetNumberOfAttempts(string scenario, string spreadsheetName)
        {
            string projectFilePath = @"..\..\..\CentralSquare.CryWolf.Tests\" + spreadsheetName;
            string spreadsheetFilePath = Path.Combine(Directory.GetCurrentDirectory(), projectFilePath);

            ClearVariables();
            LoadVariables(spreadsheetFilePath, scenario);
            int numAttempts = int.Parse(dict["Number of Attempts"]);
            ClearVariables();

            return numAttempts;
        }
    }
}
