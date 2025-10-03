using System.Windows.Media;

namespace ida_picker
{
    public class IdaInstallation
    {
        public string DisplayName { get; init; }
        public string Version { get; init; }
        public string InstallPath { get; init; }
        public ImageSource IconSource { get; set; }

        public override string ToString()
        {
            return $"{DisplayName} ({Version})";
        }
    }
}