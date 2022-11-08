using NUnit.Framework;
using OpenQA.Selenium;
using Utils;
using Utils.PersonFactory;
using Web.PageObjects.AdminAccess;
using Web.PageObjects.CitizenPortal;

namespace Web
{
    public partial class Scenarios
    {
        private void AdminPortalLogin(string agency, string signon, string password)
        {
            Home home = new Home(driver);
            home.NavigateToLogin();
            Library.TakeScreenShot(driver, "Admin Access Login Page", out string path);

            LogIn logIn = new LogIn(driver);
            exitStatus = logIn.Login(agency, signon, password);
            Library.TakeScreenShot(driver, "Login to Admin Access", out path);
            ReportBuilder.ArrayBuilder("Login to Admin Access website with desktop credentials", exitStatus, "Login Validation");
        }

        public static void HomePageLogin(string userName, string password, IWebDriver _driver = null)
        {
            if (_driver != null)
            {
                driver = _driver;
            }

            HomePage homePage = new HomePage(driver);

            exitStatus = homePage.Login(userName, password);

            Library.TakeScreenShot(driver, "Login", out string path);
            ReportBuilder.ArrayBuilder($"Login to {driver.Url} as {userName}", exitStatus, "Login Validation");
            Assert.IsTrue(exitStatus);
        }

        public static void CompleteRegistration(Person person, Address address, string password, IWebDriver _driver = null)
        {
            if (_driver != null)
            {
                driver = _driver;
            }

            OnlineRegistration onlineRegistration = new OnlineRegistration(driver);
            CommonFields commonFields = new CommonFields(driver);

            onlineRegistration.UseAlarmedLocationInformation(true);
            commonFields.ChooseDLState(address.State);
            commonFields.EnterDLNumber(person.DlNumber);
            Library.TakeScreenShot(driver, "Mailing & Billing Information", out string path);

            onlineRegistration.CreatePassword(password);
            onlineRegistration.AcceptTermsAndConditions();
            onlineRegistration.SubmitRegistrationInformation();

            //NewRegFormPreview newRegFormPreview = new NewRegFormPreview(driver);
            //Library.TakeScreenShot(driver, "New Registration Form Preview", out path);

            //newRegFormPreview.SignOnToPay();

            //UserMain userMain = new UserMain(driver);
            //exitStatus = userMain.ValidateAccountNumber(out string accountNumber);
            //Library.TakeScreenShot(driver, "New Account Main Page", out path);
            //ReportBuilder.ArrayBuilder($"New Account Created with Alarm Number: [{accountNumber}]", exitStatus, "UI Validation");
            //Assert.IsTrue(exitStatus);
        }

        public static void ValidateNewRegistration(out string accountNumber, IWebDriver _driver = null)
        {
            if (_driver != null)
            {
                driver = _driver;
            }

            NewRegFormPreview newRegFormPreview = new NewRegFormPreview(driver);
            Library.TakeScreenShot(driver, "New Registration Form Preview", out string path);

            newRegFormPreview.SignOnToPay();

            UserMain userMain = new UserMain(driver);
            exitStatus = userMain.ValidateAccountNumber(out accountNumber);
            Library.TakeScreenShot(driver, "New Account Main Page", out path);
            ReportBuilder.ArrayBuilder($"New Account Created with Alarm Number: [{accountNumber}]", exitStatus, "UI Validation");
            Assert.IsTrue(exitStatus);
        }
    }
}
