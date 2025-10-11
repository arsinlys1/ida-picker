using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ida_picker
{
    public partial class MainWindow
    {
        private readonly string _filePath;
        private List<IdaInstallation>? _idaInstallations;

        public MainWindow(string? filePath)
        {
            InitializeComponent();
            _filePath = filePath;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadIdaInstallations();
            
            if (_idaInstallations is { Count: 0 })
            {
                MessageBox.Show("No IDA installations found in the registry.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            
            if (string.IsNullOrEmpty(_filePath))
            {
                MessageBox.Show("No file path provided. Please specify a file path as a command-line argument.", 
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
                return;
            }
            
            IdaItemsControl.ItemsSource = _idaInstallations;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
                e.Handled = true;
                return;
            }

            int selectedIndex = -1;
            
            switch (e.Key)
            {
                case Key.D1:
                case Key.NumPad1:
                    selectedIndex = 0;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    selectedIndex = 1;
                    break;
                case Key.D3:
                case Key.NumPad3:
                    selectedIndex = 2;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    selectedIndex = 3;
                    break;
                case Key.D5:
                case Key.NumPad5:
                    selectedIndex = 4;
                    break;
                case Key.D6:
                case Key.NumPad6:
                    selectedIndex = 5;
                    break;
                case Key.D7:
                case Key.NumPad7:
                    selectedIndex = 6;
                    break;
                case Key.D8:
                case Key.NumPad8:
                    selectedIndex = 7;
                    break;
                case Key.D9:
                case Key.NumPad9:
                    selectedIndex = 8;
                    break;
            }

            if (selectedIndex >= 0 && _idaInstallations != null && selectedIndex < _idaInstallations.Count)
            {
                LaunchIda(_idaInstallations[selectedIndex]);
                e.Handled = true;
            }
        }

        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border { Tag: IdaInstallation installation })
            {
                LaunchIda(installation);
            }
        }

        private void LoadIdaInstallations()
        {
            _idaInstallations = new List<IdaInstallation>();
            var registry = new RegistryEnumerator();
            var installations = registry.FindIdaInstallations();
            
            _idaInstallations.AddRange(installations);
        }

        private void LaunchIda(IdaInstallation installation)
        {
            try
            {
                string idaExePath = Path.Combine(installation.InstallPath, "ida.exe");
                
                if (!File.Exists(idaExePath))
                {
                    idaExePath = Path.Combine(installation.InstallPath, "ida64.exe");
                }
                
                if (File.Exists(idaExePath))
                {
                    Process.Start(idaExePath, $"\"{_filePath}\"");
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show($"Could not find ida.exe or ida64.exe in {installation.InstallPath}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching IDA: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
