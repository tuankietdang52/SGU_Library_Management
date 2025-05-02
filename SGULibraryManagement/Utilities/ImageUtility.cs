using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SGULibraryManagement.Utilities
{
    public class ImageUtility
    {
        private readonly string folderPath = "pack://application:,,,/Resources/Images";
        private readonly List<string>? folders;

        public ImageUtility() { }

        public ImageUtility(List<string> folders)
        {
            this.folders = folders;
        }

        public Image GetImage(string fileName)
        {
            Image image = new();
            BitmapImage source = new(new Uri(GetImagePath(fileName), UriKind.Relative));
            image.Source = source;

            return image;
        }

        private string GetImagePath(string fileName)
        {
            string source = folderPath;
            if (folders == null) return $"{folderPath}/{fileName}";

            foreach (string folder in folders)
            {
                source += $"/{folder}";
            }

            source += $"/{fileName}";
            return source;
        }
    }
}
