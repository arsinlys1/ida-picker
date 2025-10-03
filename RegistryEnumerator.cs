using System.IO;
using Microsoft.Win32;

namespace ida_picker
{
    public class RegistryEnumerator
    {
        private readonly List<string> _registryPaths =
        [
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        ];

        public List<IdaInstallation> FindIdaInstallations()
        {
            var installations = new List<IdaInstallation>();

            FindInstallationsInHive(RegistryHive.LocalMachine, installations);
            FindInstallationsInHive(RegistryHive.CurrentUser, installations);

            return installations;
        }

        private void FindInstallationsInHive(RegistryHive hive, List<IdaInstallation> installations)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64))
            {
                foreach (var registryPath in _registryPaths)
                {
                    try
                    {
                        using (var uninstallKey = baseKey.OpenSubKey(registryPath))
                        {
                            if (uninstallKey == null) continue;

                            foreach (var subKeyName in uninstallKey.GetSubKeyNames())
                            {
                                using (var subKey = uninstallKey.OpenSubKey(subKeyName))
                                {
                                    if (subKey == null) continue;

                                    var displayName = subKey.GetValue("DisplayName")?.ToString();
                                    
                                    if (!string.IsNullOrEmpty(displayName) && 
                                        displayName.Contains("IDA", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var installLocation = subKey.GetValue("InstallLocation")?.ToString();
                                        
                                        if (string.IsNullOrEmpty(installLocation))
                                        {
                                            var installPath = subKey.GetValue("InstallPath")?.ToString();
                                            if (!string.IsNullOrEmpty(installPath))
                                            {
                                                installLocation = installPath;
                                            }
                                        }
                                        
                                        if (string.IsNullOrEmpty(installLocation))
                                        {
                                            var uninstallString = subKey.GetValue("UninstallString")?.ToString();
                                            if (!string.IsNullOrEmpty(uninstallString))
                                            {
                                                installLocation = ExtractInstallPath(uninstallString);
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(installLocation) && 
                                            Directory.Exists(installLocation))
                                        {
                                            if (File.Exists(Path.Combine(installLocation, "ida.exe")) ||
                                                File.Exists(Path.Combine(installLocation, "ida64.exe")))
                                            {
                                                var version = subKey.GetValue("DisplayVersion")?.ToString() ?? "Unknown";
                                                
                                                var installation = new IdaInstallation
                                                {
                                                    DisplayName = displayName,
                                                    Version = version,
                                                    InstallPath = installLocation,
                                                    IconSource = IconExtractor.ExtractIcon(installLocation)
                                                };

                                                installations.Add(installation);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Skip inaccessible registry keys
                    }
                }
            }
        }

        private string? ExtractInstallPath(string uninstallString)
        {
            try
            {
                var cleanPath = uninstallString.Replace("\"", "");
                var directory = Path.GetDirectoryName(cleanPath);
                return directory;
            }
            catch
            {
                return null;
            }
        }
    }
}
