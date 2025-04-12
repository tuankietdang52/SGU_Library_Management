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

namespace SGULibraryManagement.BUS
{
    public class AccountBUS
    {
        private AccountDAO userDAO = new ();
        private RoleBUS roleBUS = new();

        private List<AccountViewModel> users = [];
        private Dictionary<long, RoleDTO> roles = [];

        public AccountBUS()
        {

        }

        public List<AccountDTO> GetAll()
        {
            return userDAO.GetAll(true);
        }

        public List<AccountViewModel> GetAllWithRole()
        {
            roles = roleBUS.GetAll().ToDictionary(role => role.Id);
            var list = GetAll();

            return users = [.. list.Select(user => new AccountViewModel()
            {
                Account = user,
                Role = roles[user.IdRole]
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

        public bool UpdateAccount(AccountDTO account)
        {
            return userDAO.Update(11, account);
        }
        public bool DeleteAccount(string username)
        {
            return userDAO.DeleteV2(username);
        }

        public List<AccountViewModel> FilterByQuery(string query, UserQueryOption queryOption, IEnumerable<AccountViewModel>? collection = null)
        {
            var list = collection ?? users;
            return queryOption switch
            {
                UserQueryOption.Username => [.. list.Where(user => user.Account.Username.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                UserQueryOption.Fullname => [.. list.Where(user => user.Account.FullName.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                UserQueryOption.Phone => [.. list.Where(user => user.Account.Phone.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                _ => []
            };
        }

        public List<AccountViewModel> FilterByRole(RoleDTO role, IEnumerable<AccountViewModel>? collection)
        {
            var list = collection ?? users;
            return [.. list.Where(user => user.Role.Id == role.Id)];
        }
    }

    public enum UserQueryOption
    {
        Username,
        Fullname,
        Phone
    }
}
