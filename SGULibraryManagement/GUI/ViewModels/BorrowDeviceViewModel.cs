using SGULibraryManagement.DTO;

namespace SGULibraryManagement.GUI.ViewModels
{
    public class BorrowDeviceViewModel
    {
        public required long Id { get; set; }
        public required DeviceDTO Device { get; set; }
        public required AccountDTO User { get; set; }
        public required int Quantity { get; set; }
        public required DateTime DateBorrow { get; set; }
        public required DateTime DateReturn { get; set; }
        public required bool IsReturn { get; set; }
        public bool IsDue => DateTime.Now.Date > DateReturn.Date && !IsReturn;
    }
}
