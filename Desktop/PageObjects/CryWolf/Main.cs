using Desktop.Libraries;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
//using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using Utils;

namespace Desktop.PageObjects.CryWolf
{
    public class Main
    {
        private readonly WindowsDriver<WindowsElement> session;
        private string reportAdd;
        private string categoryAdd;
        private bool status = false;

        public Main(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        #region Connection Selection
        private WindowsElement winCxnSelection => session.FindElementByName("Connection Selection");
        private WindowsElement btnChooseDBoK => winCxnSelection.FindElementByName("OK") as WindowsElement;
        #endregion

        #region Login
        private WindowsElement frmSplash => session.FindElementByAccessibilityId("frmSplash");
        private WindowsElement txtUserName => frmSplash.FindElementByName("txtUserName") as WindowsElement;
        private WindowsElement txtPassword => frmSplash.FindElementByName("txtPassword") as WindowsElement;
        private WindowsElement btnLoginOk => frmSplash.FindElementByName("OK") as WindowsElement;
        private WindowsElement lblVersion => frmSplash.FindElementByAccessibilityId("lblVersion") as WindowsElement;
        private WindowsElement btnExit => frmSplash.FindElementByName("Exit") as WindowsElement;
        #endregion

        private WindowsElement frmMain => session.FindElementByAccessibilityId("mdiMain");

        #region Menu Bar
        private WindowsElement mnuFile => frmMain.FindElementByName("File") as WindowsElement;
        private WindowsElement mnuOptions => frmMain.FindElementByName("Options") as WindowsElement;
        private WindowsElement btnClose => frmMain.FindElementByName("Close") as WindowsElement;
        #endregion

        #region Toolbar
        private WindowsElement btnProcessAlarms => frmMain.FindElementByName("Process Alarms") as WindowsElement;
        private WindowsElement btnProcessPayments => frmMain.FindElementByName("Process Payments") as WindowsElement;
        private WindowsElement btnProcessRegistrations => frmMain.FindElementByName("Process Registrations") as WindowsElement;
        private WindowsElement btnProcessAlarmCompanies => frmMain.FindElementByName("Process Alarm Companies") as WindowsElement;
        #endregion

        #region Status Bar
        private WindowsElement barStatus => frmMain.FindElementByName("StatusStrip") as WindowsElement;
        #endregion

        //Basic interactions
        private void ClickClose()
        {
            btnClose.Click();
        }

        #region Connection Selection
        private void SetDBSelection(string dbName)
        {
            TestContext.WriteLine($"Choosing Database: [{dbName}]");
            winCxnSelection.FindElementByName(dbName).Click();
        }
        private void ClickChooseDBOK()
        {
            TestContext.WriteLine("Attempting to click OK on Connection Selection");
            winCxnSelection.Click();
            btnChooseDBoK.Click();
        }
        #endregion

        #region Login
        private void EnterUserName(string userName)
        {
            txtUserName.SendKeys(userName);
        }
        private void EnterPassword(string password)
        {
            txtPassword.SendKeys(password);
        }
        private void ClickLoginOK()
        {
            btnLoginOk.Click();
        }
        private void GetDesktopVersion()
        {
            CommonTestSettings.BuildVersionDesktop = lblVersion.Text.Replace("v. ", "");
        }
        #endregion

        #region Menu Bar
        private void OpenRunAutoProcess()
        {
            mnuFile.SelectMenuItemByName("Run AutoProcess", session);

        }
        private void OpenOutstandingCorrespondence()
        {
            mnuFile.SelectMenuItemByName("Outstanding Correspondence", session);
        }
        private void OpenTableMaintenance()
        {
            mnuOptions.SelectMenuItemByName("Table Maintenance", session);
        }
        #endregion

        #region Toolbar

        private void ClickProcessAlarmCompanies()
        {
            btnProcessAlarmCompanies.Click();
        }
        private void ClickProcessAlarms()
        {
            btnProcessAlarms.Click();
        }
        private void ClickProcessRegistrationsButton()
        {
            btnProcessRegistrations.Click();
        }
        private void ClickProcessPaymentsButton()
        {
            btnProcessPayments.Click();
        }
        #endregion

        //Short functional methods
        public void DeleteReminders()
        {
            WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(15));
            var Reminders = session.FindElementsByAccessibilityId("btnDelete");
            foreach (var Reminder in Reminders)
            {
                Reminder.Click();
                wait.Until(ExpectedConditions.StalenessOf(Reminder));
            }
        }

        public void Close()
        {
            ClickClose();
        }

        public void WaitForDatabase()
        {
            try
            {
                do
                {
                    Thread.Sleep(750);
                } while (session == null);
            }
            catch (Exception)
            {


            }

        }

        public void ChooseDatabase()
        {
            try //Choosing Db
            {
                SetDBSelection(CommonTestSettings.dbName);
            }
            catch (InvalidOperationException)
            {

                if (txtUserName.Enabled)
                {
                    Console.WriteLine("No DB Choice given");
                    return;
                }
            }

            try //Clicking OK
            {
                ClickChooseDBOK();
            }
            catch (InvalidOperationException ioe)
            {
                reportAdd = $"Failed to Click {btnChooseDBoK.Text}";
                categoryAdd = "WebDriver Exception";
                ReportBuilder.ArrayBuilder(reportAdd, false, categoryAdd, ioe.Source);
            }
        }

        public bool Login(string username, string password, bool isNew = false)
        {
            WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(15));

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(txtUserName));
                txtUserName.SendKeys(username);
            }
            catch (Exception)
            {

                if (winCxnSelection.Enabled)
                {
                    ChooseDatabase();

                    EnterUserName(username);
                }
            }

            EnterPassword(password);
            GetDesktopVersion();
            ClickLoginOK();

            if (isNew)
            {
                session.Keyboard.SendKeys(Keys.Enter);
            }

            try
            {
                WindowsElement elmUserName = barStatus.FindElementByName(username) as WindowsElement;
                wait.Until(ExpectedConditions.ElementToBeClickable(elmUserName));
                status = elmUserName.Displayed;
                DeleteReminders();
            }
            catch (InvalidOperationException)
            {
                status = false;

                reportAdd = $"Failed to login as {username}";
                categoryAdd = "InvalidOperation Exception";
                ReportBuilder.ArrayBuilder(reportAdd, status, categoryAdd);
            }
            //catch (NoSuchElementException)
            //{
            //    session.Keyboard.SendKeys(Keys.Enter);
            //    status = barStatus.FindElementByName(username).Displayed;
            //}


            return status;
        }

        public bool LoginAsAdmin()
        {
            return Login("admin", CommonTestSettings.DesktopPw);
        }

        public bool ChooseDbLoginAsAdmin()
        {
            ChooseDatabase();
            return LoginAsAdmin();
        }

        public void OpenCryWolfMaint()
        {
            TestContext.WriteLine($"Launching CryWolfMaintenance");
            OpenTableMaintenance();
        }

        public void OpenAutoProcess()
        {
            OpenRunAutoProcess();
        }
        public void OpenOutstandingCorrespondance()
        {
            OpenOutstandingCorrespondence();
        }
        public void OpenProcessPayments()
        {
            ClickProcessPaymentsButton();
        }
        public bool OpenProcessRegistrations()
        {
            ClickProcessRegistrationsButton();

            try
            {
                WebDriverWait wait = new WebDriverWait(session, TimeSpan.FromSeconds(3));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Name("Add")));
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine(ErrorStrings.TimeoutException);
                return false;
            }
        }

        public void OpenProcessAlarms()
        {
            ClickProcessAlarms();
        }

        public void OpenProcessAlarmCompanies()
        {
            ClickProcessAlarmCompanies();
        }
    }
}
