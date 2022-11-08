using OpenQA.Selenium.Appium.Windows;

namespace Desktop.PageObjects
{
    public class Root
    {
        private readonly WindowsDriver<WindowsElement> windowsDriver;

        public Root(WindowsDriver<WindowsElement> _session)
        {
            windowsDriver = _session;
        }

        //Object Identification
        private WindowsElement dlgStandardAlert => windowsDriver.FindElementByClassName(@"#32770");
        private WindowsElement txtAlertMessage => dlgStandardAlert.FindElementByTagName("Text") as WindowsElement;
        private WindowsElement btnOK => dlgStandardAlert.FindElementByName("OK") as WindowsElement;

        //Basic interactions
        private void ClickAlertOK()
        {
            btnOK.Click();
        }

        //Short functional methods
        public bool IsAlertDisplayed()
        {
            return dlgStandardAlert.Displayed;
        }
        public string GetAlertText()
        {
            return txtAlertMessage.Text;
        }
        public void AcceptAlert()
        {
            ClickAlertOK();
        }
    }
}
