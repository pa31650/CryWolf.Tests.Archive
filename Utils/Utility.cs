//******************************************************************************
//
// Copyright (c) 2017 Microsoft Corporation. All rights reserved.
//
// This code is licensed under the MIT License (MIT).
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//******************************************************************************

using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;


namespace Utils
{
    public class Utility
    {
        private static WindowsDriver<WindowsElement> _session;

        private static Process _driver;
        private static ProcessStartInfo startinfo = new ProcessStartInfo();

        ~Utility(){}

        public static Process StopRDP()
        {
            //startinfo.FileName = App.Default.RDPSTOP_PATH;
            _driver = Process.Start(startinfo);
            return _driver;
        }
        public static Process StartRDP()
        {
            //startinfo.FileName = App.Default.RDPINIT_PATH;
            _driver = Process.Start(startinfo);
            return _driver;
        }

        #region WinAppDriver

        public static Process StartWinAppDriver()
        {
            Console.WriteLine($"Starting WinAppDriver @ {Utils.Default.WINAPPDRIVER_PATH}");

            try
            {
                //Start WinAppDriver
                startinfo.FileName = Utils.Default.WINAPPDRIVER_PATH;
                _driver = Process.Start(startinfo);

                Console.WriteLine("WinAppDriver Started");
            }catch(Exception e)
            {
                Console.WriteLine($"Could not locate WinAppDriver.exe @ {Utils.Default.WINAPPDRIVER_PATH}, get it from https://github.com/Microsoft/WinAppDriver/releases and change the winAppPath in app.settings accordingly");
                throw new FileNotFoundException("Could not locate File WinAppDriver.exe", e);
            }

            return _driver;
        }
        public static void StopWinAppDriver()
        {
            ProcessHandler.CloseProcessesByName("WinAppDriver");
        }
        #endregion

        #region WindowsDriver session management
        public static void SwitchWindow(string windowName, WindowsDriver<WindowsElement> windowsDriver)
        {
            var currentWindowHandle = windowsDriver.CurrentWindowHandle;
            var allWindowHandles = windowsDriver.WindowHandles;

            foreach (var windowHandle in allWindowHandles)
            {
                windowsDriver.SwitchTo().Window(windowHandle);
                if (windowsDriver.Title == windowName)
                {
                    var title = windowsDriver.Title;
                    break;
                }
            }
        }
        public static WindowsDriver<WindowsElement> CreateNewSession(string appId, string runLocation = "local",string argument = null)
        {
            Console.WriteLine($"Creating New Session App ID: {appId} / Location: {runLocation}");
            DesiredCapabilities appCapabilities = new DesiredCapabilities();

            appCapabilities.SetCapability("app", appId);
            appCapabilities.SetCapability("deviceName", "Windows");
            appCapabilities.SetCapability("newCommandTimeout", CommonTestSettings.NEW_COMMAND_TIMEOUT_REMOTE);

            if (argument != null)
            {
                appCapabilities.SetCapability("apparguments", argument);
            }

            switch (runLocation.ToLower())
            {
                case "local":
                    Console.WriteLine("Leaving CreateNewSession - local");
                    CommonTestSettings.dbHost = Utils.Default.DBHOST_LOCAL;
                    CommonTestSettings.dbName = Utils.Default.DBNAME_LOCAL;

                    _session = new WindowsDriver<WindowsElement>(new Uri(CommonTestSettings.WinAppDriverLocal), appCapabilities);
                    SetImplicitWait(_session, CommonTestSettings.PAGE_TIMEOUT);
                    
                    return _session;
                case "grid":
                    
                    Console.WriteLine($"Command Timeout set to {CommonTestSettings.NEW_COMMAND_TIMEOUT_REMOTE} seconds");
                    _session = new WindowsDriver<WindowsElement>(
                        new Uri(CommonTestSettings.WinAppDriverRemote), 
                        appCapabilities,
                        TimeSpan.FromSeconds(CommonTestSettings.NEW_COMMAND_TIMEOUT_REMOTE));
                    SetImplicitWait(_session, CommonTestSettings.PAGE_TIMEOUT_REMOTE);
                    
                    return _session;
                default:
                    Console.WriteLine($"Run Location :{runLocation} was not recognized");
                    return null;
            }
        }
        public static WindowsDriver<WindowsElement> CreateNewSession(string appTopLevelWindow)
        {
            Console.WriteLine($"Create New Session by Top Level Window: {appTopLevelWindow}");
            DesiredCapabilities appCapabilities = new DesiredCapabilities();
            appCapabilities.SetCapability("appTopLevelWindow", appTopLevelWindow);
            appCapabilities.SetCapability("deviceName", "Windows");

            switch (CommonTestSettings.RunLocation)
            {
                case "local":
                    return new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appCapabilities);
                case "grid":
                    return new WindowsDriver<WindowsElement>(new Uri(CommonTestSettings.WinAppDriverRemote), appCapabilities);
                    //return new WindowsDriver<WindowsElement>(new Uri(CommonTestSettings.WinAppDriverRemote), appCapabilities);
                default:
                    return null;
            }
        }
        //public static WindowsDriver<WindowsElement> CreateNewSession(string appID, string argument = null,string runLocation = null)
        //{
        //    int timeOut;
        //    _session = CreateNewSession(appID, argument,runLocation);

        //    runLocation == "local" ? timeOut = CommonTestSettings.PAGE_TIMEOUT : timeOut = CommonTestSettings.PAGE_TIMEOUT_REMOTE;
        //    SetImplicitWait(_session, timeOut);

        //    return _session;
        //}
        public static void SetImplicitWait(WindowsDriver<WindowsElement> session, int timeOut)
        {
            Console.WriteLine($"Implicit Wait Set to {timeOut} seconds ");
            session.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeOut));
        }
        public static void SetPageLoadTimeout(WindowsDriver<WindowsElement> session, int timeOut)
        {
            session.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(timeOut));
        }
        public static string GetWindowHandleByName(WindowsDriver<WindowsElement> deskTopSession, string elementName)
        {
            //Thread.Sleep(2500);
            int count = 0;

            if (deskTopSession.FindElementsByName(elementName).Count == 0)
            {
                do
                {
                    if (count > 5 && deskTopSession.FindElementsByName(elementName).Count == 0)
                    {
                        break;
                    }
                    Thread.Sleep(400);
                    count++;
                } while (deskTopSession.FindElementsByName(elementName).Count == 0);
            }

            var childWindow = deskTopSession.FindElementByName(elementName);
            var childWindowHandle = childWindow.GetAttribute("NativeWindowHandle");

            return childWindowHandle = (int.Parse(childWindowHandle)).ToString("x"); // Convert to Hex
        }
        public static string GetWindowHandleByAccId(WindowsDriver<WindowsElement> deskTopSession, string elementName)
        {
            //Thread.Sleep(2500);
            int count = 0;
            
            if (deskTopSession.FindElementsByAccessibilityId(elementName).Count == 0)
            {
                do
                {
                    if (count > 5 && deskTopSession.FindElementsByAccessibilityId(elementName).Count == 0)
                    {
                        break;
                    }
                    Thread.Sleep(400);
                    count++;
                } while (deskTopSession.FindElementsByAccessibilityId(elementName).Count == 0);
            }

            var childWindow = deskTopSession.FindElementByAccessibilityId(elementName);
            var childWindowHandle = childWindow.GetAttribute("NativeWindowHandle");

            return childWindowHandle = (int.Parse(childWindowHandle)).ToString("x"); // Convert to Hex
        }
        public static bool CurrentWindowIsAlive(WindowsDriver<WindowsElement> remoteSession)
        {
            bool windowIsAlive = false;

            if (remoteSession != null)
            {
                try
                {
                    windowIsAlive = !string.IsNullOrEmpty(remoteSession.CurrentWindowHandle) && remoteSession.CurrentWindowHandle != "0";
                    windowIsAlive = true;
                }
                catch { }
            }

            return windowIsAlive;
        }
        #endregion

        #region Other Utilites
        private void RunProcess(object sender, EventArgs e,string fileName,string path)
        {
            Process proc = null;
            try
            {
                string batDir = string.Format(path);
                proc = new Process();
                proc.StartInfo.WorkingDirectory = batDir;
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
                //MessageBox.Show("Bat file executed !!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }
        public static void KillExcelProcesses()
        {
            ProcessHandler.CloseProcessesByName("Microsoft Excel");
        }
        public static string getRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
        public static void HandleException(Exception exception, WindowsDriver<WindowsElement> windowsDriver, string testName, ReportBuilder reportBuilder)
        {
            if ((exception.InnerException is InvalidOperationException) || (exception.InnerException is WebDriverException))
            {
                throw exception;
            }
            else
            {
                bool exitStatus = false;
                Library.TakeScreenShot(windowsDriver, testName);
                ReportBuilder.ArrayBuilder(exception.ToString(), exitStatus, $"Exception: {exception.Message}");

                windowsDriver.Quit();
                //throw new AssertFailedException(exception.Message);
            }
        }
        public static void HandleException(Exception exception, IWebDriver webDriver, string testName, ReportBuilder reportBuilder)
        {
            if ((exception.InnerException is InvalidOperationException) ||
                (exception.InnerException is WebDriverException))
            {
                throw exception;
            }
            else
            {
                bool exitStatus = false;
                Library.TakeScreenShot(webDriver, testName);
                ReportBuilder.ArrayBuilder(exception.ToString(), exitStatus, $"Exception: {exception.Message}");
            }
        }
        #endregion
    }
}