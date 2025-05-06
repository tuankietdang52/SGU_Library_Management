using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;

namespace SGULibraryManagement.BUS
{
    public class BorrowDevicesBUS
    {
        private readonly BorrowDevicesDAO dao = new();
        private readonly DeviceBUS deviceBUS = new();
        private readonly AccountBUS accountBUS = new();

        private List<BorrowDeviceViewModel>? BorrowDevices;

        public List<BorrowDevicesDTO> GetAll()
        {
            return dao.GetAll(true);
        }

        public List<BorrowDeviceViewModel> GetAllWithDetail()
        {
            var list = GetAll();
            Dictionary<long, AccountDTO> accounts = accountBUS.GetAll().ToDictionary(pr => pr.Mssv);
            Dictionary<long, DeviceDTO> devices = deviceBUS.GetAll().ToDictionary(pr => pr.Id);

            return BorrowDevices = [.. list.Select(item => {
                if (!devices.TryGetValue(item.DeviceId, out var device)) {
                    return null;
                }

                if (!accounts.TryGetValue(item.UserId, out var account)) {
                    return null;
                }

                return new BorrowDeviceViewModel() {
                    Id = item.Id,
                    Device = device,
                    User = account,
                    Code = item.Code!,
                    Quantity = item.Quantity,
                    DateBorrow = item.DateBorrow,
                    DateReturnExpected = item.DateReturnExpected,
                    DateReturn = item.DateReturn,
                    IsReturn = item.IsReturn
                };
            })];
        }

        public BorrowDevicesDTO FindById(long id)
        {
            return dao.FindById(id);
        }

        public BorrowDevicesDTO FindByCode(string code)
        {
            return dao.FindByCode(code);
        }

        public List<BorrowDevicesDTO> FindByAccount(AccountDTO account)
        {
            return dao.FindByAccountMssv(account.Mssv);
        }

        public List<BorrowDevicesDTO> FindByDevice(DeviceDTO device)
        {
            return dao.FindByDeviceId(device.Id);
        }

        public BorrowDevicesDTO Create(BorrowDevicesDTO request)
        {
            string code = CodeUtility.GenerateRandom(6);
            request.Code = code;

            return dao.Create(request);
        }

        public bool Update(long id, BorrowDevicesDTO request)
        {
            return dao.Update(id, request);
        }

        public bool Delete(long id)
        {
            return dao.Delete(id);
        }

        public IEnumerable<BorrowDeviceViewModel> FilterByQuery(string query, string searchBy, IEnumerable<BorrowDeviceViewModel>? collections = null)
        {
            var list = collections ?? BorrowDevices;
            list ??= GetAllWithDetail();

            return searchBy switch
            {
                "Device Name" => list.Where(item => item.Device.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                "User Email" => list.Where(item => item.User.Email.Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                "Code" => list.Where(item => item.Code.Contains(query)),
                _ => []
            };
        }

        public IEnumerable<BorrowDeviceViewModel> FilterByStatus(string status, IEnumerable<BorrowDeviceViewModel>? collections = null)
        {
            var list = collections ?? BorrowDevices;
            list ??= GetAllWithDetail();

            return status switch
            {
                "All" => list,
                "Return" => list.Where(item => item.IsReturn),
                "Return Late" => list.Where(item => item.IsReturn && item.IsDue),
                "Not Return" => list.Where(item => !item.IsReturn && item.IsDue),
                "Not yet due" => list.Where(item => DateTime.Now.Date < item.DateReturn.Date),
                _ => []
            };
        }

        /// <summary>
        /// Get all by date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="fromStart">if true will get from date, else will get to date</param>
        /// <returns></returns>
        public List<BorrowDevicesDTO> GetAllByBorrowDate(DateTime date, bool fromStart)
        {
            return dao.GetAllByBorrowDate(date, fromStart);
        }

        public List<BorrowDevicesDTO> GetAllByBorrowDate(DateTime start, DateTime end)
        {
            return dao.GetAllByBorrowDate(start, end);
        }
    }
}