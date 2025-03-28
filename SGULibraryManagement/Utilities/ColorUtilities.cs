using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SGULibraryManagement.Utilities
{
    public class ColorUtilities
    {
        public static SolidColorBrush FromArgb(byte a, byte r, byte g, byte b)
        {
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        public static SolidColorBrush FromRgb(byte r, byte g, byte b)
        {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public static SolidColorBrush FromHex(string hex)
        {
            return new SolidColorBrush(ConvertHexToColor(hex));
        }

        private static Color ConvertHexToColor(string hex)
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }
    }
}
