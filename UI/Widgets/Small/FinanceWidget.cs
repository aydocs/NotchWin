using aydocs.NotchWin.Resources;
using aydocs.NotchWin.UI.UIElements;
using aydocs.NotchWin.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aydocs.NotchWin.UI.Widgets.Small
{
    class RegisterFinanceWidget : IRegisterableWidget
    {
        public bool IsSmallWidget => true;
        public string WidgetName => "Finance Ticker";

        public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        {
            return new FinanceWidget(parent, position, alignment);
        }
    }

    class RegisterFinanceWidgetSettings : IRegisterableSetting
    {
        public string SettingID => "financewidget";
        public string SettingTitle => "Finance Ticker";

        public static FinanceWidgetSave saveData;

        public struct FinanceWidgetSave
        {
            public string symbol;
            public float price;
            public float changePercent;
            public long lastUpdate;

            public FinanceWidgetSave()
            {
                symbol = "BTC";
                price = 45000f;
                changePercent = 2.5f;
                lastUpdate = DateTime.Now.Ticks;
            }
        }

        public void LoadSettings()
        {
            if (SaveManager.Contains(SettingID))
            {
                saveData = JsonConvert.DeserializeObject<FinanceWidgetSave>((string)SaveManager.Get(SettingID));
            }
            else
            {
                saveData = new FinanceWidgetSave()
                {
                    symbol = "BTC",
                    price = 45000f,
                    changePercent = 2.5f,
                    lastUpdate = DateTime.Now.Ticks
                };
            }
        }

        public void SaveSettings()
        {
            SaveManager.Add(SettingID, JsonConvert.SerializeObject(saveData));
        }

        public List<UIObject> SettingsObjects()
        {
            var objects = new List<UIObject>();

            var symbolLabel = new DWText(null, "Symbol:", new Vec2(25, 0), UIAlignment.TopLeft)
            {
                TextSize = 12,
                Font = Resources.Res.SFProRegular,
                Color = Theme.TextMain
            };
            objects.Add(symbolLabel);

            return objects;
        }
    }

    public class FinanceWidget : SmallWidgetBase
    {
        DWText symbolText;
        DWText priceText;
        DWText changeText;

        private float updateTimer = 0f;
        private const float UPDATE_INTERVAL = 30.0f; // Update every 30 seconds

        public FinanceWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
        {
            // Initialize saveData if not already loaded
            try
            {
                if (RegisterFinanceWidgetSettings.saveData.symbol == null)
                {
                    RegisterFinanceWidgetSettings.saveData = new RegisterFinanceWidgetSettings.FinanceWidgetSave()
                    {
                        symbol = "BTC",
                        price = 45000f,
                        changePercent = 2.5f,
                        lastUpdate = DateTime.Now.Ticks
                    };
                }
            }
            catch
            {
                RegisterFinanceWidgetSettings.saveData = new RegisterFinanceWidgetSettings.FinanceWidgetSave()
                {
                    symbol = "BTC",
                    price = 45000f,
                    changePercent = 2.5f,
                    lastUpdate = DateTime.Now.Ticks
                };
            }

            // Symbol text (BTC, AAPL, etc.)
            symbolText = new DWText(this, "BTC", new Vec2(-15, -5), UIAlignment.Center);
            symbolText.TextSize = 10;
            symbolText.Font = Res.SFProBold;
            symbolText.Color = Theme.TextSecond;
            AddLocalObject(symbolText);

            // Price text
            priceText = new DWText(this, "$45K", new Vec2(-15, 6), UIAlignment.Center);
            priceText.TextSize = 11;
            priceText.Font = Res.SFProRegular;
            priceText.Color = Theme.TextMain;
            AddLocalObject(priceText);

            // Change percentage
            changeText = new DWText(this, "+2.5%", new Vec2(15, 0), UIAlignment.Center);
            changeText.TextSize = 10;
            changeText.Font = Res.SFProBold;
            changeText.Color = Theme.Success;
            AddLocalObject(changeText);

            UpdateFinanceData();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            updateTimer += deltaTime;
            if (updateTimer >= UPDATE_INTERVAL)
            {
                UpdateFinanceData();
                updateTimer = 0f;
            }
        }

        private void UpdateFinanceData()
        {
            try
            {
                symbolText.Text = RegisterFinanceWidgetSettings.saveData.symbol;
                priceText.Text = FormatPrice(RegisterFinanceWidgetSettings.saveData.price);

                // Update change text and color
                float change = RegisterFinanceWidgetSettings.saveData.changePercent;
                changeText.Text = (change >= 0 ? "+" : "") + change.ToString("F1") + "%";
                changeText.Color = change >= 0 ? Theme.Success : Theme.Error;
            }
            catch
            {
                symbolText.Text = "N/A";
                priceText.Text = "--";
                changeText.Text = "0%";
            }
        }

        private string FormatPrice(float price)
        {
            if (price >= 1000)
            {
                return "$" + (price / 1000f).ToString("F0") + "K";
            }
            else if (price >= 1)
            {
                return "$" + price.ToString("F0");
            }
            else
            {
                return "$" + price.ToString("F2");
            }
        }

        protected override float GetWidgetWidth() { return 55; }
    }
}
