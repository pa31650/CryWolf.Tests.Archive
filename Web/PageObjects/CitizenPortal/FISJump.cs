using OpenQA.Selenium;

namespace Web.PageObjects.Web
{
    public class FISJump : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;
        public string categoryName = "FIS Jump Page";

        #region Constructor
        public FISJump(IWebDriver driver)
        {
            this.driver = driver;
        }
        #endregion

        #region Page Elements
        private IWebElement btnContinue => driver.FindElement(By.Id("ctl00_ContentPlaceHolder1_btnContinue"));
        private IWebElement lblAbout => driver.FindElement(By.XPath("//span[starts-with(@id,'ctl00_ContentPlaceHolder1_lblAbout') and @class='important']"));
        #endregion

        #region Basic UI Interactions
        private void ClickPayWithFISDirect()
        {
            btnContinue.Click();
        }
        private string GetAboutLabelText()
        {
            return lblAbout.Text;
        }
        #endregion

        #region Short Business Processes
        
        public void JumpToFIS()
        {
            ClickPayWithFISDirect();
        }
        public bool ValidateAgencyMessage(string expectedMessage, out string actualMessage)
        {
            actualMessage = GetAboutLabelText();
            return actualMessage.Trim().CompareTo(expectedMessage.Trim()) == 0;
        }
        #endregion

    }
}
