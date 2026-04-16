using aydocs.NotchWin.Resources;
using aydocs.NotchWin.UI.UIElements;
using aydocs.NotchWin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aydocs.NotchWin.UI.Widgets.Small
{
    class RegisterCalendarWidget : IRegisterableWidget
    {
        public bool IsSmallWidget => true;
        public string WidgetName => "Calendar";

        public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        {
            return new CalendarWidget(parent, position, alignment);
        }
    }

    public class CalendarWidget : SmallWidgetBase
    {
        DWText dateText;
        DWText dayText;

        private DateTime lastUpdate = DateTime.MinValue;

        public CalendarWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
        {
            // Day of week text
            dayText = new DWText(this, GetDayOfWeek(), new Vec2(0, -5), UIAlignment.Center);
            dayText.TextSize = 9;
            dayText.Font = Res.SFProRegular;
            dayText.Color = Theme.TextSecond;
            AddLocalObject(dayText);

            // Date text
            dateText = new DWText(this, GetDateString(), new Vec2(0, 6), UIAlignment.Center);
            dateText.TextSize = 12;
            dateText.Font = Res.SFProBold;
            dateText.Color = Theme.TextMain;
            AddLocalObject(dateText);

            lastUpdate = DateTime.Now;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Update once per minute (when minute changes)
            if (DateTime.Now.Minute != lastUpdate.Minute || DateTime.Now.Hour != lastUpdate.Hour)
            {
                dateText.Text = GetDateString();
                dayText.Text = GetDayOfWeek();
                lastUpdate = DateTime.Now;
            }
        }

        private string GetDateString()
        {
            DateTime now = DateTime.Now;
            return $"{now.Day}";
        }

        private string GetDayOfWeek()
        {
            string day = DateTime.Now.ToString("ddd").ToUpper(); // Sun, Mon, Tue, etc.
            return day.Substring(0, 3); // Just the 3-letter abbreviation
        }

        protected override float GetWidgetWidth() { return 35; }
    }
}
