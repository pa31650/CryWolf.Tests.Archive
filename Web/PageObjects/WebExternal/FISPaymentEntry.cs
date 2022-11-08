using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using static Utils.CommonTestSettings;

namespace Web.PageObjects.WebExternal
{
    public class FISPaymentEntry : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;
        
        //CommonElements commonElements = new CommonElements();

        #region Constructor
        public FISPaymentEntry(IWebDriver driver)
        {
            this.driver = driver;
        }
        #endregion

        #region Page Elements
        private IWebElement imgCryWolf => driver.FindElement(By.Id("headerImageContainer"));
        private IWebElement txtCardNumber => driver.FindElement(By.Id("CardUserInput_CardNumber"));
        private SelectElement cbMonth => new SelectElement(driver.FindElement(By.Id("CardUserInput_ExpirationMonth")));
        private SelectElement cbYear => new SelectElement(driver.FindElement(By.Id("CardUserInput_ExpirationYear")));
        private IWebElement txtCVV => driver.FindElement(By.Id("CardUserInput_VerificationCode"));
        private IWebElement txtName => driver.FindElement(By.Id("BillingInfoUserInput_Name"));
        private IWebElement txtAddress => driver.FindElement(By.Id("BillingInfoUserInput_Address"));
        private IWebElement txtCity => driver.FindElement(By.Id("BillingInfoUserInput_City"));
        private SelectElement cbState => new SelectElement(driver.FindElement(By.Id("BillingInfoUserInput_State")));
        private IWebElement txtZip => driver.FindElement(By.Id("BillingInfoUserInput_PostalCode"));
        private IWebElement txtPhone => driver.FindElement(By.Id("BillingInfoUserInput_Phone"));
        private IWebElement txtEmail => driver.FindElement(By.Id("BillingInfoUserInput_Email"));
        private IWebElement btnContinue => driver.FindElement(By.Id("SubmitButton"));
        //Payment Preview
        private IWebElement btnProcessPayment => driver.FindElement(By.Id("SubmitButton"));
        #endregion

        #region Basic UI Interactions
        private void SetCardNumber(string cc)
        {
            txtCardNumber.SendKeys(cc);
        }
        private void SetExpMonth(string month)
        {
            cbMonth.SelectByText(month);
        }
        private void SetExpYear(string year)
        {

        }
        
        #endregion

        #region Short Business Processes
        public bool VerifyLanding()
        {
            return imgCryWolf.Displayed;
        }
        public void ProcessPayment(string name,string address,string city,string state,string zip)
        {
            txtCardNumber.SendKeys(ValidCCNumbers.VISA);
            cbMonth.SelectByValue(DateTime.Today.Month.ToString());
            cbYear.SelectByValue(DateTime.Today.AddYears(3).Year.ToString());
            txtCVV.SendKeys(ValidCCNumbers.CVV);
            txtName.SendKeys(name);
            txtAddress.SendKeys(address);
            txtCity.SendKeys(city);
            cbState.SelectByValue(state);
            txtZip.SendKeys(zip);
            txtPhone.SendKeys("3368675309");
            txtEmail.SendKeys("test@test.com");
            btnContinue.Click();

            btnProcessPayment.Click();
        }
        #endregion

    }
}
