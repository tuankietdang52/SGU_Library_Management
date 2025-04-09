using SGULibraryManagement.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SGULibraryManagement.Components.Dialogs
{
    public delegate void OnCloseDialogHandler(object sender);

    public interface IDialog
    {
        public event OnCloseDialogHandler OnCloseDialog;
        public ContentPresenter? PopupHost { get; set; }
    }
}
