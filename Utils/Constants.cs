using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class Constants
    {
        public const string CryWolfAPI = "http://localhost:57531/api/";
        public const string X_API_Key = "5DDE83C4-68AF-4C9F-8D84-0D9B40516721";

        public const string ReportDrive = @"\\spssrv1\common";
        public const string FailureResultsFile = "_REPORT-RESULTS-Failure_results.txt";
        public const string UserSimFailureFile = "_UserSimulation-Failure_results.txt";

        //public static bool writeToSql = false;

        public static string dbName = "crywolfautoreport";
        public static string dbHost = "spshprmsauto";
        public const string dbUser = "QA";
        public const string dbP = "$unGard1";
    }
}
