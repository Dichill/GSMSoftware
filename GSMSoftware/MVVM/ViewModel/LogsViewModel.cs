using GSMSoftware.Core;
using GSMSoftware.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GSMSoftware.MVVM.ViewModel
{
    public class LogsViewModel : ObservableObject
    {
        public GlobalViewModel GlobalViewModel { get; } = GlobalViewModel.Instance;
        public RelayCommand ClearLogsCommand { get; set; }
        public LogsViewModel()
        {
            ClearLogsCommand = new RelayCommand(o =>
            {
                GlobalViewModel.GSMLogs = "";
            });
        }
    }
}
