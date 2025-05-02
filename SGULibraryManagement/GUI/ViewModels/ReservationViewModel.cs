using SGULibraryManagement.DTO;

namespace SGULibraryManagement.GUI.ViewModels
{
    public class ReservationViewModel
    {
        public required long Id { get; set; }
        public required DeviceDTO Device { get; set; }
        public required AccountDTO User { get; set; }
        public required int Quantity { get; set; }
        public required DateTime DateCreate { get; set; }
        public required DateTime DateBorrow { get; set; }
        public required DateTime DateReturn { get; set; }
        public required bool IsCheckedOut { get; set; }
        public bool IsExpired => DateBorrow.Date < DateTime.Now.Date;
        public bool IsReady => DateBorrow.Date == DateTime.Now.Date && !IsCheckedOut;
    }
}
