using System;
using System.Windows;
using System.Windows.Controls;
using aydocs.NotchWin.Main;
using aydocs.NotchWin.Utils;
using aydocs.NotchWin.UI.UIElements;

namespace aydocs.NotchWin.UI
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadGeneralSettings();
        }

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryList.SelectedIndex == 0) LoadGeneralSettings();
            else if (CategoryList.SelectedIndex == 1) LoadAppearanceSettings();
            else if (CategoryList.SelectedIndex == 2) LoadWidgetSettings();
            else if (CategoryList.SelectedIndex == 3) LoadAboutSettings();
        }

        private void LoadGeneralSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("General Settings");

            var startupCb = AddCheckbox("Start on Login", Settings.RunOnStartup);
            startupCb.Checked += (s, e) => Settings.RunOnStartup = true;
            startupCb.Unchecked += (s, e) => Settings.RunOnStartup = false;

            var topmostCb = AddCheckbox("Keep interface always topmost", Settings.AlwaysTopmost);
            topmostCb.Checked += (s, e) => Settings.AlwaysTopmost = true;
            topmostCb.Unchecked += (s, e) => Settings.AlwaysTopmost = false;

            AddTitle("Selected Monitor");
            var monitorSelector = new ComboBox { Margin = new Thickness(0, 0, 0, 20), Padding = new Thickness(5) };
            int monitorCount = MainForm.GetMonitorCount();
            for (int i = 0; i < monitorCount; i++)
            {
                var screen = System.Windows.Forms.Screen.AllScreens[i];
                string label = (i == 0) ? "Primary" : $"Monitor {i + 1}";
                monitorSelector.Items.Add($"{label} ({screen.Bounds.Width}x{screen.Bounds.Height})");
            }
            monitorSelector.SelectedIndex = Math.Clamp(Settings.ScreenIndex, 0, monitorCount - 1);
            monitorSelector.SelectionChanged += (s, e) => {
                Settings.ScreenIndex = monitorSelector.SelectedIndex;
                MainForm.Instance?.SetMonitor(Settings.ScreenIndex);
            };
            SettingsContent.Children.Add(monitorSelector);

            AddTitle("Island Mode");
            var modeSelector = new ComboBox { Margin = new Thickness(0, 0, 0, 20), Padding = new Thickness(5) };
            modeSelector.Items.Add("Island");
            modeSelector.Items.Add("Notch");
            modeSelector.SelectedIndex = Settings.IslandMode == IslandObject.IslandMode.Island ? 0 : 1;
            modeSelector.SelectionChanged += (s, e) => {
                Settings.IslandMode = modeSelector.SelectedIndex == 0 ? IslandObject.IslandMode.Island : IslandObject.IslandMode.Notch;
            };
            SettingsContent.Children.Add(modeSelector);
        }

        private void LoadAppearanceSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("Appearance");

            AddTitle("Theme");
            var themeSelector = new ComboBox { Margin = new Thickness(0, 0, 0, 20), Padding = new Thickness(5) };
            string[] themeOptions = { "Custom", "Dark", "Light", "Candy", "Forest Dawn", "Sunset Glow" };
            foreach (var t in themeOptions) themeSelector.Items.Add(t);
            themeSelector.SelectedIndex = Settings.Theme + 1;
            themeSelector.SelectionChanged += (s, e) => {
                Settings.Theme = themeSelector.SelectedIndex - 1;
                aydocs.NotchWin.UI.UIElements.Theme.Instance.UpdateTheme(true);
            };
            SettingsContent.Children.Add(themeSelector);

            var blurCb = AddCheckbox("Enable Blur Effect", Settings.AllowBlur);
            blurCb.Checked += (s, e) => Settings.AllowBlur = true;
            blurCb.Unchecked += (s, e) => Settings.AllowBlur = false;

            var animCb = AddCheckbox("Enable Animations", Settings.AllowAnimation);
            animCb.Checked += (s, e) => Settings.AllowAnimation = true;
            animCb.Unchecked += (s, e) => Settings.AllowAnimation = false;

            var shadowCb = AddCheckbox("Island Shadow", Settings.ToggleIslandShadow);
            shadowCb.Checked += (s, e) => Settings.ToggleIslandShadow = true;
            shadowCb.Unchecked += (s, e) => Settings.ToggleIslandShadow = false;
        }

        private void LoadWidgetSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("Widget Management");
            AddDescription("Widget management will be improved in future updates. Currently using default configuration.");
        }

        private void LoadAboutSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("About NotchWin");
            AddDescription($"Version: {NotchWinMain.Version}");
            AddDescription("Created and developed by aydocs");
            AddDescription("A modern Dynamic Island experience for Windows.");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Save();
            NotchWinMain.UpdateStartup();
            this.Close();
        }

        // Helpers
        private void AddTitle(string text)
        {
            SettingsContent.Children.Add(new TextBlock { 
                Text = text, 
                FontSize = 18, 
                FontWeight = FontWeight.Bold, 
                Margin = new Thickness(0, 10, 0, 10),
                Foreground = System.Windows.Media.Brushes.White
            });
        }

        private void AddDescription(string text)
        {
            SettingsContent.Children.Add(new TextBlock { 
                Text = text, 
                FontSize = 14, 
                Margin = new Thickness(0, 0, 0, 10),
                Foreground = System.Windows.Media.Brushes.Gray,
                TextWrapping = TextWrapping.Wrap
            });
        }

        private CheckBox AddCheckbox(string text, bool isChecked)
        {
            var cb = new CheckBox { 
                Content = text, 
                IsChecked = isChecked, 
                Margin = new Thickness(0, 5, 0, 5),
                Foreground = System.Windows.Media.Brushes.White
            };
            SettingsContent.Children.Add(cb);
            return cb;
        }
    }
}
