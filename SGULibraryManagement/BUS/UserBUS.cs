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
    public class UserBUS
    {
        private UserDAO userDAO = new UserDAO();
        private List<UserDTO> users = [];

        public UserBUS()
        {
            Fetch();
        }

        public List<UserDTO> GetAll()
        {
            return users = userDAO.GetAll(true);
        }

        private void Fetch()
        {

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
