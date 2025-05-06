using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SGULibraryManagement.BUS
{
    public class RoleBUS
    {
        private readonly RoleDAO RoleDAO = new();

        private Dictionary<ERole, SolidColorBrush>? roleColors;
        public Dictionary<ERole, SolidColorBrush> RoleColors
        {
            get => roleColors ??= InitializeRoleColors();
            set => roleColors = value;
        }

        public RoleBUS()
        {

        }

        private Dictionary<ERole, SolidColorBrush> InitializeRoleColors()
        {
            var app = App.Instance!;
            if (app.Resources["ErrorColor"] is not SolidColorBrush adminColor)
            {
                adminColor = Brushes.Red;
            }
            if (app.Resources["Turquoise91"] is not SolidColorBrush userColor)
            {
                userColor = Brushes.Blue;
            }

            return new() {
                { ERole.Admin, adminColor },
                { ERole.User, userColor }
            };
        }

        public List<RoleDTO> GetAll()
        {
            return RoleDAO.GetAll(true);
        }

        public RoleDTO FindById(long id)
        {
            return RoleDAO.FindById(id);
        }
    }

    public enum ERole
    {
        Admin,
        User
    }
}
