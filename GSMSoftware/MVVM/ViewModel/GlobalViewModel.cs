using GSMSoftware.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMSoftware.MVVM.ViewModel
{
    public class GlobalViewModel : ObservableObject
    {
        public static GlobalViewModel Instance { get; } = new GlobalViewModel();

        private string? _gsmLogs;

        public string? GSMLogs
        {
            get { return _gsmLogs; }
            set { _gsmLogs = value; OnPropertyChanged(); }
        }


        private bool _isPortOpen;

        public bool IsPortOpen
        {
            get { return _isPortOpen; }
            set { _isPortOpen = value; OnPropertyChanged(); }
        }

    }
}
