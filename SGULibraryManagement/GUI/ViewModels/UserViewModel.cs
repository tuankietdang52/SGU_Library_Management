using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SGULibraryManagement.GUI.ViewModels
{
    public class UserViewModel
    {
        public long Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsAvailable { get; set; }

        public ICommand? ViewCommand { get; set; }
        public ICommand? EditCommand {  get; set; }
        public ICommand? DeleteCommand { get; set; }
    }
}
