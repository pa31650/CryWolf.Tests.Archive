using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using static Utils.CommonTestSettings;

namespace Utils
{
    public class CryWolfUtil
    {
        protected static WindowsDriver<WindowsElement> session;
        private static WindowsDriver<WindowsElement> childSession = null;
        private static WindowsDriver<WindowsElement> desktopSession = null;

        private bool status = false;
        private static Process _driver;

        #region Process & Session stuff

        public static WindowsDriver<WindowsElement> GetDesktopSession()
        {
            return Utility.CreateNewSession(CommonTestSettings.DesktopAppId,CommonTestSettings.RunLocation);
        }
        
        public static WindowsDriver<WindowsElement> SwitchToProcessByAccId(string process, int implicitWait=10)
        {
            Thread.Sleep(2500);
            //Create new session based on desktop to get CryWolfMaint window handle
            desktopSession = GetDesktopSession();

            //Attach second session to CryWolf Maintenance window by finding WindowHandle (using deskTopSession) and passing to second session
            childSession = Utility.CreateNewSession(Utility.GetWindowHandleByAccId(desktopSession, process));
            
            Utility.SetImplicitWait(childSession, implicitWait);
            //childSession.Manage().Timeouts().ImplicitWait.Add(TimeSpan.FromSeconds(implicitWait));//.ImplicitlyWait(TimeSpan.FromSeconds(implicitWait));

            return childSession;
        }
        public static WindowsDriver<WindowsElement> SwitchToProcessByName(string process, int implicitWait = 10)
        {
            Thread.Sleep(2500);
            //Create new session based on desktop to get CryWolfMaint window handle
            desktopSession = GetDesktopSession();

            //Attach second session to CryWolf Maintenance window by finding WindowHandle (using deskTopSession) and passing to second session
            
            childSession = Utility.CreateNewSession(Utility.GetWindowHandleByName(desktopSession, process));
            Utility.SetImplicitWait(childSession, implicitWait);
            //childSession.Manage().Timeouts().ImplicitWait.Add(TimeSpan.FromSeconds(implicitWait));//.ImplicitlyWait(TimeSpan.FromSeconds(implicitWait));

            return childSession;
        }
        public static WindowsDriver<WindowsElement> AttachToCryWolfMaint()
        {
            return SwitchToProcessByName(CommonTestSettings.CryWolfMaintName);
        }
        public static WindowsDriver<WindowsElement> AttachToAutoProcess()
        {
            return SwitchToProcessByAccId(CommonTestSettings.AutoProcessAccessibiltyID);
        }
        public static WindowsDriver<WindowsElement> AttachToCryWolf()
        {
            return SwitchToProcessByAccId(CommonTestSettings.CryWolfAccessibilityID);
        }
        public static void CloseCryWolf()
        {
            string appName = "CryWolf";
            if (CommonTestSettings.RunLocation == "local")
            {
                ProcessHandler.CloseProcessesByName(appName);
            }
            else
            {        
                //session = GetDesktopSession();
                ProcessHandler.CloseRemoteProcessesByName(appName, "crywolf-user");
                //session.Dispose();    
            }
        }
        public static void KillCryWolfMaint()
        {
            string appName = "CryWolfMaint";
            if (CommonTestSettings.RunLocation == "local")
            {
                ProcessHandler.CloseProcessesByName(appName);
            }
            else
            {
                try
                {
                    //session = GetDesktopSession();
                    ProcessHandler.CloseRemoteProcessesByName(appName, "crywolf-user");
                    //session.Dispose();
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine($"{appName} app not found");
                }
            }
        }
        public static void KillChrome()
        {
            if (CommonTestSettings.RunLocation == "grid")
            {
                ProcessHandler.CloseRemoteProcessesByName("chrome", "crywolf-user");
            }
        }
        public static void CloseAutoProcess()
        {
            string appName = "AutoProcess";
            if (CommonTestSettings.RunLocation == "local")
            {
                ProcessHandler.CloseProcessesByName("AutoProcess");
            }
            else
            {
                try
                {
                    ProcessHandler.CloseRemoteProcessesByName(appName, "crywolf-user");
                }
                catch (NoSuchElementException)
                {

                    Console.WriteLine($"{appName} app not found");
                }
            }
        }
        public static void KillAllCryWolfProcesses()
        {
            CloseCryWolf();
            KillCryWolfMaint();
            CloseAutoProcess();
        }
        #endregion

        #region Database stuff
        public static void DeleteOnlinePaymentNotes()
        {
            DeleteNotesByLetterType("Online Payment");
        }
        public static void DeleteNotesByLetterType(string letterType)
        {
            SQLHandler.DeleteDatabaseValue(
                $"DELETE FROM ALARM_NOTES WHERE LetterType = '{letterType}'",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);
        }
        public static string GetIncDateByAlarmNumberLetter(string alarmNumber, string alarmLetter)
        {
            return SQLHandler.GetDatabaseValue(
                $"SELECT incidentDate FROM ALARM_ACTIONS WHERE alarmNo = '{alarmNumber}' AND letterType LIKE '%{alarmLetter}%'",
                "incidentDate",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);
        }
        public static string GetNameOnAccount(string AlarmNo)
        {
            List<string> columnNames = new List<string> { "lastName", "firstName" }; //you did this because GetDatabaseValues takes a List
            string sqlSatement = $"Select {columnNames[0]},{columnNames[1]} from ALARM_LICENSES where AlarmNo = {AlarmNo};";
            List<string> vs = SQLHandler.GetDatabaseValues(sqlSatement, columnNames, CommonTestSettings.dbHost, CommonTestSettings.dbName);

            if (vs[1] == "")
            {
                return vs[0];
            }
            return String.Join(", ", vs);
        }
        public static DateTime GetIssueDate(string AlarmNo)
        {
            string sqlStatement = $"Select dateIssued from ALARM_LICENSES where AlarmNo = {AlarmNo};";
            return DateTime.Parse(SQLHandler.GetDatabaseValue(sqlStatement, "dateIssued", CommonTestSettings.dbHost, CommonTestSettings.dbName));
        }
        public static DateTime GetExpDate(string AlarmNo)
        {
            string sqlStatement = $"Select dateExpires from ALARM_LICENSES where AlarmNo = {AlarmNo};";
            return DateTime.Parse(SQLHandler.GetDatabaseValue(sqlStatement, "dateExpires", CommonTestSettings.dbHost, CommonTestSettings.dbName));
        }
        public static string GetStreetAddress(string AlarmNo)
        {
            List<string> columnNames = new List<string> { "strNum", "Street" };
            string sqlStatement = $"Select {columnNames[0]},{columnNames[1]} from ALARM_LICENSES where AlarmNo = {AlarmNo};";
            List<string> vs = SQLHandler.GetDatabaseValues(sqlStatement, columnNames, CommonTestSettings.dbHost, CommonTestSettings.dbName);

            return String.Join(",", vs);
        }

        public static string GetStreetNum(string AlarmNo)
        {
            string sqlStatement = $"SELECT strNum FROM ALARM_LICENSES WHERE AlarmNo = {AlarmNo};";
            return SQLHandler.GetDatabaseValue(sqlStatement, "strNum", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetStreetName(string AlarmNo)
        {
            string sqlStatement = $"SELECT Street FROM ALARM_LICENSES WHERE AlarmNo = {AlarmNo};";
            return SQLHandler.GetDatabaseValue(sqlStatement, "Street", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetZipCode(string AlarmNo)
        {
            return SQLHandler.GetDatabaseValue($"Select zip from ALARM_LICENSES where AlarmNo = {AlarmNo};","zip",CommonTestSettings.dbHost,CommonTestSettings.dbName);
        }

        public static string GetCity(string AlarmNo)
        {
            return SQLHandler.GetDatabaseValue($"Select city from ALARM_LICENSES where AlarmNo = {AlarmNo};", "city", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetAlarmByInvoice(string invoiceNo,string dbName = null)
        {
            if (dbName == null)
            {
                dbName = CommonTestSettings.dbName;
            }

            return SQLHandler.GetDatabaseValue($"SELECT alarmNo FROM ALARM_ACTIONS WHERE InvoiceNo = '{invoiceNo}';", "alarmNo", CommonTestSettings.dbHost, dbName);
        }

        public static string GetACNo(string sqlStatement,string dbName = null)
        {
            if (dbName == null)
            {
                dbName = CommonTestSettings.dbName;
            }

            return SQLHandler.GetDatabaseValue(sqlStatement, "AlarmNo", CommonTestSettings.dbHost, dbName);
        }

        public static string GetAlarmNo(string sqlStatement, string dbName = null)
        {
            if (dbName == null)
            {
                dbName = CommonTestSettings.dbName;
            }

            return SQLHandler.GetDatabaseValue(sqlStatement, "AlarmNo", CommonTestSettings.dbHost, dbName);
        }

        public static string GetAlarmNoByAC(string alarmCo)
        {
            return SQLHandler.GetDatabaseValue($"SELECT * FROM ALARM_LICENSES WHERE currentStatus = 'Active' AND monitoredBy = '{alarmCo}'", "AlarmNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetInvoiceByAlarmNo(string AlarmNo)
        {
            Thread.Sleep(500);
            string invoiceNumber;

            invoiceNumber = SQLHandler.GetDatabaseValue(
                $"select Top 1 InvoiceNo from ALARM_ACTIONS where AlarmNo = '{AlarmNo}';", "InvoiceNo",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);

            return invoiceNumber;
        }

        public static string GetServicedByAlarmNo(string AlarmNo)
        {
            return SQLHandler.GetDatabaseValue(
                $"SELECT ServicedBy FROM ALARM_LICENSES WHERE AlarmNo = '{AlarmNo}'", "ServicedBy",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);
        }

        public static string GetInstalledByAlarmNo(string AlarmNo)
        {
            return SQLHandler.GetDatabaseValue(
                $"SELECT InstalledBy FROM ALARM_LICENSES WHERE AlarmNo = '{AlarmNo}'", "InstalledBy",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);
        }

        public static string GetCartByInvoice(string invoice)
        {
            string cart;

            cart = SQLHandler.GetDatabaseValue(
                $"SELECT AutoCount1 from ALARM_ARAC_EXTENDED WHERE mValue LIKE '{invoice}'",
                "AutoCount1",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);

            return cart.ToString();
        }

        public static string GetInvoiceAmount(string InvoiceNo)
        {
            return SQLHandler.GetDatabaseValue($"SELECT Charge from ALARM_ACTIONS where InvoiceNo = '{InvoiceNo}'", "Charge", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetInvoiceNo(string sqlStatement)
        {
            return SQLHandler.GetDatabaseValue(sqlStatement, "InvoiceNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetInvoiceAmount(string InvoiceNo, string dbName)
        {
            return SQLHandler.GetDatabaseValue($"SELECT Charge from ALARM_ACTIONS where InvoiceNo = '{InvoiceNo}'", "Charge", CommonTestSettings.dbHost, dbName);
        }

        public static string GetInvoicePayments(string InvoiceNo, string dbName)
        {
            return SQLHandler.GetDatabaseValue($"SELECT Payment from ALARM_ACTIONS where InvoiceNo = '{InvoiceNo}'", "Payment", CommonTestSettings.dbHost, dbName);
        }

        public static string GetGeoCodeAutoCount()
        {
            return SQLHandler.GetDatabaseValue(SQLStrings.RandomGeoCodeRow, "AutoCount1", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        [Obsolete]
        public static void GetEncryptSiteData(out string URL,out string password)
        {
            URL = $"http://crywolf-preprod.ps.lcl:{CommonTestSettings.port}/NC-HighPoint-Encrypt/";
            password = CommonTestSettings.pw;
        }

        public static string GetCitizenPassword(string AlarmNo)
        {
            string sqlStatement = $"Select Password from ALARM_LICENSES where AlarmNo = {AlarmNo}";
            return SQLHandler.GetDatabaseValue(sqlStatement, "Password", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetDefaultState()
        {
            return SQLHandler.GetDatabaseValue("select DefaultState from LU2_DEFAULT ", "DefaultState", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        public static string GetDefaultCity()
        {
            return SQLHandler.GetDatabaseValue("select returnCity from LU2_DEFAULT;", "returnCity", CommonTestSettings.dbHost, CommonTestSettings.dbName);
        }

        

        #endregion

        #region Common CryWolf methods
        [Obsolete]
        public static void LaunchCryWolfMaint()
        {
            switch (CommonTestSettings.RunLocation)
            {
                case "local":
                    Process cryWolfMaint = Process.Start(CommonTestSettings.CryWolfMaintId, "\"CryWolf.exe\" \"admin\" \"<Default>\"");
                    break;
                case "grid":
                    ScriptBlock scriptBlock = ScriptBlock.Create($"Start-Process -FilePath C:\builds\\crywolf\v2\\Enterprise\\18.5\release\\CryWolfMaint.exe -Arguments \"CryWolf.exe\" \"admin\" \"<Default>\" -Force");
                    PowerShell shell = PowerShell.Create().AddCommand("invoke-command").AddParameter("ComputerName", "CryWolf-User").AddParameter("ScriptBlock", scriptBlock);
                    shell.Invoke();
                    break;
                default:
                    break;
            }
        }
        public static WindowsDriver<WindowsElement> TestInitialize(string arguments,string RunLocation)
        {
            Console.WriteLine("Entering Test Initialize");
            // Create a new session
            try
            {
                session = Utility.CreateNewSession(CommonTestSettings.CryWolfPath, RunLocation, arguments);
            }
            catch (WebDriverException wde)
            {
                if (CommonTestSettings.RunLocation == "local")
                {
                    _driver = null;

                    _driver = Utility.StartWinAppDriver();

                    session = Utility.CreateNewSession(CommonTestSettings.CryWolfPath, RunLocation, arguments);
                }
                else
                {
                    throw wde;
                }
            }

            Assert.IsNotNull(session);
            Console.WriteLine("Leaving TestInitialize");

            return session;
        }
        public static string GetUniqueFileNumber()
        {
            string fileNo = $"{DateTime.Now.Year.ToString()}" +
                $"{DateTime.Now.Month.ToString()}" +
                $"{DateTime.Now.Day.ToString()}" +
                $"{DateTime.Now.Hour.ToString()}" +
                $"{DateTime.Now.Minute.ToString()}" +
                $"{DateTime.Now.Second.ToString()}";

            return fileNo;
        }
        public static string GetUniqueCaseNumber()
        {
            string caseNo = $"{DateTime.Now.Year.ToString()}" +
                $"{DateTime.Now.Month.ToString()}" +
                $"{DateTime.Now.Day.ToString()}" +
                $"{DateTime.Now.Hour.ToString()}" +
                $"{DateTime.Now.Minute.ToString()}" +
                $"{DateTime.Now.Second.ToString()}";

            try
            {
                string result = SQLHandler.GetDatabaseValue($"select CaseNo from ALARM_ACTIONS where CaseNo = '{caseNo}';", "CaseNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            }
            catch (Exception)
            {
                //no results/unique
            }
            finally
            {
                caseNo = $"{DateTime.Now.Year.ToString()}" +
                    $"{DateTime.Now.Month.ToString()}" +
                    $"{DateTime.Now.Day.ToString()}" +
                    $"{DateTime.Now.Hour.ToString()}" +
                    $"{DateTime.Now.Minute.ToString()}" +
                    $"{DateTime.Now.Second.ToString()}";
            }
            
            return caseNo;
        }
        public static string GetDateStringNoFormat(DateTime dateTime)
        {
            return dateTime.ToString("MM/dd/yyyy").Replace("/","");
            //return dateTime.ToLongDateString().Replace("/", "");
        }
        public static DateTime ApplyRenewalRuleIssueDate(DateTime dateToConsider, string renewalType)
        {
            switch (renewalType.Replace(" ",string.Empty).ToLower())
            {
                //case "FirstDayOfMonth":
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return new DateTime(permit.Issued.Value.Year, permit.Issued.Value.Month, 1);
                //        return new DateTime(permit.Issued.Value.Year, permit.Issued.Value.Month, 1).AddMonths(monthsInRenewal);
                //    }

                //    return dateToConsider.AddDays((dateToConsider.Day * -1) + 1);

                //case RenewalType.FromCurrentDate:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value;
                //        return permit.Issued.Value.AddMonths(monthsInRenewal);
                //    }

                //    return getIssueDate ? currentTimestamp : currentTimestamp.AddMonths(monthsInRenewal);

                //case RenewalType.LastDayOfMonth:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1);

                //        //The rule about issuing on current and renewing on the next month, with the expiration happening on the
                //        //  last day of the original issuing month (unless it's the first of the month).  This is calculated
                //        //  slightly differently.
                //        if (issueRule == RenewalType.TodayForNewFirstOfMonthAfterExpiresForExisting)
                //        {
                //            if (permit.Issued.Value.Day == 1)
                //                return permit.Issued.Value.AddMonths(monthsInRenewal).AddDays(-1);
                //            return permit.Issued.Value.AddMonths(permit.Issued.Value.Day != 0 ? 1 : 0).AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //        }

                //        return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //    }

                //    if (dateToConsider.Value.Day == 1)
                //        dateToConsider = dateToConsider.Value.AddDays(-1);

                //    dateToConsider = dateToConsider.Value.AddMonths(1);
                //    dateToConsider = dateToConsider.Value.AddDays(dateToConsider.Value.Day * -1);
                //    return dateToConsider;

                //case RenewalType.LastDayOfNextMonth:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1);
                //        return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //    }

                //    dateToConsider = dateToConsider.Value.AddMonths(2);
                //    dateToConsider = dateToConsider.Value.AddDays(dateToConsider.Value.Day * -1);
                //    return dateToConsider;

                //case RenewalType.LastDayOfPreviousMonth:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1);
                //        return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //    }

                //    return dateToConsider.Value.AddDays(dateToConsider.Value.Day * -1);

                //case RenewalType.SpecificDay:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        var thisDate = new DateTime(dateToConsider.Value.Year,
                //            fixedMonth.Value == 0 ? currentTimestamp.Month : fixedMonth.Value,
                //            fixedDay.Value == 0 ? currentTimestamp.Day : fixedDay.Value);

                //        if (getIssueDate && thisDate > currentTimestamp)
                //            return thisDate.AddMonths(monthsInRenewal * -1);

                //        if (!getIssueDate && thisDate < currentTimestamp)
                //            return thisDate.AddMonths(monthsInRenewal);
                //    }

                //    return new DateTime(dateToConsider.Value.Year,
                //        fixedMonth.Value == 0 ? currentTimestamp.Month : fixedMonth.Value,
                //        fixedDay.Value == 0 ? currentTimestamp.Day : fixedDay.Value);

                case "firstissuelastexpire":
                    
                    if (dateToConsider.Day == 1)
                        return dateToConsider;

                    //if (dateToConsider.Value.AddDays(1).Day != 1)
                    dateToConsider = dateToConsider.AddMonths(1);

                    dateToConsider = dateToConsider.AddDays((dateToConsider.Day * -1) + 1);

                    return dateToConsider;

                default:
                    throw new NotImplementedException($"The renewal type {renewalType.ToString()} has not been implemented.");
            }
        }
        public static DateTime ApplyRenewalRuleExpirationDate(DateTime dateToConsider, string renewalType)
        {
            switch (renewalType.Replace(" ","").ToLower())
            {
                //case "FirstDayOfMonth":
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return new DateTime(permit.Issued.Value.Year, permit.Issued.Value.Month, 1);
                //        return new DateTime(permit.Issued.Value.Year, permit.Issued.Value.Month, 1).AddMonths(monthsInRenewal);
                //    }

                //    return dateToConsider.AddDays((dateToConsider.Day * -1) + 1);

                //case RenewalType.FromCurrentDate:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value;
                //        return permit.Issued.Value.AddMonths(monthsInRenewal);
                //    }

                //    return getIssueDate ? currentTimestamp : currentTimestamp.AddMonths(monthsInRenewal);

                //case RenewalType.LastDayOfMonth:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1);

                //        //The rule about issuing on current and renewing on the next month, with the expiration happening on the
                //        //  last day of the original issuing month (unless it's the first of the month).  This is calculated
                //        //  slightly differently.
                //        if (issueRule == RenewalType.TodayForNewFirstOfMonthAfterExpiresForExisting)
                //        {
                //            if (permit.Issued.Value.Day == 1)
                //                return permit.Issued.Value.AddMonths(monthsInRenewal).AddDays(-1);
                //            return permit.Issued.Value.AddMonths(permit.Issued.Value.Day != 0 ? 1 : 0).AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //        }

                //        return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //    }

                //    if (dateToConsider.Value.Day == 1)
                //        dateToConsider = dateToConsider.Value.AddDays(-1);

                //    dateToConsider = dateToConsider.Value.AddMonths(1);
                //    dateToConsider = dateToConsider.Value.AddDays(dateToConsider.Value.Day * -1);
                //    return dateToConsider;

                //case RenewalType.LastDayOfNextMonth:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1);
                //        return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //    }

                //    dateToConsider = dateToConsider.Value.AddMonths(2);
                //    dateToConsider = dateToConsider.Value.AddDays(dateToConsider.Value.Day * -1);
                //    return dateToConsider;

                //case RenewalType.LastDayOfPreviousMonth:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        if (getIssueDate)
                //            return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1);
                //        return permit.Issued.Value.AddDays(permit.Issued.Value.Day * -1).AddMonths(monthsInRenewal);
                //    }

                //    return dateToConsider.Value.AddDays(dateToConsider.Value.Day * -1);

                //case RenewalType.SpecificDay:
                //    //If the citizen hasn't registered, we're going to return a new permit period.
                //    if (permit != null && !permit.Expires.HasValue && permit.Issued.HasValue)
                //    {
                //        var thisDate = new DateTime(dateToConsider.Value.Year,
                //            fixedMonth.Value == 0 ? currentTimestamp.Month : fixedMonth.Value,
                //            fixedDay.Value == 0 ? currentTimestamp.Day : fixedDay.Value);

                //        if (getIssueDate && thisDate > currentTimestamp)
                //            return thisDate.AddMonths(monthsInRenewal * -1);

                //        if (!getIssueDate && thisDate < currentTimestamp)
                //            return thisDate.AddMonths(monthsInRenewal);
                //    }

                //    return new DateTime(dateToConsider.Value.Year,
                //        fixedMonth.Value == 0 ? currentTimestamp.Month : fixedMonth.Value,
                //        fixedDay.Value == 0 ? currentTimestamp.Day : fixedDay.Value);

                case "firstissuelastexpire":
                    //This is supposed to calculate only the issue date.
                    //if (getIssueDate == false)
                    //    throw new InvalidOperationException($"The rule {renewalType} cannot be applied to expiration dates.");

                    //if (permit == null || !permitDateToConsider.HasValue)
                    //    return currentTimestamp;

                    //The permit will exist below here.
                    //dateToConsider = permit.Expires;

                    //If we don't have an expiration date, that means the citizen was never registered in the first place.  The issue date will
                    //  contain the first incident date.  We'll want to preserve that, then set the expiration date to whatever it would have been
                    //  for a new registration.
                    //if (!dateToConsider.HasValue)
                    //{
                    //    //We need to preserve the initial issue date
                    //    if (getIssueDate)
                    //        return permit.Issued.Value;

                    //    dateToConsider = permit.Issued.HasValue ? permit.Issued.Value : currentTimestamp;
                    //}

                    //if (dateToConsider.Day == 1)
                      //  return dateToConsider;

                    dateToConsider = dateToConsider.AddYears(1);

                    dateToConsider = dateToConsider.AddDays(-1);

                    if (DateTime.IsLeapYear(dateToConsider.Year))
                    {
                        if (dateToConsider.Month == 2)
                        {
                            dateToConsider.AddDays(1);
                        }
                    }

                    return dateToConsider;

                default:
                    throw new NotImplementedException($"The renewal type {renewalType.ToString()} has not been implemented.");
            }
        }

        public static string CreateFaImportMansfieldDispatch(List<string> alarmNumbers, out string path, string format, DateTime date, string dispatchCode)
        {
            return CreateFaImportFile(alarmNumbers, out path, format, date.ToShortDateString(), "", dispatchCode);
        }

        public static string CreateFaImportFileWithSpace(List<string> alarmNumbers, out string path, string format = "HP_OSSI", string date = "", string newFile = "File with space")
        {
            return CreateFaImportFile(alarmNumbers, out path, format, date, newFile);
        }

        public static string CreateFaImportFile(List<string> alarmNumbers, out string path, string format = "HP_OSSI", string date = "", string newFile = "", string dispatchCode = "")
        {
            string sqlFieldName = $"select fieldName from LU2_FILE_FORMAT_DETAIL where FormatName = '{format}' and fieldPosition > 0 order by fieldPosition;";
            string sqlFieldPosition = $"select fieldPosition from LU2_FILE_FORMAT_DETAIL where FormatName = '{format}' and fieldPosition > 0 order by fieldPosition;";
            string sqlDelimiter = $"select FileDelimiter from LU2_FILE_FORMAT_MAIN where FormatName = '{format}';";
            string sqlSkipHeaders = $"select skipFirstRecord from LU2_FILE_FORMAT_MAIN where FormatName = '{format}';";

            if (newFile == "")
            {
                newFile = CryWolfUtil.GetUniqueFileNumber();
            }
            else
            {
                newFile += CryWolfUtil.GetUniqueFileNumber();
            }

            if (date == "")
            {
                date = DateTime.Today.ToString("MM/dd/yyyy");
            }
            else
            {
                date = DateTime.Parse(date).ToString("MM/dd/yyyy");
            }

            //set path
            path = $@"{CommonTestSettings.NetworkShare}\share\{newFile}.txt";
            List<int> fieldPosition = SQLHandler.GetSmallIntResults(sqlFieldPosition, "fieldPosition", CommonTestSettings.dbHost, CommonTestSettings.dbName);

            //get format from db
            List<string> fieldName = SQLHandler.GetStringResults(sqlFieldName, CommonTestSettings.dbHost, CommonTestSettings.dbName);
            List<string> vs = new List<string>();

            string delimiter = SQLHandler.GetDatabaseValue(sqlDelimiter, "FileDelimiter", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            int skipHeaders = SQLHandler.GetDatabaseInt(sqlSkipHeaders, "skipFirstRecord", CommonTestSettings.dbHost, CommonTestSettings.dbName);

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    var headerDic = fieldPosition.Zip(fieldName, (k, v) => new { k, v })
                            .ToDictionary(x => x.k, x => x.v);
                    int linePos = 1; //to tie position in line to field position obtained from db

                    if (skipHeaders == 1)
                    {
                        for (int i = 0; i < headerDic.Last().Key; i++)
                        {
                            if (headerDic.TryGetValue(linePos, out string value))
                            {
                                sw.Write(value);

                                if (value != headerDic.Last().Value)
                                {
                                    sw.Write(delimiter); //add ',' to end of value unless its the end of the dictionary
                                }
                            }
                            else
                            {
                                sw.Write(delimiter); //write a 'blank' entry in the file
                            }

                            linePos++; //move the file position up
                        }

                        sw.WriteLine();
                    }

                    foreach (var alarmNumber in alarmNumbers)
                    {

                        linePos = 1;

                        #region Field Population
                        vs = new List<string>(fieldName);

                        //CaseNo
                        vs[vs.FindIndex(ind => ind.Equals("CaseNo"))] = CryWolfUtil.GetUniqueCaseNumber();
                        //AlarmCo -- Dallas doesn't use
                        //vs[vs.FindIndex(ind => ind.Equals("AlarmCo"))] = ProcessAlarmValues.AlarmCo;
                        //StrNum&Street
                        string streetAddress = GetStreetAddress(alarmNumber).Replace(",", " ");
                        vs[vs.FindIndex(ind => ind.Equals("StrNum&Street"))] = streetAddress;
                        TestContext.WriteLine($"Street Address found for Alarm Number [{alarmNumber}]: [{streetAddress}]");
                        //Apt
                        vs[vs.FindIndex(ind => ind.Equals("Apt"))] = "";
                        //City
                        vs[vs.FindIndex(ind => ind.Equals("City"))] = "";
                        //IncidentDate
                        vs[vs.FindIndex(ind => ind.Equals("IncidentDate"))] = date;
                        //IncidentTime
                        vs[vs.FindIndex(ind => ind.Equals("IncidentTime"))] = ProcessAlarmValues.IncidentTime;
                        //TimeDispatched
                        vs[vs.FindIndex(ind => ind.Equals("TimeDispatched"))] = ProcessAlarmValues.TimeDispatched;
                        //TimeOnScene
                        vs[vs.FindIndex(ind => ind.Equals("TimeOnScene"))] = ProcessAlarmValues.TimeOnScene;
                        //TimeCleared
                        vs[vs.FindIndex(ind => ind.Equals("TimeCleared"))] = ProcessAlarmValues.TimeCleared;
                        //DispatcherInfo --Dallas doesn't use
                        //vs[vs.FindIndex(ind => ind.Equals("DispatcherInfo"))] = ProcessAlarmValues.DispatcherInfo;
                        //CallTakerInfo
                        vs[vs.FindIndex(ind => ind.Equals("CallTakerInfo"))] = ProcessAlarmValues.CallTakerInfo;
                        //Extra1 --Dallas doesn't use
                        //vs[vs.FindIndex(ind => ind.Equals("Extra1"))] = "";
                        //UnitsAssigned 
                        //vs[vs.FindIndex(ind => ind.Equals("UnitsAssigned"))] = ProcessAlarmValues.UnitsAssigned;
                        //DispatchType
                        vs[vs.FindIndex(ind => ind.Equals("DispatchType"))] = dispatchCode == "" ? GetValidDispatch() : dispatchCode;
                        //vs[vs.FindIndex(ind => ind.Equals("DispatchType"))] = "12R - RESIDENTIAL ALARM";
                        //Cleared
                        vs[vs.FindIndex(ind => ind.Equals("Cleared"))] = "AF";
                        //Beat --Dallas doesn't use
                        //vs[vs.FindIndex(ind => ind.Equals("Beat"))] = "BEAT 5";
                        //CADName
                        vs[vs.FindIndex(ind => ind.Equals("CADName"))] = "";
                        //OfficerComments
                        vs[vs.FindIndex(ind => ind.Equals("OfficerComments"))] = "FALSE NO ONE THERE";
                        #endregion

                        //for each row in the array
                        var dic = fieldPosition.Zip(vs, (k, v) => new { k, v })
                            .ToDictionary(x => x.k, x => x.v);

                        for (int i = 0; i < dic.Last().Key; i++)
                        {
                            if (dic.TryGetValue(linePos, out string value))
                            {
                                sw.Write(value);

                                if (value != dic.Last().Value)
                                {
                                    sw.Write(delimiter); //add ',' to end of value unless its the end of the dictionary
                                }
                            }
                            else
                            {
                                sw.Write(delimiter); //write a 'blank' entry in the file
                            }

                            linePos++; //move the file position up
                        }

                        sw.WriteLine();
                        vs = null;
                    }

                    sw.Close();
                }
            }

            return path;
        }
        public static DateTime CalculateNewIssueDate(string issueExpSetting, out int issueMonth, out int issueDay)
        {
            issueMonth = 0;
            issueDay = 0;

            DateTime issueDateToCheck = new DateTime();

            switch (issueExpSetting.Replace(" ","").ToLower())
            {
                case "usetodaysdate":
                    issueDateToCheck = DateTime.Today;
                    break;
                case "usetodaysandlastdayofmonth":
                    issueDateToCheck = DateTime.Parse($"{DateTime.Today.Month}/1/{DateTime.Today.Year}");
                    break;
                case "use1stofmonth":
                    issueDateToCheck = DateTime.Parse($"{DateTime.Today.Month}/1/{DateTime.Today.Year}");
                    break;
                case "usespecificdate":
                    issueDateToCheck = DateTime.Today;

                    //use specific issue date
                    //issueMonth = int.Parse(testContext.DataRow["issue_month"].ToString());
                    //issueDay = int.Parse(testContext.DataRow["issue_day"].ToString());

                    if (issueMonth != 0 && issueDay != 0)
                    {
                        issueDateToCheck = DateTime.Parse($"{issueMonth}/{issueDay}/{DateTime.Today.Year}");
                    }

                    break;
                case "firstissuelastexpire":
                    issueDateToCheck = DateTime.Today;
                    break;
                default:
                    break;
            }

            return issueDateToCheck;
        }  
        public static DateTime CalulateNewExpDate(string issueExpSetting, out int expirationMonth, out int expirationDay)
        {
            expirationMonth = 0;
            expirationDay = 0;

            DateTime expirationDateToCheck = new DateTime();

            switch (issueExpSetting.Replace(" ","").ToLower())
            {
                case "usetodaysdate":
                    
                    expirationDateToCheck = DateTime.Today.AddYears(1);
                    break;
                case "usetodaysandlastdayofmonth":
                    
                    expirationDateToCheck = DateTime.Parse(
                        $"{DateTime.Today.Month}/{DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month)}/{DateTime.Today.Year}").AddYears(1);
                    break;
                case "use1stofmonth":
                    expirationDateToCheck = DateTime.Parse($"{DateTime.Today.Month}/1/{DateTime.Today.Year}").AddYears(1);
                    break;
                case "usespecificdate":
                    expirationDateToCheck = DateTime.Today.AddYears(1);
                    
                    //use specific expiration date
                    //expirationMonth = int.Parse(testContext.DataRow["expiration_month"].ToString());
                    //expirationDay = int.Parse(testContext.DataRow["expiration_day"].ToString());

                    if (expirationMonth != 0 && expirationDay != 0)
                    {
                        expirationDateToCheck = DateTime.Parse($"{expirationMonth}/{expirationDay}/{DateTime.Today.AddYears(1).Year}");
                    }

                    break;
                case "firstissuelastexpire":
                    expirationDateToCheck = CalculateNewIssueDate(issueExpSetting, out int issueMonth, out int issueDay).AddMonths(1);
                    expirationDateToCheck = expirationDateToCheck.AddDays((expirationDateToCheck.Day * -1)).AddYears(1);
                    break;
                default:
                    break;
            }

            return expirationDateToCheck;
        }

        public static string GetValidDispatch()
        {
            string dispatchCodes = SQLHandler.GetDatabaseValue(SQLStrings.ValidDispatchCode, "NOTES", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            string[] vs = dispatchCodes.Split(new char[] { ',' });

            return vs.FirstOrDefault().Replace("'", "");
        }

        public static bool ValidateStateList(List<string> actualStates)
        {
            do
            {
                actualStates.Remove("");
            } while (actualStates.Contains(""));

            List<string> expectedStates = CommonTestSettings.States;
            expectedStates.Sort();

            var missing = actualStates.Except(expectedStates).ToList();
            return missing.Count == 0;
        }

        public static string GetAgencyUrl(string agency)
        {
            string url = "";
            switch (agency.Replace(" ", string.Empty).ToLower())
            {
                case "nv-washoe":
                case "washoe":
                    url = Washoe.url;
                    break;
                case "tx-dallas":
                case "dallas":
                    url = Dallas.url;
                    break;
                case "nc-highpoint":
                case "highpoint":
                    url = HighPoint.url;
                    break;
                case "ca-winnipeg":
                case "winnipeg":
                    url = Winnipeg.url;
                    break;
                default:
                    url = Dallas.url;
                    break;
            }
            return url;
        }
        public static string GetAgencyMessage(string agency)
        {
            string message = "";
            switch (agency.ToLower())
            {
                case "nv-washoe":
                    message = JumpPageMessages.WashoeNV;
                    break;
                default:
                    break;
            }
            return message;
        }

        //public static void GetEncryptSiteData(out string URL, out string password)
        //{
        //    URL = $"http://crywolf-preprod.ps.lcl:{CommonTestSettings.port}/NC-HighPoint-Encrypt/";
        //    password = CommonTestSettings.pw;
        //}
        public static void SetDBName(string agency)
        {
            switch (agency.ToLower())
            {
                case "tx-dallas":
                case "dallas":
                    CommonTestSettings.dbName = Dallas.dbName;
                    break;
                case "nc-highpoint":
                case "highpoint":
                    CommonTestSettings.dbName = HighPoint.dbName;
                    break;
                default:
                    break;
            }
        }

        public static DateTime GetNextBusinessDay()
        {
            DateTime date = DateTime.Today.AddDays(1);

            if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday))
            {
                do
                {
                    date = date.AddDays(1);
                } while ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday));
            }

            return date;
        }

        public static string GetOnlinePaymentIDByAccount(string account)
        {
            return GetNotificationIdByAccount(account, "Online Payment");
        }
        public static string GetNotificationIdByAccount(string account, string letterType)
        {
            int autoCount = SQLHandler.GetDatabaseInt(
                $"SELECT * FROM ALARM_NOTES WHERE LetterType = '{letterType}' AND Notes like '%{account}%' AND created = '{DateTime.Today.ToShortDateString()}'",
                "AutoCount1",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);

            return autoCount.ToString();
        }

        public static string GetNoteByAccount(string account)
        {
            string note;
            string autoCount = GetOnlinePaymentIDByAccount(account);
            return note = GetNotificationInfoById(autoCount, "Notes");
        }

        public static string GetNotificationInfoById(string notificationID, string item)
        {
            return SQLHandler.GetDatabaseValue(
                $"SELECT * FROM ALARM_NOTES WHERE AutoCount1 = '{notificationID}'",
                item,
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);
        }

        public static string GetNotificationByAccount(string account, string letterType)
        {
            return SQLHandler.GetDatabaseValue(
                $"SELECT * FROM ALARM_NOTES WHERE LetterType = '{letterType}' AND Notes like '%{account}%'",
                "Notes",
                CommonTestSettings.dbHost,
                CommonTestSettings.dbName);
        }

        public static void ValidateDbConnection()
        {
            try
            {
                var query = "select 1";
                Console.WriteLine($"Connecting to: {CommonTestSettings.dbHost}::{CommonTestSettings.dbName}");
                using (var connection = Library.ConnectToDatabase(CommonTestSettings.dbHost, CommonTestSettings.dbName, CommonTestSettings.dbUser, CommonTestSettings.dbP))
                //using (SqlCommand command = new SqlCommand(query, connection))
                {

                    Console.WriteLine("Executing: {0}", query);

                    var command = new SqlCommand(query, connection);

                    //connection.Open();
                    Console.WriteLine("SQL Connection successful.");

                    command.ExecuteScalar();
                    Console.WriteLine("SQL Query execution successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure: {0}", ex.Message);
            }
        }

        public static string GetAgencyAlarmCoUrl(string agency)
        {
            string url = "";
            switch (agency.Replace(" ", string.Empty).ToLower())
            {
                case "nv-washoe":
                    url = Washoe.acUrl;
                    break;
                case "tx-dallas":
                case "dallas":
                    url = Dallas.acUrl;
                    break;
                case "nc-highpoint":
                    url = HighPoint.acUrl;
                    break;
                case "ca-winnipeg":
                    url = Winnipeg.acUrl;
                    break;
                default:
                    url = Dallas.acUrl;
                    break;
            }
            return url;
        }
        #endregion

        #region API Helper
        public static string GetAgencyToken(string agency)
        {
            switch (agency.ToLower())
            {
                case "highpoint":
                    return HighPoint.Token;
                case "dallas":
                    return Dallas.Token;
                default:
                    break;
            }
            return "notfound";
        }
        public static string GetAgencyKey(string agency)
        {
            switch (agency.ToLower())
            {
                case "highpoint":
                    return HighPoint.X_API_Key;
                case "dallas":
                    return Dallas.X_API_Key;
                default:
                    break;
            }
            return "notfound";
        }
        #endregion

        
    }
}
