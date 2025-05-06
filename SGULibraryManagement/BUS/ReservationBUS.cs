using SGULibraryManagement.DAO;
using SGULibraryManagement.DTO;
using SGULibraryManagement.GUI.ViewModels;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.BUS
{
    public class ReservationBUS
    {
        private readonly ReservationDAO DAO = new();
        private readonly DeviceBUS deviceBUS = new();
        private readonly AccountBUS accountBUS = new();
        private readonly BorrowDevicesBUS borrowDevicesBUS = new();

        private List<ReservationViewModel>? Reservations;

        public List<ReservationDTO> GetAll()
        {
            return DAO.GetAll(true);
        }

        public List<ReservationViewModel> GetAllWithDetail()
        {
            var list = GetAll();
            Dictionary<long, AccountDTO> accounts = accountBUS.GetAll().ToDictionary(pr => pr.Mssv);
            Dictionary<long, DeviceDTO> devices = deviceBUS.GetAll().ToDictionary(pr => pr.Id);

            return Reservations = [.. list.Select(item => {
                var device = devices[item.DeviceId];
                var account = accounts[item.UserId];

                return new ReservationViewModel() {
                    Id = item.Id,
                    Device = device,
                    User = account,
                    Quantity = item.Quantity,
                    IsCheckedOut = item.IsCheckedOut,
                    DateCreate = item.DateCreate,
                    DateBorrow = item.DateBorrow,
                    DateReturn = item.DateReturn
                };
            })];
        }

        public ReservationDTO FindById(long id)
        {
            return DAO.FindById(id);
        }

        public List<ReservationDTO> FindByAccount(AccountDTO account)
        {
            return DAO.FindByAccountId(account.Mssv);
        }

        public List<ReservationDTO> FindByDevice(DeviceDTO device)
        {
            return DAO.FindByDeviceId(device.Id);
        }

        public ReservationDTO Create(ReservationDTO request)
        {
            return DAO.Create(request);
        }

        /// <summary>
        /// Borrow a device for user
        /// </summary>
        /// <returns></returns>
        public bool Checkout(ReservationViewModel reservation)
        {
            if (!DAO.Checkout(reservation.Id)) return false;

            ReservationDTO model = DAO.FindById(reservation.Id);
            BorrowDevicesDTO borrowDevice = model.ToBorrowDevice();

            if (borrowDevicesBUS.Create(borrowDevice) is not null) return true;
            else return false;
        }

        public bool Update(long id, ReservationDTO request)
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

        public IEnumerable<ReservationViewModel> FilterByQuery(string query, string searchBy, IEnumerable<ReservationViewModel>? collections = null)
        {
            var list = collections ?? Reservations;
            list ??= GetAllWithDetail();

            return searchBy switch
            {
                "Device Name" => list.Where(item => item.Device.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                "User Email" => list.Where(item => item.User.Email.Contains(query, StringComparison.CurrentCultureIgnoreCase)),
                _ => []
            };
        }

        public IEnumerable<ReservationViewModel> FilterByStatus(string status, IEnumerable<ReservationViewModel>? collections = null)
        {
            var list = collections ?? Reservations;
            list ??= GetAllWithDetail();

            return status switch
            {
                "All" => list,
                "Ready" => list.Where(item => item.IsReady),
                "Incoming" => list.Where(item => item.DateBorrow.Date > DateTime.Now.Date),
                "Expired" => list.Where(item => item.IsExpired),
                _ => []
            };
        }

        public IEnumerable<ReservationViewModel> FilterByCheckOutStatus(bool isShowCheckedout, IEnumerable<ReservationViewModel>? collections = null)
        {
            var list = collections ?? Reservations;
            list ??= GetAllWithDetail();

            if (!isShowCheckedout)
            {
                return list.Where(item => !item.IsCheckedOut);
            }

            return list;
        }
    }
}
