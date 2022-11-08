using OpenQA.Selenium.Appium.Windows;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Security;

namespace Utils
{
    
    /// <summary>
    /// Handles processes to create conditions unavailable via selenium, such as disabling network access
    /// </summary>
    class ProcessHandler
    {
        public const String HostName = null;

        private static void CreateCommandlineProcess(string arguments)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var process = Process.Start(startInfo);
            process.WaitForExit(TimeSpan.FromMinutes(3).Milliseconds);
        }

        /// <summary>
        /// Executes the command: 'ipconfig /release' to disable network access
        /// </summary>
        public static void ReleaseIPConfig()
        {
            CreateCommandlineProcess("/C ipconfig /release");

            ReportBuilder.ArrayBuilder("Interrupted network connectivity", true, "Disable Network Access");
        }
        /// <summary>
        /// Executes the command: 'ipconfig /renew' to re-enable network access
        /// </summary>
        public static void RenewIPConfig()
        {
            CreateCommandlineProcess("/C ipconfig /renew");

            ReportBuilder.ArrayBuilder("Restored network connectivity", true, "Enable Network Access");
        }

        public static void CloseChromeDriverProcesses()
        {
            Process[] chromedriverProcesses =  Process.GetProcessesByName("chromedriver");
            foreach (Process chromedriverProcess in chromedriverProcesses)
                chromedriverProcess.Kill();
        }

        public static void CloseProcessesByName(string processName)
        {
            Console.WriteLine($"Closing Process {processName}");
            
            Process[] Processes = Process.GetProcessesByName(processName);
            foreach (Process process in Processes)
                process.Kill();
        }
        public static void CloseRemoteProcessesByName(string processName,string machineName)
        {
            
            string password = "N0rthl1n30";
            SecureString secureString = new SecureString();
            for (int i = 0; i < password.Length; i++)
            {
                secureString.AppendChar(password[i]);
            }
            secureString.MakeReadOnly();
            PSCredential pSCredential = new PSCredential(@"crywolf-user\Automation", secureString);
            ScriptBlock scriptBlock = ScriptBlock.Create($"Stop-Process -Name {processName} -Force;");
            PowerShell shell = PowerShell.Create().AddCommand("Invoke-Command").AddParameter("ComputerName", machineName).AddParameter("Credential",pSCredential).AddParameter("ScriptBlock", scriptBlock);
            shell.Invoke();
        }

        public static string GetMachineName(WindowsDriver<WindowsElement> session)
        {
            string url = $"{CommonTestSettings.SeleniumApiUrl}/testsession?session=" + session.SessionId;
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(reader.ReadLine());
            string host = new Uri(jObject.GetValue("proxyId").ToString()).Host;
            stream.Close();

            return host;
        }
        
        /// <summary>
        /// Setup cannot create a new ChromeDriver if the host app is still running.
        /// Kills the process if it's still running.
        /// </summary>
        //public static void EndHostAppProcess()
        //{
        //    var hostAppProcesses = GetHostAppProcesses();
        //    foreach (var hostAppProcess in hostAppProcesses)
        //        hostAppProcess.Kill();
        //}
        //public static bool IsHostAppProcessRunning()
        //{
        //    var hostAppProcesses = GetHostAppProcesses();
        //    return hostAppProcesses.Length > 0;
        //}
        //private static Process[] GetHostAppProcesses()
        //{
        //    //string pathToExecutable = ConfigurationManager.AppSettings.Get(HostName);
        //    //string processName = Path.GetFileNameWithoutExtension(pathToExecutable);

        //    //return Process.GetProcessesByName(processName);
        //}
        
        public static void EndAllProcessesByName(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (Process process in processes)
                process.Kill();

            System.Threading.Thread.Sleep(1000);
        }
    }
}
