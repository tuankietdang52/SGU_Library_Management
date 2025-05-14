using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using SGULibraryManagement.BUS;
using SGULibraryManagement.DTO;
using SGULibraryManagement.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZstdSharp.Unsafe;

namespace SGULibraryManagement.GUI.Contents
{
    public partial class StudyAreaView : UserControl
    {
        public StudyAreaView()
        {
            InitializeComponent();
        }

        private readonly StudyAreaBUS MainBus = new();
        private readonly AccountBUS UserBus = new();
        private readonly AccountViolationBUS ViolateBus = new();
        private readonly ViolationBUS violationBUS = new();
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            OnSearch();
            SearchInput.Text = "";
        }

        private long ValidationField(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Vui lòng nhập MSSV của sinh viên ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;
            }
            if (!long.TryParse(query, out long value))
            {
                MessageBox.Show("Vui lòng nhập số ! ", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
            return value;
        }

        private void showMessageIsLocked(AccountViolationDTO ViolationInfo)
        {
            long vioID = ViolationInfo.ViolationId;
            ViolationDTO violation = violationBUS.FindById(vioID);
            MessageBox.Show($"Thành viên đang vi phạm : {violation.Name} ", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            

        }
        private void showMessageUserIsNull()
        {
            MessageBox.Show("Không phải thành viên !");
            
        }
        private void OnSearch()
        {
            
            string searchQuery = SearchInput.Text.Trim();
            long request_mssv = ValidationField(searchQuery);
            if (request_mssv == 0) return;
            
            AccountDTO user = UserBus.FindById(request_mssv);

            if (user == null)
            {
                showMessageUserIsNull();
                return;
            }

            bool isLocked = ViolateBus.IsAccountLocked(user, out AccountViolationDTO ViolationInfo);

            if (isLocked)
            {
                showMessageIsLocked(ViolationInfo);
                return;
            }
            DateTime checkinTime = DateTime.Now;
            StudyAreaDTO studyAreaRequest = new StudyAreaDTO()
            {
                MSSV = user.Mssv,
                CheckinDate = checkinTime,
                IsDeleted = false
            };


            StudyAreaDTO studyArea = MainBus.Create(studyAreaRequest);

            Mssv.Text = user.Mssv.ToString();
            FullName.Text = user.FullName.ToString();
            Phone.Text = user.Phone.ToString();
            Email.Text = user.Email.ToString();
            Falculity.Text = user.Faculty.ToString();
            Major.Text = user.Major.ToString();
        }
    }
}
