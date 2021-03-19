namespace crossPlatformWebDriverDownloader.ChromeWebDriver
{
    internal class ChromeVersion
    {
        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Build { get; private set; }
        public int Patch { get; private set; }

        public ChromeVersion(string version)
        {
            if (!string.IsNullOrEmpty(version))
            {
                string[] versionIdentifiers = version.Split(".");

                //ToDo: Error Handling

                Major = int.Parse(versionIdentifiers[0]);
                Minor = int.Parse(versionIdentifiers[1]);
                Build = int.Parse(versionIdentifiers[2]);
                Patch = int.Parse(versionIdentifiers[3]);
            }
        }

        public string GetVersionExcludingPatchVersion()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Build);
        }

        public bool EqualsExcludingPatchVersion(ChromeVersion otherVersion)
        {
            return (Major == otherVersion.Major && Minor == otherVersion.Minor && Build == otherVersion.Build);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Patch);
        }
    }
}
