using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using Utils.PersonFactory;

namespace Web.PageObjects.WebMobile
{
    public class NewAccount : CommonElements
    {
        private IWebDriver driver = null;

        //ReportBuilder report = new ReportBuilder();
        string reportAdd;
        string categoryAdd;
        bool status = true;

        public NewAccount(IWebDriver driver)
        {
            this.driver = driver;
        }

        #region Page Elements
        private IWebElement newAccountPage => driver.FindElement(By.Id("page5"));
        private SelectElement cbLocation => new SelectElement(newAccountPage.FindElement(By.Id("cbLocation")));
        private IWebElement txtLastName => newAccountPage.FindElement(By.Id("nlastname"));
        private IWebElement txtFirstName => newAccountPage.FindElement(By.Id("nfirstname"));
        private IWebElement txtStrNum => newAccountPage.FindElement(By.Id("nstrnum"));
        private IWebElement txtStrName => newAccountPage.FindElement(By.Id("nstreet"));
        private IWebElement txtCity => newAccountPage.FindElement(By.Id("ncity"));
        private IWebElement txtState => newAccountPage.FindElement(By.Id("nstate"));
        private IWebElement txtZip => newAccountPage.FindElement(By.Id("nzip"));
        private IWebElement txtMainPhone => newAccountPage.FindElement(By.Id("nphone1"));
        private IWebElement txtEmail => newAccountPage.FindElement(By.Id("nemail"));
        private IWebElement ckUseAlarmedLocation => newAccountPage.FindElement(By.Id("ckUseLocation"));
        private IWebElement lblUseAlarmedLocation => newAccountPage.FindElement(By.XPath("//label[@class='ui-btn ui-corner-all ui-btn-inherit ui-btn-icon-left ui-checkbox-off']"));
        private IWebElement txtPassword => newAccountPage.FindElement(By.Id("npassword"));
        private IWebElement txtConfirmPassword => newAccountPage.FindElement(By.Id("nconfirm_password"));
        private IWebElement btnSubmit => newAccountPage.FindElement(By.Id("submit5"));
        #endregion

        #region Basic UI Interactions
        private void SelectLocation(string location)
        {
            cbLocation.SelectByText(location);
        }
        private void SetLastName(string lastName)
        {
            txtLastName.SendKeys(lastName);
        }
        private void SetFirstName(string firstName)
        {
            txtFirstName.SendKeys(firstName);
        }
        private void SetStreetNum(string streetNum)
        {
            txtStrNum.SendKeys(streetNum);
        }
        private void SetStreetName(string streetName)
        {
            txtStrName.SendKeys(streetName);
        }
        private void SetCity(string city)
        {
            txtCity.SendKeys(city);
        }
        private void SetState(string state)
        {
            txtState.SendKeys(state);
        }
        private void SetZip(string zip)
        {
            txtZip.SendKeys(zip);
        }
        private void SetMainPhone(string mainPhone)
        {
            txtMainPhone.SendKeys(mainPhone);
        }
        private void SetEmail(string email)
        {
            txtEmail.SendKeys(email);
        }
        private void CheckUseAlarmedLocation()
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(ckUseAlarmedLocation);
            actions.SendKeys(Keys.PageDown);
            actions.Perform();

            if (!ckUseAlarmedLocation.Selected)
            {
                lblUseAlarmedLocation.Click();
            }
        }
        private void UnCheckUseAlarmedLocation()
        {
            if (ckUseAlarmedLocation.Selected)
            {
                ckUseAlarmedLocation.Click();
            }
        }
        private void SetPassword(string password)
        {
            txtPassword.SendKeys(password);
        }
        private void SetConfirmPassword(string password)
        {
            txtConfirmPassword.SendKeys(password);
        }
        private void ClickSubmit()
        {
            btnSubmit.Click();
        }
        #endregion
        
        #region Short Business Processes
        public bool CompleteNewUserForm(string location,string password, Person person,Address address, out string AlarmNo)
        {
            TestContext.WriteLine(
                $"New User Info:{Environment.NewLine}" +
                $"Name: {person.LastName}, {person.FirstName}{Environment.NewLine}" +
                $"Address: {Environment.NewLine}" +
                $"[{address.StreetNumber}] [{address.StreetName}]{Environment.NewLine}" +
                $"[{address.City}], [{address.State}] [{address.ZipCode}]{Environment.NewLine}" +
                $"Phone: {person.primaryPhone().Number()}{Environment.NewLine}" +
                $"Email: {person.primaryEmail().GetEmail()}");
            SelectLocation(location);
            SetLastName(person.LastName);
            SetFirstName(person.FirstName);
            SetZip(address.ZipCode);
            SetStreetNum(address.StreetNumber);
            SetStreetName(address.StreetName);
            SetCity(address.City);
            SetState(address.State);
            SetMainPhone(person.primaryPhone().Number());
            SetEmail(person.primaryEmail().GetEmail());
            CheckUseAlarmedLocation();
            SetPassword(password);
            SetConfirmPassword(password);
            ClickSubmit();
            Thread.Sleep(1000);
            WaitFor(driver, driver.FindElement(By.Id("acct1")));
            WaitFor(driver, driver.FindElement(By.Id("tblNote")));
            AlarmNo = driver.FindElement(By.Id("acct1")).Text;
            TestContext.WriteLine($"Alarm Number created: [{AlarmNo}] ");
            return driver.FindElement(By.Id("tblNote")).Text == "Account Successfully Added";

        }
        #endregion
    }
}
