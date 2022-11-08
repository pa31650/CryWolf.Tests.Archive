using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Web.PageObjects.CitizenPortal
{
    public class AcDefaultMain : CommonElements
    {
        private IWebDriver driver = null;

        string reportAdd;
        string categoryAdd;
        bool status = true;

        public string categoryName = "Alarm Company Main Page";

        public AcDefaultMain(IWebDriver driver)
        {
            this.driver = driver;
        }

        //Object Identification
        #region Page Elements

        private IWebElement lblRegisterAccounts => driver.FindElement(By.Id("lblNewReg"));

        private IWebElement lblCompanyUpdate => driver.FindElement(By.Id("lblUpdate"));

        private IWebElement btnChangePd => driver.FindElement(By.Id("btnChangePd"));
        #endregion

        #region Change Password Dialog
        private IWebElement txtCurrentPassword => driver.FindElement(By.Id("pd0"));

        private IWebElement txtNewPassword => driver.FindElement(By.Id("pd1"));

        private IWebElement txtVerifyPassword => driver.FindElement(By.Id("pd2"));

        private IWebElement btnSubmit => driver.FindElement(By.Id("btnSubmit"));

        private IWebElement lblWarning => driver.FindElement(By.Id("lblWarning"));
        private IWebElement btnClose => driver.FindElement(By.XPath(@"//button[contains(text(),'Close')]"));
        #endregion

        //Basic Interactions
        private void ClickClose()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                wait.Until(ExpectedConditions.ElementToBeClickable(btnClose));
                btnClose.Click();
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine(ErrorStrings.TimeoutException);
            }
        }

        private string GetWarningText()
        {
            return lblWarning.Text;
        }
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
        private void ClickChangePassword()
        {
            btnChangePd.Click();
        }
        private void ClickRegisterAccounts()
        {
            lblRegisterAccounts.Click();
        }

        private void ClickCompanyUpdate()
        {
            lblCompanyUpdate.Click();
        }

        //Short Functional Methods
        public bool RegisterNewAccount()
        {
            try
            {
                ClickRegisterAccounts();
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Register Account link was not found as defined");
                return false;
            }

            TestContext.WriteLine("Register Account link was found and clicked");
            return true;
        }

        public bool UpdateCompany()
        {
            try
            {
                ClickCompanyUpdate();
            }
            catch (NoSuchElementException)
            {
                TestContext.WriteLine("Company Update link was not found as defined");
                return false;
            }

            TestContext.WriteLine("Company Update link was found and clicked");
            return true;
        }
        public bool ChangePassword(string password, string newPassword)
        {
            ClickChangePassword();

            //Set current password
            SetCurrentPassword(password);

            //Set new password
            SetNewPassword(newPassword);

            //Verify new password
            SetVerifyPassword(newPassword);

            ClickSubmit();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementToBeClickable(lblWarning));

            string message = GetWarningText();

            return message.Contains("Password successfully changed");
        }

        public bool ResetPassword(string password, string newPassword)
        {
            //Set current password
            SetCurrentPassword(password);

            //Set new password
            SetNewPassword(newPassword);

            //Verify new password
            SetVerifyPassword(newPassword);

            ClickSubmit();

            string message = GetWarningText();

            return message.Contains("Password successfully changed");
        }
    }
}
