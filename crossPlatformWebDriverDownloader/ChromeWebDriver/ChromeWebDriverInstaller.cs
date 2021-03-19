using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace crossPlatformWebDriverDownloader.ChromeWebDriver
{
    public class ChromeWebDriverInstaller
    {
        private static HttpClient httpClient = new HttpClient();

        private static void DownloadChromeDriver(ChromeVersion chromeVersion)
        {
            ChromeVersion chromeDriverVersion = ChromeVersions.GetChromeDriverVersionByChromeVersion(chromeVersion);

            string chromeDriverUrl = "";
            string chromeDriverZipFilePath = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                chromeDriverUrl = "https://chromedriver.storage.googleapis.com/" + chromeDriverVersion + "/chromedriver_win32.zip";
                chromeDriverZipFilePath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "chromedriver_win32.zip");
            }

            Console.WriteLine("Downloading chromedriver version {0} from {1} to local folder {2}", chromeDriverVersion, chromeDriverUrl, chromeDriverZipFilePath);

            using (HttpResponseMessage response = httpClient.GetAsync(chromeDriverUrl).Result)
            {
                response.EnsureSuccessStatusCode();

                using (Stream contentStream = response.Content.ReadAsStreamAsync().Result)
                {

                    string targetFile = chromeDriverZipFilePath;

                    using (FileStream fileStream = File.Create(targetFile))
                    {
                        contentStream.Seek(0, SeekOrigin.Begin);
                        contentStream.CopyTo(fileStream);
                    }
                }
            }

            Console.WriteLine("Extracting {0} to {1}", chromeDriverZipFilePath, Environment.CurrentDirectory);

            ZipFile.ExtractToDirectory(chromeDriverZipFilePath, ".", true);
        }

        public static void DownloadChromeDriver()
        {
            ChromeVersion localChromeVersion = ChromeVersions.LocalChrome;
            if(localChromeVersion == null)
            {
                throw new FileNotFoundException("No chrome installed. Please install chrome first.");
            }

            ChromeVersion localChromeDriverVersion = ChromeVersions.LocalChromeDriver;
            if (localChromeDriverVersion != null)
            {
                Console.WriteLine("chromedriver.exe already available in folder {0}. Checking version...", Environment.CurrentDirectory);

                if (localChromeVersion.EqualsExcludingPatchVersion(localChromeDriverVersion))
                {
                    Console.WriteLine("Version {0} of local chromedriver.exe fits to version {1} of chrome", localChromeDriverVersion, localChromeVersion);
                    return;
                }
                else
                {
                    Console.WriteLine("Version {0} of local chromedriver.exe does not fit to version {1} of chrome", localChromeDriverVersion, localChromeVersion);
                }
            }
            else
            {
                Console.WriteLine("No local version of chromedriver.exe found in folder {0}", Environment.CurrentDirectory);
            }

            DownloadChromeDriver(localChromeVersion);
        }
    }
}
