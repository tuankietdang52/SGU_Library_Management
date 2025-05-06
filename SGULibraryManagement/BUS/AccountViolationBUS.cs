using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;

namespace SGULibraryManagement.BUS
{
    public class AccountViolationBUS
    {
        private readonly AccountViolationDAO DAO = new();

        public AccountViolationBUS()
        {

        }

        public AccountViolationDTO FindById(long id)
        {
            return DAO.FindById(id);
        }

        public List<AccountViolationDTO> FindByAccount(AccountDTO account)
        {
            return DAO.FindByAccountId(account.Mssv);
        }

        public List<AccountViolationDTO> FindByViolation(ViolationDTO violation)
        {
            return DAO.FindByViolationId(violation.Id);
        }

        public List<AccountViolationDTO> GetAll()
        {
            return DAO.GetAll(true);
        }

        public AccountViolationDTO Create(AccountViolationDTO request)
        {
            if (DAO.IsAccountLocked(request.UserId) is not null) return null!;
            return DAO.Create(request);
        }

        public AccountViolationDTO ChangeViolation(long id, AccountViolationDTO request)
        {
            if (DAO.IsAccountLocked(request.UserId) is null) return null!;
            DAO.Delete(id);

            request.Id = id;
            return Create(request);
        }

        public bool Update(long id, AccountViolationDTO request)
        {
            return DAO.Update(id, request);
        }

        public bool Delete(long id)
        {
            return DAO.Delete(id);
        }

        public bool DeleteMultipleByAccount(List<AccountDTO> accounts)
        {
            return DAO.DeleteMultipleByStudentCode([.. accounts.Select(a => a.Mssv)]);
        }

        public bool IsAccountLocked(AccountDTO account, out AccountViolationDTO accountViolation)
        {
            accountViolation = DAO.IsAccountLocked(account.Mssv)!;
            return accountViolation != null;
        }

        public bool IsRuleViolatedByUser(ViolationDTO violation)
        {
            return DAO.IsRuleViolatedByUser(violation.Id);
        }

        public List<AccountViolationDTO> IsRulesViolatedByUser(List<ViolationDTO> violations)
        {
            return DAO.IsRulesViolatedByUser([.. violations.Select(v => v.Id)]);
        }

        public HashSet<AccountViolationDTO> GetAllLockedUsers()
        {
            return [.. DAO.GetAllLockedUsers()];
        }
    }
}
