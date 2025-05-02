using SGULibraryManagement.Helper;
using SGULibraryManagement.Utilities;
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
        public static App ConfigureMySql(this App app)
        {
            _ = MySqlConnector.Instance;
            return app;
        }

        public static App ConfigureSize(this App app)
        {
            double appHeight = (double)app.Resources["AppHeight"];
            double appWidth = (double)app.Resources["AppWidth"];
            double sideMenuWidth = (double)app.Resources["SideMenuWidth"];
            double headerHeight = (double)app.Resources["HeaderHeight"];

            app.Resources["AppContentWidth"] = appWidth - sideMenuWidth;
            app.Resources["AppContentHeight"] = appHeight - headerHeight;

            return app;
        }

        public static App ConfigureColor(this App app)
        {
            app.Resources["TableItemSelected"] = ColorUtility.FromArgb(143, 225, 106, 0);
            app.Resources["LockedBackground"] = ColorUtility.FromArgb(128, 255, 0, 0);
            app.Resources["ActiveBackground"] = ColorUtility.FromArgb(128, 0, 255, 0);

            return app;
        }
    }
}
