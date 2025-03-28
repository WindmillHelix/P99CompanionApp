using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WindmillHelix.Companion99.App
{
    public static partial class ExtensionMethods
    {
        public static void SetupDefaults(this Window window)
        {
            window.KeyUp += (s, e) =>
            {
                if(e.Key == Key.System)
                {
                    e.Handled = true;
                }
            };
        }
    }
}
