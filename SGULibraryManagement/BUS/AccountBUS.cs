using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SGULibraryManagement.BUS
{
    public class AccountBUS
    {
        private readonly AccountDAO userDAO = new();
        private readonly AccountViolationBUS accountViolationBUS = new();
        private readonly RoleBUS roleBUS = new();

        private List<AccountViewModel> users = [];
        private Dictionary<long, RoleDTO> roles = [];
        private HashSet<long>? lockedUser;

        public AccountBUS()
        {

        }

        public List<AccountDTO> GetAll()
        {
            return userDAO.GetAll(true);
        }

        public AccountDTO FindById(long id)
        {
            return userDAO.FindById(id);
        }

        public List<AccountViewModel> GetAllWithRole()
        {
            roles = roleBUS.GetAll().ToDictionary(role => role.Id);
            lockedUser = [.. accountViolationBUS.GetAllLockedUsers().Select(item => item.UserId)];

            var list = GetAll();

            return users = [.. list.Select(user => {
                var role = roles[user.IdRole];
                SolidColorBrush roleBg;
                
                if (!Enum.TryParse(role.Name, out ERole eRole)) {
                    roleBg = Brushes.Green;
                }
                else roleBg = roleBUS.RoleColors[eRole];

                return new AccountViewModel()
                {
                    Account = user,
                    Role = roles[user.IdRole],
                    RoleBackgroundColor = roleBg,
                    BgColor = !lockedUser.Contains(user.Id) ? Brushes.Transparent : (SolidColorBrush)App.Instance!.Resources["LockedBackground"]
                };
            })];
        }

        public AccountDTO? FindByUsername(string username)
        {
            return userDAO.FindByUsername(username);
        }

        public AccountDTO CreateAccount(AccountDTO account)
        {
            return userDAO.Create(account);
        }

        public bool UpdateAccount(long id, AccountDTO account)
        {
            return userDAO.Update(id, account);
        }

        public bool DeleteAccount(AccountDTO account)
        {
            return userDAO.Delete(account.Id);
        }

        public List<AccountViewModel> FilterByQuery(string query, UserQueryOption queryOption, IEnumerable<AccountViewModel>? collection = null)
        {
            var list = collection ?? users;
            return queryOption switch
            {
                UserQueryOption.Username => [.. list.Where(user => user.Account.Username.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                UserQueryOption.Fullname => [.. list.Where(user => user.Account.FullName.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                UserQueryOption.Phone => [.. list.Where(user => user.Account.Phone.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                UserQueryOption.Email => [.. list.Where(user => user.Account.Email.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                _ => []
            };
        }

        public List<AccountViewModel> FilterByRole(RoleDTO role, IEnumerable<AccountViewModel>? collection = null)
        {
            var list = collection ?? users;
            return [.. list.Where(user => user.Role.Id == role.Id)];
        }

        public List<AccountViewModel> FilterByLockStatus(string status, IEnumerable<AccountViewModel>? collection = null)
        {
            var list = collection ?? users;
            if (status == "All") return [.. list];

            lockedUser ??= [.. accountViolationBUS.GetAllLockedUsers().Select(item => item.UserId)];
            bool isLocked = status == "Locked";

            return [.. list.Where(user => isLocked ? lockedUser.Contains(user.Account.Id) : !lockedUser.Contains(user.Account.Id))];
        }
    }

    public enum UserQueryOption
    {
        Username,
        Fullname,
        Phone,
        Email
    }
}
