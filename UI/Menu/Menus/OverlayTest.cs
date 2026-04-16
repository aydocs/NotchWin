using aydocs.NotchWin.Main;
using aydocs.NotchWin.UI.UIElements;
using aydocs.NotchWin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aydocs.NotchWin.UI.Menu.Menus
{
    public class OverlayTest : BaseMenu
    {
        public override List<UIObject> InitializeMenu(IslandObject island)
        {
            var objects = base.InitializeMenu(island);

            objects.Add(new DWText(island, "Overlay", Vec2.zero, UIAlignment.Center));

            return objects;
        }
    }
}
