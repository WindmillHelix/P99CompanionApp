using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WindmillHelix.Companion99.App.Models;

namespace WindmillHelix.Companion99.App
{
    public static class SingleWindowManager
    {
        private static Dictionary<Type, Window> _windows = new Dictionary<Type, Window>();

        public static SingleWindow<T> GetWindow<T>()
            where T : Window
        {
            var type = typeof(T);

            if (_windows.ContainsKey(type))
            {
                return new SingleWindow<T>((T)_windows[type], SingleWindowStatus.Existing);
            }

            var window = DependencyInjector.Resolve<T>();
            _windows.Add(type, window);

            var closer = new WindowCloser(type);
            window.Closed += closer.Window_Closed;

            var result = new SingleWindow<T>(window, SingleWindowStatus.New); ;

            return result;
        }

        private class WindowCloser
        {
            private readonly Type _type;

            public WindowCloser(Type type)
            {
                _type = type;
            }

            public void Window_Closed(object? sender, EventArgs e)
            {
                _windows.Remove(_type);
            }
        }
    }
}
