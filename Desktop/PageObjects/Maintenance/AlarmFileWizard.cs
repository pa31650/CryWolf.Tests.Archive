using OpenQA.Selenium.Appium.Windows;

namespace Desktop.PageObjects.Maintenance
{
    public class AlarmFileWizard
    {
        private readonly WindowsDriver<WindowsElement> session;
        private bool status = false;
        public string categoryName = "Alarm File Wizard";

        public AlarmFileWizard(WindowsDriver<WindowsElement> _session)
        {
           session = _session;
        }

        //Object Identification
        #region Main Page
        private WindowsElement winAlarmFileWizard => session.FindElementByName("Alarm Call File Wizard") as WindowsElement;
        private WindowsElement btnCloseAlarmFileWizard => winAlarmFileWizard.FindElementByName("Close") as WindowsElement;
        #endregion

        //Basic Interactions
        private void Close()
        {
            btnCloseAlarmFileWizard.Click();
        }

        //Short functional methods
        
        
    }
}
