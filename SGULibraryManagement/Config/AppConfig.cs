using SGULibraryManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SGULibraryManagement.Config
{
    public static class AppConfig
    {
        public static double AppWidth => 1440;
        public static double AppHeight => 1024;
        public static double SideMenuWidth => 219;
        public static double HeaderHeight => 90;

        public static double AppContentWidth => AppWidth - SideMenuWidth;
        public static double AppContentHeight => AppHeight - HeaderHeight;

        public static App ConfigureMySql(this App app)
        {
            _ = MySqlConnector.Instance;
            return app;
        }

        public static App ConfigureSize(this App app)
        {
            app.Resources[nameof(AppWidth)] = AppWidth;
            app.Resources[nameof(AppHeight)] = AppHeight;
            app.Resources[nameof(SideMenuWidth)] = SideMenuWidth;
            app.Resources[nameof(HeaderHeight)] = HeaderHeight;

            app.Resources[nameof(AppContentWidth)] = AppContentWidth;
            app.Resources[nameof(AppContentHeight)] = AppContentHeight;

            return app;
        }
    }
}
