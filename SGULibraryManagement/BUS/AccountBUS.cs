using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
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
        private List<AccountDTO> users = [];

        public AccountBUS()
        {
            Fetch();
        }

        public List<AccountDTO> GetAll()
        {
            return users = userDAO.GetAll(true);
        }

        private void Fetch()
        {

        }

        public AccountDTO createAccount(AccountDTO account)
        {
            return userDAO.Create(account);
        }

        public bool updateAccount(AccountDTO account)
        {
            return userDAO.Update(11, account);
        }
        public bool deleteAccount(string username)
        {
            return userDAO.DeleteV2(username);
        }

        //public List<UserDTO> FilterByQuery(string query, UserQueryOption queryOption, List<UserDTO>? list = null)
        //{
        //    var users = list ?? this.users;
        //    return queryOption switch
        //    {
        //        UserQueryOption.Username => [.. users.Where(user => user.Username.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
        //        UserQueryOption.Fullname => [.. users.Where(user => user.FullName.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
        //        UserQueryOption.Phone => [.. users.Where(user => user.Phone.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
        //        _ => []
        //    };
        //}

        //public List<UserDTO> FilterByStatus(bool isAvailable, List<UserDTO>? list = null)
        //{
        //    var users = list ?? this.users;
        //    return [.. users.Where(user => user.IsAvailable == isAvailable)];
        //}
    }

    //public enum UserQueryOption
    //{
    //    Username,
    //    Fullname,
    //    Phone
    //}
}
