using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ida_picker
{
    public enum AppTheme
    {
        Light,
        Dark
    }

    public static class ThemeHelper
    {
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string RegistryValueName = "AppsUseLightTheme";
        
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static AppTheme GetWindowsTheme()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
                var registryValueObject = key?.GetValue(RegistryValueName);
                
                if (registryValueObject == null)
                {
                    return AppTheme.Light;
                }
                
                int registryValue = (int)registryValueObject;
                return registryValue > 0 ? AppTheme.Light : AppTheme.Dark;
            }
            catch
            {
                return AppTheme.Light;
            }
        }

        public static void ApplyTheme(AppTheme theme)
        {
            var lightTheme = new ResourceDictionary 
            { 
                Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) 
            };
            
            var darkTheme = new ResourceDictionary 
            { 
                Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) 
            };

            Application.Current.Resources.MergedDictionaries.Clear();

            Application.Current.Resources.MergedDictionaries.Add(theme == AppTheme.Dark ? darkTheme : lightTheme);
        }

        public static void UpdateTitleBarTheme(Window window, AppTheme theme)
        {
            try
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd != IntPtr.Zero)
                {
                    int useImmersiveDarkMode = theme == AppTheme.Dark ? 1 : 0;
                    DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, 
                        ref useImmersiveDarkMode, sizeof(int));
                }
            }
            catch
            {
                // Ignore errors on older Windows versions
            }
        }

        public static void WatchForThemeChanges(Window window, Action<AppTheme> onThemeChanged)
        {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(window);
            source.AddHook((IntPtr _, int msg, IntPtr _, IntPtr lParam, ref bool _) =>
            {
                const int WM_SETTINGCHANGE = 0x001A;
                
                if (msg == WM_SETTINGCHANGE)
                {
                    string? paramName = Marshal.PtrToStringUni(lParam);
                    if (paramName == "ImmersiveColorSet")
                    {
                        var newTheme = GetWindowsTheme();
                        onThemeChanged(newTheme);
                    }
                }
                
                return IntPtr.Zero;
            });
        }
    }
}
