using OpenQA.Selenium;

namespace Web.PageObjects.WebMobile
{
    public class Account : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;

        public Account(IWebDriver driver)
        {
            this.driver = driver;
        }

        #region Page Elements
        private IWebElement accountPage => driver.FindElement(By.Id("page1"));
        private IWebElement btnPayment => accountPage.FindElement(By.Id("pay1")) as IWebElement;
        private IWebElement btnChngPW => accountPage.FindElement(By.Id("chngpw1")) as IWebElement;
        #endregion

        #region Basic UI Interactions
        private void ClickPayment()
        {
            btnPayment.Click();
        }
        private void ClickChangePassword()
        {
            btnChngPW.Click();
        }
        #endregion

        #region Short Business Processes
        public void NavigateToOutstanding()
        {
            ClickPayment();
        }
        public void NavigateToChangePassword()
        {
            ClickChangePassword();
        }
        #endregion
    }
}
