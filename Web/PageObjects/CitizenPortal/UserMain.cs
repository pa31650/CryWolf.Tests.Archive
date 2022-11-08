using NUnit.Framework;
using OpenQA.Selenium;

namespace Web.PageObjects.CitizenPortal
{
    public class UserMain : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        //string reportAdd;
        //string categoryAdd;
        //bool status = true;
        //CommonElements commonElements = new CommonElements();

        #region Constructor
        public UserMain(IWebDriver driver)
        {
            this.driver = driver;
        }
        #endregion

        #region Page Elements
        private IWebElement lblHome => driver.FindElement(By.Id("lblHome"));
        private IWebElement lblUpdate => driver.FindElement(By.Id("lblUpdate"));
        private IWebElement lblHistory => driver.FindElement(By.Id("lblHistory"));
        private IWebElement lblSchool => driver.FindElement(By.Id("lblSchool"));
        private IWebElement btnChangePassword => driver.FindElement(By.Id("spchpwrd1"));
        private IWebElement lblPay => driver.FindElement(By.Id("lblPay"));
        private IWebElement lblAccount => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_lblAccount"));
        #endregion

        #region Basic UI Interactions
        private string GetAccountNumber()
        {
            return lblAccount.Text;
        }
        private bool IsAccountNumber()
        {
            return lblAccount.Displayed;
        }
        private void ClickUpdate()
        {
            lblUpdate.Click();
        }
        public void ClickChangePassword()
        {
            btnChangePassword.Click();
        }
        public bool IsChangePasswordDisplayed()
        {
            return btnChangePassword.Displayed;
        }
        private void ClickPayOnline()
        {
            lblPay.Click();
        }
        #endregion

        #region Short Business Processes
        public bool ValidateAccountNumber(out string accountNumber)
        {
            bool status = IsAccountNumber();
            accountNumber = "NotFound";

            if (status)
            {
                accountNumber = GetAccountNumber();
            }

            return status;
        }
        public void NavigateToUpdatePage()
        {
            TestContext.WriteLine("Navigating to Update page");
            ClickUpdate();
        }
        public void NavigatetoPayOnlinePage()
        {
            TestContext.WriteLine("Navigating to Pay Online page");
            ClickPayOnline();
        }
        #endregion

    }
}
