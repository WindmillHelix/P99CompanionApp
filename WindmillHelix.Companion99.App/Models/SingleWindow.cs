using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindmillHelix.Companion99.App.Models
{
    public class SingleWindow<T>
        where T : Window
    {
        public SingleWindow(T window, SingleWindowStatus status)
        {
            Window = window;
            Status = status;
        }

        public T Window { get; private set; }

        public SingleWindowStatus Status { get; private set; }

        public void ShowOrActivate()
        {
            if(Status == SingleWindowStatus.Existing)
            {
                Window.Activate();
            }
            else
            {
                Window.Show();
            }
        }
    }
}
