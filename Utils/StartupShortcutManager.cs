using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace aydocs.NotchWin.Utils
{
    public class StartupShortcutManager
    {
        private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "NotchWin";

        public static void CreateShortcut()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                if (string.IsNullOrEmpty(exePath)) return;

                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, writable: true);
                key?.SetValue(AppName, $"\"{exePath}\"");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[STARTUP] Failed to create startup entry: {ex.Message}");
            }
        }

        public static bool RemoveShortcut()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, writable: true);
                if (key?.GetValue(AppName) != null)
                {
                    key.DeleteValue(AppName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[STARTUP] Failed to remove startup entry: {ex.Message}");
            }
            return false;
        }
    }
}
