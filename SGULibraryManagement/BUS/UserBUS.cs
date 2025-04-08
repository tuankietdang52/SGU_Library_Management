using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.BUS
{
    public class UserBUS
    {
        public List<UserDTO> Users { get; private set; } = [];

        public UserBUS()
        {
            Fetch();
        }

        private void Fetch()
        {
            UserDTO model = new()
            {
                Id = 1,
                FirstName = "SHIBA",
                LastName = "LAKAKA",
                Phone = "0321321",
                Username = "shiba123",
                Password = "123",
                IsAvailable = false
            };

            UserDTO model2 = new()
            {
                Id = 1,
                FirstName = "SHIBA",
                LastName = "ALIBABA",
                Phone = "0321321",
                Username = "shiba123",
                Password = "123",
                IsAvailable = true
            };

            Users.Add(model);
            Users.Add(model2);
        }

        public List<UserDTO> FilterByQuery(string query, UserQueryOption queryOption)
        {
            return queryOption switch
            {
                UserQueryOption.Username => [.. Users.Where(user => user.Username.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                UserQueryOption.Fullname => [.. Users.Where(user => user.FullName.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                UserQueryOption.Phone => [.. Users.Where(user => user.Phone.Contains(query, StringComparison.CurrentCultureIgnoreCase))],
                _ => []
            };
        }
    }

    public enum UserQueryOption
    {
        Username,
        Fullname,
        Phone
    }
}
