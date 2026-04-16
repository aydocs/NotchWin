using NotchWin.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotchWin.UI.UIElements
{
    public class NWTextButton : NWButton
    {
        NWText text;

        public NWText Text { get { return text; } set => text = value; }

        public float normalTextSize = 14;
        public float textSizeSmoothSpeed = 15f;

        public NWTextButton(UIObject? parent, string buttonText, Vec2 position, Vec2 size, Action clickCallback, UIAlignment alignment = UIAlignment.TopCenter) : base(parent, position, size, clickCallback, alignment)
        {
            text = new NWText(this, buttonText, Vec2.zero, UIAlignment.Center);
            AddLocalObject(text);

            Text.TextSize = normalTextSize;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            float currentTextSize = normalTextSize;

            if (IsHovering && !IsMouseDown)
                currentTextSize *= hoverScaleMulti.Magnitude;
            else if (IsMouseDown)
                currentTextSize *= clickScaleMulti.Magnitude;
            else if (!IsHovering && !IsMouseDown)
                currentTextSize *= normalScaleMulti.Magnitude;
            else
                currentTextSize *= normalScaleMulti.Magnitude;

            Text.TextSize = Mathf.Lerp(Text.TextSize, currentTextSize, textSizeSmoothSpeed * deltaTime);
        }
    }
}
