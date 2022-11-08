using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class MaintenanceHelper
    {
        private readonly string dbName;
        private readonly SQLHandler SQLHandler = new SQLHandler();

        public MaintenanceHelper(string dbName)
        {
            this.dbName = dbName;
        }

        #region Alarm Company Status Configuration
        public void SetAcStatusActive(string AcNumber)
        {
            SQLHandler.UpdateDatabaseValue(
                $"UPDATE ALARM_COMPANIES SET currentstatus = 'AC Active' WHERE AlarmNo = '{AcNumber}';",
                CommonTestSettings.dbHost,
                dbName);
        }

        public void SetAcStatusClosed(string AcNumber)
        {
            SQLHandler.UpdateDatabaseValue(
            $"UPDATE ALARM_COMPANIES SET currentstatus = 'AC Closed' WHERE AlarmNo = '{AcNumber}';",
                CommonTestSettings.dbHost,
                dbName);
        }
        #endregion

        #region Alarm Registration Settings
        public void SetCopyToRP(string behavior, string agency)
        {
            string setting = string.Empty;

            switch (behavior.ToLower())
            {
                case "default":
                    setting = "0";
                    break;
                case "address only":
                    setting = "1";
                    break;
                case "name residential":
                    setting = "2";
                    break;
                case "hidden":
                    setting = "3";
                    break;
                default:
                    break;
            }

            SQLHandler.UpdateDatabaseValue(
                $"UPDATE LU2_VALUES SET iValue1 = {setting} WHERE Agency = '{agency}' AND Search1 = 'ARSettings' AND Search2 = 'CopyToRPButtonBehavior';",
                CommonTestSettings.dbHost,
                dbName);
        }
        #endregion

        #region Lock Previous Records
        public void SetLockPreviousRecords(string DayOfMonth)
        {
            SQLHandler.InsertDatabaseValue(
                $"INSERT INTO LU2_VALUES VALUES('<ALL>','LockPreviousRecords','','',{DayOfMonth},'','','',0,0,0,0,0,0,0,0,'')",
                CommonTestSettings.dbHost,
                dbName);
        }

        public void DeleteLockPreviousRecords()
        {
            SQLHandler.DeleteDatabaseValue(
                $"DELETE FROM LU2_VALUES where Search1 = 'LockPreviousRecords';",
                CommonTestSettings.dbHost,
                dbName);
        }
        #endregion

        #region Alarm Registration Issue Expiration Date Types
        public void SetIssueExpiration(string IssueExpSetting, string agency)
        {
            switch (IssueExpSetting.Replace(" ", string.Empty).ToLower())
            {
                case "usetodaysdate":
                    IssueExpSetting = "UseToday";
                    break;
                case "usetodaysandlastdayofmonth":
                    IssueExpSetting = "UseExpLastOfMonth";
                    break;
                case "use1stofmonth":
                    IssueExpSetting = "Use1stOfMonth";
                    break;
                case "usespecificdate":
                    IssueExpSetting = "UseSpecificDate";
                    break;
                case "firstissuelastexpire":
                    IssueExpSetting = "UseFirstIssueLastExpire";
                    break;
                default:
                    TestContext.WriteLine($"{IssueExpSetting} is not coded for.");
                    break;
            }

            SQLHandler.UpdateDatabaseValue(
                $"UPDATE LU2_DEFAULT set ARIssueExpDateTypes = '{IssueExpSetting}' where AGENCY = '{agency}'",
                CommonTestSettings.dbHost,
                dbName);

            TestContext.WriteLine($"Issue Expiration Setting set to: {IssueExpSetting} for Agency: {agency}");
        }

        public void SetARIssue(int ARIssueMonth, int ARIssueDay, string agency)
        {
            SQLHandler.UpdateDatabaseValue(
                $"UPDATE LU2_DEFAULT SET ARIssueMonth = '{ARIssueMonth}', ARIssueDay = '{ARIssueDay}' WHERE AGENCY = '{agency}'",
                CommonTestSettings.dbHost,
                dbName);

            TestContext.WriteLine($"Issue Month/Day set to: {ARIssueMonth}/{ARIssueDay}");
        }

        public void SetARExp(int ARExpMonth, int ARExpDay, string agency)
        {
            SQLHandler.UpdateDatabaseValue(
                $"UPDATE LU2_DEFAULT SET ARExpMonth = '{ARExpMonth}', ARExpDay = '{ARExpDay}' WHERE AGENCY = '{agency}'",
                CommonTestSettings.dbHost,
                dbName);

            TestContext.WriteLine($"Expiration Month/Day set to: {ARExpMonth}/{ARExpDay}");
        }
        #endregion

        #region Special Actions
        public void SetSpecialAction(
            string actionType, string currentStatus, string paidLetterType, string prepareLetter, string options, string agency = "<Default>")
        {
            SQLHandler.InsertDatabaseValue(
                $"INSERT INTO LU2_VALUES VALUES('{agency}','SpecialActions','{actionType}','{currentStatus}','{paidLetterType}','{prepareLetter}','{options}','',0,0,0,0,0,0,0,0,'')",
                CommonTestSettings.dbHost,
                dbName);
        }

        public void DeleteSpecialAction(
            string actionType, string currentStatus, string paidLetterType, string prepareLetter, string options, string agency = "<Default>")
        {
            SQLHandler.DeleteDatabaseValue(
                $"DELETE FROM LU2_VALUES WHERE Search1 = 'SpecialActions' AND Search2 = '{actionType}' AND Search3 = '{currentStatus}' AND Text1 = '{paidLetterType}' AND Text2 = '{prepareLetter}' AND Text3 = '{options}'",
                CommonTestSettings.dbHost,
                dbName);
        }
        #endregion

        #region Alarm Counting
        public void SetAlarmCountingPeriod(string agency, string location, string timePeriod, string month, int days, bool isDispatchCode)
        {
            string dispatchCode = isDispatchCode ? "1" : "0";

            SQLHandler.UpdateDatabaseValue(
                $"UPDATE LU2_VALUES SET Search2 = '{location}',Text1 = '{timePeriod}',Text2 = '{month}',iValue1 = '{days.ToString()}', iValue2 = '{dispatchCode}' WHERE Agency = '{agency}' AND Search1 = 'Charging Period'",
                CommonTestSettings.dbHost,
                dbName);
        }

        public void SetDispatchGroups(string agency, string dispatchGroup, string dispatchCodes)
        {
            SQLHandler.InsertDatabaseValue(
                $"INSERT INTO LU2_VALUES (Agency, Search1, Search2, Search3, Text1, Text2, Text3, Text4, iValue1, iValue2, iValue3, iValue4, dValue1, dValue2, dValue3, dValue4, Notes) VALUES('{agency}', 'DispatchCodes', '', '', '{dispatchGroup}', '', '', '', 0, 0, 0, 0, 0, 0, 0, 0, '{dispatchCodes}')",
                CommonTestSettings.dbHost,
                dbName);
        }

        public void SetDallasDispatchGroups()
        {
            string agency = "<Default>";
            SetDispatchGroups(agency, "Burglar", CommonTestSettings.Dallas.burglar);
            SetDispatchGroups(agency, "Panic", CommonTestSettings.Dallas.panic);
        }

        public void DeleteDispatchGroups(string agency)
        {
            SQLHandler.DeleteDatabaseValue(
                $"DELETE FROM LU2_VALUES WHERE Search1 = 'DispatchCodes' AND Agency = '{agency}'",
                CommonTestSettings.dbHost,
                dbName);
        }
        #endregion

        #region General System Settings
        public void EnableNextBusinessDayPayment(DateTime cutoffTime, string agency = "<Default>")
        {
            SQLHandler.UpdateDatabaseValue(
                $"UPDATE LU2_VALUES SET Text1 = 1, Text2 = '{cutoffTime.ToShortTimeString()}' WHERE Agency = '{agency}' AND Search2 = 'Payments After 2pm'",
                CommonTestSettings.dbHost,
                dbName);
            TestContext.WriteLine($"NextBusinessDayPayment enabled. Time set to: {cutoffTime}");
        }

        public void DisableNextBusinessDayPayment(string agency = "<Default>")
        {
            SQLHandler.UpdateDatabaseValue(
                $"UPDATE LU2_VALUES SET Text1 = 0, Text2 = '' WHERE Agency = '{agency}' AND Search2 = 'Payments After 2pm'",
                CommonTestSettings.dbHost,
                dbName);
            TestContext.WriteLine($"NextBusinessDayPayment disabled");
        }
        #endregion
    }
}
