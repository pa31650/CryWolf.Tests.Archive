using OpenQA.Selenium;
using System.Threading;

namespace Web.PageObjects.WebMobile
{
    public class cwMobile : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;

        public cwMobile(IWebDriver driver) 
        {
            this.driver = driver;
        }

        #region Page Elements
        private IWebElement btnNewUser => driver.FindElement(By.Id("newUser"));
        private IWebElement btnLogon => driver.FindElement(By.Id("settings"));
        private IWebElement dlgSignon => driver.FindElement(By.Id("dialog1"));
        private IWebElement txtSignon => dlgSignon.FindElement(By.Id("logon")) as IWebElement;
        private IWebElement txtPassword => dlgSignon.FindElement(By.Id("passwrd")) as IWebElement;
        private IWebElement btnSubmit => dlgSignon.FindElement(By.Id("btnLogon1")) as IWebElement;

        #endregion

        #region Basic UI Interactions
        private void ClickNewUser()
        {
            WaitFor(driver, btnNewUser);
            btnNewUser.Click();
        }
        private void ClickLogOn()
        {
            WaitFor(driver, btnLogon);
            btnLogon.Click();
        }
        private void SetAlarmNo(string AlarmNo)
        {
            txtSignon.SendKeys(AlarmNo);
        }
        private void SetPassword(string Password)
        {
            txtPassword.SendKeys(Password);
        }
        private void ClickSubmit()
        {
            btnSubmit.Click();
        }
        #endregion

        #region Short Business Processes
        public void CreateNewUser()
        {
            ClickNewUser();
        }
        public bool LoginAsCitizen(string AlarmNo, string Password)
        {
            ClickLogOn();
            SetAlarmNo(AlarmNo);
            SetPassword(Password);
            ClickSubmit();
            WaitFor(driver, driver.FindElement(By.Id("acct1")));
            Thread.Sleep(500);
            return driver.FindElement(By.Id("acct1")).Text == AlarmNo;
        }
        #endregion
    }
}
