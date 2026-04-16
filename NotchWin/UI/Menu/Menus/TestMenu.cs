using NotchWin.Main;
using NotchWin.UI.UIElements;
using NotchWin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotchWin.UI.Menu.Menus
{
    public class TestMenu : BaseMenu
    {
        public override List<UIObject> InitializeMenu(IslandObject island)
        {
            var objects = base.InitializeMenu(island);

            objects.Add(new NWText(island, "Test", Vec2.zero, UIAlignment.TopCenter));

            var btn = new NWTextButton(island, "Hello", new Vec2(0, 0), new Vec2(125, 25), () =>
            {
                MenuManager.OpenOverlayMenu(new TestMenu());

            }, UIAlignment.Center);

            objects.Add(btn);

            return objects;
        }
    }
}
