using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SGULibraryManagement.Utilities
{
    public static class ColorUtility
    {
        public static OxyColor ParseToOxyColor(this SolidColorBrush solidColorBrush)
        {
            Color color = solidColorBrush.Color;
            var a = color.A;
            var r = color.R;
            var g = color.G;
            var b = color.B;

            return OxyColor.FromArgb(a, r, g, b);
        }

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

        public static SolidColorBrush Random()
        {
            var random = new Random();
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);

            return FromRgb(r, g, b);
        }

        private static Color ConvertHexToColor(string hex)
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }
    }
}
