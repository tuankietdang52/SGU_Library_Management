using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Components.Dialogs
{
    public class SimpleDialog
    {
        public object? Content { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public enum SimpleDialogType
    {
        OK,
        OKCancel,
        YesNo
    }

    public enum SimpleDialogResult
    {
        OK,
        Cancel,
        Yes,
        No
    }
}
