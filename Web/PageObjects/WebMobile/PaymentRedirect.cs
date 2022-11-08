using OpenQA.Selenium;

namespace Web.PageObjects.WebMobile
{
    public class PaymentRedirect : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;

        public PaymentRedirect(IWebDriver driver)
        {
            this.driver = driver;
        }

        #region Page Elements
        private IWebElement PaymentRedirPage => driver.FindElement(By.Id("page4"));
        private IWebElement btnPayNow => PaymentRedirPage.FindElement(By.Id("btnRedirect4"));
        
        #endregion

        #region Basic UI Interactions
        
        private void ClickPayNow()
        {
            btnPayNow.Click();
        }
        #endregion

        #region Short Business Processes
        public void NavigateToPaymentProvider()
        {
            ClickPayNow();
        }
        #endregion
    }
}
