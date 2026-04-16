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
    public class OverlayTest : BaseMenu
    {
        public override List<UIObject> InitializeMenu(IslandObject island)
        {
            var objects = base.InitializeMenu(island);

            objects.Add(new NWText(island, "Overlay", Vec2.zero, UIAlignment.Center));

            return objects;
        }
    }
}
