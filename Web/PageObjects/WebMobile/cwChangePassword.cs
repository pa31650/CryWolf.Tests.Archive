using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using Utils;

namespace Web.PageObjects.WebMobile
{
    public class cwChangePassword : CommonElements
    {
        private IWebDriver driver = null;

        ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;

        public cwChangePassword(IWebDriver driver)
        {
            this.driver = driver;
        }

        #region Page Elements
        private IWebElement changePasswordPage => driver.FindElement(By.Id("frmChngPW"));
        private IWebElement txtCurrentPassword => changePasswordPage.FindElement(By.Id("cpswrd0"));
        private IWebElement txtNewPassword => changePasswordPage.FindElement(By.Id("cpswrd1"));
        private IWebElement txtVerifyPassword => changePasswordPage.FindElement(By.Id("cpswrd2"));
        private IWebElement btnSubmit => changePasswordPage.FindElement(By.Id("btnLogonPW"));
        private IWebElement lblSuccess => driver.FindElement(By.Id("tblNote"));
        #endregion

        #region Basic UI Interactions
        private void ClickSubmit()
        {
            btnSubmit.Click();
        }
        private void SetCurrentPassword(string password)
        {
            txtCurrentPassword.SendKeys(password);
        }
        private void SetNewPassword(string password)
        {
            txtNewPassword.SendKeys(password);
        }
        private void SetVerifyPassword(string password)
        {
            txtVerifyPassword.SendKeys(password);
        }
        #endregion

        #region Short Business Processes
        public bool ChangePassword(string currentPassword,string newPassword)
        {
            SetCurrentPassword(currentPassword);
            SetNewPassword(newPassword);
            SetVerifyPassword(newPassword);
            ClickSubmit();
            Thread.Sleep(1500);
            WaitFor(driver, driver.FindElement(By.Id("acct1")));
            WaitFor(driver, driver.FindElement(By.Id("tblNote")));
            //WaitFor(driver, lblSuccess);
            string success = lblSuccess.Text;
            TestContext.WriteLine(success);
            if (success == "Password successfully changed")
            {
                return true;
            }
            else
            {
                WaitFor(driver, lblSuccess);
                return lblSuccess.Text == "Password successfully changed";
            }
            
        }
        #endregion
    }
}
