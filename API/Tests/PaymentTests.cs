using API.Models.v1;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utils;
using static Utils.CommonTestSettings;

namespace API.Tests
{
    class PaymentTests : Payments
    {
        private DatabaseReader databaseReader = new DatabaseReader();

        private List<String> scenariosTested = new List<String>();

        private string categoryName = "Account Invoices API Test";
        private string res;
        private static string primeKey;

        Auth auth = new Auth();
        Account account = new Account();
        SendAutoLetter sendAutoLetter = new SendAutoLetter();
        Invoice invoice = new Invoice();

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            if (CommonTestSettings.WriteToSQL)
            {
                primeKey = DatabaseReader.CreateParentRecord();
            }
        }

        [SetUp]
        public void Setup()
        {
            if (CommonTestSettings.WriteToSQL)
            {
                databaseReader.AddToScenarioList(primeKey, TestContext.CurrentContext.Test.MethodName);
                scenariosTested.Add(TestContext.CurrentContext.Test.Name);
                ReportBuilder.getStartTime();
            }
        }

        [TearDown]
        public void Cleanup()
        {
            if (CommonTestSettings.WriteToSQL)
            {
                ReportBuilder.getEndTime();
                Library.ReportResults(TestContext.CurrentContext.Test.MethodName, TestContext.CurrentContext.Test.MethodName, out res, primeKey, scenariosTested);
                databaseReader.UpdateEndTimeAndBuildVersion(primeKey, "version");
            }
        }

        [TestCase("Dallas", "Payment", HttpStatusCode.OK)]
        [TestCase("Dallas", "Payment", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", "Payment", HttpStatusCode.Conflict)]
        [TestCase("HighPoint", "Payment", HttpStatusCode.OK)]
        [TestCase("HighPoint", "Payment", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", "Payment", HttpStatusCode.Conflict)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario052_Put_Payment_Invoice(string jurisdiction, string paymentType, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;

            //string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();

            CryWolfUtil.SetDBName(jurisdiction);

            string invoiceId = SQLHandler.GetDatabaseValue(SQLStrings.OutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, dbName));

            if (expectedStatusCode == HttpStatusCode.Conflict)
            {
                invoiceIds.Add(999999999);
            }
            else
            {
                invoiceIds.Add(int.Parse(invoiceId));
            }    
            
            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = PutPaymentInvoiceOverPay(token, invoiceIds,paymentType,amount,"true");

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            var paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);
        }

        [TestCase("Dallas","Alarm Registration", HttpStatusCode.OK)]
        [TestCase("Dallas", "Alarm Company", HttpStatusCode.OK)]
        [TestCase("HighPoint", "Alarm Registration", HttpStatusCode.OK)]
        [TestCase("HighPoint", "Alarm Company", HttpStatusCode.OK)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario053_Put_Payment_Invoice_SeparateRegistration(string jurisdiction, string paidByAccountType, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            //string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();
            string paidByAccountNumber = string.Empty;

            ////switch (jurisdiction.ToLower())
            ////{
            ////    case "highpoint":
            ////        dbName = HighPoint.dbName;
            ////        break;
            ////    case "dallas":
            ////        dbName = Dallas.dbName;
            ////        break;
            ////    default:
            ////        TestContext.WriteLine($"Jurisdiction: {jurisdiction} is not recognized.");
            ////        break;
            ////}

            CryWolfUtil.SetDBName(jurisdiction);

            string invoiceId = SQLHandler.GetDatabaseValue(
                SQLStrings.OutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, CommonTestSettings.dbName));

            switch (paidByAccountType.Replace(" ","").ToLower())
            {
                case "alarmregistration":
                    paidByAccountNumber = CryWolfUtil.GetAlarmNo(SQLStrings.ActiveResident, CommonTestSettings.dbName);
                    paidByAccountType = "AR";
                    break;
                case "alarmcompany":
                    paidByAccountNumber = CryWolfUtil.GetACNo(SQLStrings.ActiveAlarmCo, CommonTestSettings.dbName);
                    paidByAccountType = "AC";
                    break;
                default:
                    TestContext.WriteLine($"Account Type: {paidByAccountType} is not recognized.");
                    break;
            }

            invoiceIds.Add(int.Parse(invoiceId));

            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = PutPaymentInvoice(token, invoiceIds, "Payment", amount, paidByAccountType, paidByAccountNumber,null,null,null,null,"true");

            var paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas","Closed")]
        [TestCase("Dallas", null, "Payment Received")]
        [TestCase("Dallas", null, null, "Today", "Residential")]
        [TestCase("HighPoint", "Closed")]
        [TestCase("HighPoint", null, "Payment Received")]
        [TestCase("HighPoint", null, null, "Today", "Residential")]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario054_Put_Payment_Invoice_AndAction(string jurisdiction, string status = null, string letter = null, string issueDate = null, string expDate = null)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();

            if (issueDate != null && issueDate.ToLower().Equals("today"))
            {
                issueDate = DateTime.Today.ToShortDateString();

                if (expDate.ToLower().Equals("residential"))
                {
                    expDate = DateTime.Today.AddYears(1).ToShortDateString();
                }
            }

            switch (jurisdiction.ToLower())
            {
                case "highpoint":
                    dbName = HighPoint.dbName;
                    break;
                case "dallas":
                    dbName = Dallas.dbName;
                    break;
                default:
                    TestContext.WriteLine($"Jurisdiction: {jurisdiction} is not recognized.");
                    break;
            }

            string invoiceId = SQLHandler.GetDatabaseValue(SQLStrings.OutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, dbName);
            ReportBuilder.ArrayBuilder($"Outstanding Invoice Id is : [{invoiceId}]",true,"Database Value");

            double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, dbName));
            ReportBuilder.ArrayBuilder($"Amount to be paid is : [{amount}]", true, "Database Value");

            invoiceIds.Add(int.Parse(invoiceId));

            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = PutPaymentInvoiceWithAction(token, invoiceIds, "Payment", amount, letter, status, issueDate, expDate,"true");

            var paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            string AlarmNo = CryWolfUtil.GetAlarmByInvoice(invoiceId, dbName);

            if (status != null)
            {
                string actualStatus = account.GetAccountStatus(token, AlarmNo);
                ReportBuilder.ReportResponseContent(restResponse);
                ReportBuilder.ArrayBuilder($"Expected Status for Alarm: [{status}]. Actual Status was [{actualStatus}]", status.Equals(actualStatus), "Validate Current Status");
                Assert.AreEqual(status, actualStatus, $"Expected Status for Alarm: [{status}]. Actual Status was [{actualStatus}]");
            }

            if (issueDate != null)
            {
                string actualIssue = account.GetAccountIssueDate(token, AlarmNo).ToShortDateString();
                ReportBuilder.ArrayBuilder($"Expected Issue Date for Alarm: [{issueDate}]. Actual Issue Date was [{actualIssue}]", issueDate.Equals(actualIssue), "Validate Issue Date");
                Assert.AreEqual(issueDate, actualIssue, $"Expected Issue Date for Alarm: [{issueDate}]. Actual Issue Date was [{actualIssue}]");

                string actualExpire = account.GetAccountExpDate(token, AlarmNo).ToShortDateString();
                ReportBuilder.ArrayBuilder($"Expected Expiration Date for Alarm: [{expDate}]. Actual Expiration Date was [{actualExpire}]", expDate.Equals(actualExpire), "Validate Expiration Date");
                Assert.AreEqual(expDate, actualExpire, $"Expected Expiration Date for Alarm: [{expDate}]. Actual Expiration Date was [{actualExpire}]");
            }

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
            
        }

        [TestCase("Dallas",HttpStatusCode.OK)]
        [TestCase("Dallas", HttpStatusCode.Unauthorized)]
        [TestCase("Dallas", HttpStatusCode.NotFound)]
        [TestCase("HighPoint", HttpStatusCode.OK)]
        [TestCase("HighPoint", HttpStatusCode.Unauthorized)]
        [TestCase("HighPoint", HttpStatusCode.NotFound)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario055_Delete_Payment_Invoice(string jurisdiction,HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();
            string paymentId = string.Empty;

            #region Get Outstanding Invoice
            switch (jurisdiction.ToLower())
            {
                case "highpoint":
                    dbName = HighPoint.dbName;
                    break;
                case "dallas":
                    dbName = Dallas.dbName;
                    break;
                default:
                    break;
            }

            TestContext.WriteLine($"Jurisdiction : [{jurisdiction}]");

            string invoiceId = SQLHandler.GetDatabaseValue(SQLStrings.OutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, dbName);
            double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, dbName));

            TestContext.WriteLine($"Outstanding Invoice : [{invoiceId}]{Environment.NewLine}Amount Owed: [{amount}]");

            invoiceIds.Add(int.Parse(invoiceId));
            #endregion

            string token = auth.GetAuthToken(jurisdiction);

            if (expectedStatusCode != HttpStatusCode.NotFound)
            { 
                #region Put Payment Invoice
                TestContext.WriteLine("Executing PutPaymentInvoice");
                IRestResponse putResponse = PutPaymentInvoiceOverPay(token, invoiceIds, "Payment", amount,"true");

                ReportBuilder.ReportResponseContent(putResponse);
                #endregion

                #region Get Payments By Invoice
                TestContext.WriteLine("Executing GetPaymentsByInvoice");
                paymentId = GetPaymentIdByInvoice(token, invoiceId);

                TestContext.WriteLine($"Payment ID for Invoice [{invoiceId}]: [{paymentId}]");
                #endregion

                token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : token;
            }
            else
            {
                paymentId = "999999999";
            }

            #region Delete Invoice
            TestContext.WriteLine("Executing DeletePayment");
            IRestResponse deleteResponse = DeletePayment(token, paymentId);
            #endregion

            actualStatusCode = deleteResponse.StatusCode;

            ReportBuilder.ReportResponseContent(deleteResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas", "Payment", "1234")]
        [TestCase("HighPoint", "Payment", "1234")]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario056_Put_Payment_Invoice_ByCheck(string jurisdiction, string paymentType, string checkNumber)
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            HttpStatusCode actualStatusCode;
            string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();

            switch (jurisdiction.ToLower())
            {
                case "highpoint":
                    dbName = HighPoint.dbName;
                    break;
                case "dallas":
                    dbName = Dallas.dbName;
                    break;
                default:
                    TestContext.WriteLine($"Jurisdiction: {jurisdiction} is not recognized.");
                    break;
            }

            string invoiceId = SQLHandler.GetDatabaseValue(SQLStrings.OutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, dbName);
            double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, dbName));

            invoiceIds.Add(int.Parse(invoiceId));

            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = PutPaymentInvoiceCheck(token, invoiceIds, paymentType, amount, checkNumber, "Check");

            var paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas","Payment", true, HttpStatusCode.OK)]
        [TestCase("Dallas", "Payment", false, HttpStatusCode.Conflict)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario057_Put_Payment_Invoice_OverPay(string jurisdiction, string paymentType,bool overPaymentGoesToEscrow,HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();

            //switch (jurisdiction.ToLower())
            //{
            //    case "highpoint":
            //        dbName = HighPoint.dbName;
            //        break;
            //    case "dallas":
            //        dbName = Dallas.dbName;
            //        break;
            //    default:
            //        TestContext.WriteLine($"Jurisdiction: {jurisdiction} is not recognized.");
            //        break;
            //}

            CryWolfUtil.SetDBName(jurisdiction);

            string invoiceId = SQLHandler.GetDatabaseValue(
                SQLStrings.OutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, dbName)) + 1;

            invoiceIds.Add(int.Parse(invoiceId));

            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = PutPaymentInvoiceOverPay(token, invoiceIds, paymentType, amount, overPaymentGoesToEscrow.ToString());

            var paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas","AC","ADT","AR", "625479", 1, HttpStatusCode.OK)]
        [TestCase("Dallas", "AR", "625479", "AC", "ADT", 1, HttpStatusCode.OK)]
        [TestCase("Dallas", "AC", "-1", "AR", "625479", 1, HttpStatusCode.Conflict)]
        [TestCase("HighPoint", "AR", "16764", "AC", "101", 1, HttpStatusCode.OK)]
        [TestCase("HighPoint", "AC", "101", "AR", "625479", 1, HttpStatusCode.OK)]
        [TestCase("HighPoint", "AC", "-1", "AR", "625479", 1, HttpStatusCode.Conflict)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario058_Put_Payment_EscrowTransfer(
            string jurisdiction, string sourceAccountType, string sourceAccountNumber, 
            string destinationAccountType, string destingationAccountNumber, int amountToTransfer, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            //string dbName = string.Empty;
            //List<int> invoiceIds = new List<int>();

            //switch (jurisdiction.ToLower())
            //{
            //    case "highpoint":
            //        dbName = HighPoint.dbName;
            //        break;
            //    case "dallas":
            //        dbName = Dallas.dbName;
            //        break;
            //    default:
            //        break;
            //}

            CryWolfUtil.SetDBName(jurisdiction);

            string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = PutPaymentTransferEscrow(
                token, sourceAccountType,sourceAccountNumber,
                destinationAccountType, destingationAccountNumber, amountToTransfer);

            //var paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            //var paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);
        }

        [TestCase("Dallas", "Payment", "Yesterday",HttpStatusCode.OK)]
        [TestCase("Dallas", "Payment", "Tomorrow", HttpStatusCode.Conflict)]
        [TestCase("HighPoint", "Payment", "Yesterday", HttpStatusCode.OK)]
        [TestCase("HighPoint", "Payment", "Tomorrow", HttpStatusCode.Conflict)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario059_Put_Payment_Invoice_WithDate(string jurisdiction, string paymentType, string paymentDate, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            //string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();

            //switch (jurisdiction.ToLower())
            //{
            //    case "highpoint":
            //        dbName = HighPoint.dbName;
            //        break;
            //    case "dallas":
            //        dbName = Dallas.dbName;
            //        break;
            //    default:
            //        TestContext.WriteLine($"Jurisdiction: {jurisdiction} is not recognized.");
            //        break;
            //}

            CryWolfUtil.SetDBName(jurisdiction);

            switch (paymentDate.ToLower())
            {
                case "yesterday":
                    paymentDate = DateTime.Today.AddDays(-1).ToShortDateString();
                    break;
                case "tomorrow":
                    paymentDate = DateTime.Today.AddDays(1).ToShortDateString();
                    break;
                default:
                    break;
            }

            string invoiceId = SQLHandler.GetDatabaseValue(SQLStrings.OutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
            ReportBuilder.ArrayBuilder($"Outstanding Invoice: [{invoiceId}]",true,"Test Data");

            double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, dbName));
            ReportBuilder.ArrayBuilder($"Invoice Original Charge: [{amount}]", true, "Test Data");

            double payments = double.Parse(CryWolfUtil.GetInvoicePayments(invoiceId, dbName));
            ReportBuilder.ArrayBuilder($"Invoice Payments Made: [{payments}]", true, "Test Data");

            double outstanding = amount - payments;
            ReportBuilder.ArrayBuilder($"Invoice Current Outstanding: [{outstanding}]", true, "Test Data");

            invoiceIds.Add(int.Parse(invoiceId));

            string token = auth.GetAuthToken(jurisdiction);

            IRestResponse restResponse = PutPaymentWithDate(token,invoiceIds, paymentType, outstanding, paymentDate);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");

            if (expectedStatusCode != HttpStatusCode.Conflict)
            {
                var paymentPUT =  Models.v1.PaymentPUT.FromJson(restResponse.Content);

                Assert.NotNull(paymentPUT.InformationMessages);
            }
        }

        [TestCase("Dallas","Payment", "PaymentAutoActions", "Expiring", "Expiring", "Payment Received", "Request Approval",true, HttpStatusCode.OK)]
        [TestCase("Dallas", "Payment", "PaymentAutoActions", "Expiring", "Expiring", "Payment Received", "Request Approval", false, HttpStatusCode.OK)]
        [TestCase("HighPoint", "Payment", "PaymentAutoActions", "Expiring", "Expiring", "Payment Received", "Request Approval", true, HttpStatusCode.OK)]
        [TestCase("HighPoint", "Payment", "PaymentAutoActions", "Expiring", "Expiring", "Payment Received", "Request Approval", false, HttpStatusCode.OK)]
        [Category("19.2_Regression")]
        [Parallelizable]
        public void Scenario060_Put_Payment_Invoice_SpecialActions(string jurisdiction, string paymentType, 
            string actionType, string currentStatus, string paidLetterType, string prepareLetter, string options, bool autoApprove,
            HttpStatusCode httpStatusCode)
        {
            {
                HttpStatusCode expectedStatusCode = httpStatusCode;
                HttpStatusCode actualStatusCode;
                
                //string dbName = string.Empty;
                List<int> invoiceIds = new List<int>();

                //switch (jurisdiction.ToLower())
                //{
                //    case "highpoint":
                //        dbName = HighPoint.dbName;
                //        break;
                //    case "dallas":
                //        dbName = Dallas.dbName;
                //        break;
                //    default:
                //        break;
                //}

                CryWolfUtil.SetDBName(jurisdiction);
                MaintenanceHelper maintenanceHelper = new MaintenanceHelper(CommonTestSettings.dbName);

                maintenanceHelper.DeleteSpecialAction(actionType, currentStatus, paidLetterType, prepareLetter, options);

                maintenanceHelper.SetSpecialAction(actionType, currentStatus, paidLetterType, prepareLetter, options);

                string invoiceId = SQLHandler.GetDatabaseValue(SQLStrings.ExpiringCitizenOutstandingInvoice, "InvoiceNo", CommonTestSettings.dbHost, CommonTestSettings.dbName);
                double amount = double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceId, CommonTestSettings.dbName));

                invoiceIds.Add(int.Parse(invoiceId));

                string token = expectedStatusCode == HttpStatusCode.Unauthorized ? CommonTestSettings.badToken : auth.GetAuthToken(jurisdiction);

                IRestResponse restResponse = PutPaymentInvoiceSpecialActions(token, invoiceIds, paymentType, amount, autoApprove);

                ReportBuilder.ReportResponseContent(restResponse);

                PaymentPUT paymentPUT = expectedStatusCode == HttpStatusCode.Conflict ? null : Models.v1.PaymentPUT.FromJson(restResponse.Content);

                if (!autoApprove)
                {
                    try
                    {
                        SendAutoLetterBody sendAutoLetterBody = new SendAutoLetterBody(
                                        paymentPUT.AutoActionsToApprove[0].InvoiceId,
                                        paymentPUT.AutoActionsToApprove[0].LetterName,
                                        paymentPUT.AutoActionsToApprove[0].AccountType,
                                        paymentPUT.AutoActionsToApprove[0].AccountNumber);

                        restResponse = sendAutoLetter.PostApproveSendAutoLetter(token, sendAutoLetterBody);
                    }
                    catch (NullReferenceException)
                    {
                        TestContext.WriteLine("No Auto Actions to Approve was returned");
                        maintenanceHelper.DeleteSpecialAction(actionType, currentStatus, paidLetterType, prepareLetter, options);
                        throw;
                    }

                    
                    ReportBuilder.ArrayBuilder($"AutoActions To Approve contains items for approval", !paymentPUT.AutoActionsToApprove.Equals(null), "Validate AutoActionsToApprove");
                    TestContext.WriteLine($"Account Number: {paymentPUT.AutoActionsToApprove[0].AccountNumber}{Environment.NewLine}" +
                        $"Account Type: {paymentPUT.AutoActionsToApprove[0].AccountType}{Environment.NewLine}" +
                        $"Invoice Id: {paymentPUT.AutoActionsToApprove[0].InvoiceId}{Environment.NewLine}" +
                        $"Letter Name: {paymentPUT.AutoActionsToApprove[0].LetterName}{Environment.NewLine}");

                    var sendAutoLetterResponse = Models.v1.SendAutoLetter.FromJson(restResponse.Content);
                }
                else
                {
                    ReportBuilder.ArrayBuilder($"AutoActions Performed contains items", !paymentPUT.AutoActionsPerformed.Equals(null), "Validate AutoActionsToApprove");
                    TestContext.WriteLine($"Account Number: {paymentPUT.AutoActionsPerformed[0].AccountNumber}{Environment.NewLine}" +
                        $"Account Type: {paymentPUT.AutoActionsPerformed[0].AccountType}{Environment.NewLine}" +
                        $"Invoice Id: {paymentPUT.AutoActionsPerformed[0].InvoiceId}{Environment.NewLine}" +
                        $"Letter Name: {paymentPUT.AutoActionsPerformed[0].LetterName}{Environment.NewLine}");
                }

                maintenanceHelper.DeleteSpecialAction(actionType, currentStatus, paidLetterType, prepareLetter, options);
                
                actualStatusCode = restResponse.StatusCode;

                ReportBuilder.ReportResponseContent(restResponse);
                
                ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
                Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
            }
        }

        [TestCase("Dallas", HttpStatusCode.Conflict)]
        [TestCase("HighPoint", HttpStatusCode.Conflict)]
        [Category("19.2_Regresssion")]
        [Parallelizable]
        public void Scenario061_Delete_Payment_Invoice_Old(string jurisdiction, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string dbName = string.Empty;

            switch (jurisdiction.ToLower())
            {
                case "highpoint":
                    dbName = HighPoint.dbName;
                    break;
                case "dallas":
                    dbName = Dallas.dbName;
                    break;
                default:
                    break;
            }

            MaintenanceHelper maintenanceHelper = new MaintenanceHelper(dbName);
            maintenanceHelper.SetLockPreviousRecords(DateTime.Today.Day.ToString());

            string invoiceId = SQLHandler.GetDatabaseValue(SQLStrings.OldInvoice, "InvoiceNo", CommonTestSettings.dbHost, dbName);

            string token = auth.GetAuthToken(jurisdiction);

            string paymentId = GetPaymentIdByInvoice(token, invoiceId);

            if (paymentId.Equals("NotFound"))
            {
                maintenanceHelper.DeleteLockPreviousRecords();
                Assert.AreEqual(expectedStatusCode, HttpStatusCode.NotFound, $"Status code is not {expectedStatusCode}");
            }

            IRestResponse restResponse = DeletePayment(token, paymentId);

            actualStatusCode = restResponse.StatusCode;

            ReportBuilder.ReportResponseContent(restResponse);

            maintenanceHelper.DeleteLockPreviousRecords();

            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }

        [TestCase("Dallas", "Payment", HttpStatusCode.OK)]
        [TestCase("HighPoint", "Payment", HttpStatusCode.OK)]
        [Parallelizable]
        public void Scenario062_Put_Payment_Invoices(string jurisdiction, string paymentType, HttpStatusCode httpStatusCode)
        {
            HttpStatusCode expectedStatusCode = httpStatusCode;
            HttpStatusCode actualStatusCode;
            string dbName = string.Empty;
            List<int> invoiceIds = new List<int>();
            string paymentId = string.Empty;
            double amount = 0.0;

            #region Get Outstanding Invoices
            switch (jurisdiction.ToLower())
            {
                case "highpoint":
                    dbName = HighPoint.dbName;
                    break;
                case "dallas":
                    dbName = Dallas.dbName;
                    break;
                default:
                    break;
            }

            TestContext.WriteLine($"Jurisdiction : [{jurisdiction}]");

            string alarmNo = SQLHandler.GetDatabaseValue(SQLStrings.AlarmNumMultipleInvoices, "alarmNo", CommonTestSettings.dbHost, dbName);
            #endregion

            string token = auth.GetAuthToken(jurisdiction);
            invoiceIds = invoice.GetInvoiceIdsByAccount(token, alarmNo, "true");

            foreach (var invoiceid in invoiceIds)
            {
                amount += double.Parse(CryWolfUtil.GetInvoiceAmount(invoiceid.ToString(), dbName));
            }

            #region Put Payments Invoice
            TestContext.WriteLine("Executing PutPaymentInvoice");
            IRestResponse putResponse = PutPaymentInvoiceOverPay(token, invoiceIds, "Payment", amount, "true");
            #endregion

            actualStatusCode = putResponse.StatusCode;

            ReportBuilder.ReportResponseContent(putResponse);
            ReportBuilder.ArrayBuilder($"Status Code {actualStatusCode} found for request.", expectedStatusCode.Equals(actualStatusCode), "Validate Status Code");
            Assert.AreEqual(expectedStatusCode, actualStatusCode, $"Status code is not {expectedStatusCode}");
        }
    }
}
