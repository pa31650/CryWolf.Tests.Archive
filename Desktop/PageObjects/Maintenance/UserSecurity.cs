using Desktop.Libraries;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
using System;
using Utils;

namespace Desktop.PageObjects.Maintenance
{
    public class UserSecurity
    {
        private readonly WindowsDriver<WindowsElement> session;
        private bool status = false;
        public string categoryName = "User Security";

        public UserSecurity(WindowsDriver<WindowsElement> _session)
        {
            this.session = _session;
        }

        //Object Identification
        private WindowsElement frmUsers => session.FindElementByAccessibilityId("frmUsers");
        private WindowsElement cbAgency => frmUsers.FindElementByAccessibilityId("cbAgency") as WindowsElement;
        private WindowsElement txtFullName => frmUsers.FindElementByAccessibilityId("txtFullName") as WindowsElement;
        private WindowsElement txtSignon => frmUsers.FindElementByAccessibilityId("txtSignon") as WindowsElement;
        private WindowsElement btnDelete => frmUsers.FindElementByAccessibilityId("btnDelete") as WindowsElement;
        private WindowsElement txtPassword => frmUsers.FindElementByAccessibilityId("txtPassword") as WindowsElement;
        private WindowsElement txtVerify => frmUsers.FindElementByAccessibilityId("txtVerify") as WindowsElement;
        private WindowsElement txtEmail => frmUsers.FindElementByAccessibilityId("txtEmail") as WindowsElement;
        private WindowsElement btnOk => frmUsers.FindElementByAccessibilityId("txtOK") as WindowsElement;
        private WindowsElement btnClose => frmUsers.FindElementByAccessibilityId("Close") as WindowsElement;
        private WindowsElement dlgWarning => session.FindElementByName("Warning");
        private WindowsElement txtWarning => dlgWarning.FindElementByAccessibilityId("65535") as WindowsElement;
        private WindowsElement btnYes => dlgWarning.FindElementByName("Yes") as WindowsElement;

        //Basic Interactions
        private string GetWarningText()
        {
            return txtWarning.Text;
        }
        private void ClickYes()
        {
            TestContext.WriteLine($"Clicking Yes on Warning dialog to delete user");
            btnYes.Click();
        }
        private void ClickDelete()
        {
            TestContext.WriteLine($"Clicking Delete button");
            btnDelete.Click();
        }
        private bool SelectAgency(string agency)
        {
            TestContext.WriteLine($"Selecting agency: [{agency}]");
            cbAgency.SelectListItem(agency);

            return cbAgency.Text.Equals(agency);
        }
        private bool SetFullName(string fullName)
        {
            TestContext.WriteLine($"Setting full name to: [{fullName}]");
            txtFullName.Clear();
            txtFullName.SendKeys(fullName);

            return txtFullName.Text.Equals(fullName);
        }
        private bool SetSignon(string signon)
        {
            TestContext.WriteLine($"Setting signon to: [{signon}]");
            txtSignon.Clear();
            txtSignon.SendKeys(signon);

            return txtSignon.Text.Equals(signon);
        }
        private void SetPassword(string password)
        {
            TestContext.WriteLine($"Setting password");
            txtPassword.Clear();
            txtPassword.SendKeys(password);
            txtVerify.SendKeys(password);
        }
        private bool SetEmail(string email)
        {
            TestContext.WriteLine($"Setting email to: [{email}]");
            txtEmail.Clear();
            txtEmail.SendKeys(email);

            return txtEmail.Text.Equals(email);
        }
        private void ClickOk()
        {
            TestContext.WriteLine($"Clicking Ok to commit changes");
            btnOk.Click();
        }
        private void ClickClose()
        {
            TestContext.WriteLine($"Closing User Security window");
            btnClose.Click();
        }

        //Short functional methods
        public void EditUserPassword(string signon, string newPassword)
        {
            TestContext.WriteLine($"Searching for user: [{signon}]");

            session.FindElementByName(signon).Click();

            SetPassword(newPassword);
            ClickOk();
        }
        public bool DeleteUserByFullName(string fullName)
        {
            TestContext.WriteLine($"Searching for user: [{fullName}]");

            session.FindElementByName(fullName).Click();

            TestContext.WriteLine($"User with full name: [{fullName}] was found");
            ClickDelete();

            TestContext.WriteLine(GetWarningText());

            ClickYes();

            return session.FindElementsByName(fullName).Count == 0;
        }
        public void Close()
        {
            ClickClose();
        }
        public bool AddUserWithNoPermissions(string agency, string fullName, string signon, string password, string email)
        {
            SelectAgency(agency);
            SetFullName(fullName);
            SetSignon(signon);
            SetPassword(password);
            SetEmail(email);
            ClickOk();

            return session.FindElementsByName(fullName).Count > 0;

        }
    }
}

