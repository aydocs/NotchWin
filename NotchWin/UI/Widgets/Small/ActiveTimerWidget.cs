using NotchWin.UI.UIElements;
using NotchWin.UI.Widgets.Big;
using NotchWin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotchWin.UI.Widgets.Small
{
    class RegisterActiveTimerWidget : IRegisterableWidget
    {
        public bool IsSmallWidget => true;

        public string WidgetName => "Active Timer Display";

        public WidgetBase CreateWidgetInstance(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter)
        {
            return new ActiveTimerWidget(parent, position, alignment);
        }
    }

    public class ActiveTimerWidget : SmallWidgetBase
    {
        NWText timeText;

        public ActiveTimerWidget(UIObject? parent, Vec2 position, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, alignment)
        {
            timeText = new NWText(this, GetTime(), Vec2.zero, UIAlignment.Center);
            timeText.TextSize = 14;
            AddLocalObject(timeText);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            timeText.SilentSetText(IsTimerActive() ? GetTime() : " ");
        }

        bool IsTimerActive()
        {
            if(TimerWidget.instance != null)
            {
                return TimerWidget.instance.IsTimerRunning;
            }

            return false;
        }

        string GetTime()
        {
            if (TimerWidget.instance != null)
            {
                TimeSpan t = TimeSpan.FromSeconds(TimerWidget.instance.CurrentTime);
                string formatedTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds);

                return formatedTime;            
            }

            return " ";
        }

        protected override float GetWidgetWidth()
        {
            return IsTimerActive() ? 60 : 0;
        }
    }
}
