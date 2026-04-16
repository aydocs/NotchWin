using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using aydocs.NotchWin.Main;
using aydocs.NotchWin.Utils;
using aydocs.NotchWin.UI.UIElements;

namespace aydocs.NotchWin.UI
{
    public partial class SettingsWindow : Window
    {
        private bool _isInitialized = false;

        public SettingsWindow()
        {
            InitializeComponent();
            _isInitialized = true;
            LoadGeneralSettings();
        }

        private void CategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (CategoryList.SelectedIndex == 0) LoadGeneralSettings();
            else if (CategoryList.SelectedIndex == 1) LoadAppearanceSettings();
            else if (CategoryList.SelectedIndex == 2) LoadWidgetSettings();
            else if (CategoryList.SelectedIndex == 3) LoadAboutSettings();
        }

        private void LoadGeneralSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("📋 General Settings");
            AddSeparator();

            AddSection("Startup");
            var startupCb = AddCheckbox("Start on Login", Settings.RunOnStartup);
            startupCb.Checked += (s, e) => Settings.RunOnStartup = true;
            startupCb.Unchecked += (s, e) => Settings.RunOnStartup = false;

            AddSection("Interface");
            var topmostCb = AddCheckbox("Keep interface always topmost", Settings.AlwaysTopmost);
            topmostCb.Checked += (s, e) => Settings.AlwaysTopmost = true;
            topmostCb.Unchecked += (s, e) => Settings.AlwaysTopmost = false;
            AddDescription("Window stays on top of all other windows");

            AddSection("Monitor Selection");
            var monitorSelector = new ComboBox
            {
                Style = FindResource("ModernComboBoxStyle") as Style,
                Margin = new Thickness(0, 5, 0, 15)
            };
            int monitorCount = MainForm.GetMonitorCount();
            for (int i = 0; i < monitorCount; i++)
            {
                var screen = System.Windows.Forms.Screen.AllScreens[i];
                string label = (i == 0) ? "Primary" : $"Monitor {i + 1}";
                var item = new ComboBoxItem
                {
                    Content = $"{label} ({screen.Bounds.Width}x{screen.Bounds.Height})",
                    Style = FindResource("ModernComboBoxItemStyle") as Style
                };
                monitorSelector.Items.Add(item);
            }
            monitorSelector.SelectedIndex = Math.Clamp(Settings.ScreenIndex, 0, monitorCount - 1);
            monitorSelector.SelectionChanged += (s, e) =>
            {
                Settings.ScreenIndex = monitorSelector.SelectedIndex;
                MainForm.Instance?.SetMonitor(Settings.ScreenIndex);
            };
            SettingsContent.Children.Add(monitorSelector);
            AddDescription("Choose which monitor to display the island on");

            AddSection("Island Mode");
            var modeSelector = new ComboBox
            {
                Style = FindResource("ModernComboBoxStyle") as Style,
                Margin = new Thickness(0, 5, 0, 0)
            };
            modeSelector.Items.Add(new ComboBoxItem { Content = "Island", Style = FindResource("ModernComboBoxItemStyle") as Style });
            modeSelector.Items.Add(new ComboBoxItem { Content = "Notch", Style = FindResource("ModernComboBoxItemStyle") as Style });
            modeSelector.SelectedIndex = Settings.IslandMode == IslandObject.IslandMode.Island ? 0 : 1;
            modeSelector.SelectionChanged += (s, e) =>
            {
                Settings.IslandMode = modeSelector.SelectedIndex == 0 ? IslandObject.IslandMode.Island : IslandObject.IslandMode.Notch;
            };
            SettingsContent.Children.Add(modeSelector);
            AddDescription("Choose between island or notch display style");
        }

        private void LoadAppearanceSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("🎨 Appearance Settings");
            AddSeparator();

            AddSection("Theme Selection");
            var themeSelector = new ComboBox
            {
                Style = FindResource("ModernComboBoxStyle") as Style,
                Margin = new Thickness(0, 5, 0, 15)
            };
            string[] themeOptions = { "Custom", "Dark", "Light", "Candy", "Forest Dawn", "Sunset Glow" };
            foreach (var t in themeOptions)
            {
                var item = new ComboBoxItem { Content = t, Style = FindResource("ModernComboBoxItemStyle") as Style };
                themeSelector.Items.Add(item);
            }
            themeSelector.SelectedIndex = Settings.Theme + 1;
            themeSelector.SelectionChanged += (s, e) =>
            {
                Settings.Theme = themeSelector.SelectedIndex - 1;
                aydocs.NotchWin.Utils.Theme.Instance.UpdateTheme(true);
            };
            SettingsContent.Children.Add(themeSelector);
            AddDescription("Select a theme or use your custom theme");

            AddSection("Visual Effects");
            var blurCb = AddCheckbox("Enable Blur Effect", Settings.AllowBlur);
            blurCb.Checked += (s, e) => Settings.AllowBlur = true;
            blurCb.Unchecked += (s, e) => Settings.AllowBlur = false;
            AddDescription("Applies blur effect to the island background");

            var animCb = AddCheckbox("Enable Animations", Settings.AllowAnimation);
            animCb.Checked += (s, e) => Settings.AllowAnimation = true;
            animCb.Unchecked += (s, e) => Settings.AllowAnimation = false;
            AddDescription("Enables smooth animations throughout the interface");

            var shadowCb = AddCheckbox("Island Shadow", Settings.ToggleIslandShadow);
            shadowCb.Checked += (s, e) => Settings.ToggleIslandShadow = true;
            shadowCb.Unchecked += (s, e) => Settings.ToggleIslandShadow = false;
            AddDescription("Shows shadow effect around the island");
        }

        private void LoadWidgetSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("📦 Widget Management");
            AddSeparator();
            AddDescription("Widget management will be improved in future updates. Currently using default configuration.");
        }

        private void LoadAboutSettings()
        {
            SettingsContent.Children.Clear();
            AddTitle("ℹ️ About NotchWin");
            AddSeparator();

            var versionText = new TextBlock
            {
                Text = $"Version {NotchWinMain.Version}",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 85, 153, 255)),
                Margin = new Thickness(0, 10, 0, 5),
                FontWeight = FontWeights.SemiBold
            };
            SettingsContent.Children.Add(versionText);

            AddDescription("Created and developed by aydocs");
            AddDescription("A modern Dynamic Island experience for Windows.");

            var separator = new Rectangle
            {
                Height = 1,
                Fill = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51)),
                Margin = new Thickness(0, 20, 0, 20)
            };
            SettingsContent.Children.Add(separator);

            AddDescription("NotchWin brings macOS-style Dynamic Island functionality to Windows, providing a modern and elegant way to display notifications, widgets, and system information.");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Save();
            NotchWinMain.UpdateStartup();
            this.Close();
        }

        // ============ Helper Methods ============

        private void AddTitle(string text)
        {
            var title = new TextBlock
            {
                Text = text,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10),
                Foreground = new SolidColorBrush(Colors.White)
            };
            SettingsContent.Children.Add(title);
        }

        private void AddSection(string text)
        {
            var section = new TextBlock
            {
                Text = text,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 20, 0, 12),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 85, 153, 255)),
                Opacity = 0.9
            };
            SettingsContent.Children.Add(section);
        }

        private void AddSeparator()
        {
            var separator = new Rectangle
            {
                Height = 1,
                Fill = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51)),
                Margin = new Thickness(0, 15, 0, 20)
            };
            SettingsContent.Children.Add(separator);
        }

        private void AddDescription(string text)
        {
            var description = new TextBlock
            {
                Text = text,
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 12),
                Foreground = new SolidColorBrush(Color.FromArgb(255, 166, 166, 166)),
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 1.4,
                Opacity = 0.85
            };
            SettingsContent.Children.Add(description);
        }

        private CheckBox AddCheckbox(string text, bool isChecked)
        {
            var cb = new CheckBox
            {
                Content = text,
                IsChecked = isChecked,
                Foreground = new SolidColorBrush(Colors.White),
                Style = FindResource("ModernCheckBoxStyle") as Style
            };
            SettingsContent.Children.Add(cb);
            return cb;
        }
    }
}
