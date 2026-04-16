using aydocs.NotchWin.Resources;
using aydocs.NotchWin.UI.UIElements;
using aydocs.NotchWin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace aydocs.NotchWin.UI.Widgets.Small
{
    class RegisterNetworkWidget : IRegisterableWidget
    {
        public bool IsSmallWidget => true;
        public string WidgetName => "Network Status";

        public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        {
            return new NetworkWidget(parent, position, alignment);
        }
    }

    public class NetworkWidget : SmallWidgetBase
    {
        DWText networkStatus;

        private float updateTimer = 0f;
        private const float UPDATE_INTERVAL = 1.0f; // Update every 1 second

        public NetworkWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
        {
            // Status text
            networkStatus = new DWText(this, "Offline", new Vec2(0, 0), UIAlignment.Center);
            networkStatus.TextSize = 10;
            networkStatus.Font = Res.SFProRegular;
            networkStatus.Color = Theme.TextMain;
            AddLocalObject(networkStatus);

            UpdateNetworkStatus();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            updateTimer += deltaTime;
            if (updateTimer >= UPDATE_INTERVAL)
            {
                UpdateNetworkStatus();
                updateTimer = 0f;
            }
        }

        private void UpdateNetworkStatus()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface ni in interfaces)
                {
                    // Check if interface is up and not loopback
                    if (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        ni.OperationalStatus == OperationalStatus.Up)
                    {
                        networkStatus.Text = "Online";
                        networkStatus.Color = Theme.Success;
                        return;
                    }
                }

                networkStatus.Text = "Offline";
                networkStatus.Color = Theme.Error;
            }
            catch
            {
                networkStatus.Text = "N/A";
                networkStatus.Color = Theme.TextSecond;
            }
        }

        protected override float GetWidgetWidth() { return 55; }
    }
}
