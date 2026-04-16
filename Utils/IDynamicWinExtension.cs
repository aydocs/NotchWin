using aydocs.NotchWin.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aydocs.NotchWin.Utils
{
    public interface Iaydocs.NotchWinExtension
    {
        public string AuthorName { get; }
        public string ExtensionName { get; }
        public string ExtensionID { get; }
        public void LoadExtension();
        public List<IRegisterableWidget> GetExtensionWidgets();
    }
}
