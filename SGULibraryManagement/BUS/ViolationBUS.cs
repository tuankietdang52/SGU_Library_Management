using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.ViewModels;

namespace SGULibraryManagement.BUS
{
    public class ViolationBUS
    {
        private readonly ViolationDAO DAO = new();
        private List<ViolationViewModel>? violationVMs;

        private Dictionary<long, ViolationDTO> violations = [];
        private Dictionary<long, int> violationCount = [];


        public ViolationBUS()
        {

        }

        public List<ViolationDTO> GetAll()
        {
            var list = DAO.GetAll(true);
            violations = list.ToDictionary(item => item.Id);
            violationCount = list.ToDictionary(violation => violation.Id, violations => 0);

            return list;
        }

        public List<ViolationViewModel> GetAllWithViolationCount()
        {
            GetAll();
            var list = DAO.GetAllWithViolationCount();

            foreach (var item in list) {
                violationCount[item.First.Id] = item.Last;
            }

            return violationVMs = [.. violations.Select(pair => new ViolationViewModel() {
                Violation = pair.Value,
                ViolationCount = violationCount[pair.Key]
            })];
        }

        public ViolationDTO FindById(long id)
        {
            return DAO.FindById(id);
        }

        public ViolationDTO Create(ViolationDTO request)
        {
            return DAO.Create(request);
        }

        public bool Update(long id, ViolationDTO request)
        {
            return DAO.Update(id, request);
        }

        public bool Delete(ViolationDTO violation)
        {
            return DAO.Delete(violation.Id);
        }

        public bool DeleteMultiple(List<ViolationDTO> violations)
        {
            return DAO.DeleteMultiple([.. violations.Select(v => v.Id)]);
        }

        public List<ViolationViewModel> FilterByQuery(string query, IEnumerable<ViolationViewModel>? collections = null)
        {
            var list = collections ?? violationVMs;
            list ??= GetAllWithViolationCount();

            return [.. list.Where(item => item.Violation.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase))];
        }
    }
}