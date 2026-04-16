using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace aydocs.NotchWin.Main
{
    public class ErrorForm : Window
    {
        public ErrorForm()
        {
            var result = MessageBox.Show("Only one instance of aydocs.NotchWin can run at a time.", "An error occured.");
            Process.GetCurrentProcess().Kill();
        }
    }
}
