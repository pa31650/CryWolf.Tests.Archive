using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using Utils;

namespace Web.PageObjects.CitizenPortal
{
    public class HomePage : CommonElements
    {
        private IWebDriver driver = null;

        ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;

        public HomePage(IWebDriver driver) 
        {
            this.driver = driver;
            CommonTestSettings.BuildVersionWeb = metaBuildVersion.GetAttribute("content");
        }

        #region Page Elements
        private IWebElement metaBuildVersion => driver.FindElement(By.Name("build-id"));
        //Register Online Button
        private IWebElement btnRegisterOnline => driver.FindElement(By.Id("spreg"));
        //Account # text input
        private IWebElement txtAccountNo => driver.FindElement(By.Id("sn"));
        //Password/Invoice password input
        private IWebElement txtPassword => driver.FindElement(By.Id("pd"));
        //Submit button
        private IWebElement btnSubmit => driver.FindElement(By.Id("btnLogOn"));
        //Change Password Button
        private IWebElement lnkChangePassword => driver.FindElement(By.Id("spchpwrd1"));
        //Invalid Account/Password message
        private IWebElement lblInvalidAccount => driver.FindElement(By.Id("lblNotice"));
        #endregion

        #region Basic UI Interactions
        public void ClickRegisterOnline()
        {
            btnRegisterOnline.Click();
        }

        public void EnterAccountNum(String accountnum)
        {
            try
            {
                WaitFor(driver, txtAccountNo);
                txtAccountNo.SendKeys(accountnum);
            }
            catch (WebDriverException)
            {
                reportAdd = $"Failed to Send Keys: {accountnum} to {txtAccountNo.Text}";
                categoryAdd = "WebDriver Exception";
                ReportBuilder.ArrayBuilder(reportAdd, false, categoryAdd);
            }
        }

        public void EnterPassword(String password)
        {
            try
            {
                //txtPassword.SendKeys(" ");
                //WaitFor(driver, txtPassword);
                txtPassword.Clear();
                WaitFor(driver, txtPassword);
                txtPassword.Click();
                txtPassword.SendKeys(password);
            }
            catch (WebDriverException)
            {
                reportAdd = $"Failed to Send Keys: {password} to {txtPassword.Text}";
                categoryAdd = "WebDriver Exception";
                ReportBuilder.ArrayBuilder(reportAdd, false, categoryAdd);
            }
        }

        public void ClickSubmit()
        {
            try
            {
                btnSubmit.Click();
            }
            catch (WebDriverException)
            {
                reportAdd = $"Failed to click {btnSubmit.Text}";
                categoryAdd = "WebDriver Exception";
                ReportBuilder.ArrayBuilder(reportAdd, false, categoryAdd);
            }
        }
        #endregion

        #region Short Business Processes
        public void GetBuildVersion()
        {
            CommonTestSettings.BuildVersionWeb = metaBuildVersion.Text;
        }

        public bool Login(String accountnum, String password)
        {
            EnterAccountNum(accountnum);
            Actions builder = new Actions(driver);
            builder.SendKeys(Keys.Tab).Perform();
            WaitFor(driver, txtPassword);
            EnterPassword(password);

            ClickSubmit();

            try
            {
                string message = driver.SwitchTo().Alert().Text;
                driver.SwitchTo().Alert().Accept();
                TestContext.WriteLine($"Alert encountered with message: [{message}]");
                return false;
            }
            catch (NoAlertPresentException)
            {

            }

            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("spchpwrd1")));
            }
            catch (WebDriverTimeoutException)
            {
                TestContext.WriteLine($"User: [{accountnum}], Password: [{password}] failed to login");
                return false;
            }

            reportAdd = $"User: [{accountnum}] was logged in successfully";
            //TestContext.WriteLine($"User: {accountnum} was logged in successfully");
            return true;

            //status = lnkChangePassword.Displayed;

            //if (!status)
            //{
            //    TestContext.WriteLine($"User: {accountnum}, Password: {password} failed to login");
            //}
            //return status;
        }
        #endregion
    }
}
