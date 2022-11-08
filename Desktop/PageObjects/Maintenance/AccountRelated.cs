using Desktop.Libraries;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Collections.Generic;
using Utils;

namespace Desktop.PageObjects.Maintenance
{
    public class AccountRelated
    {
        private readonly WindowsDriver<WindowsElement> session;
        private bool status = false;
        public string categoryName = "Account Related";

        public AccountRelated(WindowsDriver<WindowsElement> driver)
        {
            this.session = driver;
        }

        //Object Identification
        #region Main Page Objects
        private WindowsElement frmAROptions => session.FindElementByAccessibilityId("frmAROptions");
        private WindowsElement btnClose => frmAROptions.FindElementByAccessibilityId("Close") as WindowsElement;
        private WindowsElement btnOK => frmAROptions.FindElementByAccessibilityId("btnOK") as WindowsElement;
        private WindowsElement cbAgency => frmAROptions.FindElementByAccessibilityId("cbAgency") as WindowsElement;
        #endregion

        #region Issue/Expiration Date Related
        private WindowsElement ckDoSetExpires => frmAROptions.FindElementByAccessibilityId("ckDoSetExpires") as WindowsElement;
        private WindowsElement opUseTodaysDate => frmAROptions.FindElementByAccessibilityId("opUseTodaysDate") as WindowsElement;
        private WindowsElement opUseTodaysAndLastDayOfMonth => frmAROptions.FindElementByAccessibilityId("opUseTodaysAndLastDayOfMonth") as WindowsElement;
        private WindowsElement opUse1stOfMonth => frmAROptions.FindElementByAccessibilityId("opUse1stOfMonth") as WindowsElement;
        private WindowsElement opUseSpecificDate => frmAROptions.FindElementByAccessibilityId("opUseSpecificDate") as WindowsElement;
        private WindowsElement ckDoSetIssued => frmAROptions.FindElementByAccessibilityId("ckDoSetIssued") as WindowsElement;
        private WindowsElement cbIssueMonth => frmAROptions.FindElementByAccessibilityId("cbIssueMonth") as WindowsElement;
        private WindowsElement cbIssueDay => frmAROptions.FindElementByAccessibilityId("cbIssueDay") as WindowsElement;
        private WindowsElement cbExpirationMonth => frmAROptions.FindElementByAccessibilityId("cbExpirationMonth") as WindowsElement;
        private WindowsElement cbExpirationDay => frmAROptions.FindElementByAccessibilityId("cbExpirationDay") as WindowsElement;
        private WindowsElement opUseFirstIssueLastExpire => frmAROptions.FindElementByName("opUseFirstIssueLastExpire") as WindowsElement;
        #endregion

        #region Other options
        private WindowsElement ckShowARPersonalFields => frmAROptions.FindElementByAccessibilityId("ckShowARPersonalFields") as WindowsElement;
        #endregion

        #region Copy to RP behavior
        private WindowsElement cbCopyToRPButtonBehavior => frmAROptions.FindElementByAccessibilityId("cbCopyToRPButtonBehavior") as WindowsElement;
        private WindowsElement btnCopyToRpOptn => cbCopyToRPButtonBehavior.FindElementByName("Open") as WindowsElement;
        #endregion

        //Basic Interactions
        private void EnableDoSetIssued()
        {
            if (!DoSetIssuedIsEnabled())
            {
                ckDoSetIssued.Click();
            }
        }

        private void DisableDoSetIssued()
        {
            if (DoSetIssuedIsEnabled())
            {
                ckDoSetIssued.Click();
            }
        }

        private bool DoSetIssuedIsEnabled()
        {
            return ckDoSetIssued.Selected;
        }

        private void ClickUseTodaysDate()
        {
            opUseTodaysDate.Click();
        }

        private bool GetUseTodaysDateIsSelected()
        {
            Library.TakeScreenShot(session, "Use Todays Date option",out string filePath);
            return opUseTodaysDate.Selected;
        }

        private void ClickUse1stOfMonth()
        {
            opUse1stOfMonth.Click();
        }

        private void ClickUseTodaysAndLastDayOfMonth()
        {
            opUseTodaysAndLastDayOfMonth.Click();
        }

        private void ClickUseSpecificDate()
        {
            opUseSpecificDate.Click();
        }
        
        private void SetIssueMonth(int month)
        {
            cbIssueMonth.SelectListItem(month.ToString());
        }

        private void SetIssueDay(int day)
        {
            cbIssueDay.SelectListItem(day.ToString());
        }

        private void SetExpirationMonth(int month)
        {
            cbExpirationMonth.SelectListItem(month.ToString());
        }

        private void SetExpirationDay(int day)
        {
            cbExpirationDay.SelectListItem(day.ToString());
        }

        private void EnableDoSetExpires()
        {
            if (!DoSetExpiresIsChecked())
            {
                ckDoSetExpires.Click();
            }
        }

        private void DisableDoSetExpires()
        {
            if (DoSetExpiresIsChecked())
            {
                ckDoSetExpires.Click();
            }
        }

        private bool DoSetExpiresIsChecked()
        {
            return ckDoSetExpires.Selected;
        }

        private void ClickUseFirstIssueLastExpire()
        {
            opUseFirstIssueLastExpire.Click();
        }

        private bool GetUseTodaysAndLastDayOfMonthIsSelected()
        {
            Library.TakeScreenShot(session, "Use 1st Day and Last Day option", out string filePath);
            return opUseTodaysAndLastDayOfMonth.Selected;
        }

        private bool GetUse1stOfMonthIsSelected()
        {
            Library.TakeScreenShot(session, "Use First Day of Current Month option", out string filePath);
            return opUse1stOfMonth.Selected;
        }

        private bool GetUseSpecificDateIsSelected()
        {
            Library.TakeScreenShot(session, "Use Specific Date option", out string filePath);
            return opUseSpecificDate.Selected;
        }

        private bool GetUseFirstIssueLastExpireIsSelected()
        {
            Library.TakeScreenShot(session, "Use First Issue Last Expire option",out string filePath);
            return opUseFirstIssueLastExpire.Selected;
        }

        private void ClickOk()
        {
            btnOK.Click();
        }

        private void SelectCopyToRp(string behavior)
        {
            ClickCopyToRpOpen();
            cbCopyToRPButtonBehavior.SelectListItemNoClick(behavior);
        }

        private bool ValidateCopyToRp(string expectedBehavior)
        {
            return cbCopyToRPButtonBehavior.Text.Contains(expectedBehavior);
        }

        private void ClickCopyToRpOpen()
        {
            btnCopyToRpOptn.Click();
        }

        private List<string> GetCopyToRpOptions()
        {
            List<string> behaviors;
            ClickCopyToRpOpen();
            cbCopyToRPButtonBehavior.GetListItemsNoClick(out behaviors);
            return behaviors;
        }

        private void ClickClose()
        {
            btnClose.Click();
        }

        public  void SelectAgency(string agency)
        {
            if (cbAgency.Text != agency)
            {
                cbAgency.SelectListItem(agency);
            }
        }

        //Short functional methods
        public void ShowPersonalARFields()
        {
            if (!ckShowARPersonalFields.Selected)
            {
                ckShowARPersonalFields.Click();
            }
        }

        public void HidePersonalARFields()
        {
            if (ckShowARPersonalFields.Selected)
            {
                ckShowARPersonalFields.Click();
            }
        }

        public void CommitAccountRelatedSettings()
        {
            ClickOk();
            
        }

        public bool SetIssueExpirationSettings(string v, string agency, int issueMonth = 0, int issueDay = 0, int expirationMonth = 0, int expirationDay = 0)
        {
            SelectAgency(agency);
            switch (v.Replace(" ",string.Empty).ToLower())
            {
                case "usetodaysdate":
                    ClickUseTodaysDate();

                    return GetUseTodaysDateIsSelected();
                case "usetodaysandlastdayofmonth":
                    ClickUseTodaysAndLastDayOfMonth();
                    return GetUseTodaysAndLastDayOfMonthIsSelected();
                case "use1stofmonth":
                    ClickUse1stOfMonth();
                    return GetUse1stOfMonthIsSelected();
                case "usespecificdate":
                    ClickUseSpecificDate();
                    if (issueMonth != 0 && issueDay != 0)
                    {
                        EnableDoSetIssued();
                        //may need wait
                        SetIssueMonth(issueMonth);
                        SetIssueDay(issueDay);
                    }
                    else //disable it if enabled
                    {
                        DisableDoSetIssued();
                    }
                    if (expirationMonth != 0 && expirationDay != 0)
                    {
                        EnableDoSetExpires();
                        //may need wait
                        SetExpirationMonth(expirationMonth);
                        SetExpirationDay(expirationDay);
                    }
                    else //disable it if enabled
                    {
                        DisableDoSetExpires();
                    }

                    return GetUseSpecificDateIsSelected();
                case "firstissuelastexpire":
                    ClickUseFirstIssueLastExpire();
                    return GetUseFirstIssueLastExpireIsSelected();
                default:
                    TestContext.WriteLine($"{v} is not coded for.");
                    break;
            }
            return false;
        }
        
        public void Close()
        {
            ClickClose();
        }

        public bool ConfigureCopyToRpOptions(string copyBehavior,string agency)
        {
            SelectAgency(agency);

            string behavior = "";
            bool status;
            switch (copyBehavior.Replace(" ",string.Empty).ToLower())
            {
                case "default":
                    behavior = "Copies name and address (default)";
                    break;
                case "hidden":
                    behavior = "Hidden";
                    break;
                case "nameresidential":
                    behavior = "Always copies address; name only for residential";
                    break;
                case "addressonly":
                    behavior = "Copies address only, never name";
                    break;
                default:
                    break;
            }

            SelectCopyToRp(behavior);

            status = ValidateCopyToRp(behavior);

            CommitAccountRelatedSettings();
            return status;

        }

        public bool ValidateCopyToRpOptions(List<string> expectedBehaviors)
        {
            List<string> actualBehaviors = GetCopyToRpOptions();

            foreach (var behavior in expectedBehaviors)
            {
                if (actualBehaviors.Contains(behavior))
                {
                    Console.Write($"Behavior: {behavior} found.");
                }
                else
                {
                    Console.Write($"Behavior: {behavior} was not found. Full list: {actualBehaviors}.");
                    return false;
                }
            }
            return true;
        }
    }
}
