using OpenQA.Selenium;
using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;
using RestSharp;
using NUnit.Framework;

namespace Utils
{
    public class ReportBuilder
    {
        public static ArrayList reportChecks = new ArrayList();
        public static ArrayList reportName = new ArrayList();
        public static ArrayList passStatus = new ArrayList();
        public static ArrayList finalReport = new ArrayList();
        public static ArrayList finalResult = new ArrayList();
        public Library Lib = new Library(CommonTestSettings.AppName);
        public static double count;
        public static double pass;
        public static int percentCounter = 0;
        Screenshot ss;
        public static DateTime startScenario;
        public static DateTime endScenario;
        public static DateTime startSuite;
        public static DateTime endSuite;

        public static void ReportResponseContent(IRestResponse rest)
        {
            TestContext.WriteLine($"{rest.Content}");
        }
        public static void ArrayBuilder(string newItem, bool reportStatus, string categoryName, [CallerFilePath] string source = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (reportStatus)
            {
                passStatus.Add("Pass");
            }
            else
            {
                passStatus.Add("Fail");
                categoryName += " (" + Path.GetFileName(source) + " " + sourceLineNumber + ")";
            }

            reportChecks.Add(newItem);
            reportName.Add(categoryName);

            TestContext.WriteLine($"{categoryName} // {newItem}");
        }

        public static void getStartTime()
        {
            startScenario = DateTime.Now; 
        }
        public static void getEndTime()
        {
            endScenario = DateTime.Now;
        }
        public void getEndTestTime()
        {
            endSuite = DateTime.Now;
        }
        public void getStartTestTime()
        {
            startSuite = DateTime.Now;
        }
        public ArrayList report(ArrayList blankList)
        {
            blankList = reportChecks;
            return blankList;
        }
        public static string status(bool passFail)
        {
            string testOutcome;
            if (passFail == true)
            {
                testOutcome = "PASS";
            }
            else
            {
                testOutcome = "FAIL";
            }
            return testOutcome;
        }
        public static void FinalReportResults(string testName, double testPercent, double counter, string outcome, double passed)
        {
            string testResult = testName;
            string passFail;
            finalReport.Add(testResult);
            if (testPercent==100)
            {
                passFail = "PASS";
                percentCounter += 1;

            }
            else
            {
                passFail = "FAIL";
            }
            finalResult.Add(passFail);
            count = counter;
            pass = passed;
        }
        public void takeScreenshot(IWebDriver driver)
        {
            try { ss = ((ITakesScreenshot)driver).GetScreenshot(); } catch { };
            
        }
        //public void writeResultsTestLog(int parentKey, string scenar, int testType, string chkPoint, string checkpointDesc, string result)
        //{
        //    string sqlstr;
        //    DataTable datab;
        //    //Load Dictionary
        //    string cutPath = Directory.GetCurrentDirectory() + "Utility\\MCTDatabase.xlsx";
        //    //string path = cutPath.Replace("bin\\Debug", "");
        //    //Get Database Login Info
        //    Lib.LoadVariables(path, "Insert");

        //    sqlstr = "INSERT INTO dbo.test_log (parent_key, scenario, test_type, chkpt, chkpt_desc,result) VALUES ("+parentKey+ ",'"+scenar+ "',"+
        //        testType + " , '"+chkPoint+ "' , '"+checkpointDesc+ "' , '"+result+"')";

        //    Lib.ConnectDB(Lib.dict["DBHost"], Lib.dict["DBName"], sqlstr, out datab);
        //    Lib.ClearVariables();
        //}
        public void WriteResults(int parentKey, string scenar, int testType, string chkPoint, string checkpointDesc, string result, SqlConnection connection)
        {
            string sqlstr;
            checkpointDesc = checkpointDesc.Replace("'", "");
            sqlstr = "INSERT INTO dbo.test_log (parent_key, scenario, test_type, chkpt, chkpt_desc,result) VALUES (" + parentKey + ",'" + scenar + "'," +
                testType + " , '" + chkPoint + "' , '" + checkpointDesc + "' , '" + result + "')";

            try { Lib.insertCheckPointData(sqlstr, connection, out DataTable datab); } catch { }
        }
    }
}
