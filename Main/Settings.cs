using aydocs.NotchWin.Resources;
using aydocs.NotchWin.UI.Menu.Menus;
using aydocs.NotchWin.UI.UIElements;
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
                    IslandMode = ((Int64)SaveManager.Get("settings.islandmode") == 0) ? IslandObject.IslandMode.Island : IslandObject.IslandMode.Notch;

                    AllowBlur = (bool)SaveManager.Get("settings.allowblur");
                    AllowAnimation = (bool)SaveManager.Get("settings.allowanimtion");
                    AntiAliasing = (bool)SaveManager.Get("settings.antialiasing");
                    ToggleHighRefreshRate = SaveManager.Contains("settings.ToggleHighRefreshRate") ? (bool)SaveManager.Get("settings.ToggleHighRefreshRate") : true;
                    LimitRefreshRateWhenIdle = SaveManager.Contains("settings.LimitRefreshRateWhenIdle") ? (bool)SaveManager.Get("settings.LimitRefreshRateWhenIdle") : false;
                    ToggleIslandShadow = SaveManager.Contains("settings.ToggleIslandShadow") ? (bool)SaveManager.Get("settings.ToggleIslandShadow") : true;
                    ToggleHomeMenuShadow = SaveManager.Contains("settings.ToggleHomeMenuShadow") ? (bool)SaveManager.Get("settings.ToggleHomeMenuShadow") : true;

                    RunOnStartup = (bool)SaveManager.Get("settings.runonstartup");

                    AlwaysTopmost = SaveManager.Contains("settings.AlwaysTopmost") ? (bool)SaveManager.Get("settings.AlwaysTopmost") : true;

                    Theme = (int)((Int64)SaveManager.Get("settings.theme"));
                    ScreenIndex = (int)((Int64)SaveManager.Get("settings.screenindex"));

                    if (SaveManager.Contains("settings.DefaultBigMenuMode"))
                    {
                        int mode = (int)(Int64)SaveManager.Get("settings.DefaultBigMenuMode");
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

                    var smallWidgetsLeft = (JArray)SaveManager.Get("settings.smallwidgetsleft");
                    var smallWidgetsRight = (JArray)SaveManager.Get("settings.smallwidgetsright");
                    var smallWidgetsMiddle = (JArray)SaveManager.Get("settings.smallwidgetsmiddle");
                    var bigWidgets = (JArray)SaveManager.Get("settings.bigwidgets");

                    foreach (var x in smallWidgetsLeft)
                        Settings.smallWidgetsLeft.Add(x.ToString());
                    foreach (var x in smallWidgetsRight)
                        Settings.smallWidgetsRight.Add(x.ToString());
                    foreach (var x in smallWidgetsMiddle)
                        Settings.smallWidgetsMiddle.Add(x.ToString());
                    foreach (var x in bigWidgets)
                        Settings.bigWidgets.Add(x.ToString());
                }
                else
                {
                    smallWidgetsLeft = new List<string>();
                    smallWidgetsRight = new List<string>();
                    smallWidgetsMiddle = new List<string>();
                    bigWidgets = new List<string>();

                    smallWidgetsRight.Add("aydocs.NotchWin.UI.Widgets.Small.RegisterSmallVisualiserWidget");
                    smallWidgetsLeft.Add("aydocs.NotchWin.UI.Widgets.Small.RegisterMediaThumbnailWidget");
                    bigWidgets.Add("aydocs.NotchWin.UI.Widgets.Big.RegisterWeatherWidget");
                    bigWidgets.Add("aydocs.NotchWin.UI.Widgets.Big.RegisterTimerWidget");

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
                MessageBox.Show("An error occured trying to load the settings. Please revert back to the default settings by deleting the \"Settings.json\" file located under \"%appdata%/aydocs.NotchWin/\".");

                smallWidgetsLeft = new List<string>();
                smallWidgetsRight = new List<string>();
                smallWidgetsMiddle = new List<string>();
                bigWidgets = new List<string>();

                AfterSettingsLoaded();
            }
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
            SaveManager.Add("settings.allowanimtion", AllowAnimation);
            SaveManager.Add("settings.antialiasing", AntiAliasing);
            SaveManager.Add("settings.ToggleHighRefreshRate", ToggleHighRefreshRate);
            SaveManager.Add("settings.LimitRefreshRateWhenIdle", LimitRefreshRateWhenIdle);
            SaveManager.Add("settings.ToggleIslandShadow", ToggleIslandShadow);
            SaveManager.Add("settings.ToggleHomeMenuShadow", ToggleHomeMenuShadow);
            SaveManager.Add("settings.runonstartup", RunOnStartup);
            SaveManager.Add("settings.AlwaysTopmost", AlwaysTopmost);

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
