using System.Windows;

namespace ida_picker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        string? filePath = null;
            
        if (e.Args.Length > 0)
        {
            filePath = e.Args[0];
        }
            
        MainWindow mainWindow = new MainWindow(filePath);
        mainWindow.Show();
    }
}

