using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Utils
{
    public class SetUp
    {
        private static Process _driver;

        public IWebDriver Setup(string runLocation)
        {
            IWebDriver driver;
            
            // Set arguments and preferences to allow automation and prevent interruptions
            ChromeOptions options = new ChromeOptions();
            
            options.AddArguments(new List<string>() { "enable-automation", "--disable-infobars", "--start-maximized" });
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            if (TestContext.CurrentContext.Test.Name.Contains("_mobile"))
            {
                options.EnableMobileEmulation(TestContext.CurrentContext.Test.Arguments.GetValue(1).ToString());
            }

            try
            {
                if (runLocation == "local")
                {
                    driver = new ChromeDriver(options);
                }
                else
                {
                    driver = new RemoteWebDriver(new Uri(CommonTestSettings.SeleniumHubUrl), options.ToCapabilities());
                }
            }
            catch (Exception e)
            {
                ReportBuilder.ArrayBuilder(e.Message, false, Library.GetCurrentMethod());
                throw;
            }
            
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(CommonTestSettings.PAGE_TIMEOUT));

            return driver;
        }

        public void StartWinAppDriver()
        {
            try
            {
                ProcessStartInfo startinfo = new ProcessStartInfo();
                startinfo.FileName = CommonTestSettings.WinAppDriverPath;// @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";
                _driver = Process.Start(startinfo);
                Console.WriteLine("WinAppDriver Started");
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not locate WinAppDriver.exe, get it from https://github.com/Microsoft/WinAppDriver/releases and change the winAppPath in app.settings accordingly");
                ReportBuilder.ArrayBuilder("Could not locate WinAppDriver.exe, get it from https://github.com/Microsoft/WinAppDriver/releases and change the winAppPath in app.settings accordingly", false, Library.GetCurrentMethod());
                throw new FileNotFoundException("Could not locate File WinAppDriver.exe", e);
            }
        }

        /// <summary>
        /// Clears library and member variables, quits driver, saves the end time, and reports final checkpoint based on the exit status
        /// </summary>
        public void Teardown(IWebDriver driver, bool exitStatus)
        {
            Teardown(driver);

            if (exitStatus)
                ReportBuilder.ArrayBuilder("Driver exited due to end of scenario", true, Library.GetCurrentMethod());
            else
                ReportBuilder.ArrayBuilder("Test was exited due to failure", false, Library.GetCurrentMethod());
        }

        /// <summary>
        /// Clears library and member variables, quits driver, saves the end time, and reports final checkpoint
        /// </summary>
        public void Teardown(IWebDriver driver)
        {                                               
            driver.Quit();
            ReportBuilder.ArrayBuilder("Successfully quit driver", true, Library.GetCurrentMethod());
            ReportBuilder.getEndTime();
        }

        public void WinAppTeardown(WindowsDriver<WindowsElement> windowsDriver, bool exitStatus)
        {
            //Teardown(windowsDriver);
            if (exitStatus)
                ReportBuilder.ArrayBuilder("Driver exited due to end of scenario", true, Library.GetCurrentMethod());
            else
                ReportBuilder.ArrayBuilder("Test was exited due to failure", false, Library.GetCurrentMethod());

            ReportBuilder.getEndTime();
        }

        public void Teardown(WindowsDriver<WindowsElement> windowsDriver)
        {
            windowsDriver.Quit();
            ReportBuilder.ArrayBuilder("Successfully quit driver", true, Library.GetCurrentMethod());
            ReportBuilder.getEndTime();
        }

        private void ClearMemberVariables()
        {
                        
        }
        
    }
}