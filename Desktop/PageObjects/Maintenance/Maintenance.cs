using Desktop.Libraries;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;


namespace Desktop.PageObjects.Maintenance
{
    public class Maintenance
    {
        private readonly WindowsDriver<WindowsElement> session;
        private bool status = false;
        public string categoryName = "CryWolf Maintenance";

        public Maintenance(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        private WindowsElement frmMaintMain => session.FindElementByAccessibilityId("frmMaintMain");
        private WindowsElement menuStrip => session.FindElementByAccessibilityId("menuStrip") as WindowsElement;
        private WindowsElement btnClose => frmMaintMain.FindElementByAccessibilityId("Close") as WindowsElement;

        #region General Menu
        private WindowsElement mnuGeneral => menuStrip.FindElementByName("General") as WindowsElement;
        private WindowsElement mnuAccountRelatedSettings => mnuGeneral.FindElementByName("Account Related Settings") as WindowsElement;
        private WindowsElement mnuLocationTypes => mnuGeneral.FindElementByName("Location Types") as WindowsElement;
        private WindowsElement mnuUserSecurity => mnuGeneral.FindElementByName("User Security") as WindowsElement;

        #endregion

        #region Alarms Menu
        private WindowsElement mnuAlarms => frmMaintMain.FindElementByName("Alarms") as WindowsElement;
        private WindowsElement winAlarmFileWizard => frmMaintMain.FindElementByName("Alarm Call File Wizard") as WindowsElement;
        #endregion

        #region Forms Menu
        private WindowsElement mnuForms => session.FindElementByName("Forms");
        #endregion

        #region Special Actions
        private WindowsElement frmSpecialActions => session.FindElementByAccessibilityId("frmSpecialActions");
        #endregion

        #region Registration Account Settings Window
        private WindowsElement frmAROptions => session.FindElementByAccessibilityId("frmAROptions");

        #endregion

        #region Form Letters Window
        private WindowsElement frmLetter2 => session.FindElementByAccessibilityId("frmLetter2");
        #endregion

        #region Alarm Counting Period
        private WindowsElement frmFACnting => frmMaintMain.FindElementByAccessibilityId("frmFACnting") as WindowsElement;
        #endregion

        //Basic Interactions
        #region General Menu
        private void ClickGeneralMenu()
        {
            mnuGeneral.Click();
        }
        private void ClickGeneralSubMenu()
        {
            mnuGeneral.FindElementsByName("General")[1].Click();
        }
        private void ClickAccountRelatedSettings()
        {
            ClickGeneralMenu();
            ClickGeneralSubMenu();

            Actions actions = new Actions(session);
            actions.MoveToElement(mnuAccountRelatedSettings);
            actions.Click();
            actions.Perform();

            //windowsDriver.Keyboard.SendKeys("r");
        }
        private void ClickLocationTypes()
        {
            ClickGeneralMenu();
            ClickGeneralSubMenu();

            Actions actions = new Actions(session);
            actions.MoveToElement(mnuLocationTypes);
            actions.Click();
            actions.Perform();
        }
        private void ClickUserSecurity()
        {
            ClickGeneralMenu();

            Actions actions = new Actions(session);
            actions.MoveToElement(mnuUserSecurity);
            actions.Click();
            actions.Perform();
        }

        #endregion

        #region Alarms Menu
        private void ClickAlarmsMenu()
        {
            mnuAlarms.Click();
        }
        private void ClickAlarmFileWizardSettings()
        {
            mnuAlarms.SelectMenuItemByName("Alarm File Wizard", session);
        }
        private void ClickAlarmCountingPeriod()
        {
            mnuAlarms.SelectMenuItemByName("Alarm Counting Period (Selected Account's Agency)", session);
        }
        #endregion

        #region Forms Menu
        private void ClickAddEditLetters()
        {
            mnuForms.SelectMenuItemByName(@"Add/Edit Letters", session);
        }
        #endregion

        //Short functional methods
        public void OpenSetSpecialActions()
        {
            mnuGeneral.SelectMenuItemByName("Set Special Actions ", session);
        }
        public void OpenAccountRelatedSettings()
        {
            TestContext.WriteLine("Open Account Related Settings");
            ClickAccountRelatedSettings();
            if (!frmAROptions.Displayed)
            {
                throw new NotFoundException();
            }
        }
        public void OpenLocationTypesSettings()
        {
            TestContext.WriteLine("Open Location Type Settings");
            ClickLocationTypes();
        }
        public void OpenAlarmFileWizard()
        {
            ClickAlarmFileWizardSettings();
        }
        public void OpenFormLetters()
        {
            TestContext.WriteLine("Open Forms/Letters Settings");
            ClickAddEditLetters();
        }
        public void OpenAlarmCountingPeriod()
        {
            TestContext.WriteLine("Open Alarm Counting Period Settings");
            ClickAlarmCountingPeriod();
        }
        public bool LocationTypesNonAdmin()
        {
            ClickGeneralMenu();

            return !mnuGeneral.FindElementsByName("General")[1].Enabled;
        }
        public void OpenUserSecurity()
        {
            TestContext.WriteLine("Open User Security Settings");
            ClickUserSecurity();
        }
        public void Close()
        {
            frmMaintMain.Click();
            btnClose.Click();
        }
    }
}
