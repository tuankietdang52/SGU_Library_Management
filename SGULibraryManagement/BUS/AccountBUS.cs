using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.ViewModels;
using System.Windows.Media;

namespace SGULibraryManagement.BUS
{
    public class AccountBUS
    {
        private readonly AccountDAO userDAO = new();
        private readonly AccountViolationBUS accountViolationBUS = new();
        private readonly RoleBUS roleBUS = new();
        private readonly BorrowDevicesDAO borrowDevicesDAO = new();
        private readonly ReservationDAO reservationBUS = new();
        private readonly StudyAreaDAO studyAreaDAO = new();

        private List<AccountViewModel> users = [];
        private Dictionary<long, RoleDTO> roles = [];
        private HashSet<AccountViolationDTO>? lockedUser;

        public AccountBUS()
        {

        }

        public bool CreateListAccount(List<AccountDTO> listAccount)
        {
            return userDAO.CreateListAccount(listAccount);
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
            lockedUser = accountViolationBUS.GetAllLockedUsers();

            var list = GetAll();

            return users = [.. list.Select(user => {
                var role = roles[user.IdRole];
                SolidColorBrush roleBg;
                
                if (!Enum.TryParse(role.Name, out ERole eRole)) {
                    roleBg = Brushes.Green;
                }
                else roleBg = roleBUS.RoleColors[eRole];

                bool isLocked = lockedUser.Where(av => av.UserId == user.Mssv).Any();

                return new AccountViewModel()
                {
                    Account = user,
                    Role = roles[user.IdRole],
                    IsLocked = isLocked,
                    RoleBackgroundColor = roleBg,
                    BgColor = !isLocked ? Brushes.Transparent : (SolidColorBrush)App.Instance!.Resources["LockedBackground"]
                };
            })];
        }

        public AccountDTO? FindByUsername(long mssv)
        {
            return userDAO.FindByUsername(mssv);
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
            return userDAO.Delete(account.Mssv);
        }

        public bool DeleteMultipleAccounts(List<AccountDTO> accounts)
        {
            List<long> ids = [.. accounts.Select(item => item.Mssv)];

            if (!borrowDevicesDAO.DeleteMultipleByStudentCode(ids)) return false;
            if (!reservationBUS.DeleteMultipleByStudentCode(ids)) return false;
            if (!accountViolationBUS.DeleteMultipleByAccount(accounts)) return false;
            if (!studyAreaDAO.DeleteMultipleByStudentCode(ids)) return false;

            return userDAO.DeleteMultiple([.. accounts.Select(a => a.Mssv)]);
        }

        public List<AccountViewModel> FilterByQuery(string query, UserQueryOption queryOption, IEnumerable<AccountViewModel>? collection = null)
        {
            var list = collection ?? users;
            return queryOption switch
            {
                UserQueryOption.Mssv => [.. list.Where(user => user.Account.Mssv.ToString().Contains(query, StringComparison.CurrentCultureIgnoreCase))],
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

        public List<AccountViewModel> FilterByLockStatus(string status, ViolationDTO? violation, IEnumerable<AccountViewModel>? collection = null)
        {
            var list = collection ?? users;
            if (status == "All") return [.. list];

            lockedUser = accountViolationBUS.GetAllLockedUsers();
            bool isLocked = status == "Locked";

            if (isLocked)
            {
                if (violation?.Id == -1) return [.. list.Where(user => user.IsLocked)];

                return [.. list.Where(user => lockedUser.Where(av => av.UserId == user.Account.Mssv && av.ViolationId == violation?.Id)
                                                        .Any())];
            }

            return [.. list.Where(user => !user.IsLocked)];
        }
    }

    public enum UserQueryOption
    {
        Mssv,
        Fullname,
        Phone,
        Email
    }
}
