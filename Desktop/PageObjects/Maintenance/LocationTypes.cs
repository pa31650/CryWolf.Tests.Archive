using Desktop.Libraries;
using OpenQA.Selenium.Appium.Windows;
using Utils;

namespace Desktop.PageObjects.Maintenance
{
    public class LocationTypes
    {
        private readonly WindowsDriver<WindowsElement> session;
        private bool status = false;
        public string categoryName = "CryWolf Maintenance";

        public LocationTypes(WindowsDriver<WindowsElement> _session)
        {
            session = _session;
        }

        //Object Identification
        private WindowsElement frmLocations => session.FindElementByAccessibilityId("frmLocations");
        private WindowsElement btnClose => frmLocations.FindElementByAccessibilityId("Close") as WindowsElement;
        private WindowsElement btnOK => frmLocations.FindElementByAccessibilityId("btnOK") as WindowsElement;
        private WindowsElement grdLocations => frmLocations.FindElementByAccessibilityId("grd") as WindowsElement;
        private WindowsElement cbAgency => frmLocations.FindElementByAccessibilityId("cbAgency") as WindowsElement;
        
        //Basic Interactions
        private void SelectAgency(string agency)
        {
            if (!cbAgency.Text.Equals(agency))
            {
                cbAgency.SelectListItem(agency);
            }
        }
        private void ClickOK()
        {
            btnOK.Click();
        }
        private void ClickClose()
        {
            btnClose.Click();
        }

        //Short functional methods
        public void Close()
        {
            ClickClose();
        }
        public void CommitLocationTypes()
        {
            ClickOK();
        }
        public bool ArchiveLocationType(string locationType,string agency)
        {
            SelectAgency(agency);
            WindowsElement ckActive = grdLocations.GetCell(grdLocations.GetRowWithCellText(locationType), grdLocations.GetColumnWithCellText("Active"));
            if (ckActive.Text == "True")
            {
                ckActive.Click();
            }
            Library.TakeScreenShot(session, "Validate Location Type Archival", out string path);

            return ckActive.Text == "False";
        }
        public bool UnArchiveLocationType(string locationType,string agency)
        {
            SelectAgency(agency);
            WindowsElement ckActive = grdLocations.GetCell(grdLocations.GetRowWithCellText(locationType), grdLocations.GetColumnWithCellText("Active"));

            if (ckActive.Text == "False")
            {
                ckActive.Click();
            }
            Library.TakeScreenShot(session, "Validate Location Type Archival", out string path);

            return ckActive.Text == "True";
        }
    }
}
