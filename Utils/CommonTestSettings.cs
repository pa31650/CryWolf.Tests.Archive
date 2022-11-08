using System;
using System.Collections.Generic;
using System.IO;

namespace Utils
{
    public class CommonTestSettings
    {
        public const string AppName = "CryWolf";

        #region Execution
        public const string WinAppDriverLocal = "http://127.0.0.1:4723";
        public const string WinAppDriverRemote = "http://crywolf-user:4723/wd/hub";

        public const string SeleniumBase = "http://crywolf-user:4444";
        public static string SeleniumHubUrl = $"{SeleniumBase}/wd/hub";
        public static string SeleniumApiUrl = $"{SeleniumBase}/grid/api";

        public static string RunLocation = Utils.Default.RUN_LOCATION;
        public static int PAGE_TIMEOUT { get => 10; set => PAGE_TIMEOUT = value; }
        public static int ELEMENT_TIMEOUT { get => 10; set => ELEMENT_TIMEOUT = value; }
        public static int DEFAULT_GLOBAL_DRIVER_TIMEOUT { get => 10; set => DEFAULT_GLOBAL_DRIVER_TIMEOUT = value; }
        public static int PAGE_TIMEOUT_REMOTE { get => 5; set => PAGE_TIMEOUT_REMOTE = value; }
        public static int NEW_COMMAND_TIMEOUT_REMOTE { get => 6000; set => NEW_COMMAND_TIMEOUT_REMOTE = value; }
        #endregion

        #region Reporting
        public static bool WriteToSQL = Utils.Default.WRITE_TO_SQL;
        public const string ReportingDBName = "crywolfautoreport";
        public const string ReportingDBHost = "spshprmsauto.ps.lcl";
        public const string ReportingDBUser = "QA";
        public const string ReportingDBP = "$unGard1";
        public const string ReportDrive = @"\\spssrv1\common";
        public const string FailureResultsFile = "_REPORT-RESULTS-Failure_results.txt";
        public const string UserSimFailureFile = "_UserSimulation-Failure_results.txt";
        #endregion

        #region Data
        public static string dbHost = Utils.Default.DBHOST_GRID;
        public static string dbName = Utils.Default.DBNAME_GRID;
        public const string dbUser = "crywolf";
        public const string dbP = "dd@rsx^2";
        public const string pw = "Passw0rd!";

        private static string GetDbName(string dbBase)
        {
            string DbName = CryWolfVersion.ToLower().Equals("trunk") ? dbBase : $"{dbBase}-{CryWolfVersion.Replace(".", "")}";
            return DbName;
        }
        #endregion

        #region API
        public static string CryWolfAPI = $"http://crywolf-preprod:{API_Port}/api/";
        public const string API_Port = "57531";
        public const string Token = "|admin|welcome";
        public const string badToken = "blah";

        public static class Dallas
        {
            public const string X_API_Key = "F8603998-EEAC-4F1D-8B59-946D574F93D5";
            public static string Token = $"Dallas{CommonTestSettings.Token}";
            public static string dbName = $"cw-qa-tx-da";
            public static string url = $"http://{Utils.Default.DBHOST_GRID}:{CommonTestSettings.port}/TX-Dallas";
            public static string burglar = "''12R - RESIDENTIAL ALARM'',''12B - BUSINESS ALARM'',''12N - BURGLAR ALARM NONDISP'',''- BURGLAR ALARM UNKNOWN''";
            public static string panic = "''21B - BUSINESS HOLD UP'',''21R - RES PANIC ALARM''";
            public static string agencyCode = "da73117tx";
            public static string acUrl = url + "/ac";
        }

        public static class HighPoint
        {
            public const string X_API_Key = "335FEA4E-CA9D-44C8-9D4A-DDB317EA1F90";
            public static string Token = $"HighPoint{CommonTestSettings.Token}";
            public static string dbName = $"cw-qa-nc-hp";
            public static string url = $"http://{Utils.Default.DBHOST_GRID}:{CommonTestSettings.port}/NC-HighPoint";
            public static string agencyCode;
            public static string acUrl = url + "/ac";
        }

        public static class Washoe
        {
            public static string dbName = GetDbName("cw-qa-tx-da");
            public static string url = $"http://{Utils.Default.DBHOST_GRID}:{CommonTestSettings.port}/NV-Washoe";
            public static string acUrl = url + "/ac";
        }

        public static class Winnipeg
        {
            public static string dbName = GetDbName("cw-qa-tx-da");
            public static string url = $"http://{Utils.Default.DBHOST_GRID}:{CommonTestSettings.port}/CA-Winnipeg";
            public static string acUrl = url + "/ac";
        }
        #endregion

        #region Desktop
        public const string NetworkShare = @"\\CRYWOLF-USER";
        public const string Temp = @"C:\tmp";
        public const string DesktopPw = "welcome";

        public static string Build = Utils.Default.BUILD_CONFIG;
        public static string CryWolfAppID = $@"{Build}\CryWolf.exe";
        public static string CryWolfVersion = Utils.Default.APP_VERSION;
        public const string CryWolfRoot = @"C:\builds\crywolf\v2\Enterprise";
        public static string CryWolfPath = Path.Combine(CryWolfRoot, CryWolfVersion, CryWolfAppID);
        public static string CryWolfMaintId = $@"{CryWolfRoot}\{CryWolfVersion}\{Build}\CryWolfMaint.exe";

        public const string DesktopAppId = "Root";

        public const string CryWolfMaintName = @"CentralSquare | CryWolf Maintenance";
        public const string CryWolfAccessibilityID = "mdiMain";
        public const string MaintAccesibilityID = "frmMaintMain";
        public const string AutoProcessAccessibiltyID = "frmAutoProcess";
        public const string WinAppDriverPath = @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
        public static string BuildVersionDesktop;
        #endregion

        #region Web
        public static string QA_Url = Utils.Default.URL_QA;
        public static string BuildVersionWeb;
        public const string port = "8000";
        public static string AdminPortal = $"http://{Utils.Default.DBHOST_GRID}:{CommonTestSettings.port}/AdminAccess";
        #endregion

        public static List<string> States = new List<string> {
            "AL",
            "AK",
            "AZ",
            "AR",
            "CA",
            "CO",
            "CT",
            "DC",
            "DE",
            "FL",
            "GA",
            "HI",
            "ID",
            "IL",
            "IN",
            "IA",
            "KS",
            "KY",
            "LA",
            "ME",
            "MD",
            "MA",
            "MI",
            "MN",
            "MS",
            "MO",
            "MT",
            "NE",
            "NV",
            "NH",
            "NJ",
            "NM",
            "NY",
            "NC",
            "ND",
            "OH",
            "OK",
            "OR",
            "PA",
            "RI",
            "SC",
            "SD",
            "TN",
            "TX",
            "UT",
            "VT",
            "VA",
            "WA",
            "WV",
            "WI",
            "WY"
        };

        public static class ProcessAlarmValues
        {
            public const string AlarmCo = "ADT";
            public const string IncidentTime = "2:00 AM";
            public const string TimeDispatched = "2:05 AM";
            public const string TimeOnScene = "2:10 AM";
            public const string TimeCleared = "2:15 AM";
            public const string DispatcherInfo = "D5";
            public const string CallTakerInfo = "CT 5";
            public const string UnitsAssigned = "3322";
        }

        public static class ValidCCNumbers
        {
            public const string AMEX = "378282246310005";
            public const string AmexCVV = "1234";
            public const string MC = "5555555555554444";
            public const string VISA = "4111111111111111";
            public const string CVV = "123";
        }

    }

    public class ErrorStrings
    {
        public const string ElementNotVisible = "An element command could not be completed because the element is not pointer- or keyboard interactable.";
        public const string NoSuchElement = "An element could not be located on the page using the given search parameters.";
        public const string NoSuchWindow = "Currently selected window has been closed";
        public const string StaleElementReference = "An element command failed because the referenced element is no longer attached to the DOM.";
        public const string UnimplementedCommandLocator = "Unexpected error. Unimplemented Command: {0} locator strategy is not supported";
        public const string UnimplementedCommandTimeoutType = "Unexpected error. Unimplemented Command: {0} timeout type is not supported";
        public const string XPathLookupError = "Invalid XPath expression: {0} (XPathLookupError)";
        public const string TimeoutException = "Item was not found within the time alotted.";
    }

    public class SQLStrings
    {
        //public const string ExpiringCitizenOutstandingInvoice = "select Top 1 InvoiceNo,alarmNo,created,currentStatus,letterSent,letterType from ALARM_ACTIONS where currentStatus = 'Active' and letterType = 'Expiring' and isOutstanding < 0 order by created desc;";
        public const string ExpiringCitizenOutstandingInvoice = "SELECT TOP 1 ALARM_ACTIONS.InvoiceNo, ALARM_ACTIONS.alarmNo, ALARM_ACTIONS.created, ALARM_LICENSES.currentStatus,ALARM_LICENSES.dateIssued, ALARM_ACTIONS.letterSent, ALARM_ACTIONS.letterType FROM ALARM_ACTIONS, ALARM_LICENSES WHERE ALARM_ACTIONS.currentStatus = 'Active' AND ALARM_ACTIONS.letterType = 'Expiring' AND ALARM_ACTIONS.isOutstanding< 0 AND ALARM_ACTIONS.alarmNo = ALARM_LICENSES.AlarmNo AND ALARM_LICENSES.currentStatus = 'Expiring' AND ALARM_LICENSES.dateIssued < DATEADD(year,-1,GETDATE()) AND ALARM_LICENSES.locationType = 'Residential' ORDER BY NEWID();";
        public string ActiveCitizenExpiringToday = $"select * from ALARM_LICENSES where currentStatus = 'Active' and dateExpires = '{DateTime.Today}';";
        public const string ActiveCitizen = "select top 1 * from ALARM_LICENSES where currentStatus = 'Active' order by NEWID();";
        public const string ActiveResident = "select top 1 * from ALARM_LICENSES where currentStatus = 'Active' and locationType = 'Residential' order by NEWID();";
        public const string ActiveCommercial = "select top 1 * from ALARM_LICENSES where currentStatus = 'Active' and locationType = 'Commercial' order by NEWID();";
        public const string RandomCitizen = "select top 1 * from ALARM_LiCENSES order by NEWID();";
        public const string CitizenWithOneFA = "select * from ALARM_ACTIONS where letterType like 'FA 1%' and incidentDate > DATEADD(yy, DATEDIFF(yy, 0, GETDATE()), 0) and currentStatus = 'Active' order by NEWID();";
        public const string RandomGeoCodeRow = "select top 1 AutoCount1 from LU2_xGEOCODE order by NEWID();";
        public const string ValidDispatchCode = "SELECT * FROM LU2_VALUES WHERE Agency = '<Default>' AND Search1 = 'DispatchCodes' ORDER By AutoCount1;";
        public const string ActiveCitizenOutstandingInvoice = "select Top 1 InvoiceNo,alarmNo,created,currentStatus,letterSent,letterType from ALARM_ACTIONS where currentStatus = 'Active' and isOutstanding < 0 order by created desc;";
        public const string OutstandingInvoice = "SELECT TOP 1 InvoiceNo, alarmNo, created, currentStatus, letterSent, letterType FROM ALARM_ACTIONS WHERE isOutstanding < 0 AND letterType <> 'Conversion' ORDER BY NEWID();";
        public static string OldOutstandingInvoice = $"SELECT TOP 1 InvoiceNo, alarmNo, created, currentStatus, letterSent, letterType FROM ALARM_ACTIONS WHERE isOutstanding < 0 AND created > '{DateTime.Today.AddMonths(-1).Month}/1/{DateTime.Today.Year}' AND created < '{DateTime.Today.AddDays(-1)}' ORDER BY NEWID();";
        public static string OldInvoice = $"SELECT TOP 1 ALARM_ACTIONS.InvoiceNo, ALARM_ACTIONS.alarmNo, ALARM_ACTIONS.created, currentStatus, letterSent, letterType,Charge,Payment,isOutstanding FROM ALARM_ACTIONS, ALARM_PAYMENTS WHERE isOutstanding = 0 AND ALARM_ACTIONS.created < '{DateTime.Today}' AND Charge > 0 AND Payment > 0 and ALARM_PAYMENTS.InvoiceNo = ALARM_ACTIONS.InvoiceNo ORDER BY NEWID();";
        public const string ActiveAlarmCo = "SELECT TOP 1 * FROM ALARM_COMPANIES WHERE NOT currentstatus LIKE '%closed%' AND NOT password is null ORDER BY NEWID();";
        public const string AlarmCoInvoice = "SELECT alarmno, invoiceno, currentStatus FROM ALARM_ACTIONS WHERE currentStatus LIKE '%AC %';";
        public const string AlarmNumMultipleInvoices = "SELECT TOP 1 a.alarmNo FROM ALARM_ACTIONS a, ALARM_ACTIONS b WHERE a.alarmNo = b.alarmno AND a.isOutstanding = -1 AND b.isOutstanding = -1 GROUP BY a.alarmNo HAVING COUNT(a.alarmNo) > 1";
        public const string InactiveAlarmCo = "SELECT TOP 1 * FROM ALARM_COMPANIES WHERE currentstatus LIKE '%closed%';";
        public const string ActiveCitizenWAcServicerInstaller = "SELECT TOP 1 alarmNo,InstalledBy,ServicedBy FROM ALARM_LICENSES WHERE (InstalledBy <> '-1' AND InstalledBy <> '') AND (ServicedBy = InstalledBy) AND currentStatus = 'Active' ORDER BY NEWID();";
    }

    public class NonNumericCharacters
    {
        public const string Alphabet = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";

    }

    public class JumpPageMessages
    {
        public const string WashoeNV = "Note: There is a $3.00 or 3% processing fee (whichever is greater) charged by the credit card processor. This fee is non-refundable.";
    }
}
