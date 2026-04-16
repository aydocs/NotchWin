using aydocs.NotchWin.Resources;
using aydocs.NotchWin.UI.Menu.Menus;
using aydocs.NotchWin.UI.UIElements;
using aydocs.NotchWin.UI.Widgets;
using aydocs.NotchWin.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace aydocs.NotchWin.Main
{
    public class Settings
    {
        private static IslandObject.IslandMode islandMode;
        private static bool allowBlur;
        private static bool allowAnimation;
        private static bool antiAliasing;
        private static bool toggleHighRefreshRate;
        private static bool limitRefreshRateWhenIdle;
        private static bool toggleIslandShadow;
        private static bool toggleHomeMenuShadow;
        private static bool runOnStartup;
        private static int theme;
        private static int activeScreenIndex;
        private static bool alwaysTopmost;
        private static float islandWidthScale = 1.0f;

        public static IslandObject.IslandMode IslandMode { get => islandMode; set => islandMode = value; }
        public static bool AllowBlur { get => allowBlur; set => allowBlur = value; }
        public static bool AllowAnimation { get => allowAnimation; set => allowAnimation = value; }
        public static bool AntiAliasing { get => antiAliasing; set => antiAliasing = value; }
        public static bool ToggleHighRefreshRate { get => toggleHighRefreshRate; set => toggleHighRefreshRate = value; }
        public static bool LimitRefreshRateWhenIdle { get => limitRefreshRateWhenIdle; set => limitRefreshRateWhenIdle = value; }
        public static bool ToggleIslandShadow { get => toggleIslandShadow; set => toggleIslandShadow = value; }
        public static bool ToggleHomeMenuShadow { get => toggleHomeMenuShadow; set => toggleHomeMenuShadow = value; }
        public static bool RunOnStartup { get => runOnStartup; set => runOnStartup = value; }
        public static int Theme { get => theme; set => theme = value; }
        public static int ScreenIndex { get => activeScreenIndex; set => activeScreenIndex = value; }
        public static float IslandWidthScale { get => islandWidthScale; set => islandWidthScale = Math.Clamp(value, 0.5f, 2.5f); }
        public static bool AlwaysTopmost
        {
            get => alwaysTopmost;
            set
            {
                alwaysTopmost = value;

                try
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                if (MainForm.Instance != null)
                                    WindowPositionHelper.CenterWindowOnMonitor(MainForm.Instance, ScreenIndex);
                            }
                            catch { }
                        }));
                    }
                    else
                    {
                        if (MainForm.Instance != null)
                            WindowPositionHelper.CenterWindowOnMonitor(MainForm.Instance, ScreenIndex);
                    }
                }
                catch { }
            }
        }

        public static List<string> smallWidgetsLeft;
        public static List<string> smallWidgetsRight;
        public static List<string> smallWidgetsMiddle;
        public static List<string> bigWidgets;

        private static HomeMenu.BigMenuMode defaultBigMenuMode = HomeMenu.BigMenuMode.Widgets;
        public static HomeMenu.BigMenuMode DefaultBigMenuMode
        {
            get => defaultBigMenuMode;
            set => defaultBigMenuMode = value;
        }

        public static void InitializeSettings()
        {
            try
            {


                if (SaveManager.Contains("settings"))
                {
                    IslandMode = (SaveManager.GetValue<int>("settings.islandmode") == 0) ? IslandObject.IslandMode.Island : IslandObject.IslandMode.Notch;

                    AllowBlur = SaveManager.GetValue<bool>("settings.allowblur", true);
                    AllowAnimation = SaveManager.GetValue<bool>("settings.allowanimation", true);
                    AntiAliasing = SaveManager.GetValue<bool>("settings.antialiasing", true);
                    ToggleHighRefreshRate = SaveManager.GetValue<bool>("settings.ToggleHighRefreshRate", true);
                    LimitRefreshRateWhenIdle = SaveManager.GetValue<bool>("settings.LimitRefreshRateWhenIdle", false);
                    ToggleIslandShadow = SaveManager.GetValue<bool>("settings.ToggleIslandShadow", true);
                    ToggleHomeMenuShadow = SaveManager.GetValue<bool>("settings.ToggleHomeMenuShadow", true);

                    RunOnStartup = SaveManager.GetValue<bool>("settings.runonstartup", false);

                    AlwaysTopmost = SaveManager.GetValue<bool>("settings.AlwaysTopmost", true);

                    IslandWidthScale = SaveManager.GetValue<float>("settings.IslandWidthScale", 1.0f);

                    Theme = SaveManager.GetValue<int>("settings.theme", 0);
                    ScreenIndex = SaveManager.GetValue<int>("settings.screenindex", 0);

                    if (SaveManager.Contains("settings.DefaultBigMenuMode"))
                    {
                        int mode = SaveManager.GetValue<int>("settings.DefaultBigMenuMode");
                        if (Enum.IsDefined(typeof(HomeMenu.BigMenuMode), mode))
                            DefaultBigMenuMode = (HomeMenu.BigMenuMode)mode;
                        else
                            DefaultBigMenuMode = HomeMenu.BigMenuMode.Widgets;
                    }
                    else
                    {
                        DefaultBigMenuMode = HomeMenu.BigMenuMode.Widgets;
                    }

                    Settings.smallWidgetsLeft = new List<string>();
                    Settings.smallWidgetsRight = new List<string>();
                    Settings.smallWidgetsMiddle = new List<string>();
                    Settings.bigWidgets = new List<string>();

                    var swLeftArr = SaveManager.GetValue<JArray>("settings.smallwidgetsleft");
                    var swRightArr = SaveManager.GetValue<JArray>("settings.smallwidgetsright");
                    var swMiddleArr = SaveManager.GetValue<JArray>("settings.smallwidgetsmiddle");
                    var bwArr = SaveManager.GetValue<JArray>("settings.bigwidgets");

                    if (swLeftArr != null)
                        foreach (var x in swLeftArr) Settings.smallWidgetsLeft.Add(x.ToString());
                    if (swRightArr != null)
                        foreach (var x in swRightArr) Settings.smallWidgetsRight.Add(x.ToString());
                    if (swMiddleArr != null)
                        foreach (var x in swMiddleArr) Settings.smallWidgetsMiddle.Add(x.ToString());
                    if (bwArr != null)
                        foreach (var x in bwArr) Settings.bigWidgets.Add(x.ToString());

                    // If lists are still empty after loading (e.g. key missing or empty array), load defaults
                    if (Settings.smallWidgetsLeft.Count == 0 && Settings.smallWidgetsRight.Count == 0 && Settings.smallWidgetsMiddle.Count == 0)
                    {
                        LoadDefaultWidgets();
                    }
                }
                else
                {
                    LoadDefaultWidgets();

                    IslandMode = IslandObject.IslandMode.Island;
                    AllowBlur = true;
                    AllowAnimation = true;
                    AntiAliasing = true;
                    ToggleHighRefreshRate = false;
                    LimitRefreshRateWhenIdle = true;
                    ToggleIslandShadow = true;
                    ToggleHomeMenuShadow = false;
                    AlwaysTopmost = true;

                    Theme = 0;

                    SaveManager.SaveData.Add("settings", 1);
                }


                // This must be run after loading all settings
                AfterSettingsLoaded();
            }catch(Exception e)
            {
                MessageBox.Show("An error occurred trying to load the settings. Please revert back to the default settings by deleting the \"Settings.json\" file located under \"%appdata%/aydocs.NotchWin/\".");

                LoadDefaultWidgets();

                AfterSettingsLoaded();
            }
        }

        private static void LoadDefaultWidgets()
        {
            smallWidgetsLeft = new List<string>();
            smallWidgetsRight = new List<string>();
            smallWidgetsMiddle = new List<string>();
            bigWidgets = new List<string>();

            smallWidgetsRight.Add("aydocs.NotchWin.UI.Widgets.Small.RegisterSmallVisualiserWidget");
            smallWidgetsRight.Add("aydocs.NotchWin.UI.Widgets.Small.RegisterNetworkWidget");
            smallWidgetsRight.Add("aydocs.NotchWin.UI.Widgets.Small.RegisterFinanceWidget");
            smallWidgetsLeft.Add("aydocs.NotchWin.UI.Widgets.Small.RegisterMediaThumbnailWidget");
            smallWidgetsMiddle.Add("aydocs.NotchWin.UI.Widgets.Small.RegisterCalendarWidget");
            bigWidgets.Add("aydocs.NotchWin.UI.Widgets.Big.RegisterWeatherWidget");
            bigWidgets.Add("aydocs.NotchWin.UI.Widgets.Big.RegisterTimerWidget");
            bigWidgets.Add("aydocs.NotchWin.UI.Widgets.Big.RegisterTasksWidget");
        }

        static void AfterSettingsLoaded()
        {
            aydocs.NotchWin.Utils.Theme.Instance.UpdateTheme();

            var customOptions = LoadCustomOptions();

            foreach (var item in customOptions)
            {
                item.LoadSettings();
            }
        }

        private static List<IRegisterableSetting>? _cachedCustomOptions;
        public static List<IRegisterableSetting> LoadCustomOptions()
        {
            if (_cachedCustomOptions != null) return _cachedCustomOptions;

            _cachedCustomOptions = new List<IRegisterableSetting>();

            var registerableSettings = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IRegisterableSetting).IsAssignableFrom(p) && p.IsClass);

            foreach (var option in registerableSettings)
            {
                try
                {
                    var optionInstance = (IRegisterableSetting)Activator.CreateInstance(option)!;
                    _cachedCustomOptions.Add(optionInstance);
                }
                catch { }
            }

            var dirPath = System.IO.Path.Combine(SaveManager.SavePath, "Extensions");

            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            else
            {
                foreach (var file in System.IO.Directory.GetFiles(dirPath))
                {
                    if (System.IO.Path.GetExtension(file).ToLower().Equals(".dll"))
                    {
                        try
                        {
                            var DLL = System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(dirPath, file));
                            var dllRegisterableSettings = DLL.GetTypes()
                                .Where(p => typeof(IRegisterableSetting).IsAssignableFrom(p) && p.IsClass);

                            foreach (var option in dllRegisterableSettings)
                            {
                                var optionInstance = (IRegisterableSetting)Activator.CreateInstance(option)!;
                                _cachedCustomOptions.Add(optionInstance);
                            }
                        }
                        catch { }
                    }
                }
            }

            return _cachedCustomOptions;
        }

        public static void Save()
        {
            SaveManager.Add("settings.islandmode", (IslandMode == IslandObject.IslandMode.Island) ? 0 : 1);

            SaveManager.Add("settings.allowblur", AllowBlur);
            SaveManager.Add("settings.allowanimation", AllowAnimation);
            SaveManager.Add("settings.antialiasing", AntiAliasing);
            SaveManager.Add("settings.ToggleHighRefreshRate", ToggleHighRefreshRate);
            SaveManager.Add("settings.LimitRefreshRateWhenIdle", LimitRefreshRateWhenIdle);
            SaveManager.Add("settings.ToggleIslandShadow", ToggleIslandShadow);
            SaveManager.Add("settings.ToggleHomeMenuShadow", ToggleHomeMenuShadow);
            SaveManager.Add("settings.runonstartup", RunOnStartup);
            SaveManager.Add("settings.AlwaysTopmost", AlwaysTopmost);
            SaveManager.Add("settings.IslandWidthScale", IslandWidthScale);

            SaveManager.Add("settings.theme", Theme);
            SaveManager.Add("settings.screenindex", ScreenIndex);

            SaveManager.Add("settings.smallwidgetsleft", smallWidgetsLeft);
            SaveManager.Add("settings.smallwidgetsright", smallWidgetsRight);
            SaveManager.Add("settings.smallwidgetsmiddle", smallWidgetsMiddle);
            SaveManager.Add("settings.bigwidgets", bigWidgets);

            SaveManager.Add("settings.DefaultBigMenuMode", (int)DefaultBigMenuMode);

            SaveManager.SaveAll();
        }
    }
}
