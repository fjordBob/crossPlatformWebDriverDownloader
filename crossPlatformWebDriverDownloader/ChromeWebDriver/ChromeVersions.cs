using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace crossPlatformWebDriverDownloader.ChromeWebDriver
{

    public static class ChromeVersions
    {
        private static HttpClient httpClient = new HttpClient();

        internal static ChromeVersion LocalChrome
        {
            get
            {
                return GetLocalChromeVersion();
            }
        }

        internal static ChromeVersion LocalChromeDriver
        {
            get
            {
                return GetLocalChromeDriverVersion();
            }
        }

        internal static ChromeVersion GetChromeDriverVersionByChromeVersion(ChromeVersion chromeVersion)
        {
            string chromeDriverVersionUrl = "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_" + chromeVersion.GetVersionExcludingPatchVersion();

            Console.WriteLine("Getting latest version of chromdriver.exe for chrome version {0} from {1}", chromeVersion, chromeDriverVersionUrl);

            HttpResponseMessage response = httpClient.GetAsync(chromeDriverVersionUrl).Result;

            string chromeDriverVersion = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine("Found chromdriver version {0} for chrome version {1}", chromeDriverVersion, chromeVersion);

            return new ChromeVersion(chromeDriverVersion);
        }

        private static ChromeVersion GetLocalChromeVersion()
        {
            ChromeVersion retVal = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string chromePathProgramFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) +
                                            "\\Google\\Chrome\\Application\\chrome.exe";
                string chromePathProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                                            "\\Google\\Chrome\\Application\\chrome.exe";
                
                string chromePath = File.Exists(chromePathProgramFiles) ? chromePathProgramFiles : chromePathProgramFilesX86;
                if (File.Exists(chromePath))
                {
                    retVal = new ChromeVersion(FileVersionInfo.GetVersionInfo(chromePath).FileVersion);
                }
            }

            return retVal;
        }

        private static ChromeVersion GetLocalChromeDriverVersion()
        {
            ChromeVersion retVal = null;
            string chromeDriverPath = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                chromeDriverPath = Path.Combine(Environment.CurrentDirectory, ".\\chromedriver.exe");
            }

            if (File.Exists(chromeDriverPath))
            {
                Process p = new Process();

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = chromeDriverPath;
                p.StartInfo.Arguments = "--version";
                p.Start();

                string[] output = p.StandardOutput.ReadToEnd().Split(" ");
                p.WaitForExit();

                retVal = new ChromeVersion(output[1]);
            }

            return retVal;
        }
    }
}
